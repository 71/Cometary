
using System;
using System.Linq.Expressions;

namespace Cometary.Expressions
{
    
    /// <summary>
    /// <see cref="ExpressionVisitor{T}"/> whose necessary methods to override
    /// are <see langword="abstract"/> instead of returning
    /// <see cref="ExpressionVisitor{T}.DefaultVisit(Expression)"/>.
    /// </summary>
    public abstract class AbstractExpressionVisitor<T> : ExpressionVisitor<T>
    {
        
        /// <inheritdoc />
        protected abstract override T VisitConditional(ConditionalExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitSwitch(SwitchExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitLoop(LoopExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitDefault(DefaultExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitConstant(ConstantExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitDebugInfo(DebugInfoExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitGoto(GotoExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitIndex(IndexExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitLabel(LabelExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitLambda(LambdaExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitNew(NewExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitParameter(ParameterExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitRuntimeVariables(RuntimeVariablesExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitNewArrayBounds(NewArrayExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitNewArrayInit(NewArrayExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitTypeIs(TypeBinaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitTypeEqual(TypeBinaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitMemberAccess(MemberExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitCall(MethodCallExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitInvoke(InvocationExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitArrayLength(UnaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitConvert(UnaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitNegate(UnaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitNot(UnaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitQuote(UnaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitTypeAs(UnaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitUnaryPlus(UnaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitThrow(UnaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitIsTrue(UnaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitIsFalse(UnaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitIncrement(UnaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitDecrement(UnaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitOnesComplement(UnaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitUnbox(UnaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitAdd(BinaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitSubtract(BinaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitMultiply(BinaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitDivide(BinaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitModulo(BinaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitAnd(BinaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitOr(BinaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitExclusiveOr(BinaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitLeftShift(BinaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitRightShift(BinaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitAndAlso(BinaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitOrElse(BinaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitEqual(BinaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitNotEqual(BinaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitGreaterThan(BinaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitGreaterThanOrEqual(BinaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitLessThan(BinaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitLessThanOrEqual(BinaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitArrayIndex(BinaryExpression node);
        
        /// <inheritdoc />
        protected abstract override T VisitAssign(BinaryExpression node);
        
    }
}
