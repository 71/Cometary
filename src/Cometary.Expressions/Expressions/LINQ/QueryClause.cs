using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Cometary.Expressions
{
    /// <summary>
    /// Describes the clause of a <see cref="QueryClause"/>.
    /// </summary>
    public enum QueryClauseType : byte
    {
        From        = 0,

        Let         = 1,
        Where       = 2,
        Join        = 3,
        JoinInto    = 4,
        OrderBy     = 5,
        GroupByInto = 6,

        Select      = 11,
        GroupBy     = 12
    }

    /// <summary>
    /// Represents a LINQ query expression.
    /// </summary>
	public abstract class QueryClause
    {
        private int index;
        private QueryExpression query;

        /// <summary>
        /// Gets the <see cref="QueryExpression"/> to which this clause
        /// belongs.
        /// </summary>
        protected QueryExpression Query => query;

        /// <summary>
        /// Gets the <see cref="QueryClauseType"/> of this clause.
        /// </summary>
        public abstract QueryClauseType ClauseType { get; }

        /// <summary>
        /// Gets the type returned by the clause.
        /// </summary>
        public abstract Type Type { get; }

        /// <summary>
        /// Reduces this <see cref="QueryClause"/> into an <see cref="Expression"/>,
        /// given the clauses that preceed and follow it.
        /// </summary>
        protected abstract Expression Reduce(QueryClause previous, QueryClause next);

        /// <inheritdoc cref="Reduce(QueryClause, QueryClause)"/>
        protected internal Expression Reduce()
        {
            return Reduce(
                index == 0 ? null : query.Clauses[index - 1].Clone(query, index - 1),
                index == query.Clauses.Length - 1 ? null : query.Clauses[index + 1].Clone(query, index + 1)
            );
        }

        /// <inheritdoc cref="Expression.VisitChildren(ExpressionVisitor)" />
        protected internal abstract QueryClause VisitChildren(ExpressionVisitor visitor);

        /// <summary>
        /// Registers a variable at the top level of the <see cref="QueryExpression"/>,
        /// removing the need to use a <see cref="BlockExpression"/> initialized with
        /// the given variable.
        /// </summary>
        protected void RegisterVariable(ParameterExpression variable)
        {
            Query.RegisterVariable(variable);
        }

        internal QueryClause Clone(QueryExpression query, int index)
        {
            QueryClause clone = this.MemberwiseClone() as QueryClause;

            clone.query = query;
            clone.index = index;

            return clone;
        }

        internal static Type EnumerableType(Type type) => typeof(IEnumerable<>).MakeGenericType(type);
        internal static Type QueryableType(Type type) => typeof(IQueryable<>).MakeGenericType(type);
    }
}
