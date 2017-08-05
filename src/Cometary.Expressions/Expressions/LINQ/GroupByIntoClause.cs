using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents a LINQ <see langword="group"/> .. <see langword="by"/>
    /// .. <see langword="into"/> clause.
    /// </summary>
	public sealed class GroupByIntoClause : QueryClause
    {
        /// <inheritdoc />
        public override Type Type => Variable.Type;

        /// <inheritdoc />
        public override QueryClauseType ClauseType => QueryClauseType.GroupByInto;

        /// <summary>
        /// Gets the <see cref="ParameterExpression"/> from which
        /// the group will be created.
        /// </summary>
        public ParameterExpression Variable { get; }

        /// <summary>
        /// Gets the <see cref="ParameterExpression"/> in which
        /// the group will be stored.
        /// </summary>
        public ParameterExpression Group { get; }

        /// <summary>
        /// Gets the <see cref="Expression"/> that will
        /// return the element to compare.
        /// </summary>
        public Expression Selector { get; }

        internal GroupByIntoClause(ParameterExpression variable, Expression selector, ParameterExpression group)
        {
            Variable = variable;
            Selector = selector;
            Group = group;
        }

        /// <inheritdoc cref="UnaryExpression.Update(Expression)" select="summary"/>
        public GroupByIntoClause Update(ParameterExpression variable, Expression selector, ParameterExpression group)
        {
            if (Variable == variable && Selector == selector && Group == group)
                return this;

            return Expressive.GroupBy(variable, selector, group);
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
            => Update(visitor.VisitAndConvert(Variable, nameof(VisitChildren)), visitor.Visit(Selector), visitor.VisitAndConvert(Group, nameof(VisitChildren)));
    }

    partial class Expressive
    {
        /// <summary>
        /// Creates a <see cref="GroupByIntoClause"/> that represents a
        /// LINQ <see langword="group"/> .. <see langword="by"/> .. <see langword="into"/> clause.
        /// </summary>
		public static GroupByIntoClause GroupBy(ParameterExpression variable, Expression selector, ParameterExpression group)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (variable == null)
                throw new ArgumentNullException(nameof(variable));

            if (!selector.IsAssignableTo<IEnumerable>())
                throw Error.ArgumentMustBeAssignableTo<IEnumerable>(nameof(selector));

            if (group == null)
                group = Variable(typeof(IGrouping<,>).MakeGenericType(variable.Type, selector.Type));

            return new GroupByIntoClause(variable, selector, group);
        }
    }
}
