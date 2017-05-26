using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using F = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Cometary.Rewriting
{
    /// <summary>
    /// <see cref="CSharpSyntaxRewriter"/> that replaces extern methods
    /// with regular methods.
    /// </summary>
    internal sealed class ExternRewriter : CSharpSyntaxRewriter
    {
        /// <summary>
        /// <see cref="ArrowExpressionClauseSyntax"/> that'll be injected
        /// in <see langword="extern"/> methods.
        /// </summary>
        public static readonly ArrowExpressionClauseSyntax ThrowBody
            = F.ArrowExpressionClause(
                F.ThrowExpression(
                    F.ObjectCreationExpression(
                        F.ParseTypeName(nameof(InvalidOperationException)),
                        F.ArgumentList(
                            F.SeparatedList(new[] {
                                F.Argument(
                                    F.LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        F.Literal("Cannot invoke extern method."))) } )),
                        null)));

        /// <summary>
        /// Rewrites <see langword="extern"/> methods to have a body.
        /// </summary>
        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            int externIndex = node.Modifiers.IndexOf(SyntaxKind.ExternKeyword);

            if (externIndex == -1)
                return node;

            return node
                .WithModifiers(node.Modifiers.RemoveAt(externIndex))
                .WithExpressionBody(ThrowBody);
        }
    }
}
