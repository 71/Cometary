using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Semantics;

namespace Cometary.Expressions
{
    partial class ExpressionParsingVisitor
    {
        /// <inheritdoc />
        public override Expression VisitInvocationExpression(IInvocationExpression operation, LocalBinder argument)
        {
            return Expression.Call(
                operation.Instance?.Accept(this, argument),
                operation.TargetMethod.GetCorrespondingMethod() as MethodInfo,
                operation.ArgumentsInParameterOrder.Select(x => x.Accept(this, argument))
            );
        }

        /// <inheritdoc />
        public override Expression VisitArgument(IArgument operation, LocalBinder argument)
        {
            return operation.Value.Accept(this, argument);
        }

        /// <inheritdoc />
        public override Expression VisitOmittedArgumentExpression(IOmittedArgumentExpression operation, LocalBinder argument)
        {
            return Expression.Constant(operation.ConstantValue.Value);
        }

        /// <inheritdoc />
        public override Expression VisitArrayElementReferenceExpression(IArrayElementReferenceExpression operation, LocalBinder argument)
        {
            return base.VisitArrayElementReferenceExpression(operation, argument);
        }

        /// <inheritdoc />
        public override Expression VisitPointerIndirectionReferenceExpression(IPointerIndirectionReferenceExpression operation, LocalBinder argument)
        {
            return base.VisitPointerIndirectionReferenceExpression(operation, argument);
        }

        /// <inheritdoc />
        public override Expression VisitLocalReferenceExpression(ILocalReferenceExpression operation, LocalBinder argument)
        {
            return base.VisitLocalReferenceExpression(operation, argument);
        }

        /// <inheritdoc />
        public override Expression VisitParameterReferenceExpression(IParameterReferenceExpression operation, LocalBinder argument)
        {
            return base.VisitParameterReferenceExpression(operation, argument);
        }

        /// <inheritdoc />
        public override Expression VisitSyntheticLocalReferenceExpression(ISyntheticLocalReferenceExpression operation, LocalBinder argument)
        {
            return base.VisitSyntheticLocalReferenceExpression(operation, argument);
        }

        /// <inheritdoc />
        public override Expression VisitInstanceReferenceExpression(IInstanceReferenceExpression operation, LocalBinder argument)
        {
            return base.VisitInstanceReferenceExpression(operation, argument);
        }

        /// <inheritdoc />
        public override Expression VisitFieldReferenceExpression(IFieldReferenceExpression operation, LocalBinder argument)
        {
            return base.VisitFieldReferenceExpression(operation, argument);
        }

        /// <inheritdoc />
        public override Expression VisitMethodBindingExpression(IMethodBindingExpression operation, LocalBinder argument)
        {
            return base.VisitMethodBindingExpression(operation, argument);
        }

        /// <inheritdoc />
        public override Expression VisitPropertyReferenceExpression(IPropertyReferenceExpression operation, LocalBinder argument)
        {
            return base.VisitPropertyReferenceExpression(operation, argument);
        }

        /// <inheritdoc />
        public override Expression VisitEventReferenceExpression(IEventReferenceExpression operation, LocalBinder argument)
        {
            return base.VisitEventReferenceExpression(operation, argument);
        }

        /// <inheritdoc />
        public override Expression VisitEventAssignmentExpression(IEventAssignmentExpression operation, LocalBinder argument)
        {
            return base.VisitEventAssignmentExpression(operation, argument);
        }

        /// <inheritdoc />
        public override Expression VisitConditionalAccessExpression(IConditionalAccessExpression operation, LocalBinder argument)
        {
            return base.VisitConditionalAccessExpression(operation, argument);
        }

        /// <inheritdoc />
        public override Expression VisitConditionalAccessInstanceExpression(IConditionalAccessInstanceExpression operation, LocalBinder argument)
        {
            return base.VisitConditionalAccessInstanceExpression(operation, argument);
        }

        /// <inheritdoc />
        public override Expression VisitPlaceholderExpression(IPlaceholderExpression operation, LocalBinder argument)
        {
            return base.VisitPlaceholderExpression(operation, argument);
        }

        /// <inheritdoc />
        public override Expression VisitIndexedPropertyReferenceExpression(IIndexedPropertyReferenceExpression operation, LocalBinder argument)
        {
            return base.VisitIndexedPropertyReferenceExpression(operation, argument);
        }

        /// <inheritdoc />
        public override Expression VisitUnaryOperatorExpression(IUnaryOperatorExpression operation, LocalBinder argument)
        {
            return base.VisitUnaryOperatorExpression(operation, argument);
        }

