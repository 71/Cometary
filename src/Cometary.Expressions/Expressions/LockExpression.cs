using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents a <see langword="lock"/> expression.
    /// </summary>
    public sealed class LockExpression : Expression
    {
        /// <inheritdoc />
        public override bool CanReduce => true;

        /// <inheritdoc />
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <inheritdoc />
        public override Type Type => Body.Type;

        /// <summary>
        /// Gets the <see cref="Expression"/> that is the body
        /// of the lock block.
        /// </summary>
        public Expression Body { get; }

        /// <summary>
        /// Gets the <see cref="Expression"/> that is the object
        /// used to lock the execution of a method.
        /// </summary>
        public Expression Object { get; }

        internal LockExpression(Expression obj, Expression body)
        {
            Object = obj;
            Body = body;
        }

        /// <inheritdoc />
        public override Expression Reduce()
        {
            ParameterExpression lockObject = Variable(typeof(object), "lockObject");

            return Block(new[] { lockObject },
                Assign(lockObject, Object),
                TryFinally(
                    Block(
                        Call(typeof(Monitor), nameof(Monitor.Enter), null, lockObject),
                        Body
                    ), 
                    Call(typeof(Monitor), nameof(Monitor.Exit), null, lockObject)
                )
            );
        }

        /// <inheritdoc cref="UnaryExpression.Update(Expression)" select="summary"/>
        public LockExpression Update(Expression obj, Expression body)
        {
            if (body == Body && obj == Object)
                return this;

            return Expressive.Lock(obj, body);
        }

        /// <inheritdoc />
        protected override Expression VisitChildren(ExpressionVisitor visitor)
            => Update(visitor.Visit(Object), visitor.Visit(Body));

        /// <inheritdoc />
        public override string ToString() => $"lock ({Object}) {{ ... }}";
    }

    partial class Expressive
    {
        /// <summary>
        /// Creates a <see cref="LockExpression"/> that represents a
        /// <see langword="lock"/> statement.
        /// </summary>
        public static LockExpression Lock(Expression obj, Expression body)
        {
            Requires.NotNull(obj, nameof(obj));
            Requires.NotNull(body, nameof(body));

            if (obj.Type.GetTypeInfo().IsValueType)
                throw new ArgumentException("The given argument must be a reference type.", nameof(obj));

            return new LockExpression(obj, body);
        }
    }
}
