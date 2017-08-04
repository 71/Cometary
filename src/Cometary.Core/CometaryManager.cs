using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Cometary
{
    /// <summary>
    ///   
    /// </summary>
    internal sealed class CometaryManager : IDisposable
    {
        /// <summary>
        ///   Gets a <see cref="DiagnosticDescriptor"/> describing an unexpected exception
        ///   thrown by a <see cref="CompilationEditor"/>.
        /// </summary>
        public static DiagnosticDescriptor EditorThrown { get; }
            = new DiagnosticDescriptor(Common.DiagnosticsPrefix + "ET01", "Unexpected error", "Exception thrown by the '{0}' editor: '{1}'", "Editing", DiagnosticSeverity.Error, true);

        /// <summary>
        ///   Gets a <see cref="DiagnosticDescriptor"/> describing an unexpected exception
        ///   encountered when modifying a <see cref="CSharpCompilation"/>.
        /// </summary>
        public static DiagnosticDescriptor EditThrown { get; }
            = new DiagnosticDescriptor(Common.DiagnosticsPrefix + "ET02", "Unexpected error", "Exception thrown during the {0} step: '{1}'. Stack trace: \n{2}.", "Editing", DiagnosticSeverity.Error, true);

        /// <summary>
        /// 
        /// </summary>
        public List<CompilationEditor> Editors { get; }

        /// <summary>
        /// 
        /// </summary>
        public FlatteningList<Edit<CSharpCompilation>> BeforeCompilationPipeline { get; } = new FlatteningList<Edit<CSharpCompilation>>();

        /// <summary>
        /// 
        /// </summary>
        public FlatteningList<Edit<CSharpCompilation>> AfterCompilationPipeline { get; } = new FlatteningList<Edit<CSharpCompilation>>();

        /// <summary>
        /// 
        /// </summary>
        public FlatteningList<Edit<ISymbol>> SymbolPipeline { get; } = new FlatteningList<Edit<ISymbol>>();

        /// <summary>
        /// 
        /// </summary>
        public FlatteningList<Edit<IOperation>> OperationPipeline { get; } = new FlatteningList<Edit<IOperation>>();

        /// <summary>
        /// 
        /// </summary>
        public FlatteningList<Edit<CSharpSyntaxTree>> SyntaxTreePipeline { get; } = new FlatteningList<Edit<CSharpSyntaxTree>>();

        /// <summary>
        /// 
        /// </summary>
        public FlatteningList<Edit<SyntaxNode>> SyntaxPipeline { get; } = new FlatteningList<Edit<SyntaxNode>>();

        /// <summary>
        /// 
        /// </summary>
        public FlatteningList<Edit<IAssemblySymbol>> SymbolTreePipeline { get; } = new FlatteningList<Edit<IAssemblySymbol>>();

        /// <summary>
        ///   List of <see cref="Exception"/>s encountered during initialization,
        ///   before diagnostics could be added.
        /// </summary>
        private readonly ImmutableArray<Exception>.Builder initializationExceptions = ImmutableArray.CreateBuilder<Exception>();

        private CometaryManager(IEnumerable<CompilationEditor> editors)
        {
            Editors = new List<CompilationEditor>(editors);
        }

        /// <summary>
        ///   Creates a new <see cref="CometaryManager"/>.
        /// </summary>
        public static CometaryManager Create(params CompilationEditor[] editors)
        {
            Debug.Assert(editors != null);
            Debug.Assert(editors.All(x => x != null));

            return new CometaryManager(editors);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RegisterAttributes(IAssemblySymbol assembly)
        {
            // Find all used editors, and register 'em
            foreach (var attr in assembly.GetAttributes())
            {
                INamedTypeSymbol attrType = attr.AttributeClass;

                // Make sure the attribute inherits CometaryAttribute
                for (;;)
                {
                    attrType = attrType.BaseType;

                    if (attrType == null)
                        goto NextAttribute;
                    if (attrType.Name == nameof(CometaryAttribute))
                        break;
                }

                // We got here: we have a cometary attribute
                IEnumerable<CompilationEditor> editors;

                try
                {
                    editors = InitializeAttribute(attr);
                }
                catch (Exception e)
                {
                    initializationExceptions.Add(e);
                    continue;
                }

                foreach (CompilationEditor editor in editors)
                {
                    if (editor == null)
                        continue;

                    Register(editor);
                }

                NextAttribute:;
            }

            Console.WriteLine($"We now have {Editors.Count.ToString()} editor(s)");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerable<CompilationEditor> InitializeAttribute(AttributeData data)
        {
            return data.Construct<CometaryAttribute>().Initialize();
        }

        /// <summary>
        ///   Registers the given <see cref="CompilationEditor"/> among the known
        ///   <see cref="Editors"/>.
        /// </summary>
        public void Register(CompilationEditor editor)
        {
            if (editor == null)
                throw new ArgumentNullException(nameof(editor));

            Editors.Add(editor);
        }

        /// <summary>
        /// 
        /// </summary>
        public CSharpCompilation EditCompilation(CSharpCompilation compilation, Action<Diagnostic> addDiagnostic, CancellationToken cancellationToken)
        {
            CSharpCompilation modified = compilation;
            List<CompilationEditor> editors = Editors;
            bool isSuccess = true;

            // Log all previously encountered exceptions
            Exception[] exceptions = initializationExceptions.ToArray();

            for (int i = 0; i < exceptions.Length; i++)
            {
                addDiagnostic(Diagnostic.Create(EditThrown, Location.None, exceptions[i].ToString()));
            }

            // Initialize all editors
            for (int i = 0; i < editors.Count; i++)
            {
                CompilationEditor editor = editors[i];

                try
                {
                    editor.TryRegister(this, addDiagnostic, modified, cancellationToken);
                }
                catch (Exception e)
                {
                    while (e is TargetInvocationException tie)
                        e = tie.InnerException;

                    addDiagnostic(Diagnostic.Create(EditorThrown, Location.None, editor?.GetType().ToString() ?? "Unknown editor", e.Message));
                    isSuccess = false;

                    goto Uninitialize;
                }
            }

            // Run the compilation
            try
            {
                RunCompilation(ref modified, editors, cancellationToken);
            }
            catch (Exception e)
            {
                while (e is TargetInvocationException tie)
                    e = tie.InnerException;

                isSuccess = false;
                addDiagnostic(Diagnostic.Create(EditThrown, Location.None, "Running", e.Message, e.Source));
            }

            // Uninitialize editors
            Uninitialize:
            for (int i = 0; i < editors.Count; i++)
            {
                editors[i].UnregisterAll(this);
            }

            // Copy new fields to original instance
            return isSuccess ? modified : compilation;
        }

        /// <summary>
        /// 
        /// </summary>
        private void RunCompilation(ref CSharpCompilation compilation, IReadOnlyList<CompilationEditor> editors, CancellationToken cancellationToken)
        {
            // Edit compilation 'before'
            for (int i = 0; i < editors.Count; i++)
                editors[i].State = CompilationEditor.CompilationState.Start;

            InvokeAll(ref compilation, BeforeCompilationPipeline, cancellationToken);

            SyntaxTreeRewriter rewriter = new SyntaxTreeRewriter(node =>
            {
                foreach (var edit in SyntaxPipeline)
                {
                    node = edit(node, cancellationToken) ?? node;
                }

                return node;
            });

            // Edit syntax trees
            var syntaxTrees = compilation.SyntaxTrees.ToBuilder();

#if DEBUG
            if (compilation.GetTypeByMetadataName("ProcessedByCometary") == null)
                syntaxTrees.Add(SyntaxFactory.ParseSyntaxTree("internal static class ProcessedByCometary { }"));
#endif

            if (SyntaxTreePipeline.Count == 0)
                goto SyntaxNodes;

            for (int i = 0; i < syntaxTrees.Count; i++)
            {
                CSharpSyntaxTree tree = syntaxTrees[i] as CSharpSyntaxTree;

                if (tree == null)
                    continue;

                foreach (var edit in SyntaxTreePipeline)
                {
                    tree = edit(tree, cancellationToken) ?? tree;
                }

                syntaxTrees[i] = tree;
            }

            // Edit syntax nodes individually
            SyntaxNodes:

            if (SyntaxPipeline.Count == 0)
                goto SymbolTrees;

            for (int i = 0; i < syntaxTrees.Count; i++)
            {
                CSharpSyntaxTree tree = syntaxTrees[i] as CSharpSyntaxTree;

                if (tree == null)
                    continue;

                CSharpSyntaxNode root = tree.GetRoot(cancellationToken);
                SyntaxNode newRoot = rewriter.Visit(root);

                if (root != newRoot)
                    syntaxTrees[i] = tree.WithRootAndOptions(newRoot, tree.Options);
            }

            // Edit symbol trees
            SymbolTrees:

            if (SymbolTreePipeline.Count == 0)
                goto SymbolsAndOperations;

            // TODO

            // Edit symbols (and operations) individually
            SymbolsAndOperations:

            if (SymbolPipeline.Count == 0 && OperationPipeline.Count == 0)
                goto End;

            // TODO

            // Copy new trees to compilation
            End:
            compilation = compilation.RemoveAllSyntaxTrees().AddSyntaxTrees(syntaxTrees);

            // Edit compilation 'after'
            for (int i = 0; i < editors.Count; i++)
                editors[i].State = CompilationEditor.CompilationState.End;

            InvokeAll(ref compilation, AfterCompilationPipeline, cancellationToken);
        }

        private static void InvokeAll<T>(ref T item, IEnumerable<Edit<T>> edits, CancellationToken cancellationToken) where T : class
        {
            foreach (Edit<T> edit in edits)
            {
                item = edit(item, cancellationToken) ?? item;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}
