using System;
using System.Collections;
using System.Linq.Expressions;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents a LINQ <see langword="join"/> .. <see langword="in"/>
    /// .. <see langword="on"/> .. <see langword="equals"/> clause.
    /// </summary>
	public sealed class JoinIntoClause : QueryClause
    {
        /// <inheritdoc />
        public override Type Type => Variable.Type;

        /// <inheritdoc />
        public override QueryClauseType ClauseType => QueryClauseType.JoinInto;

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

        /// <summary>
        /// Gets the <see cref="Expression"/> that is
        /// the object to compare to <see cref="Right"/> for equality.
        /// </summary>
        public Expression Left { get; }

        /// <summary>
        /// Gets the <see cref="Expression"/> that is
        /// the object to compare to <see cref="Left"/> for equality.
        /// </summary>
        public Expression Right { get; }

        /// <summary>
        /// Gets the <see cref="ParameterExpression"/> in which
        /// the group will be stored.
        /// </summary>
        public ParameterExpression Group { get; }

        internal JoinIntoClause(ParameterExpression variable, Expression enumerable, Expression left, Expression right, ParameterExpression group)
        {
            Variable = variable;
            Enumerable = enumerable;
            Left = left;
            Right = right;
            Group = group;
        }

        /// <inheritdoc cref="UnaryExpression.Update(Expression)" select="summary"/>
        public JoinIntoClause Update(ParameterExpression variable, Expression enumerable, Expression left, Expression right, ParameterExpression group)
        {
            if (Variable == variable && Enumerable == enumerable && Left == left && Right == right && Group == group)
                return this;

            return Expressive.Join(variable, enumerable, left, right, group);
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
            => Update(visitor.VisitAndConvert(Variable, nameof(VisitChildren)), visitor.Visit(Enumerable), visitor.Visit(Left), visitor.Visit(Right), visitor.VisitAndConvert(Group, nameof(VisitChildren)));
    }

    partial class Expressive
    {
        /// <summary>
        /// Creates a <see cref="JoinIntoClause"/> that represents a
        /// LINQ <see langword="join"/> .. <see langword="in"/>
        /// .. <see langword="on"/> .. <see langword="equals"/> .. <see langword="into"/> clause.
        /// </summary>
        public static JoinIntoClause Join(ParameterExpression variable, Expression enumerable, Expression left, Expression right, ParameterExpression group)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));

            if (!enumerable.IsAssignableTo<IEnumerable>())
                throw Error.ArgumentMustBeAssignableTo<IEnumerable>(nameof(enumerable));

            if (variable == null)
                variable = Variable(Utils.GetItemType(enumerable));

            return new JoinIntoClause(variable, enumerable, left, right, group);
        }
    }
}
