using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.CodeAnalysis.CSharp;
using Mono.Cecil;

namespace Cometary
{
    using Rewriting;

    /// <summary>
    /// Provides the ability to print IL code directly.
    /// </summary>
    public static class IL
    {
        /// <summary>
        /// A list of actions to execute on saving.
        /// </summary>
        internal static readonly List<Action<AssemblyDefinition>> Actions = new List<Action<AssemblyDefinition>>();

        /// <summary>
        /// Gets the assembly definition of the
        /// assembly being processed.
        /// </summary>
        internal static AssemblyDefinition Assembly { get; set; }

        /// <summary>
        /// Gets the main module definition of the assembly being processed.
        /// </summary>
        internal static ModuleDefinition Module => Assembly.MainModule;

        /// <summary>
        /// Registers the given <paramref name="action"/> to be executed
        /// when the assembly is saved.
        /// </summary>
        public static void Do(Action<AssemblyDefinition> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            Actions.Add(action);
        }

        /// <summary>
        /// Emits the given <paramref name="opcode"/>.
        /// </summary>
        public static void Emit(OpCode opcode, object obj = null, Quote quote = null)
        {
            // TODO: Things.
            quote.Unquote(SyntaxFactory.EmptyStatement());
        }
    }
}
