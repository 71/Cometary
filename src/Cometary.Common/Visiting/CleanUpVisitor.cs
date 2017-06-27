using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cometary
{
    using Common;

    /// <summary>
    ///   <see cref="LightAssemblyVisitor"/> that cleans up references to
    ///   assemblies related to Cometary.
    /// </summary>
    internal sealed class CleanUpVisitor : LightAssemblyVisitor
    {
        /// <inheritdoc />
        /// <remarks>
        ///   The tree will be rewritten if, and only if, <see cref="CleanUp.ShouldCleanUp" /> is <see langword="true"/>.
        /// </remarks>
        public override bool RewritesTree => CometaryAttribute.Instance.CleanUp;

        /// <inheritdoc />
        /// <remarks>
        ///   This method always gives a higher priority to other visitors, in order for this
        ///   rewriter to be called last.
        /// </remarks>
        public override int CompareTo(LightAssemblyVisitor other) => 1;

        /// <summary>
        ///   Ensures Cometary-related usings are removed.
        /// </summary>
        public override SyntaxNode VisitUsingDirective(UsingDirectiveSyntax node)
        {
            if (node.Name.ToString().StartsWith(nameof(Cometary)))
                return null;

            return node;
        }

        /// <summary>
        ///   Ensures nodes marked for deletion are removed, and allows subscribers
        ///   to <see cref="CleanUp.ShouldDelete"/> to remove other nodes.
        /// </summary>
        public override SyntaxNode Visit(SyntaxNode node)
        {
            if (node == null)
                return null;

            int markedNodesIndex = CleanUp.NodesMarkedForDeletion.IndexOf(node);

            if (markedNodesIndex != -1)
            {
                CleanUp.NodesMarkedForDeletion.RemoveAt(markedNodesIndex);
                return null;
            }

            var visitors = CleanUp.Visitors;

            for (int i = 0; i < visitors.Count; i++)
            {
                if (visitors[i](node))
                    return null;
            }

            return base.Visit(node);
        }

        /// <summary>
        ///  Removes Cometary from the referenced assemblies.
        /// </summary>
        public override CSharpCompilation Visit(Assembly assembly, CSharpCompilation compilation)
        {
            return !RewritesTree ? compilation : compilation.RemoveReferences(compilation.References.Where(x => x.Display.StartsWith(nameof(Cometary))));
        }
    }

    /// <summary>
    /// <para>
    ///   Provides the ability to mark members for deletion,
    ///   in order to clean up a project of all members related
    ///   to Cometary and related libraries.
    /// </para>
    /// <para>
    ///   Please note that all the members declared in the class
    ///   will have no effect, except if <see cref="ShouldCleanUp"/>
    ///   is <see langword="true"/>.
    /// </para>
    /// </summary>
    public static class CleanUp
    {
        /// <summary>
        ///   List containing all visitors added to the clean-up process.
        /// </summary>
        internal static List<Func<SyntaxNode, bool>> Visitors = new List<Func<SyntaxNode, bool>>();

        /// <summary>
        ///   List containg all nodes that should be removed by the <see cref="CleanUpVisitor"/>.
        /// </summary>
        internal static List<SyntaxNode> NodesMarkedForDeletion = new List<SyntaxNode>();

        /// <summary>
        ///   Gets or sets whether or not the clean up is enabled.
        /// </summary>
        public static bool ShouldCleanUp => CometaryAttribute.Instance.CleanUp;

        /// <summary>
        ///   Event invoked that returns whether or
        ///   not the given node is to be deleted.
        /// </summary>
        public static event Func<SyntaxNode, bool> ShouldDelete
        {
            add => Visitors.Add(value);
            remove => Visitors.Remove(value);
        }

        /// <summary>
        /// <para>
        ///   Marks the given <paramref name="node"/> for deletion.
        /// </para>
        /// <para>
        ///   When Cometary is done, it will be removed.
        /// </para>
        /// </summary>
        public static void MarkForDeletion(SyntaxNode node)
        {
            if (node == null || NodesMarkedForDeletion.Contains(node))
                return;

            NodesMarkedForDeletion.Add(node);
        }
    }
}
