
using System;
using System.Linq.Expressions;

namespace Cometary.Expressions
{
    
    partial class ExpressionVisitor<T>
    {
        // Generated methods: 83 / 83

        #region PrivateVisit(Expression)
        private T PrivateVisit(Expression node)
        {
            if (node.NodeType == ExpressionType.Extension)
                return VisitExtension(node);

            switch (node)
            {
                            
                case BlockExpression exprBlock:
                
                    return VisitBlock(exprBlock);
                                            
                case ConditionalExpression exprConditional:
                
                    return VisitConditional(exprConditional);
                                            
                case SwitchExpression exprSwitch:
                
                    return VisitSwitch(exprSwitch);
                                            
                case LoopExpression exprLoop:
                
                    return VisitLoop(exprLoop);
                                            
                case TryExpression exprTry:
                
                    return VisitTry(exprTry);
                                            
                case DefaultExpression exprDefault:
                
                    return VisitDefault(exprDefault);
                                            
                case ConstantExpression exprConstant:
                
                    return VisitConstant(exprConstant);
                                            
                case DebugInfoExpression exprDebugInfo:
                
                    return VisitDebugInfo(exprDebugInfo);
                                            
                case GotoExpression exprGoto:
                
                    return VisitGoto(exprGoto);
                                            
                case IndexExpression exprIndex:
                
                    return VisitIndex(exprIndex);
                                            
                case LabelExpression exprLabel:
                
                    return VisitLabel(exprLabel);
                                            
                case LambdaExpression exprLambda:
                
                    return VisitLambda(exprLambda);
                                            
                case ListInitExpression exprListInit:
                
                    return VisitListInit(exprListInit);
                                            
                case NewExpression exprNew:
                
                    return VisitNew(exprNew);
                                            
                case MemberInitExpression exprMemberInit:
                
                    return VisitMemberInit(exprMemberInit);
                                            
                case ParameterExpression exprParameter:
                
                    return VisitParameter(exprParameter);
                                            
                case RuntimeVariablesExpression exprRuntimeVariables:
                
                    return VisitRuntimeVariables(exprRuntimeVariables);
                                            
                case NewArrayExpression exprNewArray:
                
                    switch (node.NodeType)
                    {
                    
                        case ExpressionType.NewArrayBounds:
                            return VisitNewArrayBounds(exprNewArray);
                    
                        case ExpressionType.NewArrayInit:
                            return VisitNewArrayInit(exprNewArray);
                                            
                        default:
                            throw new NotSupportedException();
                    }
                                            
                case TypeBinaryExpression exprTypeBinary:
                
                    switch (node.NodeType)
                    {
                    
                        case ExpressionType.TypeIs:
                            return VisitTypeIs(exprTypeBinary);
                    
                        case ExpressionType.TypeEqual:
                            return VisitTypeEqual(exprTypeBinary);
                                            
                        default:
                            throw new NotSupportedException();
                    }
                                            
                case MemberExpression exprMember:
                
                    switch (node.NodeType)
                    {
                    
                        case ExpressionType.MemberAccess:
                            return VisitMemberAccess(exprMember);
                                            
                        default:
                            throw new NotSupportedException();
                    }
                                            
                case MethodCallExpression exprMethodCall:
                
                    switch (node.NodeType)
                    {
                    
                        case ExpressionType.Call:
                            return VisitCall(exprMethodCall);
                                            
                        default:
                            throw new NotSupportedException();
                    }
                                            
                case InvocationExpression exprInvocation:
                
                    switch (node.NodeType)
                    {
                    
                        case ExpressionType.Invoke:
                            return VisitInvoke(exprInvocation);
                                            
                        default:
                            throw new NotSupportedException();
                    }
                                            
                case UnaryExpression exprUnary:
                
                    switch (node.NodeType)
                    {
                    
                        case ExpressionType.ArrayLength:
                            return VisitArrayLength(exprUnary);
                    
                        case ExpressionType.Convert:
                            return VisitConvert(exprUnary);
                    
                        case ExpressionType.ConvertChecked:
                            return VisitConvertChecked(exprUnary);
                    
                        case ExpressionType.Negate:
                            return VisitNegate(exprUnary);
                    
                        case ExpressionType.NegateChecked:
                            return VisitNegateChecked(exprUnary);
                    
                        case ExpressionType.Not:
                            return VisitNot(exprUnary);
                    
                        case ExpressionType.Quote:
                            return VisitQuote(exprUnary);
                    
                        case ExpressionType.TypeAs:
                            return VisitTypeAs(exprUnary);
                    
                        case ExpressionType.UnaryPlus:
                            return VisitUnaryPlus(exprUnary);
                    
                        case ExpressionType.Throw:
                            return VisitThrow(exprUnary);
                    
                        case ExpressionType.IsTrue:
                            return VisitIsTrue(exprUnary);
                    
                        case ExpressionType.IsFalse:
                            return VisitIsFalse(exprUnary);
                    
                        case ExpressionType.Increment:
                            return VisitIncrement(exprUnary);
                    
                        case ExpressionType.Decrement:
                            return VisitDecrement(exprUnary);
                    
                        case ExpressionType.OnesComplement:
                            return VisitOnesComplement(exprUnary);
                    
                        case ExpressionType.Unbox:
                            return VisitUnbox(exprUnary);
                    
                        case ExpressionType.PreIncrementAssign:
                            return VisitPreIncrementAssign(exprUnary);
                    
                        case ExpressionType.PreDecrementAssign:
                            return VisitPreDecrementAssign(exprUnary);
                    
                        case ExpressionType.PostIncrementAssign:
                            return VisitPostIncrementAssign(exprUnary);
                    
                        case ExpressionType.PostDecrementAssign:
                            return VisitPostDecrementAssign(exprUnary);
                                            
                        default:
                            throw new NotSupportedException();
                    }
                                            
                case BinaryExpression exprBinary:
                
                    switch (node.NodeType)
                    {
                    
                        case ExpressionType.Add:
                            return VisitAdd(exprBinary);
                    
                        case ExpressionType.AddChecked:
                            return VisitAddChecked(exprBinary);
                    
                        case ExpressionType.Subtract:
                            return VisitSubtract(exprBinary);
                    
                        case ExpressionType.SubtractChecked:
                            return VisitSubtractChecked(exprBinary);
                    
                        case ExpressionType.Multiply:
                            return VisitMultiply(exprBinary);
                    
                        case ExpressionType.MultiplyChecked:
                            return VisitMultiplyChecked(exprBinary);
                    
                        case ExpressionType.Divide:
                            return VisitDivide(exprBinary);
                    
                        case ExpressionType.Power:
                            return VisitPower(exprBinary);
                    
                        case ExpressionType.Modulo:
                            return VisitModulo(exprBinary);
                    
                        case ExpressionType.And:
                            return VisitAnd(exprBinary);
                    
                        case ExpressionType.Or:
                            return VisitOr(exprBinary);
                    
                        case ExpressionType.ExclusiveOr:
                            return VisitExclusiveOr(exprBinary);
                    
                        case ExpressionType.LeftShift:
                            return VisitLeftShift(exprBinary);
                    
                        case ExpressionType.RightShift:
                            return VisitRightShift(exprBinary);
                    
                        case ExpressionType.AndAlso:
                            return VisitAndAlso(exprBinary);
                    
                        case ExpressionType.OrElse:
                            return VisitOrElse(exprBinary);
                    
                        case ExpressionType.Equal:
                            return VisitEqual(exprBinary);
                    
                        case ExpressionType.NotEqual:
                            return VisitNotEqual(exprBinary);
                    
                        case ExpressionType.GreaterThan:
                            return VisitGreaterThan(exprBinary);
                    
                        case ExpressionType.GreaterThanOrEqual:
                            return VisitGreaterThanOrEqual(exprBinary);
                    
                        case ExpressionType.LessThan:
                            return VisitLessThan(exprBinary);
                    
                        case ExpressionType.LessThanOrEqual:
                            return VisitLessThanOrEqual(exprBinary);
                    
                        case ExpressionType.Coalesce:
                            return VisitCoalesce(exprBinary);
                    
                        case ExpressionType.ArrayIndex:
                            return VisitArrayIndex(exprBinary);
                    
                        case ExpressionType.Assign:
                            return VisitAssign(exprBinary);
                    
                        case ExpressionType.AddAssign:
                            return VisitAddAssign(exprBinary);
                    
                        case ExpressionType.AddAssignChecked:
                            return VisitAddAssignChecked(exprBinary);
                    
                        case ExpressionType.SubtractAssign:
                            return VisitSubtractAssign(exprBinary);
                    
                        case ExpressionType.SubtractAssignChecked:
                            return VisitSubtractAssignChecked(exprBinary);
                    
                        case ExpressionType.MultiplyAssign:
                            return VisitMultiplyAssign(exprBinary);
                    
                        case ExpressionType.MultiplyAssignChecked:
                            return VisitMultiplyAssignChecked(exprBinary);
                    
                        case ExpressionType.DivideAssign:
                            return VisitDivideAssign(exprBinary);
                    
                        case ExpressionType.PowerAssign:
                            return VisitPowerAssign(exprBinary);
                    
                        case ExpressionType.ModuloAssign:
                            return VisitModuloAssign(exprBinary);
                    
                        case ExpressionType.AndAssign:
                            return VisitAndAssign(exprBinary);
                    
                        case ExpressionType.OrAssign:
                            return VisitOrAssign(exprBinary);
                    
                        case ExpressionType.ExclusiveOrAssign:
                            return VisitExclusiveOrAssign(exprBinary);
                    
                        case ExpressionType.LeftShiftAssign:
                            return VisitLeftShiftAssign(exprBinary);
                    
                        case ExpressionType.RightShiftAssign:
                            return VisitRightShiftAssign(exprBinary);
                                            
                        default:
                            throw new NotSupportedException();
                    }
                            
                default:
                    return dynamicVisit != null ? dynamicVisit(node) : DefaultVisit(node);
            }
        }
        #endregion

        
        /// <summary>
        /// Transforms a <see cref="ConditionalExpression"/> of type <see cref="ExpressionType.Conditional"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitConditional(ConditionalExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="SwitchExpression"/> of type <see cref="ExpressionType.Switch"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitSwitch(SwitchExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="LoopExpression"/> of type <see cref="ExpressionType.Loop"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitLoop(LoopExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="DefaultExpression"/> of type <see cref="ExpressionType.Default"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitDefault(DefaultExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="ConstantExpression"/> of type <see cref="ExpressionType.Constant"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitConstant(ConstantExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="DebugInfoExpression"/> of type <see cref="ExpressionType.DebugInfo"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitDebugInfo(DebugInfoExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="GotoExpression"/> of type <see cref="ExpressionType.Goto"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitGoto(GotoExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="IndexExpression"/> of type <see cref="ExpressionType.Index"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitIndex(IndexExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="LabelExpression"/> of type <see cref="ExpressionType.Label"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitLabel(LabelExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="LambdaExpression"/> of type <see cref="ExpressionType.Lambda"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitLambda(LambdaExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="ListInitExpression"/> of type <see cref="ExpressionType.ListInit"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitListInit(ListInitExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="NewExpression"/> of type <see cref="ExpressionType.New"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitNew(NewExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="MemberInitExpression"/> of type <see cref="ExpressionType.MemberInit"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitMemberInit(MemberInitExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="ParameterExpression"/> of type <see cref="ExpressionType.Parameter"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitParameter(ParameterExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="RuntimeVariablesExpression"/> of type <see cref="ExpressionType.RuntimeVariables"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitRuntimeVariables(RuntimeVariablesExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="NewArrayExpression"/> of type <see cref="ExpressionType.NewArrayBounds"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitNewArrayBounds(NewArrayExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="NewArrayExpression"/> of type <see cref="ExpressionType.NewArrayInit"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitNewArrayInit(NewArrayExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="TypeBinaryExpression"/> of type <see cref="ExpressionType.TypeIs"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitTypeIs(TypeBinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="TypeBinaryExpression"/> of type <see cref="ExpressionType.TypeEqual"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitTypeEqual(TypeBinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="MemberExpression"/> of type <see cref="ExpressionType.MemberAccess"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitMemberAccess(MemberExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="MethodCallExpression"/> of type <see cref="ExpressionType.Call"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitCall(MethodCallExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="InvocationExpression"/> of type <see cref="ExpressionType.Invoke"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitInvoke(InvocationExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="UnaryExpression"/> of type <see cref="ExpressionType.ArrayLength"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitArrayLength(UnaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="UnaryExpression"/> of type <see cref="ExpressionType.Convert"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitConvert(UnaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="UnaryExpression"/> of type <see cref="ExpressionType.ConvertChecked"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitConvertChecked(UnaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="UnaryExpression"/> of type <see cref="ExpressionType.Negate"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitNegate(UnaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="UnaryExpression"/> of type <see cref="ExpressionType.NegateChecked"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitNegateChecked(UnaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="UnaryExpression"/> of type <see cref="ExpressionType.Not"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitNot(UnaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="UnaryExpression"/> of type <see cref="ExpressionType.Quote"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitQuote(UnaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="UnaryExpression"/> of type <see cref="ExpressionType.TypeAs"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitTypeAs(UnaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="UnaryExpression"/> of type <see cref="ExpressionType.UnaryPlus"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitUnaryPlus(UnaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="UnaryExpression"/> of type <see cref="ExpressionType.Throw"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitThrow(UnaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="UnaryExpression"/> of type <see cref="ExpressionType.IsTrue"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitIsTrue(UnaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="UnaryExpression"/> of type <see cref="ExpressionType.IsFalse"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitIsFalse(UnaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="UnaryExpression"/> of type <see cref="ExpressionType.Increment"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitIncrement(UnaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="UnaryExpression"/> of type <see cref="ExpressionType.Decrement"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitDecrement(UnaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="UnaryExpression"/> of type <see cref="ExpressionType.OnesComplement"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitOnesComplement(UnaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="UnaryExpression"/> of type <see cref="ExpressionType.Unbox"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitUnbox(UnaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="UnaryExpression"/> of type <see cref="ExpressionType.PreIncrementAssign"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitPreIncrementAssign(UnaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="UnaryExpression"/> of type <see cref="ExpressionType.PreDecrementAssign"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitPreDecrementAssign(UnaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="UnaryExpression"/> of type <see cref="ExpressionType.PostIncrementAssign"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitPostIncrementAssign(UnaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="UnaryExpression"/> of type <see cref="ExpressionType.PostDecrementAssign"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitPostDecrementAssign(UnaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.Add"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitAdd(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.AddChecked"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitAddChecked(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.Subtract"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitSubtract(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.SubtractChecked"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitSubtractChecked(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.Multiply"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitMultiply(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.MultiplyChecked"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitMultiplyChecked(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.Divide"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitDivide(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.Power"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitPower(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.Modulo"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitModulo(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.And"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitAnd(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.Or"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitOr(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.ExclusiveOr"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitExclusiveOr(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.LeftShift"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitLeftShift(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.RightShift"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitRightShift(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.AndAlso"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitAndAlso(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.OrElse"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitOrElse(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.Equal"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitEqual(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.NotEqual"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitNotEqual(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.GreaterThan"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitGreaterThan(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.GreaterThanOrEqual"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitGreaterThanOrEqual(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.LessThan"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitLessThan(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.LessThanOrEqual"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitLessThanOrEqual(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.Coalesce"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitCoalesce(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.ArrayIndex"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitArrayIndex(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.Assign"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitAssign(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.AddAssign"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitAddAssign(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.AddAssignChecked"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitAddAssignChecked(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.SubtractAssign"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitSubtractAssign(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.SubtractAssignChecked"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitSubtractAssignChecked(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.MultiplyAssign"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitMultiplyAssign(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.MultiplyAssignChecked"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitMultiplyAssignChecked(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.DivideAssign"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitDivideAssign(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.PowerAssign"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitPowerAssign(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.ModuloAssign"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitModuloAssign(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.AndAssign"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitAndAssign(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.OrAssign"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitOrAssign(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.ExclusiveOrAssign"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitExclusiveOrAssign(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.LeftShiftAssign"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitLeftShiftAssign(BinaryExpression node) => DefaultVisit(node);
        
        /// <summary>
        /// Transforms a <see cref="BinaryExpression"/> of type <see cref="ExpressionType.RightShiftAssign"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitRightShiftAssign(BinaryExpression node) => DefaultVisit(node);
        
    }
}
