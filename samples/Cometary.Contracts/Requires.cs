using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using F = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Cometary.Contracts
{
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

        private static string Filtered(Quote quote) => Filtered(quote.ArgumentSyntax.ToFullString());
        private static string Filtered(string str)  => str.Replace("\"", "\\\"");

        private static ThrowStatementSyntax MakeThrowStatement(Quote quote, string msg)
        {
            msg = Filtered(msg);
            FileLinePositionSpan span = quote.ArgumentSyntax.GetLocation().GetLineSpan();

            string path;
            int startLine, startChar;

            if (IncludePosition)
            {
                path = $"\"{span.Path}\"";
                startLine = span.StartLinePosition.Line;
                startChar = span.StartLinePosition.Character;
            }
            else
            {
                path = "null";
                startLine = -1;
                startChar = -1;
            }

            return F.ThrowStatement(
                $"new globals::{typeof(AssertionException).FullName}(\"{msg}\", \"{Filtered(quote)}\", {path}, {startLine}, {startChar})".Syntax<ExpressionSyntax>()
            ).WithSemicolonToken(SemicolonToken);
        }
        #endregion

        /// <summary>
        /// Gets whether or not assertion statements should be added to
        /// method bodies.
        /// </summary>
        public static bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets whether or not assertion statements should include the source file,
        /// line and character numbers.
        /// </summary>
        public static bool IncludePosition { get; set; } = true;

        /// <summary>
        /// Ensures that the given object is not <see langword="null"/>.
        /// </summary>
        public static void NotNull(Quote<object> obj)
        {
            if (!IsEnabled)
                return;

            obj += F.IfStatement(
                F.BinaryExpression(SyntaxKind.EqualsExpression, obj.ArgumentSyntax.Expression, NullExpr),
                MakeThrowStatement(obj, "Expected expression to not be null.")
            ).WithSpan(obj.Syntax);
        }

        /// <summary>
        /// Ensures that the given object is <see langword="null"/>.
        /// </summary>
        public static void Null(Quote<object> obj)
        {
            if (!IsEnabled)
                return;

            obj += F.IfStatement(
                F.BinaryExpression(SyntaxKind.NotEqualsExpression, obj.ArgumentSyntax.Expression, NullExpr),
                MakeThrowStatement(obj, "Expected expression to be null.")
            ).WithSpan(obj.Syntax);
        }

        /// <summary>
        /// Ensures that the given expression evaluates to <see langword="true"/>.
        /// </summary>
        public static void True(Quote<bool> condition)
        {
            if (!IsEnabled)
                return;

            condition += F.IfStatement(
                F.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, condition.ArgumentSyntax.Expression),
                MakeThrowStatement(condition, "Expected expression to be true.")
            ).WithSpan(condition.Syntax);
        }

        /// <summary>
        /// Ensures that the given expression evaluates to <see langword="false"/>.
        /// </summary>
        public static void False(Quote<bool> condition)
        {
            if (!IsEnabled)
                return;

            condition += F.IfStatement(
                condition.ArgumentSyntax.Expression,
                MakeThrowStatement(condition, "Expected expression to be false.")
            ).WithSpan(condition.Syntax);
        }
    }
}
