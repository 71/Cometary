using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using F = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using K = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

namespace Cometary
{
    /// <summary>
    ///   <see cref="CompilationEditor"/> that replaces all extern methods
    ///   by a method that throws an <see cref="NotImplementedException"/>.
    /// </summary>
    internal sealed class ExternsReplacingEditor : CompilationEditor
    {
        /// <summary>
        ///   Error message to display to the user when they invoke an extern method.
        /// </summary>
        private const string ErrorMessage = "Extern method invocation is forbidden.";

        /// <summary>
        ///   Syntax for the <see cref="BaseMethodDeclarationSyntax.ExpressionBody"/> of the
        ///   originally extern method.
        /// </summary>
        private static readonly ArrowExpressionClauseSyntax ThrowingBody = F.ArrowExpressionClause(
            F.ThrowExpression(
                F.ObjectCreationExpression(
                    F.ParseTypeName($"global::{typeof(NotImplementedException).FullName}")
                ).AddArgumentListArguments(
                    F.Argument(F.LiteralExpression(
                        K.StringLiteralExpression,
                        F.Literal($"{ErrorMessage}")
                    ))
                )
            )
        );

        /// <inheritdoc />
        protected override void Initialize(CSharpCompilation compilation, CancellationToken cancellationToken)
        {
            this.EditSyntax(VisitSyntaxNode);
        }

        /// <summary>
        ///   Visits a <see cref="SyntaxNode"/>. If it is a <see cref="MethodDeclarationSyntax"/>,
        ///   and it has a <see cref="SyntaxKind.ExternKeyword"/> modifier, it will be modified
        ///   to throw an <see cref="NotImplementedException"/>.
        /// </summary>
        private static SyntaxNode VisitSyntaxNode(SyntaxNode node, CSharpCompilation compilation, CancellationToken cancellationToken)
        {
            if (!(node is MemberDeclarationSyntax))
                return node;

            if (node is MethodDeclarationSyntax method)
            {
                int externModifierIndex = method.Modifiers.IndexOf(K.ExternKeyword);

                if (externModifierIndex == -1)
                    return node;

                return method.WithModifiers(method.Modifiers.RemoveAt(externModifierIndex))
                             .WithExpressionBody(ThrowingBody);
            }

            if (node is ConstructorDeclarationSyntax ctor)
            {
                int externModifierIndex = ctor.Modifiers.IndexOf(K.ExternKeyword);

                if (externModifierIndex == -1)
                    return node;

                return ctor.WithModifiers(ctor.Modifiers.RemoveAt(externModifierIndex))
                           .WithExpressionBody(ThrowingBody);
            }

            if (node is PropertyDeclarationSyntax prop)
            {
                int externModifierIndex = prop.Modifiers.IndexOf(K.ExternKeyword);

                if (externModifierIndex == -1)
                    return node;

                return prop.WithModifiers(prop.Modifiers.RemoveAt(externModifierIndex))
                           .WithExpressionBody(ThrowingBody);
            }

            return node;
        }
    }
}
