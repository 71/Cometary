using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using F = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Cometary.Contracts
{
    using Rewriting;
    using Extensions;

    /// <summary>
    /// Assertions powered by your source code.
    /// </summary>
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    public static class Requires
    {
        #region Utils
        private static readonly SyntaxToken SemicolonToken = F.Token(SyntaxKind.SemicolonToken);
        private static readonly ExpressionSyntax NullExpr = F.LiteralExpression(SyntaxKind.NullLiteralExpression);

        private static string Filtered(Quote quote) => quote.ArgumentSyntax.ToFullString().Replace("\"", "\\\"");
        private static string Filtered(string str) => str.Replace("\"", "\\\"");

        private static ThrowStatementSyntax MakeThrowStatement(Quote quote, string msg)
        {
            msg = Filtered(msg);
            FileLinePositionSpan span = quote.ArgumentSyntax.GetLocation().GetLineSpan();

            return F.ThrowStatement(
                $"new globals::{typeof(AssertionException).FullName}(\"{msg}\", \"{Filtered(quote)}\", \"{span.Path}\", {span.StartLinePosition.Line}, {span.StartLinePosition.Character})".Syntax<ExpressionSyntax>()
            ).WithSemicolonToken(SemicolonToken);
        }
        #endregion

        /// <summary>
        /// Ensures that the given object is not <see langword="null"/>.
        /// </summary>
        public static void NotNull(Quote<object> obj)
        {
            obj += F.IfStatement(
                F.BinaryExpression(SyntaxKind.EqualsExpression, obj.ArgumentSyntax.Expression, NullExpr),
                MakeThrowStatement(obj, "Expected expression to not be null.")
            );
        }

        /// <summary>
        /// Ensures that the given object is <see langword="null"/>.
        /// </summary>
        public static void Null(Quote<object> obj)
        {
            obj += F.IfStatement(
                F.BinaryExpression(SyntaxKind.NotEqualsExpression, obj.ArgumentSyntax.Expression, NullExpr),
                MakeThrowStatement(obj, "Expected expression to be null.")
            );
        }

        /// <summary>
        /// Ensures that the given expression evaluates to <see langword="true"/>.
        /// </summary>
        public static void True(Quote<bool> condition)
        {
            condition += F.IfStatement(
                F.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, condition.ArgumentSyntax.Expression),
                MakeThrowStatement(condition, "Expected expression to be true.")
            );
        }

        /// <summary>
        /// Ensures that the given expression evaluates to <see langword="false"/>.
        /// </summary>
        public static void False(Quote<bool> condition)
        {
            condition += F.IfStatement(
                condition.ArgumentSyntax.Expression,
                MakeThrowStatement(condition, "Expected expression to be false.")
            );
        }
    }
}
