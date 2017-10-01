﻿using System;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary
{
    /// <summary>
    ///   Mutable object that represents the current state of the process,
    ///   and contains various references to objects used to modify the emitted assembly.
    /// </summary>
    public sealed class CometaryState
    {
        /// <summary>
        ///   Gets the <see cref="CometaryOptions"/> set for the process.
        /// </summary>
        public CometaryOptions Options => CometaryOptions.Options;

        /// <summary>
        ///   Gets the <see cref="Stream"/> containing the emitted assembly.
        /// </summary>
        public MemoryStream AssemblyStream { get; }

        /// <summary>
        ///   Gets the <see cref="Stream"/> containing the symbols of the emitted assembly.
        /// </summary>
        public MemoryStream SymbolsStream { get; }

        /// <summary>
        ///   Gets the previously emitted <see cref="System.Reflection.Assembly"/>.
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        ///   Gets or sets the <see cref="CSharpCompilation"/> associated with the
        ///   project to modify.
        /// </summary>
        public CSharpCompilation Compilation { get; set; }

        /// <summary>
        ///   Event triggered after emitting the finished assembly.
        /// </summary>
        public event Hook Emitted;

        internal CometaryState(MemoryStream aStream, MemoryStream sStream, Assembly assembly, CSharpCompilation compilation)
        {
            Compilation = compilation;
            Assembly = assembly;
            AssemblyStream = aStream;
            SymbolsStream = sStream;
        }

        /// <summary>
        ///   Apply the given <paramref name="projector"/> on all the syntax trees
        ///   of the compilation.
        /// </summary>
        public void ChangeSyntaxTrees(Func<CSharpSyntaxTree, CSharpSyntaxTree> projector)
        {
            var compilation = Compilation;
            int changes = 0;

            for (int i = 0; i < compilation.SyntaxTrees.Length; i++)
            {
                CSharpSyntaxTree originalSyntaxTree = (CSharpSyntaxTree)compilation.SyntaxTrees[i];
                CSharpSyntaxTree syntaxTree = projector(originalSyntaxTree);

                if (syntaxTree == null)
                    compilation = compilation.RemoveSyntaxTrees(originalSyntaxTree);
                else if (syntaxTree != originalSyntaxTree)
                    compilation = compilation.ReplaceSyntaxTree(originalSyntaxTree, syntaxTree);
                else
                    continue;

                changes++;
            }

            if (changes != 0)
                Compilation = compilation;
        }

        /// <summary>
        ///   Indicates that the assembly has been emitted a second time,
        ///   thus triggering the <see cref="Emitted"/> event.
        /// </summary>
        internal void OnEmitted() => Emitted?.Invoke(this);
    }
}