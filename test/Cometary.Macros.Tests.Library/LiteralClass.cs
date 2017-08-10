using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cometary.Tests
{
    /// <summary>
    ///   Class that allows getting numbers using their English word representation.
    /// </summary>
    public static class Literal
    {
        private static readonly string[] SupportedNumbers = { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve" };

        private static ExpressionSyntax Bind(MemberAccessExpressionSyntax node)
        {
            int index = Array.IndexOf(SupportedNumbers, node.Name.Identifier.Text);

            if (index == -1)
                throw new DiagnosticException("Invalid property", node.GetLocation());

            return SyntaxFactory.LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal(index)
            );
        }
    }
}
