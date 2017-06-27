using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary
{
    using Extensions;

    /// <summary>
    /// Provides utilities related to meta-programming.
    /// </summary>
    public static class MetaExtensions
    {
        internal static readonly Dictionary<int, SemanticModel> models = new Dictionary<int, SemanticModel>();

        internal static readonly Dictionary<int, List<(SyntaxNode, Func<SyntaxNode, SyntaxNode>)>> Changes
            = new Dictionary<int, List<(SyntaxNode, Func<SyntaxNode, SyntaxNode>)>>();

        /// <summary>
        /// Gets the <see cref="SemanticModel"/> associated with the given
        /// <see cref="SyntaxTree"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static SemanticModel Model(this SyntaxTree syntaxTree)
        {
            if (syntaxTree == null)
                throw new ArgumentNullException(nameof(syntaxTree));

            Meta.EnsureCompiling();

            int id = Meta.GetID(syntaxTree);

            if (models.TryGetValue(id, out SemanticModel model))
                return model;

            try
            {
                return models[id] = Meta.Compilation.GetSemanticModel(syntaxTree, true);
            }
            catch
            {
                return null;
            }
        }

        #region Modifying output
        /// <summary>
        /// Registers a node replacement: the <paramref name="old"/>
        /// node will be replaced by the <paramref name="new"/> node
        /// during compilation.
        /// </summary>
        [DebuggerStepThrough]
        public static void Replace<T>(this T old, Func<T, T> @new) where T : SyntaxNode
        {
            if (old == null)
                throw new ArgumentNullException(nameof(old));
            if (@new == null)
                throw new ArgumentNullException(nameof(@new));

            if (!Changes.TryGetValue(Meta.GetID(old.SyntaxTree), out List<(SyntaxNode, Func<SyntaxNode, SyntaxNode>)> list))
                Changes[Meta.GetID(old.SyntaxTree)] = list = new List<(SyntaxNode, Func<SyntaxNode, SyntaxNode>)>();

            list.Add((old, x => @new(x as T)));
        }

        /// <summary>
        /// Registers the given <paramref name="node"/> for deletion during compilation.
        /// </summary>
        [DebuggerStepThrough]
        public static void Delete(this SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            node.Parent.Replace(x => x.RemoveNode(node, SyntaxRemoveOptions.KeepNoTrivia));
        }
        #endregion

        #region With
        /// <summary>
        /// Helper used to do nested calls to With* immutable methods.
        /// </summary>
        public static T With<T, TChanged>(this T node, Func<T, TChanged> sub, Func<TChanged, TChanged> selector)
            where T : SyntaxNode
            where TChanged : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            if (sub == null)
                throw new ArgumentNullException(nameof(sub));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            TChanged replacedNode = sub(node);
            TChanged updatedNode  = selector(replacedNode);

            if (updatedNode == null)
                throw new ArgumentNullException(nameof(updatedNode), $"The {nameof(selector)} cannot return null.");

            return node.ReplaceNode(replacedNode, updatedNode);
        }

        /// <summary>
        /// Helper used to do nested calls to With* immutable methods.
        /// </summary>
        public static T With<T>(this T node, Func<T, SyntaxToken> sub, Func<SyntaxToken, SyntaxToken> selector)
            where T : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            if (sub == null)
                throw new ArgumentNullException(nameof(sub));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            SyntaxToken replacedNode = sub(node);
            SyntaxToken updatedNode = selector(replacedNode);

            if (updatedNode == null)
                throw new ArgumentNullException(nameof(updatedNode), $"The {nameof(selector)} cannot return null.");

            return node.ReplaceToken(replacedNode, updatedNode);
        }
        #endregion
    }
}
