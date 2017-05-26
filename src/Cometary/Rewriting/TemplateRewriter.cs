using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Cometary.Internal;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Semantics;

namespace Cometary.Rewriting
{
    /// <summary>
    /// <see cref="CSharpSyntaxRewriter"/> that transforms template calls.
    /// </summary>
    internal sealed class TemplateRewriter : CSharpSyntaxRewriter
    {
        /// <summary>
        /// Gets whether or not the given parameter is a quote.
        /// </summary>
        private static bool IsQuoteParameter(IParameterSymbol parameter)
        {
            return parameter.Type.Name == nameof(Quote) || parameter.Type.MetadataName == nameof(Quote) + "`1";
        }

        /// <summary>
        /// Ensures void quotes are correctly reduced.
        /// </summary>
        public override SyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            if (node.Expression is InvocationExpressionSyntax invocation)
            {
                SyntaxNode newInvocation = VisitInvocationExpression(invocation);

                return newInvocation != invocation && newInvocation is StatementSyntax
                    ? newInvocation
                    : node.WithExpression(newInvocation as ExpressionSyntax);
            }

            return base.VisitExpressionStatement(node);
        }

        /// <summary>
        /// Calls template methods.
        /// </summary>
        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            // Get symbol of invocation
            IInvocationExpression invocation = node.SyntaxTree.Model().GetOperation(node) as IInvocationExpression;

            if (invocation == null || invocation.TargetMethod.Parameters.IsDefaultOrEmpty)
                return node;

            // Verifies if target is a template method
            ImmutableArray<IParameterSymbol> parameters = invocation.TargetMethod.Parameters;

            foreach (IParameterSymbol parameter in parameters)
            {
                if (IsQuoteParameter(parameter))
                    goto Next;
            }

            return base.VisitInvocationExpression(node);

            // We got here, meaning we have a template method.
            Next:

            object[] arguments = new object[parameters.Length];
            Quote quote = new Quote(node, invocation);

            for (int i = 0; i < parameters.Length; i++)
            {
                // Find all parameters
                IParameterSymbol parameter = parameters[i];
                INamedTypeSymbol parameterType = parameter.Type as INamedTypeSymbol;

                if (parameterType == null || !IsQuoteParameter(parameter))
                    arguments[i] = parameter.HasExplicitDefaultValue
                        ? parameter.ExplicitDefaultValue
                        : invocation.ArgumentsInParameterOrder[i].Value.ConstantValue.HasValue
                            ? invocation.ArgumentsInParameterOrder[i].Value.ConstantValue.Value
                            : null;
                else if (parameterType.IsGenericType)
                    arguments[i] = quote.For(i, parameterType.TypeArguments[0].Info().AsType());
                else
                    arguments[i] = quote.For(i);
            }

            // Help find valid method
            IMethodSymbol target = invocation.TargetMethod;

            bool IsTargetMethod(MethodInfo method)
            {
                if (method.IsStatic != target.IsStatic
                    || method.IsGenericMethod != target.IsGenericMethod)
                    return false;

                ParameterInfo[] pis = method.GetParameters();

                if (pis.Length != parameters.Length)
                    return false;

                for (int i = 0; i < pis.Length; i++)
                {
                    if (pis[i].ParameterType.Name != parameters[i].Type.Name)
                        return false;
                }

                return true;
            }

            // Invoke method!
            MethodInfo targetMethod = target.ContainingType
                .Info()
                .GetDeclaredMethods(target.MetadataName)
                .FirstOrDefault(IsTargetMethod);

            if (targetMethod.IsGenericMethod)
                targetMethod = targetMethod.MakeGenericMethod(target.TypeArguments.Select(x => x.Info().AsType()).ToArray());

            targetMethod.Invoke(null, arguments);

            if (quote.Result != null)
            {
                Unquote unquote = quote.Result;

                if (!unquote.IsVoidQuote)
                    return Visit(unquote.Syntax);

                MethodDeclarationSyntax methodDecl = quote.MethodSyntax as MethodDeclarationSyntax;

                if (unquote.Syntax is ExpressionStatementSyntax expr)
                    // Making sure a semicolon is present, if an expression was returned without one.
                    return expr.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
                if (unquote.Syntax is StatementSyntax)
                    return unquote.Syntax;

                if (unquote.Syntax is ArrowExpressionClauseSyntax arrowExpr)
                    methodDecl.Replace(x => x.WithExpressionBody(arrowExpr));
                else if (unquote.Syntax is MethodDeclarationSyntax method)
                    methodDecl.Replace(x => method);
                else
                    throw new InvalidOperationException();

                return node.Accept(this);
            }

            return node.Accept(this);
        }
    }
}
