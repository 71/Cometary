using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Xunit;

using F = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Cometary.Tests
{
    using Attributes;
    using Extensions;

    public class FeaturesTests
    {
        #region Extern support
        public extern void DontThrow();

        [Invoke]
        public static void ModifyMethod(TypeDeclarationSyntax type)
        {
#pragma warning disable RS1014 // Do not ignore values returned by methods on immutable objects.
            type.GetMethod(nameof(DontThrow))
                .Replace(x => x.RemoveModifiers(Modifiers.Extern)
                               .WithBody(F.Block())
                               .WithExpressionBody(null));
#pragma warning restore RS1014 // Do not ignore values returned by methods on immutable objects.
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
            LiteralExpressionSyntax one = F.LiteralExpression(SyntaxKind.NumericLiteralExpression, F.Literal(1));
            LiteralExpressionSyntax two = F.LiteralExpression(SyntaxKind.NumericLiteralExpression, F.Literal(2));
            ExpressionSyntax binary = F.BinaryExpression(SyntaxKind.AddExpression, one, one);
            ExpressionSyntax expr = F.BinaryExpression(SyntaxKind.MultiplyExpression, binary, two);

            ExpressionStatementSyntax stmt  = F.ExpressionStatement(expr);
            ExpressionStatementSyntax mStmt = stmt.With(x => (x.Expression as BinaryExpressionSyntax)?.Right, x => one);

            (stmt.Expression as BinaryExpressionSyntax)?.Right.ToString().ShouldBe(two.ToString());
            (mStmt.Expression as BinaryExpressionSyntax)?.Right.ToString().ShouldBe(one.ToString());
        }

        //[Fact]
        //public void ShouldSupportDeepWith()
        //{
        //    ExpressionStatementSyntax exprStmt = "2 * 3;".Syntax<ExpressionStatementSyntax>();

        //    exprStmt.DeepWith(

        //        (exprStmt.Expression as BinaryExpressionSyntax)?.Left,
        //        left => F.LiteralExpression(SyntaxKind.NumericLiteralExpression, F.Literal(1))

        //    ).ToString().ShouldBe("1 * 3;");
        //}
        #endregion

        [CTFIFact]
        public void ShouldFindAssembly()
        {
            Meta.TargetAssembly.ShouldNotBeNull();
        }

        [CTFIFact]
        public void FluentExtensionsShouldWork(MethodDeclarationSyntax method)
        {
            method.GetModifiers().ShouldBe(Modifiers.Public);
        }
    }
}
