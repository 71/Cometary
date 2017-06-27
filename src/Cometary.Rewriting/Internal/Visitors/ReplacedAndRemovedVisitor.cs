using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary
{
    using Core;

    /// <summary>
    /// <see cref="CSharpSyntaxRewriter"/> that replaces
    /// nodes modified or removed through <see cref="MetaExtensions.Replace{T}"/>,
    /// and <see cref="MetaExtensions.Delete"/>.
    /// </summary>
    internal sealed class ReplacedAndRemovedVisitor : LightAssemblyVisitor
    {
        /// <inheritdoc />
        public override bool RewritesTree => false;

        /// <inheritdoc />
        public override int CompareTo(LightAssemblyVisitor other) => 1;

        /// <summary>
        /// List of all changes made to the syntax tree currently being visited.
        /// </summary>
        private List<(SyntaxNode node, Func<SyntaxNode, SyntaxNode> apply)> ChangesList;

        /// <summary>
        /// Applies changes made through <see cref="MetaExtensions.Replace{T}"/>
        /// and <see cref="MetaExtensions.Delete"/>.
        /// </summary>
        public override CSharpCompilation Visit(Assembly assembly, CSharpCompilation compilation)
        {
            for (int i = 0; i < compilation.SyntaxTrees.Length; i++)
            {
                SyntaxTree syntaxTree = compilation.SyntaxTrees[i];

                if (!MetaExtensions.Changes.TryGetValue(Meta.GetID(syntaxTree), out ChangesList))
                    continue;

                compilation = compilation.ReplaceSyntaxTree(syntaxTree, 
                    syntaxTree.WithRootAndOptions(Visit(syntaxTree.GetRoot()), syntaxTree.Options));
            }

            return compilation;
        }

        public override SyntaxNode Visit(SyntaxNode node)
        {
            if (node == null)
                return null;

            foreach (var change in ChangesList)
            {
                if (change.node.RawKind == node.RawKind && change.node.IsEquivalentTo(node))
                    return change.apply(node);
            }

            return ((CSharpSyntaxNode)node).Accept(this);
        }
    }
}
