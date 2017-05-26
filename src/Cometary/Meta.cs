using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

[assembly: InternalsVisibleTo("Cometary.Remote")]

namespace Cometary
{
    using Rewriting;

    /// <summary>
    /// Provides utilities for logging, and syntax modifications.
    /// </summary>
    public static class Meta
    {
        internal static readonly Dictionary<SyntaxTree, SemanticModel> models = new Dictionary<SyntaxTree, SemanticModel>();

        internal static readonly List<SyntaxNode> changedNodes = new List<SyntaxNode>();
        internal static readonly List<Func<SyntaxNode, SyntaxNode>> changedNodesFuncs = new List<Func<SyntaxNode, SyntaxNode>>();

        internal static Action<object, string, SyntaxNode> LogWarningInternal;
        internal static Action<object, string, SyntaxNode> LogMessageInternal;
        internal static Action<object, string, SyntaxNode> LogDebugInternal;

        /// <summary>
        /// Ensures that the program is being compiled.
        /// </summary>
        internal static void EnsureCTFE()
        {
            if (!CTFE)
                throw new InvalidOperationException("This operation can only be performed during compilation.");
        }

        /// <summary>
        /// Gets the <see cref="CSharpCompilation"/> associated with the currently running program.
        /// </summary>
        public static CSharpCompilation Compilation { get; internal set; }

        /// <summary>
        /// Gets whether or not the running assembly is being compiled.
        /// </summary>
        public static bool CTFE { get; internal set; }

        /// <summary>
        /// Gets the <see cref="SemanticModel"/> associated with the given
        /// <see cref="SyntaxTree"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static SemanticModel Model(this SyntaxTree syntaxTree)
        {
            if (syntaxTree == null)
                throw new ArgumentNullException(nameof(syntaxTree));

            EnsureCTFE();

            if (!models.TryGetValue(syntaxTree, out SemanticModel model))
                models[syntaxTree] = model = Compilation.GetSemanticModel(syntaxTree, true);

            return model;
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

            changedNodes.Add(old);
            changedNodesFuncs.Add(x => @new(x as T));
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

        #region Logging
        /// <summary>
        /// Logs a message to the build process.
        /// </summary>
        public static void LogMessage(string message, SyntaxNode node)
        {
            EnsureCTFE();

            LogMessageInternal(null, message, node);
        }

        /// <summary>
        /// Logs a warning message to the build process.
        /// </summary>
        public static void LogWarning(string warning, SyntaxNode node)
        {
            EnsureCTFE();

            LogWarningInternal(null, warning, node);
        }

        /// <summary>
        /// Logs a debug message to the build process.
        /// <para>
        /// This method has no effect in release builds.
        /// </para>
        /// </summary>
        [Conditional("DEBUG")]
        public static void LogDebug(string message, SyntaxNode node)
        {
            EnsureCTFE();

            LogDebugInternal(null, message, node);
        }

        /// <summary>
        /// Logs a message to the build process.
        /// </summary>
        public static void LogMessage(object sender, string message, SyntaxNode node)
        {
            EnsureCTFE();

            LogMessageInternal(sender, message, node);
        }

        /// <summary>
        /// Logs a warning message to the build process.
        /// </summary>
        public static void LogWarning(object sender, string warning, SyntaxNode node)
        {
            EnsureCTFE();

            LogWarningInternal(sender, warning, node);
        }

        /// <summary>
        /// Logs a debug message to the build process.
        /// <para>
        /// This method has no effect in release builds.
        /// </para>
        /// </summary>
        [Conditional("DEBUG")]
        public static void LogDebug(object sender, string message, SyntaxNode node)
        {
            EnsureCTFE();

            LogDebugInternal(sender, message, node);
        }

        /// <summary>
        /// Logs a message to the build process.
        /// </summary>
        public static void LogMessage(string message)
        {
            EnsureCTFE();

            LogMessageInternal(null, message, null);
        }

        /// <summary>
        /// Logs a warning message to the build process.
        /// </summary>
        public static void LogWarning(string warning)
        {
            EnsureCTFE();

            LogWarningInternal(null, warning, null);
        }

        /// <summary>
        /// Logs a debug message to the build process.
        /// <para>
        /// This method has no effect in release builds.
        /// </para>
        /// </summary>
        [Conditional("DEBUG")]
        public static void LogDebug(string message)
        {
            EnsureCTFE();

            LogDebugInternal(null, message, null);
        }

        /// <summary>
        /// Logs a message to the build process.
        /// </summary>
        public static void LogMessage(object sender, string message)
        {
            EnsureCTFE();

            LogMessageInternal(sender, message, null);
        }

        /// <summary>
        /// Logs a warning message to the build process.
        /// </summary>
        public static void LogWarning(object sender, string warning)
        {
            EnsureCTFE();

            LogWarningInternal(sender, warning, null);
        }

        /// <summary>
        /// Logs a debug message to the build process.
        /// <para>
        /// This method has no effect in release builds.
        /// </para>
        /// </summary>
        [Conditional("DEBUG")]
        public static void LogDebug(object sender, string message)
        {
            EnsureCTFE();

            LogDebugInternal(sender, message, null);
        }
        #endregion

        #region Utils
        /// <summary>
        /// Inserts the given <see cref="string"/> in the syntax tree, as if it
        /// were an actual statement.
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public static void Mixin(this string str, Quote quote = null)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            EnsureCTFE();

            quote.Unquote(SyntaxFactory.ParseStatement(str));
        }

        /// <summary>
        /// Inserts the given <see cref="string"/> in the syntax tree, as if it
        /// were an actual expression.
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public static T Mixin<T>(this string str, Quote quote = null)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            EnsureCTFE();

            return quote.Unquote<T>(SyntaxFactory.ParseExpression(str));
        }


        #region With
        /// <summary>
        /// Helper used to do nested calls to With* immutable methods.
        /// </summary>
        public static T With<T, TChanged>(this T node, Expression<Func<T, TChanged>> sub, Func<TChanged, TChanged> selector)
            where T : SyntaxNode
            where TChanged : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            if (sub == null)
                throw new ArgumentNullException(nameof(sub));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            TChanged replacedNode = sub.Compile()(node);
            TChanged updatedNode  = selector(replacedNode);

            if (updatedNode == null)
                throw new ArgumentNullException(nameof(updatedNode), $"The {nameof(selector)} cannot return null.");

            return node.ReplaceNode(replacedNode, updatedNode);
        }

        /// <summary>
        /// Helper used to do nested calls to With* immutable methods.
        /// </summary>
        public static T With<T>(this T node, Expression<Func<T, SyntaxToken>> sub, Func<SyntaxToken, SyntaxToken> selector)
            where T : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            if (sub == null)
                throw new ArgumentNullException(nameof(sub));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            SyntaxToken replacedNode = sub.Compile()(node);
            SyntaxToken updatedNode = selector(replacedNode);

            if (updatedNode == null)
                throw new ArgumentNullException(nameof(updatedNode), $"The {nameof(selector)} cannot return null.");

            return node.ReplaceToken(replacedNode, updatedNode);
        }
        #endregion
        #endregion
    }
}
