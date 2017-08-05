using System;
using System.Linq.Expressions;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents a <see langword="yield"/> expression, typically found in
    /// an <see cref="IteratorExpression"/>.
    /// </summary>
    public sealed class YieldExpression : Expression
    {
        /// <inheritdoc />
        public override bool CanReduce => false;

        /// <inheritdoc />
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <inheritdoc />
        public override Type Type => Expression.Type;

        /// <summary>
        /// Gets the <see cref="System.Linq.Expressions.Expression"/> that is the value to return.
        /// </summary>
        public Expression Expression { get; }

        internal YieldExpression(Expression value)
        {
            Expression = value;
        }

        /// <inheritdoc cref="UnaryExpression.Update(System.Linq.Expressions.Expression)" select="summary"/>
        public YieldExpression Update(Expression value)
        {
            if (Expression == value)
                return this;

            return Expressive.Yield(value);
        }

        /// <inheritdoc />
        protected override Expression VisitChildren(ExpressionVisitor visitor)
            => Update(visitor.Visit(Expression));

        /// <inheritdoc />
        public override string ToString() => $"yield {Expression}";
    }

    partial class Expressive
    {
        /// <summary>
        /// Creates a <see cref="YieldExpression"/> that represents
        /// a <see langword="yield"/> expression.
        /// </summary>
        public static YieldExpression Yield(Expression value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return new YieldExpression(value);
        }
    }
}
