using System.Reflection;

// At first a custom expression visitor for Meta.With() that created a new
// syntax tree was created, but it has been such a pain to debug
// I just gave up.
// Keep this here just in case.

namespace Cometary.Internal
{
#if false
    /// <summary>
    /// <see cref="ExpressionVisitor"/> that builds a <c>With*</c>
    /// method used on immutable data.
    /// </summary>
    internal sealed class MakeWithExpressionVisitor : ExpressionVisitor
    {
        [DebuggerStepThrough]
        private static MethodInfo GetWithMethod(Type exprType, MemberInfo member)
        {
            Type memberType;

            if (member is FieldInfo field)
                memberType = field.FieldType;
            else if (member is PropertyInfo property)
                memberType = property.PropertyType;
            else
                throw new NotSupportedException();

            return exprType.GetRuntimeMethod($"With{member.Name}", new[] { memberType });
        }

        [DebuggerStepThrough]
        private static MethodInfo GetReplaceMethod(Type exprType)
        {
            if (!exprType.IsConstructedGenericType)
                return null;

            return exprType.GetRuntimeMethod("Replace", new[] { exprType.GenericTypeArguments[0], exprType.GenericTypeArguments[0] });
        }

        private static MethodInfo createSyntaxList
            = typeof(SyntaxFactory).GetTypeInfo()
                                   .GetDeclaredMethods(nameof(SyntaxFactory.List))
                                   .First(x => x.GetParameters().Length == 1);

        private static MethodInfo createSeparatedSyntaxList
            = typeof(SyntaxFactory).GetTypeInfo()
                                   .GetDeclaredMethods(nameof(SyntaxFactory.SeparatedList))
                                   .First(x => x.GetParameters().Length == 1);

        private static TypeInfo ienumerableType
            = typeof(IEnumerable<>).GetTypeInfo();

        private static MethodInfo GetDeepReplaceMethod(Type exprType, ref Expression convertedExpression)
        {
            if (!exprType.IsConstructedGenericType)
                return null;
            if (!ienumerableType.IsAssignableFrom(exprType.GetGenericTypeDefinition().GetTypeInfo()))
                return null;

            Type itemType = exprType.GenericTypeArguments[0];

            convertedExpression = Expression.Call(createSyntaxList.MakeGenericMethod(itemType), convertedExpression);

            return exprType.GetRuntimeMethod("Replace", new[] { itemType, itemType });
        }

        private bool isFirstNode;
        private Expression built;

        internal MakeWithExpressionVisitor(object def, Type exprType)
        {
            built = Expression.Constant(def, exprType);
            isFirstNode = true;
        }

        /// <inheritdoc />
        public override Expression Visit(Expression node)
        {
            if (isFirstNode)
            {
                isFirstNode = false;

                base.Visit(node);

                return built;
            }

            if (node == null)
                return null;

            MethodInfo replaceMethod = GetReplaceMethod(node.Type);

            if (replaceMethod != null)
                built = Expression.Call(node, replaceMethod, node, built);

            return base.Visit(node);
        }

        /// <inheritdoc />
        protected override Expression VisitMember(MemberExpression node)
        {
            MethodInfo withMethod = GetWithMethod(node.Expression.Type, node.Member);

            if (withMethod != null)
                built = Expression.Call(node.Expression, withMethod, built);

            return base.VisitMember(node);
        }

        /// <inheritdoc />
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            Expression instance = node.Object;

            if (instance == null)
            {
                if (node.Arguments.Count == 0)
                    return base.VisitMethodCall(node);

                instance = node.Arguments[0];
            }

            MethodInfo replaceMethod;

            if ((replaceMethod = GetReplaceMethod(instance.Type)) != null
             || (replaceMethod = GetDeepReplaceMethod(instance.Type, ref instance)) != null)
                built = Expression.Call(instance, replaceMethod, node, built);

            return base.VisitMethodCall(node);
        }
    } 
#endif
}