        /// <inheritdoc />
        public override Expression VisitBinaryOperatorExpression(IBinaryOperatorExpression operation, LocalBinder argument)
        {
            return base.VisitBinaryOperatorExpression(operation, argument);
        }

        /// <inheritdoc />
        public override Expression VisitConversionExpression(IConversionExpression operation, LocalBinder argument)
        {
            return Expression.Convert(
                operation.Operand.Accept(this, argument),
                operation.Type.GetCorrespondingType(),
                operation.OperatorMethod?.GetCorrespondingMethod() as MethodInfo
            );
        }

        /// <inheritdoc />
        public override Expression VisitConditionalChoiceExpression(IConditionalChoiceExpression operation, LocalBinder argument)
        {
            return Expression.Condition(
                operation.Condition.Accept(this, argument),
                operation.IfTrueValue.Accept(this, argument),
                operation.IfFalseValue.Accept(this, argument)
            );
        }

        /// <inheritdoc />
        public override Expression VisitNullCoalescingExpression(INullCoalescingExpression operation, LocalBinder argument)
        {
            return Expression.Coalesce(
                operation.PrimaryOperand.Accept(this, argument),
                operation.SecondaryOperand.Accept(this, argument)
            );
        }

        /// <inheritdoc />
        public override Expression VisitIsTypeExpression(IIsTypeExpression operation, LocalBinder argument)
        {
            return Expression.TypeIs(operation.Operand.Accept(this, argument), operation.IsType.GetCorrespondingType());
        }

        /// <inheritdoc />
        public override Expression VisitSizeOfExpression(ISizeOfExpression operation, LocalBinder argument)
        {
            return Expression.Constant(operation.ConstantValue.Value);
        }

        /// <inheritdoc />
        public override Expression VisitTypeOfExpression(ITypeOfExpression operation, LocalBinder argument)
        {
            return Expressive.TypeOf(operation.TypeOperand.GetCorrespondingType());
        }

        /// <inheritdoc />
        public override Expression VisitAwaitExpression(IAwaitExpression operation, LocalBinder argument)
        {
            return Expressive.Await(operation.AwaitedValue.Accept(this, argument));
        }

        /// <inheritdoc />
        public override Expression VisitAddressOfExpression(IAddressOfExpression operation, LocalBinder argument)
        {
            throw NotSupported(operation);
        }

        /// <inheritdoc />
        public override Expression VisitObjectCreationExpression(IObjectCreationExpression operation, LocalBinder argument)
        {
            return base.VisitObjectCreationExpression(operation, argument);
        }

        /// <inheritdoc />
        public override Expression VisitFieldInitializer(IFieldInitializer operation, LocalBinder argument)
        {
            return base.VisitFieldInitializer(operation, argument);
        }

        /// <inheritdoc />
        public override Expression VisitPropertyInitializer(IPropertyInitializer operation, LocalBinder argument)
        {
            return base.VisitPropertyInitializer(operation, argument);
        }

        /// <inheritdoc />
        public override Expression VisitParameterInitializer(IParameterInitializer operation, LocalBinder argument)
        {
            return base.VisitParameterInitializer(operation, argument);
        }

        /// <inheritdoc />
        public override Expression VisitArrayCreationExpression(IArrayCreationExpression operation, LocalBinder argument)
        {
            return base.VisitArrayCreationExpression(operation, argument);
        }

        /// <inheritdoc />
        public override Expression VisitArrayInitializer(IArrayInitializer operation, LocalBinder argument)
        {
            return base.VisitArrayInitializer(operation, argument);
        }

        /// <inheritdoc />
        public override Expression VisitAssignmentExpression(IAssignmentExpression operation, LocalBinder argument)
        {
            return base.VisitAssignmentExpression(operation, argument);
        }

        /// <inheritdoc />
        public override Expression VisitCompoundAssignmentExpression(ICompoundAssignmentExpression operation, LocalBinder argument)
        {
            return base.VisitCompoundAssignmentExpression(operation, argument);
        }

        /// <inheritdoc />
        public override Expression VisitIncrementExpression(IIncrementExpression operation, LocalBinder argument)
        {
            return Expression.Increment(operation.Value.Accept(this, argument));
        }

        /// <inheritdoc />
        public override Expression VisitParenthesizedExpression(IParenthesizedExpression operation, LocalBinder argument)
        {
            return operation.Operand.Accept(this, argument);
        }

        /// <inheritdoc />
        public override Expression VisitLateBoundMemberReferenceExpression(ILateBoundMemberReferenceExpression operation, LocalBinder argument)
        {
            return Expression.MakeMemberAccess(
                operation.Instance.Accept(this, argument),
                operation.Instance.Type.GetCorrespondingType().GetMember(operation.MemberName)
            );
        }

