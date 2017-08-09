using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary
{
    /// <summary>
    ///   
    /// </summary>
    internal sealed class CometaryManager : IDisposable
    {
        #region Static
        /// <summary>
        ///   Gets a <see cref="DiagnosticDescriptor"/> describing an unexpected exception
        ///   thrown by a <see cref="CompilationEditor"/>.
        /// </summary>
        public static DiagnosticDescriptor EditorError { get; }
            = new DiagnosticDescriptor("EditorError", "Unexpected error", "Exception thrown by the '{0}' editor: '{1}'", Common.DiagnosticsCategory, DiagnosticSeverity.Error, true);

        /// <summary>
        ///   Gets a <see cref="DiagnosticDescriptor"/> describing an unexpected exception
        ///   encountered when modifying a <see cref="CSharpCompilation"/>.
        /// </summary>
        public static DiagnosticDescriptor ProcessingError { get; }
            = new DiagnosticDescriptor("ProcessingError", "Unexpected error", "Exception thrown during the {0} step: '{1}'. Stack trace: {2}.", Common.DiagnosticsCategory, DiagnosticSeverity.Error, true);

        /// <summary>
        ///   Gets a <see cref="DiagnosticDescriptor"/> describing an unexpected exception
        ///   encountered when initializing a <see cref="CometaryAttribute"/>.
        /// </summary>
        public static DiagnosticDescriptor InitializationError { get; }
            = new DiagnosticDescriptor("InitializationError", "Unexpected error", "Exception thrown during the initialization by the '{0}' attribute: '{1}'. Stack trace: {2}.", Common.DiagnosticsCategory, DiagnosticSeverity.Error, true);
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public List<CompilationEditor> Editors { get; }

        /// <summary>
        /// 
        /// </summary>
        public FlatteningList<Edit<CSharpCompilation>> CompilationPipeline { get; } = new FlatteningList<Edit<CSharpCompilation>>();

        /// <summary>
        /// 
        /// </summary>
        public FlatteningList<Edit<ISourceAssemblySymbol>> AssemblyPipeline { get; } = new FlatteningList<Edit<ISourceAssemblySymbol>>();

        /// <summary>
        /// 
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsInitializationSuccessful { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Action<Diagnostic> AddDiagnostic { get; }

        /// <summary>
        /// 
        /// </summary>
        public Store SharedStorage { get; }

        /// <summary>
        /// 
        /// </summary>
        internal readonly Func<CSharpCompilation, object> getModuleBuilder;

        /// <summary>
        ///   List of <see cref="Exception"/>s encountered during initialization,
        ///   before diagnostics could be added.
        /// </summary>
        internal readonly ImmutableArray<(Exception Exception, AttributeData Data)>.Builder initializationExceptions = ImmutableArray.CreateBuilder<(Exception, AttributeData)>();

        private CometaryManager(Func<CSharpCompilation, object> moduleBuilderGetter, Action<Diagnostic> addDiagnostic, IEnumerable<CompilationEditor> editors)
        {
            Editors = new List<CompilationEditor>(editors);
            AddDiagnostic = addDiagnostic;
            SharedStorage = new Store();

            getModuleBuilder = moduleBuilderGetter;
        }

        /// <summary>
        ///   Creates a new <see cref="CometaryManager"/>.
        /// </summary>
        public static CometaryManager Create(Func<CSharpCompilation, object> moduleBuilderGetter, Action<Diagnostic> addDiagnostic, params CompilationEditor[] editors)
        {
            Debug.Assert(editors != null);
            Debug.Assert(editors.All(x => x != null));

            return new CometaryManager(moduleBuilderGetter, addDiagnostic, editors);
        }

        #region Initialization
        /// <summary>
        /// 
        /// </summary>
        public void RegisterAttributes(IAssemblySymbol assembly)
        {
            // Sort the attributes based on order and declaration
            // Note: Since we're getting symbols here, the order is based
            // on the order in which the files were read, and the order in code.
            var attributes = assembly.GetAttributes();

            // Find all used editors, and register 'em
            var allEditors = new Dictionary<int, IList<CompilationEditor>>(attributes.Length);
            int editorsCount = 0;

            for (int i = 0; i < attributes.Length; i++)
            {
                AttributeData attr = attributes[i];
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
                int order;

                try
                {
                    editors = InitializeAttribute(attr, out order);
                }
                catch (TargetInvocationException e)
                {
                    initializationExceptions.Add((e.InnerException, attr));
                    continue;
                }
                catch (TypeInitializationException e)
                {
                    initializationExceptions.Add((e.InnerException, attr));
                    continue;
                }
                catch (Exception e)
                {
                    initializationExceptions.Add((e, attr));
                    continue;
                }

                if (!allEditors.TryGetValue(order, out var editorsOfSameOrder))
                {
                    editorsOfSameOrder = new LightList<CompilationEditor>();
                    allEditors[order] = editorsOfSameOrder;
                }

                foreach (CompilationEditor editor in editors)
                {
                    if (editor == null)
                        continue;

                    editorsOfSameOrder.Add(editor);
                    editorsCount++;
                }

                NextAttribute:;
            }

            Editors.Capacity = Editors.Count + editorsCount;
            Editors.AddRange(allEditors.OrderBy(x => x.Key).SelectMany(x => x.Value));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerable<CompilationEditor> InitializeAttribute(AttributeData data, out int order)
        {
            CometaryAttribute attribute = data.Construct<CometaryAttribute>();
            order = attribute.Order;
            return attribute.Initialize();
        }

        /// <summary>
        ///   Initializes the <see cref="CometaryManager"/>, and all its registered members.
        /// </summary>
        public bool TryInitialize(CSharpCompilation compilation, CancellationToken cancellationToken)
        {
            if (IsInitialized && IsInitializationSuccessful)
                return true;

            List<CompilationEditor> editors = Editors;
            var addDiagnostic = AddDiagnostic;
            CSharpCompilation clone = compilation.Clone();

            IsInitialized = true;

            // Log all previously encountered exceptions
            initializationExceptions.Capacity = initializationExceptions.Count;

            var exceptions = initializationExceptions.MoveToImmutable();

            for (int i = 0; i < exceptions.Length; i++)
            {
                var (exception, data) = exceptions[i];
                var location = data.ApplicationSyntaxReference.ToLocation();

                addDiagnostic(Diagnostic.Create(InitializationError, location, data.AttributeClass, exception.Message.Filter(), exception.StackTrace.Filter()));
            }

            if (exceptions.Length > 0)
                return false;

            // Initialize all editors
            int editorsCount = editors.Count;

            for (int i = 0; i < editorsCount; i++)
            {
                CompilationEditor editor = editors[i];

                try
                {
                    // Register
                    if (!editor.TryRegister(this, addDiagnostic, clone, cancellationToken, out var children))
                        return false;

                    // Optionally register some children
                    if (children == null || children.Length == 0)
                        continue;

                    editors.Capacity += children.Length;

                    for (int j = 0; j < children.Length; j++)
                    {
                        CompilationEditor child = children[j];

                        if (child == null)
                        {
                            addDiagnostic(Diagnostic.Create(
                                id: "MissingChild", category: Common.DiagnosticsCategory,
                                message: $"A child returned by the '{editor}' editor is null.",
                                severity: DiagnosticSeverity.Warning, defaultSeverity: DiagnosticSeverity.Warning,
                                isEnabledByDefault: true, warningLevel: 1, isSuppressed: false));

                            continue;
                        }

                        editors.Insert(i + j + 1, child);
                        editorsCount++;
                    }
                    // Since we insert them right after this one, the for loop will take care of initializing them easily
                    // => No recursion, baby
                }
                catch (Exception e)
                {
                    while (e is TargetInvocationException tie)
                        e = tie.InnerException;

                    addDiagnostic(Diagnostic.Create(EditorError, Location.None, editor?.GetType().ToString() ?? "Unknown editor", e.Message.Filter()));

                    return false;
                }
            }

            // We got this far: the initialization is a success.
            IsInitializationSuccessful = true;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool TryUninitialize()
        {
            if (!IsInitialized || !IsInitializationSuccessful)
                return false;

            var editors = Editors;

            // Uninitialize editors
            for (int i = 0; i < editors.Count; i++)
            {
                editors[i].UnregisterAll(this);
            }

            return true;
        }
        #endregion

        #region Editing
        /// <summary>
        ///   
        /// </summary>
        public bool TryEditCompilation(CSharpCompilation compilation, CancellationToken cancellationToken, out CSharpCompilation modified, out object outputBuilder)
        {
            modified = compilation;

            if (!IsInitialized && !TryInitialize(compilation, cancellationToken) ||
                 IsInitialized && !IsInitializationSuccessful)
            {
                outputBuilder = null;
                return false;
            }

            List<CompilationEditor> editors = Editors;
            string step = "NotifyCompilationStart";

            // Run the compilation
            try
            {
                // Run the compilation
                for (int i = 0; i < editors.Count; i++)
                    editors[i].TriggerCompilationStart(compilation);

                step = "Preprocessing";

                foreach (var edit in CompilationPipeline)
                    modified = edit(modified, cancellationToken) ?? modified;

                step = "NotifyCompilationEnd";

#if DEBUG
                if (compilation.GetTypeByMetadataName("ProcessedByCometary") == null)
                    modified = modified.AddSyntaxTrees(SyntaxFactory.ParseSyntaxTree("internal static class ProcessedByCometary { }"));
#endif
                // Notify of end of compilation, and start of emission
                for (int i = 0; i < editors.Count; i++)
                    editors[i].TriggerCompilationEnd(compilation);

                step = "NotifyEmissionStart";

                for (int i = 0; i < editors.Count; i++)
                    editors[i].TriggerEmissionStart();

                step = "Processing";

                // Emit the assembly, and notify of start of emission
                object moduleBuilder = getModuleBuilder(modified);
                Type assemblySymbolInterf = typeof(IAssemblySymbol);

                FieldInfo assemblyField = moduleBuilder.GetType().GetAllFields()
                    .First(x => x.FieldType.GetInterfaces().Contains(assemblySymbolInterf));

                ISourceAssemblySymbol assemblySymbol = assemblyField.GetValue(moduleBuilder) as ISourceAssemblySymbol;
                ISourceAssemblySymbol originalAssembly = assemblySymbol;

                foreach (var edit in AssemblyPipeline)
                    assemblySymbol = edit(assemblySymbol, cancellationToken) ?? assemblySymbol;

                step = "NotifyEmissionEnd";

                // Notify of overall end
                for (int i = 0; i < editors.Count; i++)
                    editors[i].TriggerEmissionEnd();

                step = "Continuing";

                // Copy modified assembly to builder
                if (!ReferenceEquals(originalAssembly, assemblySymbol))
                    assemblyField.SetValue(moduleBuilder, assemblySymbol);

                outputBuilder = moduleBuilder;
                return true;
            }
            catch (Exception e)
            {
                do
                {
                    if (e is DiagnosticException de)
                    {
                        AddDiagnostic(de.Diagnostic);
                    }
                    else if (e is AggregateException ae)
                    {
                        foreach (Exception ex in ae.InnerExceptions)
                        {
                            ReportDiagnostic(step, ex.Message, ex.Source);
                        }
                    }
                    else
                    {
                        ReportDiagnostic(step, e.Message, e.Source);
                    }
                }
                while ((e = e.InnerException) != null);
            }

            outputBuilder = null;
            return false;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void ReportDiagnostic(string step, string message, string stackTrace)
        {
            AddDiagnostic(Diagnostic.Create(ProcessingError, Location.None, step, message.Filter(), stackTrace.Filter()));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var editor in Editors)
            {
                editor.UnregisterAll(this);

                try
                {
                    editor.Dispose();
                }
                catch
                {
                    // Right now we don't care
                }
            }
        }
    }
}
