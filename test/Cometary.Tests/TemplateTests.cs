using System.Linq;
using Cometary.Extensions;
using Cometary.Rewriting;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Xunit;

using F = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Cometary.Tests
{
    public sealed class TemplateTests
    {
        public static bool ReturnTrue(Quote quote = null)
        {
            return quote.Unquote<bool>("true".Syntax<ExpressionSyntax>());
        }

        public static void IncrementAllDeclaredVariables(Quote quote = null)
        {
            quote.Unquote(F.Block(
                from stmt in (quote.BodySyntax as BlockSyntax).Statements.OfType<LocalDeclarationStatementSyntax>()
                where stmt.SpanStart < quote.Syntax.SpanStart
                from variable in stmt.Declaration.Variables
                where Meta.Model(variable.SyntaxTree).GetTypeInfo(variable.Initializer.Value).Type.Name == "Int32"
                select F.ExpressionStatement(F.PostfixUnaryExpression(
                    SyntaxKind.PostIncrementExpression, F.IdentifierName(variable.Identifier)
                ))
            ));
        }

        [Fact]
        public void ShouldModifyBody()
        {
            int x = 0;

            IncrementAllDeclaredVariables();

            int y = 0;

            IncrementAllDeclaredVariables();

            x.ShouldBe(2);
            y.ShouldBe(1);
        }

        [Fact]
        public void ShouldModifyCall()
        {
            bool value = ReturnTrue();

            value.ShouldBeTrue();
        }
    }
}
