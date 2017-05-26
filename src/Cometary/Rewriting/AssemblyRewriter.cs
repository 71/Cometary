using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary.Rewriting
{
    /// <summary>
    /// <see cref="CSharpSyntaxRewriter"/> that replaces
    /// nodes modified or removed through <see cref="Meta.Replace{T}"/>,
    /// and <see cref="Meta.Delete"/>.
    /// </summary>
    internal sealed class AssemblyRewriter : CSharpSyntaxRewriter
    {
        /// <summary>
        /// Replaces nodes.
        /// </summary>
        public override SyntaxNode Visit(SyntaxNode node)
        {
            if (node == null)
                return base.Visit(null);

            int projectorIndex = 0;
            SyntaxNode rewrittenNode = node;

            // We need to change things from bottom to top.
            // Unfortunately, if something is changed at the top level,
            // smaller syntax nodes will not be replaced.
            // So, instead of replacing this node directly, we
            // first visit its children, and see who wanted to
            // visit it.

            List<SyntaxNode> changedNodes = Meta.changedNodes;
            List<Func<SyntaxNode, SyntaxNode>> changedNodesFuncs = Meta.changedNodesFuncs;

            while (projectorIndex < changedNodes.Count)
            {
                projectorIndex = changedNodes.FindIndex(projectorIndex,
                    x => x.FullSpan == node.FullSpan && x.GetType() == node.GetType()
                );

                if (projectorIndex == -1)
                    break;

                rewrittenNode = changedNodesFuncs[projectorIndex++](rewrittenNode);
            }

            return base.Visit(rewrittenNode);
        }
    }
}
