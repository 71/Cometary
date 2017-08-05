using System;
using System.Collections;
using System.Linq.Expressions;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents a LINQ <see langword="group"/> .. <see langword="by"/>
    /// termination clause.
    /// </summary>
	public sealed class GroupByClause : QueryClause
    {
        /// <inheritdoc />
        public override Type Type => Variable.Type;

        /// <inheritdoc />
        public override QueryClauseType ClauseType => QueryClauseType.GroupBy;

        /// <summary>
        /// Gets the <see cref="ParameterExpression"/> from which
        /// the group will be created.
        /// </summary>
        public ParameterExpression Variable { get; }

        /// <summary>
        /// Gets the <see cref="Expression"/> that will
        /// return the element to compare.
        /// </summary>
        public Expression Selector { get; }

        internal GroupByClause(ParameterExpression variable, Expression selector)
        {
            Variable = variable;
            Selector = selector;
        }

        /// <inheritdoc cref="UnaryExpression.Update(Expression)" select="summary"/>
        public GroupByClause Update(ParameterExpression variable, Expression selector)
        {
            if (Variable == variable && Selector == selector)
                return this;

            return Expressive.GroupBy(variable, selector);
        }

        /// <inheritdoc />
        protected override Expression Reduce(QueryClause previous, QueryClause next)
        {
            return Expressive.ForEach(
                Variable,
                Selector,
                next.Reduce()
            );
        }

        /// <inheritdoc />
        protected internal override QueryClause VisitChildren(ExpressionVisitor visitor)
            => Update(visitor.VisitAndConvert(Variable, nameof(VisitChildren)), visitor.Visit(Selector));

        /// <inheritdoc />
        public override string ToString() => $"group {Variable} by {Selector}";
    }

    partial class Expressive
    {
        /// <summary>
        /// Creates a <see cref="GroupByClause"/> that represents a
        /// LINQ <see langword="group"/> .. <see langword="by"/> termination clause.
        /// </summary>
		public static GroupByClause GroupBy(ParameterExpression variable, Expression selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (!selector.IsAssignableTo<IEnumerable>())
                throw Error.ArgumentMustBeAssignableTo<IEnumerable>(nameof(selector));

            if (variable == null)
                variable = Variable(Utils.GetItemType(selector));

            return new GroupByClause(variable, selector);
        }
    }
}
