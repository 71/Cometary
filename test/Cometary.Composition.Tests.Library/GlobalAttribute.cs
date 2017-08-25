using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using F = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using K = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

namespace Cometary.Tests
{
    using Composition;

    public sealed class GlobalAttribute : CompositionAttribute
    {
        public string Name { get; }

        public GlobalAttribute(string name = "Instance") : base(new GlobalComponent(name))
        {
            Name = name;
        }

        private sealed class GlobalComponent : Component
        {
            private readonly string name;

            public GlobalComponent(string name)
            {
                this.name = name;
            }

            public override ClassDeclarationSyntax Apply(ClassDeclarationSyntax node, INamedTypeSymbol symbol, CancellationToken cancellationToken)
            {
                return node.AddMembers(
                    F.PropertyDeclaration(
                        F.ParseTypeName(node.Identifier.Text), name
                    ).WithAccessorList(
                        F.AccessorList().AddAccessors(F.AccessorDeclaration(K.GetAccessorDeclaration))
                    ).WithModifiers(
                        F.TokenList(F.Token(K.PublicKeyword), F.Token(K.StaticKeyword))
                    ).WithInitializer(
                        F.EqualsValueClause(F.ObjectCreationExpression(F.ParseTypeName(node.Identifier.Text)))
                    ).NormalizeWhitespace()
                );
            }
        }
    }
}
