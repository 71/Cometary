﻿using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary
{
    partial class CompilationEditor
    {
        /// <summary>
        ///   Event representing a pipeline of edits to apply to the <see cref="ISourceAssemblySymbol"/>
        ///   generated by the previously edited <see cref="CSharpCompilation"/>.
        /// </summary>
        public event Edit<ISourceAssemblySymbol> AssemblyPipeline
        {
            add
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (AssemblyEdits.Contains(value))
                    return;

                AssemblyEdits.Add(value);
            }

            remove => AssemblyEdits.Remove(value ?? throw new ArgumentNullException(nameof(value)));
        }

        /// <summary>
        ///   Event representing a pipeline of edits to apply to a <see cref="CSharpCompilation"/>
        ///   before it is emitted.
        /// </summary>
        public event Edit<CSharpCompilation> CompilationPipeline
        {
            add
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (CompilationEdits.Contains(value))
                    return;

                CompilationEdits.Add(value);
            }

            remove => CompilationEdits.Remove(value ?? throw new ArgumentNullException(nameof(value)));
        }

        private readonly LightList<Edit<CSharpCompilation>> CompilationEdits  = new LightList<Edit<CSharpCompilation>>();
        private readonly LightList<Edit<ISourceAssemblySymbol>> AssemblyEdits = new LightList<Edit<ISourceAssemblySymbol>>();

        /// <summary>
        ///   Initializes the <see cref="CompilationEditor"/>, allowing it to register edits and overrides.
        /// </summary>
        internal bool TryRegister(
            CompilationProcessor manager, Action<Diagnostic> reportDiagnostic,
            CSharpCompilation compilation, CancellationToken token,
            out CompilationEditor[] children)
        {
            // Initialize the editor.
            ReportDiagnostic = reportDiagnostic;
            SharedStorage = manager.SharedStorage;

            void AddUnlessEmpty<T>(FlatteningList<T> list, IReadOnlyList<T> items)
            {
                if (items.Count != 0)
                    list.AddRange(items);
            }

            // Initialize the editor.
            try
            {
                Initialize(compilation, token);

                // Register all callbacks
                AddUnlessEmpty(manager.CompilationPipeline, CompilationEdits);
                AddUnlessEmpty(manager.AssemblyPipeline, AssemblyEdits);

                children = Children;

                return true;
            }
            catch
            {
                children = null;

                return false;
            }
        }

        /// <summary>
        ///   Unregisters all callbacks registered in <see cref="Initialize"/>.
        /// </summary>
        internal void UnregisterAll(CompilationProcessor manager)
        {
            manager.CompilationPipeline.RemoveRange(CompilationEdits);
            manager.AssemblyPipeline.RemoveRange(AssemblyEdits);
        }
    }
}
