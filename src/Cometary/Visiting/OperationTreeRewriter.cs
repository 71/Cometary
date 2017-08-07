using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Semantics;

namespace Cometary.Visiting
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class OperationTreeRewriter : OperationVisitor<object, IOperation>
    {
        private readonly CompilationEditor editor;
        private readonly Func<IOperation, IOperation> callback;

        private ImmutableArray<T> VisitList<T>(ImmutableArray<T> array, object argument = null) where T : IOperation
        {
            var newArray = array.ToBuilder();

            for (int i = 0; i < array.Length; i++)
            {
                var item = array[i];
                var newItem = item.Accept(this, argument);

                if (newItem is T t)
                {
                    newArray[i] = t;
                }
                else
                {
                    newArray.RemoveAt(i--);
                }
            }

            newArray.Capacity = newArray.Count;

            return newArray.MoveToImmutable();
        }

        public override IOperation Visit(IOperation operation, object argument) => callback(operation)?.Accept(this, argument);

        public override IOperation DefaultVisit(IOperation operation, object argument) => null;

        /// <inheritdoc />
        public override IOperation VisitBlockStatement(IBlockStatement operation, object argument)
        {
            return operation.Update(operation.Locals.As<ISymbol>(), operation.Locals.As<ISymbol>(), VisitList(operation.Statements));
        }

        /// <inheritdoc />
        public override IOperation VisitVariableDeclarationStatement(IVariableDeclarationStatement operation, object argument)
        {
            return operation.Update(VisitList(operation.Variables).As<IOperation>());
        }

        /// <inheritdoc />
        public override IOperation VisitVariableDeclaration(IVariableDeclaration operation, object argument)
        {
            return operation;
        }

        /// <inheritdoc />
        public override IOperation VisitSwitchStatement(ISwitchStatement operation, object argument)
        {
            return operation.Update()
        }

        /// <inheritdoc />
        public override IOperation VisitSwitchCase(ISwitchCase operation, object argument)
        {
            return base.VisitSwitchCase(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitSingleValueCaseClause(ISingleValueCaseClause operation, object argument)
        {
            return base.VisitSingleValueCaseClause(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitRelationalCaseClause(IRelationalCaseClause operation, object argument)
        {
            return base.VisitRelationalCaseClause(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitRangeCaseClause(IRangeCaseClause operation, object argument)
        {
            return base.VisitRangeCaseClause(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitIfStatement(IIfStatement operation, object argument)
        {
            return base.VisitIfStatement(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitWhileUntilLoopStatement(IWhileUntilLoopStatement operation, object argument)
        {
            return base.VisitWhileUntilLoopStatement(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitForLoopStatement(IForLoopStatement operation, object argument)
        {
            return base.VisitForLoopStatement(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitForEachLoopStatement(IForEachLoopStatement operation, object argument)
        {
            return base.VisitForEachLoopStatement(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitLabelStatement(ILabelStatement operation, object argument)
        {
            return base.VisitLabelStatement(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitBranchStatement(IBranchStatement operation, object argument)
        {
            return base.VisitBranchStatement(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitYieldBreakStatement(IReturnStatement operation, object argument)
        {
            return base.VisitYieldBreakStatement(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitEmptyStatement(IEmptyStatement operation, object argument)
        {
            return base.VisitEmptyStatement(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitThrowStatement(IThrowStatement operation, object argument)
        {
            return base.VisitThrowStatement(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitReturnStatement(IReturnStatement operation, object argument)
        {
            return base.VisitReturnStatement(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitLockStatement(ILockStatement operation, object argument)
        {
            return base.VisitLockStatement(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitTryStatement(ITryStatement operation, object argument)
        {
            return base.VisitTryStatement(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitCatch(ICatchClause operation, object argument)
        {
            return base.VisitCatch(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitUsingStatement(IUsingStatement operation, object argument)
        {
            return base.VisitUsingStatement(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitFixedStatement(IFixedStatement operation, object argument)
        {
            return base.VisitFixedStatement(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitExpressionStatement(IExpressionStatement operation, object argument)
        {
            return base.VisitExpressionStatement(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitWithStatement(IWithStatement operation, object argument)
        {
            return base.VisitWithStatement(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitStopStatement(IStopStatement operation, object argument)
        {
            return base.VisitStopStatement(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitEndStatement(IEndStatement operation, object argument)
        {
            return base.VisitEndStatement(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitInvocationExpression(IInvocationExpression operation, object argument)
        {
            return base.VisitInvocationExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitArgument(IArgument operation, object argument)
        {
            return base.VisitArgument(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitOmittedArgumentExpression(IOmittedArgumentExpression operation, object argument)
        {
            return base.VisitOmittedArgumentExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitArrayElementReferenceExpression(IArrayElementReferenceExpression operation, object argument)
        {
            return base.VisitArrayElementReferenceExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitPointerIndirectionReferenceExpression(IPointerIndirectionReferenceExpression operation, object argument)
        {
            return base.VisitPointerIndirectionReferenceExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitLocalReferenceExpression(ILocalReferenceExpression operation, object argument)
        {
            return base.VisitLocalReferenceExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitParameterReferenceExpression(IParameterReferenceExpression operation, object argument)
        {
            return base.VisitParameterReferenceExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitSyntheticLocalReferenceExpression(ISyntheticLocalReferenceExpression operation, object argument)
        {
            return base.VisitSyntheticLocalReferenceExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitInstanceReferenceExpression(IInstanceReferenceExpression operation, object argument)
        {
            return base.VisitInstanceReferenceExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitFieldReferenceExpression(IFieldReferenceExpression operation, object argument)
        {
            return base.VisitFieldReferenceExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitMethodBindingExpression(IMethodBindingExpression operation, object argument)
        {
            return base.VisitMethodBindingExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitPropertyReferenceExpression(IPropertyReferenceExpression operation, object argument)
        {
            return base.VisitPropertyReferenceExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitEventReferenceExpression(IEventReferenceExpression operation, object argument)
        {
            return base.VisitEventReferenceExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitEventAssignmentExpression(IEventAssignmentExpression operation, object argument)
        {
            return base.VisitEventAssignmentExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitConditionalAccessExpression(IConditionalAccessExpression operation, object argument)
        {
            return base.VisitConditionalAccessExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitConditionalAccessInstanceExpression(IConditionalAccessInstanceExpression operation, object argument)
        {
            return base.VisitConditionalAccessInstanceExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitPlaceholderExpression(IPlaceholderExpression operation, object argument)
        {
            return base.VisitPlaceholderExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitIndexedPropertyReferenceExpression(IIndexedPropertyReferenceExpression operation, object argument)
        {
            return base.VisitIndexedPropertyReferenceExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitUnaryOperatorExpression(IUnaryOperatorExpression operation, object argument)
        {
            return base.VisitUnaryOperatorExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitBinaryOperatorExpression(IBinaryOperatorExpression operation, object argument)
        {
            return base.VisitBinaryOperatorExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitConversionExpression(IConversionExpression operation, object argument)
        {
            return base.VisitConversionExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitConditionalChoiceExpression(IConditionalChoiceExpression operation, object argument)
        {
            return base.VisitConditionalChoiceExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitNullCoalescingExpression(INullCoalescingExpression operation, object argument)
        {
            return base.VisitNullCoalescingExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitIsTypeExpression(IIsTypeExpression operation, object argument)
        {
            return base.VisitIsTypeExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitSizeOfExpression(ISizeOfExpression operation, object argument)
        {
            return base.VisitSizeOfExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitTypeOfExpression(ITypeOfExpression operation, object argument)
        {
            return base.VisitTypeOfExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitLambdaExpression(ILambdaExpression operation, object argument)
        {
            return base.VisitLambdaExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitLiteralExpression(ILiteralExpression operation, object argument)
        {
            return base.VisitLiteralExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitAwaitExpression(IAwaitExpression operation, object argument)
        {
            return base.VisitAwaitExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitAddressOfExpression(IAddressOfExpression operation, object argument)
        {
            return base.VisitAddressOfExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitObjectCreationExpression(IObjectCreationExpression operation, object argument)
        {
            return base.VisitObjectCreationExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitFieldInitializer(IFieldInitializer operation, object argument)
        {
            return base.VisitFieldInitializer(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitPropertyInitializer(IPropertyInitializer operation, object argument)
        {
            return base.VisitPropertyInitializer(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitParameterInitializer(IParameterInitializer operation, object argument)
        {
            return base.VisitParameterInitializer(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitArrayCreationExpression(IArrayCreationExpression operation, object argument)
        {
            return base.VisitArrayCreationExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitArrayInitializer(IArrayInitializer operation, object argument)
        {
            return base.VisitArrayInitializer(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitAssignmentExpression(IAssignmentExpression operation, object argument)
        {
            return base.VisitAssignmentExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitCompoundAssignmentExpression(ICompoundAssignmentExpression operation, object argument)
        {
            return base.VisitCompoundAssignmentExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitIncrementExpression(IIncrementExpression operation, object argument)
        {
            return base.VisitIncrementExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitParenthesizedExpression(IParenthesizedExpression operation, object argument)
        {
            return base.VisitParenthesizedExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitLateBoundMemberReferenceExpression(ILateBoundMemberReferenceExpression operation, object argument)
        {
            return base.VisitLateBoundMemberReferenceExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitUnboundLambdaExpression(IUnboundLambdaExpression operation, object argument)
        {
            return base.VisitUnboundLambdaExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitDefaultValueExpression(IDefaultValueExpression operation, object argument)
        {
            return base.VisitDefaultValueExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitTypeParameterObjectCreationExpression(ITypeParameterObjectCreationExpression operation, object argument)
        {
            return base.VisitTypeParameterObjectCreationExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitInvalidStatement(IInvalidStatement operation, object argument)
        {
            return base.VisitInvalidStatement(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitInvalidExpression(IInvalidExpression operation, object argument)
        {
            return base.VisitInvalidExpression(operation, argument);
        }

        /// <inheritdoc />
        public override IOperation VisitLocalFunctionStatement(IOperation operation, object argument)
        {
            return base.VisitLocalFunctionStatement(operation, argument);
        }
    }
}
