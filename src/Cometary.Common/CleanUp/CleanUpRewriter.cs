using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cometary
{
    /// <summary>
    ///   <see cref="CSharpSyntaxRewriter"/> that cleans up references to
    ///   assemblies related to Cometary.
    /// </summary>
    internal sealed class CleanUpRewriter : CSharpSyntaxRewriter
    {
        /// <summary>
        ///   Ensures nodes marked for deletion are removed, and allows subscribers
        ///   to <see cref="CleanUp.ShouldDelete"/> to remove other nodes.
        /// </summary>
        public override SyntaxNode Visit(SyntaxNode node)
        {
            // Ensures "using Cometary" statements are removed.
            if (node == null || node is UsingDirectiveSyntax @using && @using.Name.ToString().StartsWith(nameof(Cometary)))
                return null;

            // Ensures marked nodes are removed.
            int markedNodesIndex = CleanUp.NodesMarkedForDeletion.IndexOf(node);

            if (markedNodesIndex != -1)
            {
                CleanUp.NodesMarkedForDeletion.RemoveAt(markedNodesIndex);
                return null;
            }

            // Ensures other libraries can also remove all references to themselves.
            var visitors = CleanUp.Visitors;

            for (int i = 0; i < visitors.Count; i++)
            {
                if (visitors[i](node))
                    return null;
            }

            // No need to remove this node, let's continue.
            return base.Visit(node);
        }
    }
}