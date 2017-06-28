using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using F = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Cometary
{
    /// <summary>
    /// <para>
    ///   <see cref="CSharpSyntaxRewriter"/> that prepares a <see cref="CSharpCompilation"/> before compilation,
    ///   by replacing its <see langword="extern"/> members and adding custom generated members.
    /// </para>
    /// </summary>
    /// <seealso href="https://github.com/dotnet/csharplang/blob/master/spec/classes.md">
    ///   Lists all members that can be <see langword="extern"/>.
    /// </seealso>
    internal sealed class PreprocessingRewriter : CSharpSyntaxRewriter
    {
        /// <summary>
        /// <see cref="AttributeListSyntax"/> that'll be injected in
        /// generated members.
        /// </summary>
        public static readonly AttributeListSyntax GeneratedAttribute
            = F.AttributeList(
                F.AttributeTargetSpecifier(F.Token(SyntaxKind.AssemblyKeyword)),
                F.SeparatedList(new[] {
                    F.Attribute(
                        F.ParseName(typeof(CompilerGeneratedAttribute).FullName)) } ));

        /// <summary>
        /// <see cref="ArrowExpressionClauseSyntax"/> that'll be injected
        /// in <see langword="extern"/> methods.
        /// </summary>
        public static readonly ArrowExpressionClauseSyntax ThrowBody
            = F.ArrowExpressionClause(
                F.ThrowExpression(
                    F.ObjectCreationExpression(
                        F.ParseTypeName(typeof(InvalidOperationException).FullName),
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

        /// <summary>
        /// Rewrites <see langword="extern"/> operators to have a body.
        /// </summary>
        public override SyntaxNode VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            int externIndex = node.Modifiers.IndexOf(SyntaxKind.ExternKeyword);

            if (externIndex == -1)
                return node;

            return node
                .WithModifiers(node.Modifiers.RemoveAt(externIndex))
                .WithExpressionBody(ThrowBody);
        }

        /// <summary>
        /// Rewrites <see langword="extern"/> constructors to have a body.
        /// </summary>
        public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            int externIndex = node.Modifiers.IndexOf(SyntaxKind.ExternKeyword);

            if (externIndex == -1)
                return node;

            return node
                .WithModifiers(node.Modifiers.RemoveAt(externIndex))
                .WithExpressionBody(ThrowBody);
        }

        /// <summary>
        /// Rewrites <see langword="extern"/> destructors to have a body.
        /// </summary>
        public override SyntaxNode VisitDestructorDeclaration(DestructorDeclarationSyntax node)
        {
            int externIndex = node.Modifiers.IndexOf(SyntaxKind.ExternKeyword);

            if (externIndex == -1)
                return node;

            return node
                .WithModifiers(node.Modifiers.RemoveAt(externIndex))
                .WithExpressionBody(ThrowBody);
        }

        /// <summary>
        /// Rewrites <see langword="extern"/> events to have a body.
        /// </summary>
        public override SyntaxNode VisitEventDeclaration(EventDeclarationSyntax node)
        {
            int externIndex = node.Modifiers.IndexOf(SyntaxKind.ExternKeyword);

            if (externIndex == -1)
                return node;

            return node
                .WithModifiers(node.Modifiers.RemoveAt(externIndex));
        }

        /// <summary>
        /// Rewrites <see langword="extern"/> indexers to have a body.
        /// </summary>
        public override SyntaxNode VisitIndexerDeclaration(IndexerDeclarationSyntax node)
        {
            int externIndex = node.Modifiers.IndexOf(SyntaxKind.ExternKeyword);

            if (externIndex == -1)
                return node;

            return node
                .WithModifiers(node.Modifiers.RemoveAt(externIndex))
                .WithAccessorList(F.AccessorList(F.List(
                    node.AccessorList.Accessors.Select(x => x.WithExpressionBody(ThrowBody))
                )));
        }

        /// <summary>
        /// Rewrites <see langword="extern"/> properties to have a body.
        /// </summary>
        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            int externIndex = node.Modifiers.IndexOf(SyntaxKind.ExternKeyword);

            if (externIndex == -1)
                return node;

            return node
                .WithModifiers(node.Modifiers.RemoveAt(externIndex))
                .WithAccessorList(F.AccessorList(F.List(
                    node.AccessorList.Accessors.Select(x => x.WithExpressionBody(ThrowBody))
                )));
        }
    }
}
