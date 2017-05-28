using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cometary.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Xunit;

using F = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Cometary.Tests
{
    using Rewriting;

    [SuppressMessage("ReSharper", "RedundantAssignment")]
    public sealed class MacrosTests
    {
        /// <summary>
        /// Returns <see langword="true"/>.
        /// </summary>
        public static bool ReturnTrue(Quote quote = null) => quote + "true";

        /// <summary>
        /// Increments all variables that were locally declared before
        /// the call to this method.
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException", Justification = "Value set by compiler.")]
        public static void IncrementAllDeclaredVariables(Quote quote = null)
        {
            IEnumerable<StatementSyntax> statements =
                from stmt in (quote.BodySyntax as BlockSyntax).Statements.OfType<LocalDeclarationStatementSyntax>()
                where stmt.SpanStart < quote.Syntax.SpanStart
                from variable in stmt.Declaration.Variables
                where variable.SyntaxTree.Model().GetTypeInfo(variable.Initializer.Value).Type.Name == "Int32"
                select F.ExpressionStatement(F.PostfixUnaryExpression(
                    SyntaxKind.PostIncrementExpression, F.IdentifierName(variable.Identifier)
                ));

            foreach (StatementSyntax stmt in statements)
                quote += stmt;
        }

        /// <summary>
        /// Loops the specified number of times.
        /// </summary>
        public static void Loop(int count, Quote quote = null, StatementSyntax block = null)
        {
            quote += $"for (int __i = 1; __i < {count}; __i++)"
                .Syntax<ForStatementSyntax>()
                .WithStatement(block);
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

        [Fact]
        public void ShouldSupportBlockStmts()
        {
            // First try: with blocks
            int loopCount = 0;

            Loop(10);
            {
                loopCount++;
            }

            loopCount.ShouldBe(10);

            // Second try: with statements
            Loop(10);
            loopCount++;

            loopCount.ShouldBe(20);
        }
    }
}
