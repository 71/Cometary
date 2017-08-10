using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Semantics;

namespace Cometary.Macros
{
    /// <summary>
    ///   <see cref="CSharpSyntaxRewriter"/> that can expand macros.
    /// </summary>
    internal sealed class MacroExpander : CSharpSyntaxRewriter
    {
        private readonly SemanticModel semanticModel;
        private readonly CancellationToken cancellationToken;

        private readonly Dictionary<StatementSyntax, StatementSyntax> changes;

        internal MacroExpander(CSharpSyntaxTree syntaxTree, CSharpCompilation compilation, CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
            this.semanticModel = compilation.GetSemanticModel(syntaxTree, true);

            this.changes = new Dictionary<StatementSyntax, StatementSyntax>();
        }

        public override SyntaxNode Visit(SyntaxNode node)
        {
            if (node is StatementSyntax stmt)
            {
                // Allow a Visit*Expression method to return a statement.
                var result = stmt.Accept(this);

                if (changes.TryGetValue(stmt, out StatementSyntax newStmt))
                {
                    changes.Remove(stmt);
                    return newStmt;
                }

                return result;
            }

            return base.Visit(node);
        }

        public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            IOperation operation = semanticModel.GetOperation(node, cancellationToken);

            if (operation == null || !operation.IsInvalid)
                return base.VisitMemberAccessExpression(node);

            // Expression is invalid, might be a late-bound object
            IOperation expression = semanticModel.GetOperation(node.Expression, cancellationToken);

            if (expression.IsInvalid)
                return base.VisitMemberAccessExpression(node);

            // Find out if it is a late-bound object...
            INamedTypeSymbol type = expression.Type as INamedTypeSymbol;

            if (type == null)
                return base.VisitMemberAccessExpression(node);

            // ... by finding its Bind method
            object[] arguments = null;

            bool IsValidBindMethod(MethodInfo mi)
            {
                if (!mi.IsStatic || mi.IsAbstract || mi.Name != "Bind")
                    return false;

                if (!typeof(ExpressionSyntax).IsAssignableFrom(mi.ReturnType))
                    return false;

                ParameterInfo[] parameters = mi.GetParameters();
                object[] args = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    Type paramType = parameters[i].ParameterType;

                    if (paramType.IsAssignableFrom(typeof(MemberAccessExpressionSyntax)))
                        args[i] = node;
                    else if (paramType == typeof(IOperation))
                        args[i] = expression;
                    else
                        return false;
                }

                arguments = args;
                return true;
            }

            Type correspondingType = type.GetCorrespondingType();

            if (correspondingType == null)
                return base.VisitMemberAccessExpression(node);

            MethodInfo bindMethod = correspondingType
                .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(IsValidBindMethod);

            if (bindMethod == null)
                return base.VisitMemberAccessExpression(node);

            // We do have a binder!
            // Call the method
            try
            {
                ExpressionSyntax result = bindMethod.Invoke(null, arguments) as ExpressionSyntax;

                return result == null
                    ? SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)
                    : base.Visit(result);
            }
            catch (Exception e)
            {
                throw new DiagnosticException("Error thrown by binding method.", e, node.GetLocation());
            }
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            IInvocationExpression invocation = semanticModel.GetOperation(node, cancellationToken) as IInvocationExpression;

            if (invocation == null)
                return base.VisitInvocationExpression(node);

            bool IsMacroMethod(ISymbol methodSymbol, out string error)
            {
                foreach (var attr in methodSymbol.GetAttributes())
                {
                    if (attr.AttributeClass.MetadataName != nameof(ExpandAttribute))
                        continue;

                    if (!methodSymbol.IsStatic)
                        error = "The target method must be static.";
                    else
                        error = null;

                    return true;
                }

                error = null;
                return false;
            }

            // Check if it's a call to a macro method
            var target = invocation.TargetMethod;

            if (!IsMacroMethod(target, out string err))
                return base.VisitInvocationExpression(node);

            // Make sure it's valid
            if (err != null)
                throw new DiagnosticException($"Cannot call the specified method as a macro method: {err}.",
                                              invocation.Syntax.GetLocation());

            // It is a method, find it
            MethodInfo method = target.GetCorrespondingMethod() as MethodInfo;

            if (method == null)
                throw new DiagnosticException("Cannot find corresponding method.", invocation.Syntax.GetLocation());

            // Found it, make the arguments
            ParameterInfo[] parameters = method.GetParameters();
            object[] arguments = new object[parameters.Length];

            for (int i = 0; i < arguments.Length; i++)
            {
                ParameterInfo param = parameters[i];
                Type paramType = param.ParameterType;
                Optional<object> constant = invocation.ArgumentsInParameterOrder[i].Value.ConstantValue;

                if (constant.HasValue)
                    arguments[i] = constant.Value;
                else if (param.HasDefaultValue)
                    arguments[i] = param.DefaultValue;
                else
                    arguments[i] = paramType.GetTypeInfo().IsValueType
                        ? Activator.CreateInstance(paramType)
                        : null;
            }

            // Set up the context
            var statementSyntax = node.FirstAncestorOrSelf<StatementSyntax>();
            var statementSymbol = new Lazy<IOperation>(() => semanticModel.GetOperation(statementSyntax, cancellationToken));

            var callerSymbol = new Lazy<IMethodSymbol>(() => semanticModel.GetEnclosingSymbol(statementSyntax.SpanStart, cancellationToken) as IMethodSymbol);
            var callerInfo = new Lazy<MethodInfo>(() => callerSymbol.Value?.GetCorrespondingMethod() as MethodInfo);

            ExpressionSyntax expr;
            StatementSyntax stmt;

            using (CallBinder.EnterContext(invocation, statementSymbol, node, statementSyntax, method, target, callerInfo, callerSymbol))
            {
                // Invoke the method
                try
                {
                    method.Invoke(null, arguments);
                }
                catch (Exception e)
                {
                    throw new DiagnosticException("Exception thrown when invoking a macro.", e, invocation.Syntax.GetLocation());
                }

                (expr, stmt) = CallBinder.Result;
            }

            // Edit the node accordingly
            if (stmt != statementSyntax)
            {
                // Return the new statement
                changes.Add(statementSyntax, stmt.WithTriviaFrom(statementSyntax).Accept(this) as StatementSyntax);
                return node;
            }

            return base.Visit(expr == node ? expr : expr.WithTriviaFrom(node));
        }
    }
}
