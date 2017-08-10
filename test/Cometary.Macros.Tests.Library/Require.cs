using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using F = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using K = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

namespace Cometary.Tests
{
    using Macros;

    /// <summary>
    ///   Class that provides an easy way to make assertions.
    /// </summary>
    public static class Require
    {
        private static StatementSyntax MakeThrowSyntax(Location location, ExpressionSyntax condition, string msg, string expr)
        {
            // message, expression, file, position, width
            string file = location.SourceTree.FilePath;

            ArgumentSyntax[] argsSyntax =
            {
                F.Argument(F.LiteralExpression(K.StringLiteralExpression, F.Literal(msg))),
                F.Argument(F.LiteralExpression(K.StringLiteralExpression, F.Literal(expr))),
                F.Argument(file == null ? F.LiteralExpression(K.NullLiteralExpression) : F.LiteralExpression(K.StringLiteralExpression, F.Literal(file))),
                F.Argument(F.LiteralExpression(K.NumericLiteralExpression, F.Literal(location.SourceSpan.Start))),
                F.Argument(F.LiteralExpression(K.NumericLiteralExpression, F.Literal(location.SourceSpan.Length)))
            };

            return F.IfStatement(
                condition,
                F.ParseStatement($"throw new {typeof(AssertionException).FullName}({string.Join<ArgumentSyntax>(", ", argsSyntax)});")
            );
        }

        private static ExpressionSyntax GetExpression()
        {
            return ((InvocationExpressionSyntax)CallBinder.InvocationSyntax).ArgumentList.Arguments[0].Expression;
        }

        /// <summary>
        ///   Ensures that the given <paramref name="expression"/> is <see langword="null"/>.
        /// </summary>
        [Expand]
        public static void Null(object expression)
        {
            ExpressionSyntax expr = GetExpression();

            CallBinder.StatementSyntax = MakeThrowSyntax(
                expr.GetLocation(),
                F.BinaryExpression(K.NotEqualsExpression, expr, F.LiteralExpression(K.NullLiteralExpression)),
                "Expected expression to be null.", expr.ToString()
            );
        }

        /// <summary>
        ///   Ensures that the given <paramref name="expression"/> isn't <see langword="null"/>.
        /// </summary>
        [Expand]
        public static void NotNull(object expression)
        {
            ExpressionSyntax expr = GetExpression();

            CallBinder.StatementSyntax = MakeThrowSyntax(
                expr.GetLocation(),
                F.BinaryExpression(K.EqualsExpression, expr, F.LiteralExpression(K.NullLiteralExpression)),
                "Expected expression to be non-null.", expr.ToString()
            );
        }

        /// <summary>
        ///   Ensures that the given <paramref name="expression"/> is <see langword="true"/>.
        /// </summary>
        [Expand]
        public static void True(bool expression)
        {
            ExpressionSyntax expr = GetExpression();

            CallBinder.StatementSyntax = MakeThrowSyntax(
                expr.GetLocation(),
                F.PrefixUnaryExpression(K.LogicalNotExpression, F.ParenthesizedExpression(expr)),
                "Expected expression to be true.", expr.ToString()
            );
        }

        /// <summary>
        ///   Ensures that the given <paramref name="expression"/> isn't <see langword="false"/>.
        /// </summary>
        [Expand]
        public static void False(bool expression)
        {
            ExpressionSyntax expr = GetExpression();

            CallBinder.StatementSyntax = MakeThrowSyntax(
                expr.GetLocation(),
                expr,
                "Expected expression to be false.", expr.ToString()
            );
        }

        /// <summary>
        ///   Ensures all arguments given to the current method aren't <see langword="null"/>.
        /// </summary>
        [Expand]
        public static void NoArgumentNull()
        {
            StatementSyntax ToStatement(IParameterSymbol parameter)
            {
                return F.IfStatement(
                    F.ParseExpression($"{nameof(ReferenceEquals)}(null, {parameter.Name})"),
                    F.ParseStatement($"throw new {typeof(ArgumentNullException).FullName}(\"{parameter.Name}\");")
                );
            }

            CallBinder.StatementSyntax = F.Block(
                from parameter in CallBinder.CallerSymbol.Parameters
                where !parameter.Type.IsValueType
                select ToStatement(parameter)
            );
        }
    }
}
