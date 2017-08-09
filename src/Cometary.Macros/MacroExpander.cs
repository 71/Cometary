using System;
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

        private StatementSyntax enclosingStmt;

        internal MacroExpander(CSharpSyntaxTree syntaxTree, CSharpCompilation compilation, CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
            this.semanticModel = compilation.GetSemanticModel(syntaxTree, true);
        }

        public override SyntaxNode Visit(SyntaxNode node)
        {
            if (node is StatementSyntax stmt)
            {
                enclosingStmt = stmt;

                // No need to get its value, we never change statements here
                var result = base.Visit(stmt);

                if (enclosingStmt == null || stmt == enclosingStmt)
                    return result;

                if (stmt is BlockSyntax && !(enclosingStmt is BlockSyntax))
                {
                    enclosingStmt = SyntaxFactory.Block(enclosingStmt);
                }

                result = enclosingStmt;

                enclosingStmt = null;

                return result;
            }

            return base.Visit(node);
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
                Type paramType = parameters[i].ParameterType;

                arguments[i] = paramType.GetTypeInfo().IsValueType
                    ? Activator.CreateInstance(paramType)
                    : null;
            }

            // Set up the context
            var statementSyntax = node.FirstAncestorOrSelf<StatementSyntax>();
            var statementSymbol = new Lazy<IOperation>(() => semanticModel.GetOperation(statementSyntax, cancellationToken));

            ExpressionSyntax expr;
            StatementSyntax stmt;

            using (CallBinder.EnterContext(invocation, statementSymbol, node, statementSyntax))
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
                enclosingStmt = base.Visit(stmt.WithSpan(statementSyntax.Span)) as StatementSyntax ?? enclosingStmt;
                return node;
            }

            return base.Visit(expr == node ? expr : expr.WithSpan(node.Span));
        }
    }
}
