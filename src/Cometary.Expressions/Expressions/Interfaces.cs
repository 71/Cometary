using System.Linq.Expressions;

namespace Cometary.Expressions
{
    internal interface IConditionalExpression
    {
        /// <inheritdoc cref="ConditionalExpression.Test" />
        Expression Test { get; }
    }

    internal interface IBodiedExpression
    {
        /// <summary>
        /// Gets the <see cref="Expression"/> that is the
        /// inner body of this expression.
        /// </summary>
        Expression Body { get; }
    }

    internal interface ILoopExpression
    {
        /// <inheritdoc cref="LoopExpression.Body"/>
        Expression Body { get; }

        /// <inheritdoc cref="LoopExpression.BreakLabel"/>
        LabelTarget BreakLabel { get; }

        /// <inheritdoc cref="LoopExpression.ContinueLabel"/>
        LabelTarget ContinueLabel { get; }
    }
}