        /// <inheritdoc />
        public override Expression VisitDefaultValueExpression(IDefaultValueExpression operation, LocalBinder argument)
        {
            return Expression.Default(operation.Type.GetCorrespondingType());
        }

        /// <inheritdoc />
        public override Expression VisitTypeParameterObjectCreationExpression(ITypeParameterObjectCreationExpression operation, LocalBinder argument)
        {
            return Expression.New(operation.Type.GetCorrespondingType());
        }

        #region Unary
        private static ExpressionType GetUnaryExpressionType(SyntaxKind kind, bool isLeft)
        {
            switch (kind)
            {
                case SyntaxKind.MinusToken:
                    return ExpressionType.Negate;
                case SyntaxKind.PlusToken:
                    return ExpressionType.UnaryPlus;
                case SyntaxKind.PlusPlusToken:
                    return isLeft ? ExpressionType.PreIncrementAssign : ExpressionType.PostIncrementAssign;
                case SyntaxKind.MinusMinusToken:
                    return isLeft ? ExpressionType.PreDecrementAssign : ExpressionType.PostDecrementAssign;
                case SyntaxKind.TildeToken:
                    return ExpressionType.OnesComplement;
                case SyntaxKind.ExclamationToken:
                    return ExpressionType.Not;

                default:
                    return ExpressionType.Extension;
            }
        }

        /// <inheritdoc />
        public override Expression VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            return Expression.MakeUnary(GetUnaryExpressionType(node.OperatorToken.Kind(), false), node.Operand.Accept(this), null);
        }

