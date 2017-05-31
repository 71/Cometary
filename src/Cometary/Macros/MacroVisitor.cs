using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Semantics;

namespace Cometary
{
    using Core;
    using Extensions;

    /// <summary>
    /// <see cref="CSharpSyntaxRewriter"/> that transforms macro calls.
    /// </summary>
    internal sealed class MacroVisitor : LightAssemblyVisitor
    {
        private readonly Stack<SyntaxList<StatementSyntax>> ModificationsStack = new Stack<SyntaxList<StatementSyntax>>();
        private readonly Stack<int> DeletionsCountStack = new Stack<int>();

        /// <summary>
        /// Gets whether or not the given parameter is a quote.
        /// </summary>
        private static bool IsQuoteParameter(IParameterSymbol parameter)
        {
            return parameter.Type.Name == nameof(Quote) || parameter.Type.MetadataName == nameof(Quote) + "`1";
        }

        /// <inheritdoc />
        public override int CompareTo(LightAssemblyVisitor other) => other is AttributesVisitor ? 1 : 0;

        /// <inheritdoc />
        public override bool RewritesTree => true;

        /// <summary>
        /// Ensures that we visit every single statement,
        /// even if the block gets modified.
        /// </summary>
        public override SyntaxNode VisitBlock(BlockSyntax node)
        {
            SyntaxList<StatementSyntax> statements = node.Statements;
            SyntaxList<StatementSyntax> rewrittenStatements = statements;

            for (int i = 0; i < statements.Count; i++)
            {
                StatementSyntax stmt = statements[i];
                StatementSyntax rewritten = Visit(stmt) as StatementSyntax;

                if (ReferenceEquals(stmt, rewritten) || rewritten == null)
                    continue;

                if (ModificationsStack.Count == 0)
                {
                    rewrittenStatements = rewrittenStatements.RemoveAt(i).Insert(i, rewritten);
                    continue;
                }

                int deletions = DeletionsCountStack.Pop();
                int diff = rewrittenStatements.Count - statements.Count;

                int index = diff + i;

                for (int k = 0; k < deletions; k++)
                    rewrittenStatements = rewrittenStatements.RemoveAt(index);

                rewrittenStatements = rewrittenStatements.InsertRange(index, ModificationsStack.Pop());

                // Jump over inserted stuff
                i += deletions - 1;
            }

            if (rewrittenStatements != statements)
                return SyntaxFactory.Block(rewrittenStatements);

            return node;
        }

        /// <summary>
        /// Ensures void quotes are correctly reduced.
        /// </summary>
        public override SyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            if (node.Expression is InvocationExpressionSyntax invocation)
            {
                SyntaxNode newInvocation = VisitInvocationExpression(invocation);

                switch (newInvocation)
                {
                    case ExpressionSyntax expr:
                        return node.WithExpression(expr);
                    case StatementSyntax stmt:
                        return stmt.WithSemicolon();

                    default:
                        throw new ProcessingException(newInvocation, "Invalid syntax node returned.");
                }
            }

            return base.VisitExpressionStatement(node);
        }

        /// <summary>
        /// Calls macros.
        /// </summary>
        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            // Get symbol of invocation
            IInvocationExpression invocation = node.SyntaxTree.Model()?.GetOperation(node) as IInvocationExpression;

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

            StatementSyntax enclosingStmt = node.FirstAncestorOrSelf<StatementSyntax>();
            BlockSyntax block = enclosingStmt?.Parent as BlockSyntax;
            int indexInBlock = block?.Statements.IndexOf(enclosingStmt) ?? -1;

            object[] arguments = new object[parameters.Length];
            Quote quote = new Quote(node, invocation);
            bool tookNextStmt = false;

