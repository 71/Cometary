using System;
using System.Collections;
using System.Linq.Expressions;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents a LINQ <see langword="from"/> .. <see langword="in"/> clause.
    /// </summary>
	public sealed class FromClause : QueryClause
    {
        /// <inheritdoc />
        public override Type Type => Enumerable.Type;

        /// <inheritdoc />
        public override QueryClauseType ClauseType => QueryClauseType.From;

        /// <summary>
        /// Gets the <see cref="ParameterExpression"/> in which
        /// the projected item will be stored.
        /// </summary>
        public ParameterExpression Variable { get; }

        /// <summary>
        /// Gets the <see cref="Expression"/> that is
        /// the enumerable to iterate over.
        /// </summary>
        public Expression Enumerable { get; }

        internal FromClause(ParameterExpression variable, Expression enumerable)
        {
            Variable = variable;
            Enumerable = enumerable;
        }

        /// <inheritdoc cref="UnaryExpression.Update(Expression)" select="summary"/>
        public FromClause Update(ParameterExpression variable, Expression enumerable)
        {
            if (Variable == variable && Enumerable == enumerable)
                return this;

            return Expressive.From(variable, enumerable);
        }

        /// <inheritdoc />
        protected override Expression Reduce(QueryClause previous, QueryClause next)
        {
            return Expressive.ForEach(
                Variable,
                Enumerable,
                next.Reduce()
            );
        }

        /// <inheritdoc />
        protected internal override QueryClause VisitChildren(ExpressionVisitor visitor)
            => Update(visitor.VisitAndConvert(Variable, nameof(VisitChildren)), visitor.Visit(Enumerable));

        /// <inheritdoc />
        public override string ToString() => $"from {Variable} in {Enumerable}";
    }

    partial class Expressive
    {
        /// <summary>
        /// Creates a <see cref="FromClause"/> that represents a
        /// LINQ <see langword="from"/> .. <see langword="in"/> clause.
        /// </summary>
		public static FromClause From(ParameterExpression variable, Expression enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            if (!enumerable.IsAssignableTo<IEnumerable>())
                throw Error.ArgumentMustBeAssignableTo<IEnumerable>(nameof(enumerable));

            if (variable == null)
                variable = Variable(Utils.GetItemType(enumerable));

            return new FromClause(variable, enumerable);
        }
    }
}