        /// <inheritdoc />
        public override Expression VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            return Expression.MakeUnary(GetUnaryExpressionType(node.OperatorToken.Kind(), true), node.Operand.Accept(this), null);
        }
        #endregion

        #region Binary
        private static ExpressionType GetBinaryExpressionType(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                    return ExpressionType.Add;
                case SyntaxKind.PlusEqualsToken:
                    return ExpressionType.AddAssign;
                case SyntaxKind.MinusToken:
                    return ExpressionType.Subtract;
                case SyntaxKind.MinusEqualsToken:
                    return ExpressionType.SubtractAssign;
                case SyntaxKind.AsteriskToken:
                    return ExpressionType.Multiply;
                case SyntaxKind.AsteriskEqualsToken:
                    return ExpressionType.MultiplyAssign;
                case SyntaxKind.SlashToken:
                    return ExpressionType.Divide;
                case SyntaxKind.SlashEqualsToken:
                    return ExpressionType.DivideAssign;
                case SyntaxKind.PercentToken:
                    return ExpressionType.Modulo;
                case SyntaxKind.PercentEqualsToken:
                    return ExpressionType.ModuloAssign;
                case SyntaxKind.AmpersandToken:
                    return ExpressionType.And;
                case SyntaxKind.AmpersandAmpersandToken:
                    return ExpressionType.AndAlso;
                case SyntaxKind.AmpersandEqualsToken:
                    return ExpressionType.AndAssign;
                case SyntaxKind.EqualsEqualsToken:
                    return ExpressionType.Equal;
                case SyntaxKind.ExclamationEqualsToken:
                    return ExpressionType.NotEqual;

                default:
                    return ExpressionType.Extension;
            }
        }

        /// <inheritdoc />
        public override Expression VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            return Expression.MakeBinary(GetBinaryExpressionType(node.OperatorToken.Kind()), node.Left.Accept(this), node.Right.Accept(this));
        }
        #endregion

        #region Ternary
        /// <inheritdoc />
        public override Expression VisitConditionalExpression(ConditionalExpressionSyntax node)
        {
            return Expression.Condition(node.Condition.Accept(this), node.WhenTrue.Accept(this), node.WhenFalse.Accept(this));
        }
        #endregion

        /// <inheritdoc />
        public override Expression VisitThrowExpression(ThrowExpressionSyntax node)
        {
            return Expression.Throw(node.Expression.Accept(this));
        }

        /// <inheritdoc />
        public override Expression VisitDefaultExpression(DefaultExpressionSyntax node)
        {
            return Expression.Default(AsType(node.Type));
        }

        /// <inheritdoc />
        public override Expression VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            return Expression.Constant(node.Token.Value);
        }

        /// <inheritdoc />
        public override Expression VisitTypeOfExpression(TypeOfExpressionSyntax node)
        {
            return Expressive.TypeOf(AsType(node.Type));
        }

        /// <inheritdoc />
        public override Expression VisitCastExpression(CastExpressionSyntax node)
        {
            return Expression.Convert(node.Expression.Accept(this), AsType(node.Type));
        }

        /// <inheritdoc />
        public override Expression VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
        {
            return node.Expression.Accept(this);
        }

        /// <inheritdoc />
        public override Expression VisitTupleExpression(TupleExpressionSyntax node)
        {
            return Expressive.Tuple(
                node.Arguments.Select(x => (ParameterExpression)x.Expression.Accept(this))
            );
        }

        /// <inheritdoc />
        public override Expression VisitAwaitExpression(AwaitExpressionSyntax node)
        {
            return Expressive.Await(node.Expression.Accept(this));
        }

        /// <inheritdoc />
        public override Expression VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            Expression expr = node.Expression.Accept(this);
            MemberInfo member = expr.Type.GetTypeInfo().GetMember(node.Name.Identifier.Text);

            return Expression.MakeMemberAccess(expr is TypeExpression ? null : expr, member);
        }

        /// <inheritdoc />
        public override Expression VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
        {
            return Expression.Coalesce(node.Expression.Accept(this), node.WhenNotNull.Accept(this));
        }

        /// <inheritdoc />
        public override Expression VisitMemberBindingExpression(MemberBindingExpressionSyntax node)
        {
            return base.VisitMemberBindingExpression(node);
        }

        /// <inheritdoc />
        public override Expression VisitElementBindingExpression(ElementBindingExpressionSyntax node)
        {
            return base.VisitElementBindingExpression(node);
        }

        /// <inheritdoc />
        public override Expression VisitImplicitElementAccess(ImplicitElementAccessSyntax node)
        {
            return base.VisitImplicitElementAccess(node);
        }

        /// <inheritdoc />
        public override Expression VisitDeclarationExpression(DeclarationExpressionSyntax node)
        {
            return base.VisitDeclarationExpression(node);
        }

        /// <inheritdoc />
        public override Expression VisitRefExpression(RefExpressionSyntax node)
        {
            return node.Expression.Accept(this);
        }

        /// <inheritdoc />
        public override Expression VisitInitializerExpression(InitializerExpressionSyntax node)
        {
            return base.VisitInitializerExpression(node);
        }

        /// <inheritdoc />
        public override Expression VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            return Expression.New(
                AsType(node.Type).GetTypeInfo().DeclaredConstructors.First(x => x.GetParameters().Length == node.ArgumentList.Arguments.Count),
                node.ArgumentList.Arguments.Select(x => Visit(x.Expression))
            );
        }

        #region Creation
        /// <inheritdoc />
        public override Expression VisitAnonymousObjectMemberDeclarator(AnonymousObjectMemberDeclaratorSyntax node)
        {
            return base.VisitAnonymousObjectMemberDeclarator(node);
        }

        /// <inheritdoc />
        public override Expression VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node)
        {
            return base.VisitAnonymousObjectCreationExpression(node);
        }

        /// <inheritdoc />
        public override Expression VisitArrayCreationExpression(ArrayCreationExpressionSyntax node)
        {
            return base.VisitArrayCreationExpression(node);
        }

        /// <inheritdoc />
        public override Expression VisitImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax node)
        {
            return base.VisitImplicitArrayCreationExpression(node);
        }

        /// <inheritdoc />
        public override Expression VisitStackAllocArrayCreationExpression(StackAllocArrayCreationExpressionSyntax node)
        {
            return base.VisitStackAllocArrayCreationExpression(node);
        }

        /// <inheritdoc />
        public override Expression VisitOmittedArraySizeExpression(OmittedArraySizeExpressionSyntax node)
        {
            return base.VisitOmittedArraySizeExpression(node);
        }
        #endregion

        #region InterpolatedString
        /// <inheritdoc />
    public override Expression VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
    {
        StringBuilder template = new StringBuilder();
        Sequence<Expression> parameters = new Sequence<Expression>();

        foreach (var content in node.Contents)
        {
            if (content is InterpolatedStringTextSyntax text)
            {
                template.Append(text.TextToken.ValueText);
            }
            else if (content is InterpolationSyntax interpolation)
            {
                template.Append('{');
                template.Append(parameters.Count);
                template.Append('}');

                parameters.Add(interpolation.Expression.Accept(this));
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        return Expression.Call(
            typeof(string).GetRuntimeMethod(nameof(string.Format), new[] { typeof(string), typeof(object[]) }),
            Expression.Constant(template.ToString()),
            Expression.NewArrayInit(typeof(object), parameters.ToArray())
        );
    }

    /// <inheritdoc />
    public override Expression VisitInterpolatedStringText(InterpolatedStringTextSyntax node)
    {
        throw Unexpected(node, nameof(VisitInterpolatedStringExpression));
    }

    /// <inheritdoc />
    public override Expression VisitInterpolation(InterpolationSyntax node)
    {
        throw Unexpected(node, nameof(VisitInterpolatedStringExpression));
    }

    /// <inheritdoc />
    public override Expression VisitInterpolationAlignmentClause(InterpolationAlignmentClauseSyntax node)
    {
        throw Unexpected(node, nameof(VisitInterpolatedStringExpression));
    }

    /// <inheritdoc />
    public override Expression VisitInterpolationFormatClause(InterpolationFormatClauseSyntax node)
    {
        throw Unexpected(node, nameof(VisitInterpolatedStringExpression));
    }
    #endregion

        #region Pattern
        /// <inheritdoc />
        public override Expression VisitIsPatternExpression(IsPatternExpressionSyntax node)
        {
            if (node.Pattern is DeclarationPatternSyntax decl)
            {
                return Expression.TypeIs(node.Expression.Accept(this), AsType(decl.Type));
            }

            if (node.Pattern is ConstantPatternSyntax cnst)
            {
                //return Expression.TypeIs(node.Expression.Accept(this), AsType(cnst.Expression));
            }

            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override Expression VisitDeclarationPattern(DeclarationPatternSyntax node)
        {
            throw Unexpected(node, nameof(VisitIsPatternExpression));
        }

        /// <inheritdoc />
        public override Expression VisitConstantPattern(ConstantPatternSyntax node)
        {
            throw Unexpected(node, nameof(VisitIsPatternExpression));
        }
        #endregion

        /// <inheritdoc />
        public override Expression VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            return Expression.Assign(node.Left.Accept(this), node.Right.Accept(this));
        }

        /// <inheritdoc />
        public override Expression VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax access)
            {
                var syntaxArgs = node.ArgumentList.Arguments;
                Expression[] arguments = new Expression[syntaxArgs.Count];

                Expression expr = access.Expression.Accept(this);

                GenericNameSyntax generic = access.Name as GenericNameSyntax;
                bool isGeneric = generic != null;
                Type[] genericTypes = null;
                string name = access.Name.Identifier.Text;
                bool isStatic = expr is TypeExpression;

                bool FindGenericMethod(MethodInfo match)
                {
                    if (genericTypes == null)
                        genericTypes = generic.TypeArgumentList.Arguments.Select(AsType);

                    return match.IsGenericMethod
                           && match.IsStatic == isStatic
                           && match.GetParameters().Length == arguments.Length
                           && match.GetGenericArguments().Length == genericTypes.Length;
                }

                bool FindMethod(MethodInfo match)
                {
                    return match.IsStatic == isStatic
                        && !match.IsGenericMethod
                        && match.GetParameters().Length == arguments.Length;
                }

                MethodInfo method = isGeneric
                    ? expr.Type.GetTypeInfo()
                          .GetMethods(name).First(FindGenericMethod)
                    : expr.Type.GetTypeInfo()
                          .GetMethods(name).First(FindMethod);

                if (isGeneric)
                    method = method.MakeGenericMethod(genericTypes);

                ParameterInfo[] parameters = method.GetParameters();

                for (int i = 0; i < arguments.Length; i++)
                {
                    arguments[i] = Convert(syntaxArgs[i].Accept(this), parameters[i].ParameterType);
                }

                return Expression.Call(expr is TypeExpression ? null : expr, method, arguments);
            }

            return node.Expression.Accept(this);
        }

        /// <inheritdoc />
        public override Expression VisitElementAccessExpression(ElementAccessExpressionSyntax node)
        {
            return base.VisitElementAccessExpression(node);
        }

        #region Not supported
        /// <inheritdoc />
        public override Expression VisitCheckedExpression(CheckedExpressionSyntax node)
        {
            throw NotSupported(node);
        }

        /// <inheritdoc />
        public override Expression VisitSizeOfExpression(SizeOfExpressionSyntax node)
        {
            throw NotSupported(node);
        }

        /// <inheritdoc />
        public override Expression VisitMakeRefExpression(MakeRefExpressionSyntax node)
        {
            throw NotSupported(node);
        }

        /// <inheritdoc />
        public override Expression VisitRefTypeExpression(RefTypeExpressionSyntax node)
        {
            throw NotSupported(node);
        }

        /// <inheritdoc />
        public override Expression VisitRefValueExpression(RefValueExpressionSyntax node)
        {
            throw NotSupported(node);
        }
        #endregion
    }
}
