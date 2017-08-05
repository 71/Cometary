using System;
using System.Linq.Expressions;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents a LINQ <see langword="select"/> termination clause.
    /// </summary>
	public sealed class SelectClause : QueryClause
    {
        /// <inheritdoc />
        public override Type Type => EnumerableType(Selector.Type);

        /// <inheritdoc />
        public override QueryClauseType ClauseType => QueryClauseType.Select;

        /// <summary>
        /// Gets the <see cref="Expression"/> that will
        /// select the <see cref="object"/> to return.
        /// </summary>
        public Expression Selector { get; }

        internal SelectClause(Expression expression)
        {
            Selector = expression;
        }

        /// <inheritdoc cref="UnaryExpression.Update(Expression)" select="summary"/>
        public SelectClause Update(Expression expression)
        {
            if (Selector == expression)
                return this;

            return Expressive.Select(expression);
        }

        /// <inheritdoc />
        protected override Expression Reduce(QueryClause previous, QueryClause next)
        {
            return Expressive.Yield(Selector);
        }

        /// <inheritdoc />
        protected internal override QueryClause VisitChildren(ExpressionVisitor visitor)
            => Update(visitor.Visit(Selector));

        /// <inheritdoc />
        public override string ToString() => $"select {Selector}";
    }

    partial class Expressive
    {
        /// <summary>
        /// Creates a <see cref="SelectClause"/> that represents a
        /// LINQ <see langword="select"/> termination clause.
        /// </summary>
		public static SelectClause Select(Expression selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return new SelectClause(selector);
        }
    }
}
