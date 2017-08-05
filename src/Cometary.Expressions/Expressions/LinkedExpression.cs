using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents an expression whose value in the compiled expression
    /// tree is synced with the <see cref="Value"/> property.
    /// </summary>
    /// <typeparam name="T">The type of the value stored by this expression.</typeparam>
    [DebuggerStepThrough]
    public sealed class LinkedExpression<T> : Expression
    {
        private static readonly FieldInfo ValueField = typeof(Link).GetRuntimeField(nameof(Link.Value));

        private sealed class Link
        {
            public T Value;
        }

        private readonly Link _link;
        private readonly Expression _reduced;

        private readonly Func<T> _compiledGetter;
        private readonly Action<T> _compiledSetter;
        private readonly bool _isConstantLink;

        /// <inheritdoc />
        public override bool CanReduce => true;

        /// <inheritdoc />
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <inheritdoc />
        public override Type Type => typeof(T);

        /// <summary>
        /// Gets whether or not the value of this expression is read-only.
        /// </summary>
        public bool IsReadOnly { get; }

        /// <summary>
        /// Gets the value of this <see cref="LinkedExpression{T}"/>.
        /// </summary>
        public T Value
        {
            get => _isConstantLink ? _link.Value : _compiledGetter();
            set
            {
                if (IsReadOnly)
                    throw new InvalidOperationException();

                if (_isConstantLink)
                    _link.Value = value;
                else
                    _compiledSetter(value);
            }
        }

        internal LinkedExpression(T val, bool readOnly)
        {
            IsReadOnly = readOnly;

            _link = new Link { Value = val };
            _isConstantLink = true;
            _reduced = typeof(T).GetTypeInfo().IsValueType ? (Expression)Field(Constant(_link), ValueField) : Constant(val);
        }

        internal LinkedExpression(Expression<Func<T>> lambda, bool readOnly)
        {
            IsReadOnly = readOnly;

            _compiledGetter = GetCompiledGetter(lambda);
            _compiledSetter = GetCompiledSetter(lambda);
            _reduced = lambda.Body;

            if (_compiledSetter == null && !readOnly)
                throw new InvalidOperationException("Cannot create read/write link with read-only expression.");
        }

        private static Func<T> GetCompiledGetter(Expression<Func<T>> lambda)
        {
            return lambda.Compile();
        }

        private static Action<T> GetCompiledSetter(Expression<Func<T>> lambda)
        {
            ParameterExpression parameter = Parameter(typeof(T), "value");

            return Lambda<Action<T>>(Assign(lambda.Body, parameter), parameter).Compile();
        }

        /// <summary>
        /// Returns a new <see cref="LinkedExpression{T}"/> whose <see cref="Value"/>
        /// property set to the given <paramref name="value"/>.
        /// </summary>
        public static implicit operator LinkedExpression<T>(T value) => new LinkedExpression<T>(value, true);

        /// <inheritdoc />
        public override Expression Reduce() => _reduced;

        /// <inheritdoc />
        protected override Expression Accept(ExpressionVisitor visitor) => visitor.Visit(Reduce());

        /// <inheritdoc />
        public override string ToString() => Value.ToString();
    }

    partial class Expressive
    {
        /// <summary>
        /// Creates a read-only <see cref="LinkedExpression{T}"/> that represents
        /// a link between a variable and its compiled value.
        /// </summary>
        public static LinkedExpression<T> Link<T>(T value)
        {
            return new LinkedExpression<T>(value, true);
        }

        /// <summary>
        /// Creates a <see cref="LinkedExpression{T}"/> that represents
        /// a link between a variable and its compiled value.
        /// </summary>
        public static LinkedExpression<T> Link<T>(T value, bool isReadOnly)
        {
            return new LinkedExpression<T>(value, isReadOnly);
        }

        /// <inheritdoc cref="Link{T}(T)" select="summary" />
        /// <remarks>
        /// This method accepts method bodies of the form:
        ///  - <see cref="ExpressionType.Index"/>;
        ///  - <see cref="ExpressionType.MemberAccess"/>;
        ///  - <see cref="ExpressionType.Parameter"/>.
        /// </remarks>
        public static LinkedExpression<T> Link<T>(Expression<Func<T>> expression)
        {
            Requires.NotNull(expression, nameof(expression));

            return new LinkedExpression<T>(expression, true);
        }

        /// <inheritdoc cref="Link{T}(T, bool)" select="summary" />
        /// <remarks>
        /// This method accepts method bodies of the form:
        ///  - <see cref="ExpressionType.Index"/>;
        ///  - <see cref="ExpressionType.MemberAccess"/>;
        ///  - <see cref="ExpressionType.Parameter"/>.
        /// </remarks>
        public static LinkedExpression<T> Link<T>(Expression<Func<T>> expression, bool isReadOnly)
        {
            Requires.NotNull(expression, nameof(expression));

            return new LinkedExpression<T>(expression, isReadOnly);
        }
    }
}
