using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents an <see cref="System.Linq.Expressions.Expression"/> that can be modified.
    /// </summary>
    /// <typeparam name="TExpr">The type of the expression that will be returned.</typeparam>
    public sealed class MutableExpression<TExpr> : Expression where TExpr : Expression
    {
        private TExpr _expression;

        /// <inheritdoc />
        public override Type Type => Expression.Type;

        /// <inheritdoc />
        public override bool CanReduce => true;

        /// <inheritdoc />
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <summary>
        /// Gets or sets the <see cref="System.Linq.Expressions.Expression"/> that this
        /// <see cref="MutableExpression{TExpr}"/> will be reduced to.
        /// </summary>
        public TExpr Expression
        {
            get => _expression;
            set => _expression = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <inheritdoc />
        public override Expression Reduce() => Expression;

        /// <summary>
        /// Creates a new <see cref="MutableExpression{TExpr}"/>,
        /// given its inner expression.
        /// </summary>
        public MutableExpression(TExpr expression)
        {
            Expression = expression;
        }

        /// <inheritdoc />
        protected override Expression VisitChildren(ExpressionVisitor visitor)
            => new MutableExpression<TExpr>(visitor.VisitAndConvert(_expression, nameof(VisitChildren)));

        /// <summary>
        /// Implicitly converts an <see cref="System.Linq.Expressions.Expression"/> (an
        /// immutable object) to a <see cref="MutableExpression{TExpr}"/>.
        /// </summary>
        public static implicit operator MutableExpression<TExpr>(TExpr expression)
        {
            return new MutableExpression<TExpr>(expression);
        }

        /// <inheritdoc />
        public override string ToString() => Expression.ToString();
    }

    [DebuggerStepThrough, EditorBrowsable(EditorBrowsableState.Never)]
    public static class MutableExpression
    {
        /// <summary>
        /// Returns the given <see cref="Expression"/> as a mutable <see cref="Expression"/>.
        /// </summary>
        public static MutableExpression<TExpression> Mutable<TExpression>(this TExpression expression) where TExpression : Expression
        {
            return new MutableExpression<TExpression>(expression);
        }
    }
}
