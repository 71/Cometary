using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents a LINQ query expression.
    /// </summary>
	public sealed class QueryExpression : Expression
    {
        /// <inheritdoc />
        public override bool CanReduce => true;

        /// <inheritdoc />
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <inheritdoc />
        public override Type Type => Clauses[Clauses.Length - 1].Type;

        /// <summary>
        /// Gets the clauses that make up this <see cref="QueryExpression"/>.
        /// </summary>
        public ImmutableArray<QueryClause> Clauses { get; }

        private readonly List<ParameterExpression> variables;

        internal QueryExpression(IEnumerable<QueryClause> clauses)
        {
            variables = new List<ParameterExpression>();
            Clauses = clauses.ToImmutableArray();

            Verify(Clauses);
        }

        /// <inheritdoc cref="UnaryExpression.Update(Expression)" select="summary"/>
        public QueryExpression Update(params QueryClause[] clauses)
        {
            if (Clauses.SequenceEqual(clauses))
                return this;

            return Expressive.Query(clauses);
        }

        /// <inheritdoc />
        public override Expression Reduce()
        {
            return Expressive.Enumerable(
                Block(variables, Clauses[0].Clone(this, 0).Reduce())
            );
        }

        internal void RegisterVariable(ParameterExpression variable)
        {
            if (!variables.Contains(variable))
                 variables.Add(variable);
        }

        internal static void Verify(ImmutableArray<QueryClause> clauses)
        {
            if (clauses.Length < 2)
                throw new ArgumentException("Invalid clause body.", nameof(clauses));
            if (clauses.Contains(null))
                throw new ArgumentNullException(nameof(clauses));

            // First clause must be in the "from .. in" form.
            if (clauses[0].ClauseType != QueryClauseType.From)
                throw new ArgumentException("A query expression must begin with a from .. in clause.");

            // Last clause must be in the "select .." or "group .." form.
            QueryClauseType lastClauseType = clauses[clauses.Length - 1].ClauseType;

            if (lastClauseType != QueryClauseType.Select && lastClauseType != QueryClauseType.GroupBy)
                throw new ArgumentException("A query expression must end with a termination clause.");

            // All other clauses cannot be termination clauses.
            for (int i = 1; i < clauses.Length - 1; i++)
            {
                switch (clauses[i].ClauseType)
                {
                    case QueryClauseType.Select:
                    case QueryClauseType.GroupBy:
                        throw new ArgumentException("A query expression cannot contain with a termination clause unless it is its last clause.");
                }
            }
        }

        /// <inheritdoc />
        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            QueryClause[] clauses = new QueryClause[Clauses.Length];

            for (int i = 0; i < clauses.Length; i++)
                clauses[i] = Clauses[i].VisitChildren(visitor);

            return Update(clauses);
        }

        /// <inheritdoc />
        public override string ToString() => string.Join(Environment.NewLine, Clauses.Select(x => (object)x.ToString()).ToArray());
    }

    partial class Expressive
    {
        /// <summary>
        /// Creates a <see cref="QueryExpression"/> that represents a
        /// LINQ query expression, given its inner clauses.
        /// </summary>
		public static QueryExpression Query(params QueryClause[] clauses)
        {
            Requires.NotNull(clauses, nameof(clauses));

            return new QueryExpression(clauses);
        }

        /// <summary>
        /// Creates a <see cref="QueryExpression"/> that represents a
        /// LINQ query expression, given its inner clauses.
        /// </summary>
		public static QueryExpression Query(IEnumerable<QueryClause> clauses)
        {
            Requires.NotNull(clauses, nameof(clauses));

            return new QueryExpression(clauses);
        }

        /// <summary>
        /// Creates a <see cref="QueryExpression"/> that represents a
        /// LINQ query expression, given its compiler-generated body.
        /// </summary>
		public static QueryExpression Query(Expression body)
        {
            Requires.NotNull(body, nameof(body));

            return new QueryExpressionTransformer(body).Query;
        }
    }
}
