using System;
using System.Reflection;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Semantics;
using Microsoft.CodeAnalysis.Text;

namespace Cometary
{
    public static partial class UpdateExtensions
    {
        public static IInvocationExpression Update(this IInvocationExpression self, IOperation @receiverOpt, IMethodSymbol @method, ImmutableArray<IOperation> @arguments) => self;
        public static IInvocationExpression Update(this IInvocationExpression self, IOperation @receiverOpt, IMethodSymbol @method, ImmutableArray<IOperation> @arguments, ImmutableArray<String> @argumentNamesOpt, ImmutableArray<RefKind> @argumentRefKindsOpt, Boolean @isDelegateCall, Boolean @expanded, Boolean @invokedAsExtensionMethod, ImmutableArray<Int32> @argsToParamsOpt, Enum @resultKind, Object @binderOpt, ITypeSymbol @type) => self;
        public static ILocalReferenceExpression Update(this ILocalReferenceExpression self, ILocalSymbol @localSymbol, Object @constantValueOpt, ITypeSymbol @type) => self;
        public static ILocalReferenceExpression Update(this ILocalReferenceExpression self, ILocalSymbol @localSymbol, Boolean @isDeclaration, Object @constantValueOpt, ITypeSymbol @type) => self;
        public static IFieldReferenceExpression Update(this IFieldReferenceExpression self, IOperation @receiver, IFieldSymbol @fieldSymbol, Object @constantValueOpt, Enum @resultKind, ITypeSymbol @typeSymbol) => self;
        public static IFieldReferenceExpression Update(this IFieldReferenceExpression self, IOperation @receiverOpt, IFieldSymbol @fieldSymbol, Object @constantValueOpt, Enum @resultKind, Boolean @isByValue, ITypeSymbol @type) => self;
        public static IPropertyReferenceExpression Update(this IPropertyReferenceExpression self, IOperation @receiverOpt, IPropertySymbol @propertySymbol, Enum @resultKind, ITypeSymbol @type) => self;
        public static IIndexedPropertyReferenceExpression Update(this IIndexedPropertyReferenceExpression self, IOperation @receiverOpt, IPropertySymbol @indexer, ImmutableArray<IOperation> @arguments, ImmutableArray<String> @argumentNamesOpt, ImmutableArray<RefKind> @argumentRefKindsOpt, Boolean @expanded, ImmutableArray<Int32> @argsToParamsOpt, Object @binderOpt, Boolean @useSetterForDefaultArgumentGeneration, ITypeSymbol @type) => self;
        public static IEventReferenceExpression Update(this IEventReferenceExpression self, IOperation @receiverOpt, IEventSymbol @eventSymbol, Boolean @isUsableAsField, Enum @resultKind, ITypeSymbol @type) => self;
        public static IParameterReferenceExpression Update(this IParameterReferenceExpression self, IParameterSymbol @parameterSymbol, ITypeSymbol @type) => self;
        public static IBinaryOperatorExpression Update(this IBinaryOperatorExpression self, Enum @operatorKind, IOperation @left, IOperation @right, Object @constantValueOpt, IMethodSymbol @methodOpt, Enum @resultKind, ITypeSymbol @type) => self;
        public static IUnaryOperatorExpression Update(this IUnaryOperatorExpression self, Enum @operatorKind, IOperation @operand, Object @constantValueOpt, IMethodSymbol @methodOpt, Enum @resultKind, ITypeSymbol @type) => self;
        public static IIncrementExpression Update(this IIncrementExpression self, Enum @operatorKind, IOperation @operand, IMethodSymbol @methodOpt, Conversion @operandConversion, Conversion @resultConversion, Enum @resultKind, ITypeSymbol @type) => self;
        public static ICompoundAssignmentExpression Update(this ICompoundAssignmentExpression self, ValueType @operator, IOperation @left, IOperation @right, Conversion @leftConversion, Conversion @finalConversion, Enum @resultKind, ITypeSymbol @type) => self;
        public static ILiteralExpression Update(this ILiteralExpression self, Object @constantValueOpt, ITypeSymbol @type) => self;
        public static IConversionExpression Update(this IConversionExpression self, IOperation @operand, Conversion @conversion, Boolean @isBaseConversion, Boolean @checked, Boolean @explicitCastInCode, Object @constantValueOpt, ITypeSymbol @type) => self;
        public static IObjectCreationExpression Update(this IObjectCreationExpression self, IMethodSymbol @constructor, ImmutableArray<IOperation> @arguments, ImmutableArray<String> @argumentNamesOpt, ImmutableArray<RefKind> @argumentRefKindsOpt, Boolean @expanded, ImmutableArray<Int32> @argsToParamsOpt, Object @constantValueOpt, IOperation @initializerExpressionOpt, Object @binderOpt, ITypeSymbol @type) => self;
        public static IObjectCreationExpression Update(this IObjectCreationExpression self, IMethodSymbol @constructor, ImmutableArray<ISymbol> @constructorsGroup, ImmutableArray<IOperation> @arguments, ImmutableArray<String> @argumentNamesOpt, ImmutableArray<RefKind> @argumentRefKindsOpt, Boolean @expanded, ImmutableArray<Int32> @argsToParamsOpt, Object @constantValueOpt, IOperation @initializerExpressionOpt, Object @binderOpt, ITypeSymbol @type) => self;
        public static ILambdaExpression Update(this ILambdaExpression self, IMethodSymbol @symbol, IBlockStatement @body, ImmutableArray<Diagnostic> @diagnostics, Object @binder, ITypeSymbol @type) => self;
        public static IDefaultValueExpression Update(this IDefaultValueExpression self, Object @constantValueOpt, ITypeSymbol @type) => self;
        public static IConditionalChoiceExpression Update(this IConditionalChoiceExpression self, Boolean @isByRef, IOperation @condition, IOperation @consequence, IOperation @alternative, Object @constantValueOpt, ITypeSymbol @type) => self;
        public static ISizeOfExpression Update(this ISizeOfExpression self, IOperation @sourceType, Object @constantValueOpt, ITypeSymbol @type) => self;
        public static IAwaitExpression Update(this IAwaitExpression self, IOperation @expression, IMethodSymbol @getAwaiter, IPropertySymbol @isCompleted, IMethodSymbol @getResult, ITypeSymbol @type) => self;
        public static IInstanceReferenceExpression Update(this IInstanceReferenceExpression self, ITypeSymbol @type) => self;
        public static IAssignmentExpression Update(this IAssignmentExpression self, IOperation @left, IOperation @right, RefKind @refKind, ITypeSymbol @type) => self;
        public static IInvalidExpression Update(this IInvalidExpression self, Enum @resultKind, ImmutableArray<ISymbol> @symbols, ImmutableArray<IOperation> @childBoundNodes, ITypeSymbol @type) => self;
        public static IReturnStatement Update(this IReturnStatement self, RefKind @refKind, IOperation @expressionOpt) => self;
        public static IBranchStatement Update(this IBranchStatement self, ILabelSymbol @label, IOperation @caseExpressionOpt, IOperation @labelExpressionOpt) => self;
        public static IBlockStatement Update(this IBlockStatement self, ImmutableArray<ISymbol> @locals, ImmutableArray<ISymbol> @localFunctions, ImmutableArray<IOperation> @statements) => self;
        public static ITryStatement Update(this ITryStatement self, IBlockStatement @tryBlock, ImmutableArray<IOperation> @catchBlocks, IBlockStatement @finallyBlockOpt, Boolean @preferFaultHandler) => self;
        public static IPlaceholderExpression Update(this IPlaceholderExpression self, ITypeSymbol @type) => self;
        public static IEventAssignmentExpression Update(this IEventAssignmentExpression self, IEventSymbol @event, Boolean @isAddition, Boolean @isDynamic, IOperation @receiverOpt, IOperation @argument, ITypeSymbol @type) => self;
        public static IConversionExpression Update(this IConversionExpression self, IOperation @operand, IOperation @targetType, Conversion @conversion, ITypeSymbol @type) => self;
        public static IIsTypeExpression Update(this IIsTypeExpression self, IOperation @operand, IOperation @targetType, Conversion @conversion, ITypeSymbol @type) => self;
        public static ITypeOfExpression Update(this ITypeOfExpression self, IOperation @sourceType, IMethodSymbol @getTypeFromHandle, ITypeSymbol @type) => self;
        public static IArrayCreationExpression Update(this IArrayCreationExpression self, ImmutableArray<IOperation> @bounds, IOperation @initializerOpt, ITypeSymbol @type) => self;
        public static IInstanceReferenceExpression Update(this IInstanceReferenceExpression self, ITypeSymbol @type) => self;
        public static ITypeParameterObjectCreationExpression Update(this ITypeParameterObjectCreationExpression self, IOperation @initializerExpressionOpt, ITypeSymbol @type) => self;
        public static INullCoalescingExpression Update(this INullCoalescingExpression self, IOperation @leftOperand, IOperation @rightOperand, Conversion @leftConversion, ITypeSymbol @type) => self;
        public static IArrayElementReferenceExpression Update(this IArrayElementReferenceExpression self, IOperation @expression, ImmutableArray<IOperation> @indices, ITypeSymbol @type) => self;
        public static IPointerIndirectionReferenceExpression Update(this IPointerIndirectionReferenceExpression self, IOperation @operand, ITypeSymbol @type) => self;
        public static IAddressOfExpression Update(this IAddressOfExpression self, IOperation @operand, Boolean @isFixedStatementAddressOf, ITypeSymbol @type) => self;
        public static IInstanceReferenceExpression Update(this IInstanceReferenceExpression self, ITypeSymbol @type) => self;
        public static IConditionalAccessExpression Update(this IConditionalAccessExpression self, IOperation @receiver, IOperation @accessExpression, ITypeSymbol @type) => self;
        public static IConditionalAccessInstanceExpression Update(this IConditionalAccessInstanceExpression self, Int32 @id, ITypeSymbol @type) => self;
        public static IBranchStatement Update(this IBranchStatement self, ILabelSymbol @label) => self;
        public static IBranchStatement Update(this IBranchStatement self, ILabelSymbol @label) => self;
        public static IEmptyStatement Update(this IEmptyStatement self, Enum @flavor) => self;
        public static IIfStatement Update(this IIfStatement self, IOperation @condition, IOperation @consequence, IOperation @alternativeOpt) => self;
        public static IWhileUntilLoopStatement Update(this IWhileUntilLoopStatement self, ImmutableArray<ISymbol> @locals, IOperation @condition, IOperation @body, ILabelSymbol @breakLabel, ILabelSymbol @continueLabel) => self;
        public static IWhileUntilLoopStatement Update(this IWhileUntilLoopStatement self, ImmutableArray<ISymbol> @locals, IOperation @condition, IOperation @body, ILabelSymbol @breakLabel, ILabelSymbol @continueLabel) => self;
        public static IForLoopStatement Update(this IForLoopStatement self, ImmutableArray<ISymbol> @outerLocals, IOperation @initializer, ImmutableArray<ISymbol> @innerLocals, IOperation @condition, IOperation @increment, IOperation @body, ILabelSymbol @breakLabel, ILabelSymbol @continueLabel) => self;
        public static IForEachLoopStatement Update(this IForEachLoopStatement self, Object @enumeratorInfoOpt, Conversion @elementConversion, IOperation @iterationVariableType, ImmutableArray<ISymbol> @iterationVariables, IOperation @expression, IOperation @deconstructionOpt, IOperation @body, Boolean @checked, ILabelSymbol @breakLabel, ILabelSymbol @continueLabel) => self;
        public static ISwitchStatement Update(this ISwitchStatement self, IOperation @loweredPreambleOpt, IOperation @expression, ILabelSymbol @constantTargetOpt, ImmutableArray<ISymbol> @innerLocals, ImmutableArray<ISymbol> @innerLocalFunctions, ImmutableArray<IOperation> @switchSections, ILabelSymbol @breakLabel, IMethodSymbol @stringEquality) => self;
        public static IFixedStatement Update(this IFixedStatement self, ImmutableArray<ISymbol> @locals, IVariableDeclarationStatement @declarations, IOperation @body) => self;
        public static IUsingStatement Update(this IUsingStatement self, ImmutableArray<ISymbol> @locals, IVariableDeclarationStatement @declarationsOpt, IOperation @expressionOpt, Conversion @iDisposableConversion, IOperation @body) => self;
        public static IThrowStatement Update(this IThrowStatement self, IOperation @expressionOpt) => self;
        public static IReturnStatement Update(this IReturnStatement self, IOperation @expression) => self;
        public static ILockStatement Update(this ILockStatement self, IOperation @argument, IOperation @body) => self;
        public static IInvalidStatement Update(this IInvalidStatement self, ImmutableArray<IOperation> @childBoundNodes) => self;
        public static IVariableDeclarationStatement Update(this IVariableDeclarationStatement self, ILocalSymbol @localSymbol, IOperation @declaredType, IOperation @initializerOpt, ImmutableArray<IOperation> @argumentsOpt) => self;
        public static IVariableDeclarationStatement Update(this IVariableDeclarationStatement self, ImmutableArray<IOperation> @localDeclarations) => self;
        public static ILabelStatement Update(this ILabelStatement self, ILabelSymbol @label) => self;
        public static ILabelStatement Update(this ILabelStatement self, ILabelSymbol @label, IOperation @body) => self;
        public static IExpressionStatement Update(this IExpressionStatement self, IOperation @expression) => self;
    }
}
