using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cometary
{
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
        ///   List containg all nodes that should be removed by the <see cref="CleanUpRewriter"/>.
        /// </summary>
        internal static List<SyntaxNode> NodesMarkedForDeletion = new List<SyntaxNode>();

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
            if (node is ExpressionSyntax)
                throw new InvalidOperationException("Cannot mark an Expression for deletion.");

            NodesMarkedForDeletion.Add(node);
        }


        #region Options

        private const string OptionsKey = "CLEAN_UP";

        /// <summary>
        ///   Gets whether or not the automatic clean up is enabled.
        /// </summary>
        public static bool ShouldCleanUp => CometaryOptions.Options.GetOrDefault(OptionsKey, false);

        /// <summary>
        ///   Instructs Cometary to clean up all references to itself at the end of the process.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static CometaryConfigurator Clean(this CometaryConfigurator configurator)
        {
            return configurator
                .AddHook(CleanHook)
                .AddData(OptionsKey, true);
        }

        private static void CleanHook(CometaryState state)
        {
            var compilation = state.Compilation;

            state.Compilation = compilation
                .RemoveReferences(compilation.References.Where(x => x.Display.StartsWith(nameof(Cometary))));

            state.ChangeSyntaxTrees(ChangeSyntaxTree);
        }

        private static CSharpSyntaxTree ChangeSyntaxTree(CSharpSyntaxTree tree)
        {
            SyntaxNode root = tree.GetRoot();
            SyntaxNode changedRoot = new CleanUpRewriter().Visit(root);

            if (root == changedRoot)
                return tree;

            return tree.WithRootAndOptions(changedRoot, tree.Options) as CSharpSyntaxTree;
        }
        #endregion
    }
}
