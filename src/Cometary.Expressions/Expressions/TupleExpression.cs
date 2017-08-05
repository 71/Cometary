using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents a tuple.
    /// </summary>
    public sealed class TupleExpression : Expression
    {
        /// <inheritdoc />
        public override bool CanReduce => true;

        /// <inheritdoc />
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <inheritdoc />
        public override Type Type { get; }

        /// <summary>
        /// Gets the <see cref="Expression"/> array that corresponds
        /// to all the members of the tuple.
        /// </summary>
        public ImmutableArray<ParameterExpression> Variables { get; }

        internal TupleExpression(IEnumerable<ParameterExpression> variables)
        {
            Variables = variables.ToImmutableArray();
            Type = Type.GetType($"{typeof(Tuple).FullName}`{Variables.Length}")?.MakeGenericType(Variables.Select(x => x.Type).ToArray());

            if (Type == null)
                throw new InvalidOperationException("Invalid variables given.");
        }

        /// <inheritdoc />
        public override Expression Reduce()
        {
            return Block(Variables,
                Call(typeof(Tuple), nameof(Tuple.Create), null, Variables.ToArray<Expression>())
            );
        }

        /// <inheritdoc cref="UnaryExpression.Update(Expression)" select="summary"/>
        public TupleExpression Update(params ParameterExpression[] variables)
        {
            if (variables == null)
                throw new ArgumentNullException(nameof(variables));
            if (Variables.SequenceEqual(variables))
                return this;

            return Expressive.Tuple(variables);
        }

        /// <inheritdoc />
        public override string ToString() => $"({string.Join(", ", Variables)})";
    }

    partial class Expressive
    {
        /// <summary>
        /// Creates a <see cref="TupleExpression"/> that represents
        /// a tuple.
        /// </summary>
        public static TupleExpression Tuple(params ParameterExpression[] variables)
        {
            return Tuple((IEnumerable<ParameterExpression>)variables);
        }

        /// <summary>
        /// Creates a <see cref="TupleExpression"/> that represents
        /// a tuple.
        /// </summary>
        public static TupleExpression Tuple(IEnumerable<ParameterExpression> variables)
        {
            if (variables == null)
                throw new ArgumentNullException(nameof(variables));
            if (variables is IReadOnlyCollection<ParameterExpression> collection && collection.Count == 0)
                throw new ArgumentOutOfRangeException(nameof(variables));

            return new TupleExpression(variables);
        }
    }
}
