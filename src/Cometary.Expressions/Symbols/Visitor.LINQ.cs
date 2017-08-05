using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cometary.Expressions
{
    partial class ExpressionParsingVisitor
    {
        private QueryClause VisitContinuationClause(QueryClauseSyntax clause)
        {
            switch (clause)
            {
                case FromClauseSyntax from:
                    return Expressive.From(null, from.Expression.Accept(this));
                case LetClauseSyntax let:
                    return Expressive.Let(null, let.Expression.Accept(this));
                case JoinClauseSyntax join:
                    return Expressive.Join(null, join.InExpression.Accept(this), join.LeftExpression.Accept(this), join.RightExpression.Accept(this));
                case WhereClauseSyntax where:
                    return Expressive.Where(where.Condition.Accept(this));
                case OrderByClauseSyntax orderby:
                    return Expressive.OrderBy(Utils.Select(@orderby.Orderings, x => Expressive.Ordering(x.Expression.Accept(this), x.AscendingOrDescendingKeyword.Text == "ascending")));

                default:
                    throw new ArgumentException("Invalid clause.");
            }
        }

        private QueryClause VisitTerminationClause(SelectOrGroupClauseSyntax node)
        {
            if (node is SelectClauseSyntax select)
                return Expressive.Select(Visit(select.Expression));
            if (node is GroupClauseSyntax group)
                return Expressive.GroupBy(null, Visit(group.GroupExpression));

            throw new ArgumentException();
        }

        private IEnumerable<QueryClause> VisitLinqBody(QueryBodySyntax node)
        {
            foreach (var clause in node.Clauses)
                yield return VisitContinuationClause(clause);

            foreach (var clause in VisitLinqBody(node.Continuation.Body))
                yield return clause;

            yield return VisitTerminationClause(node.SelectOrGroup);
        }

        /// <inheritdoc />
        public override Expression VisitQueryExpression(QueryExpressionSyntax node)
        {
            return Expressive.Query(
                Enumerable.Repeat(VisitContinuationClause(node.FromClause), 1)
                          .Concat(VisitLinqBody(node.Body))
            );
        }

        #region Unexpected
        /// <inheritdoc />
        public override Expression VisitQueryBody(QueryBodySyntax node)
        {
            throw Unexpected(node, nameof(VisitQueryExpression));
        }

        /// <inheritdoc />
        public override Expression VisitFromClause(FromClauseSyntax node)
        {
            throw Unexpected(node, nameof(VisitQueryExpression));
        }

        /// <inheritdoc />
        public override Expression VisitLetClause(LetClauseSyntax node)
        {
            throw Unexpected(node, nameof(VisitQueryExpression));
        }

        /// <inheritdoc />
        public override Expression VisitJoinClause(JoinClauseSyntax node)
        {
            throw Unexpected(node, nameof(VisitQueryExpression));
        }

        /// <inheritdoc />
        public override Expression VisitJoinIntoClause(JoinIntoClauseSyntax node)
        {
            throw Unexpected(node, nameof(VisitQueryExpression));
        }

        /// <inheritdoc />
        public override Expression VisitWhereClause(WhereClauseSyntax node)
        {
            throw Unexpected(node, nameof(VisitQueryExpression));
        }

        /// <inheritdoc />
        public override Expression VisitOrderByClause(OrderByClauseSyntax node)
        {
            throw Unexpected(node, nameof(VisitQueryExpression));
        }

        /// <inheritdoc />
        public override Expression VisitOrdering(OrderingSyntax node)
        {
            throw Unexpected(node, nameof(VisitQueryExpression));
        }

        /// <inheritdoc />
        public override Expression VisitSelectClause(SelectClauseSyntax node)
        {
            throw Unexpected(node, nameof(VisitQueryExpression));
        }

        /// <inheritdoc />
        public override Expression VisitGroupClause(GroupClauseSyntax node)
        {
            throw Unexpected(node, nameof(VisitQueryExpression));
        }

        /// <inheritdoc />
        public override Expression VisitQueryContinuation(QueryContinuationSyntax node)
        {
            throw Unexpected(node, nameof(VisitQueryExpression));
        }
        #endregion
    }
}
