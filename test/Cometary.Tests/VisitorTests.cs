using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;
using F = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Cometary.Tests
{
    using Shouldly;

    /// <summary>
    /// <see cref="AssemblyVisitor"/> that sets all declared
    /// enum flags to the same value.
    /// </summary>
    public sealed class EvilVisitor : AssemblyVisitor
    {
        /// <inheritdoc />
        public override bool RewritesTree => true;

        /// <inheritdoc />
        public override EnumDeclarationSyntax Visit(TypeInfo @enum, EnumDeclarationSyntax node)
        {
            EqualsValueClauseSyntax clause = F.EqualsValueClause(
                F.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                    F.Literal(0)));

            return node.WithMembers(
                F.SeparatedList(node.Members.Select(x => x.WithEqualsValue(clause)))
            );
        }
    }

    public enum Numbers
    {
        Zero  = 0,
        One   = 1,
        Two   = 2,
        Three = 3
    }

    public class VisitorTests
    {
        [Fact]
        public void AllValuesShouldBeTheSame()
        {
            ((int)Numbers.Zero).ShouldBe(0);

            Numbers.One.ShouldBe(Numbers.Zero);
            Numbers.Two.ShouldBe(Numbers.Zero);
            Numbers.Three.ShouldBe(Numbers.Zero);

            (Numbers.Three == Numbers.Two).ShouldBe(true);
        }
    }
}
