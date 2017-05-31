using System.Linq;
using System.Reflection;
using Cometary;
using Cometary.Attributes;
using Cometary.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Xunit;

using F = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

[assembly: Cometary]

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

        [CTFEFact]
        public void ShouldHaveGlobalCometaryInstance()
        {
            typeof(CometaryAttribute)
                .GetTypeInfo()
                .GetDeclaredField("instance")
                .GetValue(null)
                .ShouldNotBeNull();
        }

        // [Fact, CTFE]
        // Doesn't work, TODO.
        public static void ShouldEnableCTFEDirective()
        {
#if CTFE
            bool inCTFE = true;
#else
            bool inCTFE = false;
#endif

            Meta.CTFE.ShouldBe(inCTFE);
        }
    }
}
