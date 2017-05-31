using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cometary
{
    using Core;

    /// <summary>
    /// <see cref="AssemblyVisitor"/> that cleans up references to
    /// assemblies related to Cometary.
    /// </summary>
    internal sealed class CleanUpVisitor : LightAssemblyVisitor
    {
        /// <inheritdoc />
        public override bool RewritesTree => CometaryAttribute.Instance.CleanUp;

        /// <inheritdoc />
        public override int CompareTo(LightAssemblyVisitor other) => 1;

        /// <summary>
        /// Ensures Cometary-related usings are removed.
        /// </summary>
        public override SyntaxNode VisitUsingDirective(UsingDirectiveSyntax node)
        {
            if (node.Name.ToString().StartsWith(nameof(Cometary)))
                return null;

            return node;
        }

        /// <summary>
        /// Removes Cometary from the referenced assemblies.
        /// </summary>
        public override CSharpCompilation Visit(Assembly assembly, CSharpCompilation compilation)
        {
            return !RewritesTree ? compilation : compilation.RemoveReferences(compilation.References.Where(x => x.Display.StartsWith(nameof(Cometary))));
        }
    }
}