            // Make sure the macro must be reduced, and not executed in another mixin
            if (quote.MethodSymbol.Parameters.Any(IsQuoteParameter))
            {
                // We're being invoked from another mixin,
                // return things normally
                return base.VisitInvocationExpression(node);
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                // Find all parameters
                IParameterSymbol parameter = parameters[i];
                INamedTypeSymbol parameterType = parameter.Type as INamedTypeSymbol;

                // Edge case: A macro may call other macros, with
                // an already existing quote. If that happens, we don't
                // need to replace this call, and just let it go.
                bool IsRecall() => invocation.GetArgumentMatchingParameter(parameter)?.Value is IParameterReferenceExpression paramRef &&
                                   IsQuoteParameter(paramRef.Parameter);

                switch (parameterType?.MetadataName)
                {
                    case nameof(Quote):
                        if (IsRecall())
                            return base.VisitInvocationExpression(node);

                        arguments[i] = quote.For(i);
                        break;
                    case nameof(Quote) + "`1":
                        if (IsRecall())
                            return base.VisitInvocationExpression(node);

                        arguments[i] = quote.For(i, parameterType.TypeArguments[0].Info().AsType());
                        break;
                    case nameof(BlockSyntax):
                        if (!parameter.HasExplicitDefaultValue)
                            // That's not a block, but an actual parameter!
                            goto default;

                        arguments[i] = block?.Statements.ElementAtOrDefault(indexInBlock + 1) as BlockSyntax
                                    ?? throw new ProcessingException(node, $"A call to {invocation.TargetMethod} must be followed by a block statement.");
                        tookNextStmt = true;
                        break;
                    case nameof(StatementSyntax):
                        if (!parameter.HasExplicitDefaultValue)
                            // That's not a block, but an actual parameter!
                            goto default;

                        arguments[i] = block?.Statements.ElementAtOrDefault(indexInBlock + 1)
                                    ?? throw new ProcessingException(node, $"A call to {invocation.TargetMethod} must be followed by a statement.");
                        tookNextStmt = true;
                        break;

                    default:
                        arguments[i] = parameter.HasExplicitDefaultValue
                            ? parameter.ExplicitDefaultValue
                            : invocation.ArgumentsInParameterOrder[i].Value.ConstantValue.HasValue
                                ? invocation.ArgumentsInParameterOrder[i].Value.ConstantValue.Value
                                : null;
                        break;
                }
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
                    if (!pis[i].ParameterType.IsGenericParameter && pis[i].ParameterType.Name != parameters[i].Type.Name)
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

            // Replace body of calling method
            List<SyntaxNode> insertedNodes = quote.InsertedNodes;

            // Store length locally since we'll be accessing it a lot
            int length = insertedNodes.Count;

            if (length == 0)
            {
                // Nothing added, either throw if we're supposed to return something,
                // or do nothing.
                if (target.ReturnsVoid || node.Parent is ExpressionStatementSyntax)
                    return SyntaxFactory.EmptyStatement();

                throw new ProcessingException(node, "The given node must be transformed into an expression.");
            }

            // At least one statement
            SyntaxNode ReturnLastNode(SyntaxNode returnNode)
            {
                if (returnNode is ExpressionSyntax)
                    // No need to worry more than that
                    return returnNode;

                if (returnNode is StatementSyntax exstmt)
                {
                    ExpressionSyntax returnExpr = exstmt.Expression();

                    if (returnExpr != null)
                        return returnExpr;

                    return exstmt;
                }

                throw new ProcessingException(returnNode, "Please report this.");
            }

            MethodDeclarationSyntax methodDecl = quote.MethodSyntax as MethodDeclarationSyntax;
            Debug.Assert(methodDecl != null);

            if (length == 1)
            {
                if (insertedNodes[0] is MethodDeclarationSyntax newMethod)
                {
                    methodDecl.Replace(x => newMethod);
                    return node;
                }
                
                return ReturnLastNode(insertedNodes[0]);
            }

            // We must use a block at this point, and even create it if necessary
            bool isBlockCreated = block == null;

            if (isBlockCreated)
                // Gotta transform that method into a method with statements
                block = SyntaxFactory.Block();

            SyntaxList<StatementSyntax> stmts = new SyntaxList<StatementSyntax>();

            // Insert all statements
            for (int i = 0; i < length; i++)
            {
                SyntaxNode inserted = insertedNodes[i];

                StatementSyntax stmt = inserted as StatementSyntax
                                    ?? SyntaxFactory.ExpressionStatement((ExpressionSyntax)inserted, SyntaxFactory.Token(SyntaxKind.SemicolonToken));

                stmts = stmts.Add(stmt);
            }

            ModificationsStack.Push(stmts);
            DeletionsCountStack.Push(tookNextStmt ? 2 : 1);

            return SyntaxFactory.EmptyStatement();
        }
    }
}
