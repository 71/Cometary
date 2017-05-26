using System;
using System.Linq;
using Cometary.Attributes;
using Cometary.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Xunit;

using F = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Cometary.Tests
{
    public class FeaturesTests
    {
        #region Extern support
        public extern void DontThrow();

        [CTFE]
        public static void ModifyMethod(TypeDeclarationSyntax type)
        {
            type.GetMethod(nameof(DontThrow))
                .Replace(x => x.WithModifiers(x.Modifiers.Remove(x.Modifiers.First(y => y.Kind() == SyntaxKind.ExternKeyword)))
                               .WithBody(F.Block())
                               .WithExpressionBody(null));
        }

        [Fact]
        public void ShouldNotThrowOnModifiedExternMethods()
        {
            Should.NotThrow(() => DontThrow());
        }
        #endregion

        #region With
        [Fact]
        public void ShouldSupportWith()
        {
            ExpressionSyntax one = F.LiteralExpression(SyntaxKind.NumericLiteralExpression, F.Literal(1));
            ExpressionSyntax two = F.LiteralExpression(SyntaxKind.NumericLiteralExpression, F.Literal(2));
            ExpressionSyntax binary = F.BinaryExpression(SyntaxKind.PlusToken, one, one);
            ExpressionSyntax expr = F.BinaryExpression(SyntaxKind.MultiplyExpression, binary, two);

            ExpressionStatementSyntax stmt  = F.ExpressionStatement(expr);
            ExpressionStatementSyntax mStmt = stmt.With(x => (x.Expression as BinaryExpressionSyntax).Right, x => one);
        }
        #endregion
    }
}
