using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary
{
    partial class CompilationEditor
    {
        /// <summary>
        ///   Registers the specified <see cref="Edit{T}"/> to be used
        ///   to edit a <see cref="ISymbol"/>.
        /// </summary>
        public void RegisterEdit(Edit<ISymbol> symbolEdit)
        {
            SymbolEdits.Add(symbolEdit ?? throw new ArgumentNullException(nameof(symbolEdit)));
        }

        /// <summary>
        ///   Registers the specified <see cref="Edit{T}"/> to be used
        ///   to edit an <see cref="IOperation"/>.
        /// </summary>
        public void RegisterEdit(Edit<IOperation> operationEdit)
        {
            OperationEdits.Add(operationEdit ?? throw new ArgumentNullException(nameof(operationEdit)));
        }

        /// <summary>
        ///   Registers the specified <see cref="Edit{T}"/> to be used
        ///   to edit a <see cref="SyntaxNode"/>.
        /// </summary>
        public void RegisterEdit(Edit<SyntaxNode> nodeEdit)
        {
            SyntaxEdits.Add(nodeEdit ?? throw new ArgumentNullException(nameof(nodeEdit)));
        }

        /// <summary>
        ///   Registers the specified <see cref="Edit{T}"/> to be used
        ///   to edit a <see cref="CSharpCompilation"/>.
        /// </summary>
        public void RegisterEdit(Edit<CSharpCompilation> compilationEdit, CompilationState state)
        {
            if (compilationEdit == null)
                throw new ArgumentNullException(nameof(compilationEdit));

            switch (state)
            {
                case CompilationState.Start:
                    BeforeCompilationEdits.Add(compilationEdit);
                    break;
                case CompilationState.End:
                    AfterCompilationEdits.Add(compilationEdit);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        /// <summary>
        ///   Registers the specified <see cref="Edit{T}"/> to be used
        ///   to edit a <see cref="CSharpSyntaxTree"/>.
        /// </summary>
        public void RegisterEdit(Edit<CSharpSyntaxTree> syntaxTreeEdit)
        {
            SyntaxTreeEdits.Add(syntaxTreeEdit ?? throw new ArgumentNullException(nameof(syntaxTreeEdit)));
        }

        /// <summary>
        ///   Registers the specified <see cref="Edit{T}"/> to be used
        ///   to edit a <see cref="ISymbol"/> tree.
        /// </summary>
        public void RegisterEdit(Edit<IAssemblySymbol> symbolTreeEdit)
        {
            SymbolTreeEdits.Add(symbolTreeEdit ?? throw new ArgumentNullException(nameof(symbolTreeEdit)));
        }

        /// <summary>
        ///   Suppresses all reported diagnostics that match the given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">
        ///   A predicate that takes a <see cref="Diagnostic"/> as input.
        ///   If it returns <see langword="true"/>, the <see cref="Diagnostic"/> will be suppressed.
        /// </param>
        protected void SuppressDiagnostic(Predicate<Diagnostic> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var predicates = Hooks.DiagnosticPredicates;

            if (!predicates.Contains(predicate))
                 predicates.Add(predicate);
        }

        private readonly LightList<Edit<ISymbol>> SymbolEdits                = new LightList<Edit<ISymbol>>();
        private readonly LightList<Edit<IOperation>> OperationEdits          = new LightList<Edit<IOperation>>();
        private readonly LightList<Edit<CSharpCompilation>> BeforeCompilationEdits = new LightList<Edit<CSharpCompilation>>();
        private readonly LightList<Edit<CSharpCompilation>> AfterCompilationEdits = new LightList<Edit<CSharpCompilation>>();
        private readonly LightList<Edit<CSharpSyntaxTree>> SyntaxTreeEdits   = new LightList<Edit<CSharpSyntaxTree>>();
        private readonly LightList<Edit<SyntaxNode>> SyntaxEdits             = new LightList<Edit<SyntaxNode>>();
        private readonly LightList<Edit<IAssemblySymbol>> SymbolTreeEdits    = new LightList<Edit<IAssemblySymbol>>();


        /// <summary>
        ///   Initializes the <see cref="CompilationEditor"/>, allowing it to register edits and overrides.
        /// </summary>
        internal bool TryRegister(
            CometaryManager manager, Action<Diagnostic> reportDiagnostic,
            CSharpCompilation compilation, CancellationToken token)
        {
            // Initialize the editor.
            ReportDiagnostic = reportDiagnostic;

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
                AddUnlessEmpty(manager.BeforeCompilationPipeline, BeforeCompilationEdits);
                AddUnlessEmpty(manager.AfterCompilationPipeline, AfterCompilationEdits);
                AddUnlessEmpty(manager.OperationPipeline, OperationEdits);
                AddUnlessEmpty(manager.SyntaxPipeline, SyntaxEdits);
                AddUnlessEmpty(manager.SymbolPipeline, SymbolEdits);
                AddUnlessEmpty(manager.SyntaxTreePipeline, SyntaxTreeEdits);
                AddUnlessEmpty(manager.SymbolTreePipeline, SymbolTreeEdits);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///   Unregisters all callbacks registered in <see cref="Initialize"/>.
        /// </summary>
        internal void UnregisterAll(CometaryManager manager)
        {
            manager.BeforeCompilationPipeline.RemoveRange(BeforeCompilationEdits);
            manager.AfterCompilationPipeline.RemoveRange(AfterCompilationEdits);
            manager.OperationPipeline.RemoveRange(OperationEdits);
            manager.SymbolPipeline.RemoveRange(SymbolEdits);
            manager.SyntaxPipeline.RemoveRange(SyntaxEdits);
            manager.SymbolTreePipeline.RemoveRange(SymbolTreeEdits);
            manager.SyntaxTreePipeline.RemoveRange(SyntaxTreeEdits);
        }
    }
}
