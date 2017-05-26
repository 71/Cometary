using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using Cometary.Visiting;
using Microsoft.CodeAnalysis.CSharp;
using F = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Cometary.Tests
{
    /// <summary>
    /// <see cref="AssemblyVisitor"/> that sets all declared
    /// enum flags to the same value.
    /// </summary>
    public sealed class EvilVisitor : AssemblyVisitor
    {
        public override float Priority => 100.0f;

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

    class VisitorTests
    {
    }
}
