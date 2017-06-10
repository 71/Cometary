using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using F = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Cometary.Extensions
{
    partial class SyntaxExtensions
    {
        #region Utilities
        /// <summary>
        /// Utility method used to perform a deep <c>With*</c> call.
        /// <para>
        /// This call will automatically be replaced by a nested <c>With*(*)</c> call.
        /// </para>
        /// </summary>
        /// <typeparam name="T">The type of the syntax node that'll be returned with its descendants replaced.</typeparam>
        /// <typeparam name="TProp">The type of the syntax node that'll be replaced.</typeparam>
        /// <param name="self">The syntax node whose descendants will be replaced.</param>
        /// <param name="node">The syntax node to replace.</param>
        /// <param name="replacement">The syntax node that will replace the old node.</param>
        /// <param name="quote"></param>
        /// <returns>A modified <typeparamref name="T"/> node.</returns>
        public static T DeepWith<T, TProp>(this T self, TProp node, TProp replacement, Quote quote = null)
            where T : SyntaxNode
            where TProp : SyntaxNode
        {
            ExpressionSyntax selfArg;
            ExpressionSyntax nodeArg;
            ExpressionSyntax replArg;

            if (quote.Syntax.ArgumentList.Arguments.Count == 2)
            {
                selfArg = (quote.Syntax.Expression as MemberAccessExpressionSyntax).Expression;
                nodeArg = quote.Syntax.ArgumentList.Arguments[0].Expression;
                replArg = quote.Syntax.ArgumentList.Arguments[1].Expression;
            }
            else
            {
                selfArg = quote.Syntax.ArgumentList.Arguments[0].Expression;
                nodeArg = quote.Syntax.ArgumentList.Arguments[1].Expression;
                replArg = quote.Syntax.ArgumentList.Arguments[2].Expression;
            }

            if (replArg is SimpleLambdaExpressionSyntax lambda)
                replArg = lambda.Body as ExpressionSyntax;

            ExpressionSyntax MakeInvocationExpression(ExpressionSyntax selfExpr, ExpressionSyntax nodeExpr, string target)
            {
                string nodeMemberName = null;
                ExpressionSyntax prevNode;

                if (selfExpr.ToString() == nodeExpr.ToString())
                    return replArg;

                if (nodeExpr is ParenthesizedExpressionSyntax parenth)
                    nodeExpr = parenth.Expression;

                if (nodeExpr is MemberAccessExpressionSyntax maxSyntax)
                {
                    nodeMemberName = maxSyntax.Name.Identifier.Text;
                    prevNode = maxSyntax.Expression;
                }
                else if (nodeExpr is InvocationExpressionSyntax ixSyntax)
                {
                    prevNode = ixSyntax.Expression;
                }
                else if (nodeExpr is ElementAccessExpressionSyntax eaxSyntax)
                {
                    prevNode = eaxSyntax.Expression;
                }
                else if (nodeExpr is ConditionalAccessExpressionSyntax caxSyntax)
                {
                    nodeMemberName = ((MemberBindingExpressionSyntax)caxSyntax.WhenNotNull).Name.Identifier.Text;
                    prevNode = caxSyntax.Expression;
                }
                else
                {
                    prevNode = nodeExpr.DescendantNodes(_ => true).OfType<ExpressionSyntax>().FirstOrDefault();
                }

                var nExpr = MakeInvocationExpression(selfExpr, prevNode, nodeMemberName);

                if (nodeMemberName == null)
                    return nExpr;

                return F.InvocationExpression(
                    F.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, prevNode, F.IdentifierName($"With{nodeMemberName}")),
                    F.ArgumentList(F.SeparatedList(new[] { F.Argument(nExpr) }))
                );
            }

            // TODO: Fix this
            return (quote > quote.Syntax) as T;
        }

        /// <inheritdoc cref="DeepWith{T, TProp}(T, TProp, TProp, Quote)" />
        public static T DeepWith<T, TProp>(this T self, TProp node, Func<TProp, TProp> replacement, Quote quote = null)
            where T : SyntaxNode
            where TProp : SyntaxNode
            => self.DeepWith(node, replacement(node), quote);
        #endregion

        #region SyntaxList
        /// <summary>
        /// Returns a new <see cref="SyntaxTokenList"/> with the specified
        /// <paramref name="tokens"/> added.
        /// </summary>
        public static SyntaxTokenList Add(this SyntaxTokenList list, params SyntaxToken[] tokens) => list.AddRange(tokens);

        /// <summary>
        /// Returns a new <see cref="SyntaxTokenList"/> where only the syntax tokens
        /// matching the given <paramref name="predicate"/> are present.
        /// </summary>
        public static SyntaxTokenList Where(this SyntaxTokenList list, Predicate<SyntaxToken> predicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                    continue;

                list = list.RemoveAt(i--);
            }

            return list;
        }

        /// <summary>
        /// Returns a new <see cref="SyntaxTokenList"/> with the specified
        /// <paramref name="tokens"/> removed.
        /// </summary>
        public static SyntaxTokenList Remove(this SyntaxTokenList list, params SyntaxToken[] tokens) => list.Where(x => Array.IndexOf(tokens, x) == -1);

        #endregion

        #region Get / Set
        /// <summary>
        /// Returns the given <paramref name="property"/> with the <see langword="get"/>
        /// accessor replaced by the given <paramref name="expression"/>.
        /// </summary>
        public static PropertyDeclarationSyntax WithGet(this PropertyDeclarationSyntax property, ExpressionSyntax expression) => property.With(
            x => x.AccessorList.Accessors.First(y => y.Keyword.Kind() == SyntaxKind.GetKeyword).ExpressionBody,
            x => x.WithExpression(expression));

        /// <summary>
        /// Returns the given <paramref name="property"/> with the <see langword="set"/>
        /// accessor replaced by the given <paramref name="expression"/>.
        /// </summary>
        public static PropertyDeclarationSyntax WithSet(this PropertyDeclarationSyntax property, ExpressionSyntax expression) => property.With(
            x => x.AccessorList.Accessors.First(y => y.Keyword.Kind() == SyntaxKind.SetKeyword).ExpressionBody,
            x => x.WithExpression(expression));

        /// <summary>
        /// Returns the given <paramref name="property"/> with the <see langword="get"/>
        /// accessor replaced by the given <paramref name="body"/>.
        /// </summary>
        public static PropertyDeclarationSyntax WithGet(this PropertyDeclarationSyntax property, BlockSyntax body) => property.With(
            x => x.AccessorList.Accessors.First(y => y.Keyword.Kind() == SyntaxKind.GetKeyword),
            x => x.WithBody(body));

        /// <summary>
        /// Returns the given <paramref name="property"/> with the <see langword="set"/>
        /// accessor replaced by the given <paramref name="body"/>.
        /// </summary>
        public static PropertyDeclarationSyntax WithSet(this PropertyDeclarationSyntax property, BlockSyntax body) => property.With(
            x => x.AccessorList.Accessors.First(y => y.Keyword.Kind() == SyntaxKind.SetKeyword),
            x => x.WithBody(body));
        #endregion

        #region Modifiers
        /// <summary>
        /// Gets the <see cref="SyntaxKind"/> corresponding to the given modifiers.
        /// </summary>
        public static SyntaxKind SyntaxKindForModifier(Modifiers modifiers)
        {
            switch (modifiers)
            {
                case Modifiers.Public:
                    return SyntaxKind.PublicKeyword;
                case Modifiers.Private:
                    return SyntaxKind.PrivateKeyword;
                case Modifiers.Protected:
                    return SyntaxKind.ProtectedKeyword;
                case Modifiers.Internal:
                    return SyntaxKind.InternalKeyword;
                case Modifiers.Abstract:
                    return SyntaxKind.AbstractKeyword;
                case Modifiers.Virtual:
                    return SyntaxKind.VirtualKeyword;
                case Modifiers.Static:
                    return SyntaxKind.StaticKeyword;
                case Modifiers.Partial:
                    return SyntaxKind.PartialKeyword;
                case Modifiers.Extern:
                    return SyntaxKind.ExternKeyword;
                case Modifiers.Async:
                    return SyntaxKind.AsyncKeyword;
                default:
                    throw new ArgumentOutOfRangeException(nameof(modifiers));
            }
        }

        /// <summary>
        /// Gets the <see cref="SyntaxKind"/> corresponding to the given modifiers.
        /// </summary>
        public static Modifiers ModifierForSyntaxKind(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PublicKeyword:
                    return Modifiers.Public;
                case SyntaxKind.PrivateKeyword:
                    return Modifiers.Private;
                case SyntaxKind.ProtectedKeyword:
                    return Modifiers.Protected;
                case SyntaxKind.InternalKeyword:
                    return Modifiers.Internal;
                case SyntaxKind.AbstractKeyword:
                    return Modifiers.Abstract;
                case SyntaxKind.VirtualKeyword:
                    return Modifiers.Virtual;
                case SyntaxKind.StaticKeyword:
                    return Modifiers.Static;
                case SyntaxKind.PartialKeyword:
                    return Modifiers.Partial;
                case SyntaxKind.ExternKeyword:
                    return Modifiers.Extern;
                case SyntaxKind.AsyncKeyword:
                    return Modifiers.Async;
                default:
                    return Modifiers.None;
            }
        }

        /// <summary>
        /// Adds the given <paramref name="modifiers"/> to the given <paramref name="member"/>.
        /// </summary>
        public static T AddModifiers<T>(this T member, Modifiers modifiers) where T : MemberDeclarationSyntax
        {
            void MakeBody(Quote quote = null)
            {
                quote += MemberSwitch(F.IdentifierName(nameof(member)), x =>
                    $"{x}.AddModifiers(modifiers.EnumerateAll<Modifiers>().Select(x => F.Token(SyntaxKindForModifier(x))).ToArray())"
                    .Syntax<ExpressionSyntax>()
                );
            }

            MakeBody();

            return default(T);
        }

        /// <summary>
        /// Removes the given <paramref name="modifiers"/> to the given <paramref name="member"/>.
        /// </summary>
        public static T RemoveModifiers<T>(this T member, Modifiers modifiers) where T : MemberDeclarationSyntax
        {
            void MakeBody(Quote quote = null)
            {
                quote += MemberSwitch(F.IdentifierName(nameof(member)), x =>
                    $"{x}.WithModifiers(member.Modifiers.Remove(modifiers.EnumerateAll<Modifiers>().Select(x => F.Token(SyntaxKindForModifier(x))).ToArray()))"
                    .Syntax<ExpressionSyntax>()
                );
            }

            MakeBody();

            return default(T);
        }

        /// <summary>
        /// Adds the given <paramref name="modifiers"/> to the given <paramref name="member"/>.
        /// </summary>
        public static T WithModifiers<T>(this T member, Modifiers modifiers) where T : MemberDeclarationSyntax
        {
            void MakeBody(Quote quote = null)
            {
                quote += MemberSwitch(F.IdentifierName(nameof(member)), x =>
                    $"{x}.WithModifiers(F.TokenList(modifiers.EnumerateAll<Modifiers>().Select(x => F.Token(SyntaxKindForModifier(x)))))"
                    .Syntax<ExpressionSyntax>()
                );
            }

            MakeBody();

            return default(T);
        }

        /// <summary>
        /// Gets the modifiers of the given <paramref name="member"/> as a <see cref="Modifiers"/> <see langword="enum"/>.
        /// </summary>
        public static Modifiers GetModifiers(this MemberDeclarationSyntax member)
        {
            void MakeBody(Quote quote = null)
            {
                quote += MemberSwitch(F.IdentifierName(nameof(member)), x =>
                    $"{x}.Modifiers.Aggregate(Modifiers.None, (modifiers, tok) => modifiers | ModifierForSyntaxKind(tok.Kind()))"
                    .Syntax<ExpressionSyntax>()
                );
            }

            MakeBody();

            return default(Modifiers);
        }
        #endregion
    }
}
