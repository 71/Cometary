using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

[assembly: InternalsVisibleTo("Cometary, PublicKey=0024000004800000940000000602000000240000525341310004000001000100ddc50907e7882cd6af3432d5c3ba2f9a257e9ea6602df0e06098aba23eed5d650e4adb8aaefcee05afd5a70c43fe058b4d6dbecddf48a99ff9729f6a9968e8915677fa29a24a3a7293788c7de96040fb40d6eaf7f2b24320ec43624189d3a66250c5c0d31823343feb6e6fa9787f6e4961f8c84af6b59993c2e5d1c981b82bcb")]
[assembly: InternalsVisibleTo("Cometary.Remote, PublicKey=0024000004800000940000000602000000240000525341310004000001000100ddc50907e7882cd6af3432d5c3ba2f9a257e9ea6602df0e06098aba23eed5d650e4adb8aaefcee05afd5a70c43fe058b4d6dbecddf48a99ff9729f6a9968e8915677fa29a24a3a7293788c7de96040fb40d6eaf7f2b24320ec43624189d3a66250c5c0d31823343feb6e6fa9787f6e4961f8c84af6b59993c2e5d1c981b82bcb")]

namespace Cometary
{
    /// <summary>
    /// Provides utilities for logging, and syntax modifications.
    /// </summary>
    public static class Meta
    {
        internal static Action<object, object, string, SyntaxNode> LogWarningCore;
        internal static Action<object, object, string, SyntaxNode> LogMessageCore;
        internal static Action<object, object, string, SyntaxNode> LogDebugCore;
        internal static Func<object, object> WorkspaceCore;
        internal static Func<object, object> ProjectCore;
        internal static Func<object, CSharpCompilation> CompilationCore;

        internal static Func<Assembly> GetCallingAssemblyCore;
        internal static Func<int> GetAssemblyIDCore;
        internal static Func<object> GetProcessorCore;

        /// <summary>
        /// Gets the workspace associated with the current compilation.
        /// </summary>
        /// <remarks>
        /// This property returns an <see cref="object"/> to avoid forcing users
        /// to reference <c>Microsoft.CodeAnalysis.Workspaces</c>.
        /// </remarks>
        public static object Workspace
        {
            get
            {
                EnsureCTFE();

                return WorkspaceCore(GetProcessorCore());
            }
        }

        /// <summary>
        /// Gets the project associated with the current compilation.
        /// </summary>
        /// <remarks>
        /// This property returns an <see cref="object"/> to avoid forcing users
        /// to reference <c>Microsoft.CodeAnalysis.Workspaces</c>.
        /// </remarks>
        public static object Project
        {
            get
            {
                EnsureCTFE();

                return ProjectCore(GetProcessorCore());
            }
        }

        /// <summary>
        /// Gets the <see cref="CSharpCompilation"/> associated with the currently running program.
        /// </summary>
        public static CSharpCompilation Compilation
        {
            get
            {
                EnsureCTFE();

                return CompilationCore(GetProcessorCore());
            }
        }

        /// <summary>
        /// Gets the <see cref="Assembly"/> being processed.
        /// </summary>
        public static Assembly TargetAssembly
        {
            get
            {
                EnsureCTFE();

                return GetCallingAssemblyCore();
            }
        }

        /// <summary>
        /// Gets the unique ID associated with the assembly to process.
        /// </summary>
        public static int ID
        {
            get
            {
                EnsureCTFE();

                return GetAssemblyIDCore();
            }
        }

        private static readonly Dictionary<SyntaxTree, int> TreesIDs = new Dictionary<SyntaxTree, int>();

        /// <summary>
        /// Ensures that the program is being compiled.
        /// </summary>
        internal static void EnsureCTFE()
        {
            if (!CTFE)
                throw new InvalidOperationException("This operation can only be performed during compilation.");
        }

        /// <summary>
        /// Gets whether or not the running assembly is being compiled.
        /// </summary>
        public static bool CTFE { get; internal set; }

        /// <summary>
        /// Gets a unique ID related to the given syntax tree.
        /// </summary>
        public static int GetID(SyntaxTree syntaxTree)
        {
            int index = Compilation.SyntaxTrees.IndexOf(syntaxTree);

            if (index != -1)
                return index;

            for (index = 0; index < Compilation.SyntaxTrees.Length; index++)
            {
                if (Compilation.SyntaxTrees[index].FilePath == syntaxTree.FilePath)
                    return index;
            }

            return TreesIDs.TryGetValue(syntaxTree, out index)
                ? index
                : TreesIDs[syntaxTree] = TreesIDs.Count + Compilation.SyntaxTrees.Length;
        }

        /// <summary>
        /// Gets the syntax tree associated with the given <paramref name="id"/>.
        /// </summary>
        public static SyntaxTree GetTree(int id)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException(nameof(id));
            if (id < Compilation.SyntaxTrees.Length)
                return Compilation.SyntaxTrees[id];

            foreach (var kvp in TreesIDs)
            {
                if (kvp.Value == id)
                    return kvp.Key;
            }

            return null;
        }

        #region Logging

        /// <summary>
        /// Logs a message to the build process.
        /// </summary>
        public static void LogMessage(string message, SyntaxNode node)
        {
            EnsureCTFE();

            LogMessageCore(GetProcessorCore(), null, message, node);
        }

        /// <summary>
        /// Logs a warning message to the build process.
        /// </summary>
        public static void LogWarning(string warning, SyntaxNode node)
        {
            EnsureCTFE();

            LogDebugCore(GetProcessorCore(), null, warning, node);
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

            LogDebugCore(GetProcessorCore(), null, message, node);
        }

        /// <summary>
        /// Logs a message to the build process.
        /// </summary>
        public static void LogMessage(object sender, string message, SyntaxNode node)
        {
            EnsureCTFE();

            LogMessageCore(GetProcessorCore(), sender, message, node);
        }

        /// <summary>
        /// Logs a warning message to the build process.
        /// </summary>
        public static void LogWarning(object sender, string warning, SyntaxNode node)
        {
            EnsureCTFE();

            LogWarningCore(GetProcessorCore(), sender, warning, node);
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

            LogDebugCore(GetProcessorCore(), sender, message, node);
        }

        /// <summary>
        /// Logs a message to the build process.
        /// </summary>
        public static void LogMessage(string message)
        {
            EnsureCTFE();

            LogMessageCore(GetProcessorCore(), null, message, null);
        }

        /// <summary>
        /// Logs a warning message to the build process.
        /// </summary>
        public static void LogWarning(string warning)
        {
            EnsureCTFE();

            LogWarningCore(GetProcessorCore(), null, warning, null);
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

            LogDebugCore(GetProcessorCore(), null, message, null);
        }

        /// <summary>
        /// Logs a message to the build process.
        /// </summary>
        public static void LogMessage(object sender, string message)
        {
            EnsureCTFE();

            LogMessageCore(GetProcessorCore(), sender, message, null);
        }

        /// <summary>
        /// Logs a warning message to the build process.
        /// </summary>
        public static void LogWarning(object sender, string warning)
        {
            EnsureCTFE();

            LogWarningCore(GetProcessorCore(), sender, warning, null);
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

            LogDebugCore(GetProcessorCore(), sender, message, null);
        }
        #endregion
    }
}
