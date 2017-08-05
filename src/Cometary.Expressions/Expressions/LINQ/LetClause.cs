using System;
using System.Collections;
using System.Linq.Expressions;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents a LINQ <see langword="let"/> clause.
    /// </summary>
	public sealed class LetClause : QueryClause
    {
        /// <inheritdoc />
        public override Type Type => Variable.Type;

        /// <inheritdoc />
        public override QueryClauseType ClauseType => QueryClauseType.Let;

        /// <summary>
        /// Gets the <see cref="ParameterExpression"/> in which
        /// the created variable will be stored.
        /// </summary>
        public ParameterExpression Variable { get; }

        /// <summary>
        /// Gets the <see cref="System.Linq.Expressions.Expression"/> that will
        /// give the created variable its value.
        /// </summary>
        public Expression Expression { get; }

        internal LetClause(ParameterExpression variable, Expression expression)
        {
            Variable = variable;
            Expression = expression;
        }

        /// <inheritdoc cref="UnaryExpression.Update(Expression)" select="summary"/>
        public LetClause Update(ParameterExpression variable, Expression expression)
        {
            if (Variable == variable && Expression == expression)
                return this;

            return Expressive.Let(variable, expression);
        }

        /// <inheritdoc />
        protected override Expression Reduce(QueryClause previous, QueryClause next)
        {
            return Expression.Block(new[] { Variable },
                Expression.Assign(Variable, Expression),
                next.Reduce()
            );
        }

        /// <inheritdoc />
        protected internal override QueryClause VisitChildren(ExpressionVisitor visitor)
            => Update(visitor.VisitAndConvert(Variable, nameof(VisitChildren)), visitor.Visit(Expression));

        /// <inheritdoc />
        public override string ToString() => $"let {Variable} = {Expression}";
    }

    partial class Expressive
    {
        /// <summary>
        /// Creates a <see cref="LetClause"/> that represents a
        /// LINQ <see langword="let"/> clause.
        /// </summary>
		public static LetClause Let(ParameterExpression variable, Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            if (variable == null)
                variable = Variable(expression.Type);

            return new LetClause(variable, expression);
        }
    }
}
