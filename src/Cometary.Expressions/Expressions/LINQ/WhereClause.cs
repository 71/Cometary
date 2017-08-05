using System;
using System.Linq.Expressions;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents a LINQ <see langword="where"/> clause.
    /// </summary>
	public sealed class WhereClause : QueryClause
    {
        /// <inheritdoc />
        public override Type Type => typeof(bool);

        /// <inheritdoc />
        public override QueryClauseType ClauseType => QueryClauseType.Where;

        /// <summary>
        /// Gets the <see cref="Expression"/> that will
        /// make the query successful or not.
        /// </summary>
        public Expression Condition { get; }

        internal WhereClause(Expression condition)
        {
            Condition = condition;
        }

        /// <inheritdoc cref="UnaryExpression.Update(Expression)" select="summary"/>
        public WhereClause Update(Expression condition)
        {
            if (Condition == condition)
                return this;

            return Expressive.Where(condition);
        }

        /// <inheritdoc />
        protected override Expression Reduce(QueryClause previous, QueryClause next)
        {
            return Expression.IfThen(Condition, next.Reduce());
        }

        /// <inheritdoc />
        protected internal override QueryClause VisitChildren(ExpressionVisitor visitor)
            => Update(visitor.Visit(Condition));

        /// <inheritdoc />
        public override string ToString() => $"where {Condition}";
    }

    partial class Expressive
    {
        /// <summary>
        /// Creates a <see cref="WhereClause"/> that represents a
        /// LINQ <see langword="where"/> clause.
        /// </summary>
		public static WhereClause Where(Expression condition)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));
            if (condition.Type != typeof(bool))
                throw Error.ArgumentMustBeBoolean(nameof(condition));

            return new WhereClause(condition);
        }
    }
}
