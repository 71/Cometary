using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;
using TypeInfo = System.Reflection.TypeInfo;

namespace Cometary
{
    using Core;

    /// <summary>
    /// Object that compiles an <see cref="Assembly"/>, and lets it
    /// edit itself before compiling it again.
    /// </summary>
    [SuppressMessage("ReSharper", "InvocationIsSkipped")]
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "()}")]
    public sealed class Processor : IDisposable
    {
        private bool _hasBeenProcessed;
        private bool _hasBeenEmitted;

        private bool _trackedChanges;
        private Dictionary<SyntaxTree, TextChange[]> _changes;

        private string GetDebuggerDisplay() => $"Processor for '{Compilation.AssemblyName}'{(_hasBeenEmitted ? "" : " (Not yet emitted)")}";

        /// <summary>
        /// Gets the <see cref="CSharpCompilation"/> generated
        /// by this <see cref="Processor"/>.
        /// </summary>
        public CSharpCompilation Compilation { get; private set; }

        /// <summary>
        /// Gets the <see cref="Microsoft.CodeAnalysis.Project"/> processed
        /// by this <see cref="Processor"/>.
        /// </summary>
        public Project Project { get; }

        /// <summary>
        /// Gets the <see cref="Stream"/> in which the compiled assembly
        /// will be written.
        /// </summary>
        public MemoryStream AssemblyStream { get; }

        /// <summary>
        /// Gets the <see cref="Stream"/> in which the symbols of the
        /// assembly will be written.
        /// </summary>
        public MemoryStream SymbolsStream { get; }

        /// <summary>
        /// Gets the unique ID of the <see cref="Processor"/>, also associated
        /// to the emitted <see cref="Assembly"/>.
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// Gets or sets whether or not changes
        /// should be tracked by the processor.
        /// </summary>
        public bool TrackChanges { get; set; }

#if NET_CORE
        /// <summary>
        /// 
        /// </summary>
        internal CometaryAssemblyLoadContext Context { get; } 
#else
        /// <summary>
        /// 
        /// </summary>
        internal AssemblyResolver Resolver { get; }
#endif

        /// <summary>
        /// 
        /// </summary>
        internal ProcessorHost Host { get; }

        /// <summary>
        /// Gets the visitors loaded by the processor.
        /// </summary>
        public ReadOnlyCollection<LightAssemblyVisitor> Visitors { get; private set; }

        /// <summary>
        /// Gets the <see cref="IDispatcher"/> used for the process.
        /// </summary>
        public IDispatcher Dispatcher { get; private set; }

        #region Events
        /// <summary>
        /// Event invoked when a message is logged.
        /// </summary>
        public event EventHandler<ProcessingMessage> MessageLogged;

        /// <summary>
        /// Event invoked when a warning is logged.
        /// </summary>
        public event EventHandler<ProcessingMessage> WarningLogged;

        /// <summary>
        /// Event invoked when a debug message is logged.
        /// </summary>
        public event EventHandler<ProcessingMessage> DebugMessageLogged;

        internal void OnMessageLogged(ProcessingMessage msg) => MessageLogged?.Invoke(this, msg);

        internal void OnWarningLogged(ProcessingMessage msg) => WarningLogged?.Invoke(this, msg);

        internal void OnDebugMessageLogged(ProcessingMessage msg) => DebugMessageLogged?.Invoke(this, msg);
        #endregion

        internal Processor(int id, ProcessorHost host, Project project)
        {
#if NET_CORE
            Context    = new CometaryAssemblyLoadContext();
#else
            Resolver   = new AssemblyResolver();
#endif
            Dispatcher = new CoreDispatcher();

            Host = host;
            ID   = id;

            Project = project;

            if (!project.ParseOptions.Features.ContainsKey(nameof(IOperation)))
            {
                Project = project.WithParseOptions(
                    project.ParseOptions.WithFeatures(
                        project.ParseOptions.Features.Concat(new[] { new KeyValuePair<string, string>(nameof(IOperation), "True") })
                    )
                );
            }

            AssemblyStream = new MemoryStream();
            SymbolsStream  = new MemoryStream();
        }

        /// <inheritdoc />
        public void Dispose()
        {
#if NET_CORE
            Context.Dispose();
#else
            Resolver.Dispose();
#endif
            AssemblyStream.Dispose();
            SymbolsStream.Dispose();
        }

        /// <summary>
        /// Loads the emitted assembly in memory.
        /// </summary>
        private Assembly LoadAssembly()
        {
#if NET_CORE
            return Context.LoadFromStream(AssemblyStream, SymbolsStream);
#else
            return Assembly.Load(AssemblyStream.GetBuffer(), SymbolsStream.GetBuffer());
#endif
        }

        /// <summary>
        /// Loads all references for later.
        /// </summary>
        private void LoadReferences(Solution solution, Compilation compilation)
        {
            void Register(string file)
            {
#if NET_CORE
                Context.Register(file);
#else
                Resolver.Register(file);
#endif
            }

            foreach (var @ref in compilation.References)
            {
                string display = @ref.Display;

                if (Host.SharedReferences.Contains(display))
                    continue;

                if (@ref is PortableExecutableReference peRef)
                {
                    Register(peRef.FilePath);
                    Host.SharedReferences.Add(display);
                }
                else if (@ref is CompilationReference cRef)
                {
                    Project project = solution?.GetProject(cRef.Compilation.Assembly);

                    if (project != null)
                    {
                        Register(project.OutputFilePath);
                        Host.SharedReferences.Add(display);
                    }
                }
            }
        }

        /// <summary>
        /// Static methods are faster, duh.
        /// </summary>
        private static IEnumerable<TypeInfo> GetTypesInAssembly(Assembly assembly) => assembly.GetTypes().Select(IntrospectionExtensions.GetTypeInfo);

        /// <summary>
        /// Finds and loads all visitors declared by this assembly,
        /// and all referenced assemblies.
        /// </summary>
        private void LoadVisitorsAndDispatcher(ICollection<LightAssemblyVisitor> visitors, Assembly assembly)
        {
            // Find visitors
            TypeInfo lavType = typeof(LightAssemblyVisitor).GetTypeInfo();
            Type dispatcherType = typeof(IDispatcher);

            void LoadDispatcher(IDispatcher dispatcher, TypeInfo type)
            {
                if (dispatcher.ShouldOverride(Dispatcher))
                    Dispatcher = dispatcher;

                Host.Log($"Loaded dispatcher {type.FullName}.");
            }

            try
            {
                foreach (TypeInfo type in GetTypesInAssembly(assembly))
                {
                    if (type.IsAbstract)
                        continue;

                    if (lavType.IsAssignableFrom(type))
                    {
                        try
                        {
                            LightAssemblyVisitor visitor = Activator.CreateInstance(type.AsType()) as LightAssemblyVisitor;

                            visitors.Add(visitor);

                            // ReSharper disable once SuspiciousTypeConversion.Global
                            if (visitor is IDispatcher dispatcher)
                                LoadDispatcher(dispatcher, type);

                            Host.Log($"Loaded visitor {type.FullName}.");
                        }
                        catch
                        {
                            // Alright, skipping this one
                            Host.Log($"Could not load visitor {type.FullName}.");
                        }
                    }
                    else if (type.GetInterfaces().Contains(dispatcherType))
                    {
                        try
                        {
                            LoadDispatcher(Activator.CreateInstance(type.AsType()) as IDispatcher, type);
                        }
                        catch
                        {
                            // Ditto.
                            Host.Log($"Could not load dispatcher {type.FullName}.");
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException e)
            {
                WarningLogged?.Invoke(this,
                    new ProcessingMessage($"Could not load some types from {e.LoaderExceptions.OfType<BadImageFormatException>().FirstOrDefault()?.FileName ?? "an unknown assembly"}, continuing..."));
            }
        }

        /// <summary>
        /// Emits the compilation to the assembly stream, and the symbols stream.
        /// </summary>
        private async Task<bool> EmitToMemoryAsync(Compilation compilation, CancellationToken token)
        {
            AssemblyStream.SetLength(0);
            SymbolsStream.SetLength(0);

            List<ProcessingException> exceptions = new List<ProcessingException>();
            EmitResult result = await Task.Factory.StartNew(() => compilation.Emit(AssemblyStream, SymbolsStream, cancellationToken: token), token);

            foreach (Diagnostic diagnostic in result.Diagnostics)
            {
                SyntaxNode node = diagnostic.Location.SourceTree.GetRoot().FindNode(diagnostic.Location.SourceSpan);

                switch (diagnostic.Severity)
                {
                    case DiagnosticSeverity.Error:
                        exceptions.Add(node == null
                            ? new ProcessingException(diagnostic.Location.SourceSpan, diagnostic.GetMessage())
                            : new ProcessingException(node, diagnostic.GetMessage()));
                        break;
                    case DiagnosticSeverity.Warning:
                    case DiagnosticSeverity.Info:
                        (diagnostic.Severity == DiagnosticSeverity.Warning ? WarningLogged : MessageLogged)?.Invoke(
                            this, node == null
                            ? new ProcessingMessage(diagnostic.GetMessage(), diagnostic.Location.SourceSpan)
                            : new ProcessingMessage(diagnostic.GetMessage(), node)
                        );
                        break;
                }
            }

            switch (exceptions.Count)
            {
                case 0:
                    _hasBeenEmitted = true;
                    return result.Success;
                case 1:
                    throw exceptions[0];
                default:
                    throw new AggregateException(exceptions);
            }
        }

        /// <summary>
        /// Initializes this processor.
        /// </summary>
        internal async Task<CSharpCompilation> InitializeAsync(CancellationToken token = default(CancellationToken))
        {
            if (Compilation != null)
                return Compilation;

            Compilation = await Project.GetCompilationAsync(token) as CSharpCompilation;

            LoadReferences(Project.Solution, Compilation);

            return Compilation;
        }

        /// <summary>
        /// Processes the target project.
        /// </summary>
        /// <exception cref="ProcessingException">Invalid syntax.</exception>
        /// <exception cref="Exception">Unknown error.</exception>
        /// <exception cref="AggregateException">Multiple issues.</exception>
        public async Task ProcessAsync(CancellationToken token = default(CancellationToken))
        {
            _hasBeenProcessed = _hasBeenEmitted = false;

            bool trackChanges = TrackChanges;
            var changes = trackChanges ? new Dictionary<SyntaxTree, TextChange[]>() : null;

            // Take care of extern methods
            PreprocessingRewriter ppRewriter = new PreprocessingRewriter(ID);

            CSharpCompilation compilation = Compilation ?? await InitializeAsync(token);
            CSharpCompilation preprocessedCompilation = compilation;

            Debug.Assert(compilation != null);

            // ReSharper disable once PossibleNullReferenceException
            for (int i = 0; i < compilation.SyntaxTrees.Length; i++)
            {
                CSharpSyntaxTree tree = (CSharpSyntaxTree)compilation.SyntaxTrees[i];

                // ReSharper disable once PossibleNullReferenceException
                preprocessedCompilation = preprocessedCompilation.ReplaceSyntaxTree(
                    tree, tree.WithRootAndOptions(ppRewriter.Visit(tree.GetRoot()), tree.Options)
                );
            }

            // Compile to stream
            bool succeeded = await EmitToMemoryAsync(preprocessedCompilation, token);

            if (!succeeded)
                throw new Exception("Unknown error encountered whilst compiling for the first time.");

            // Retrieve assembly from stream
            Host.Log("Loading emitted assembly.");

#if NET_CORE
            Assembly assembly = Context.EmittedAssembly = LoadAssembly();
#else
            Assembly assembly = Resolver.EmittedAssembly = LoadAssembly();
#endif
            Host.EmittedAssemblies.Add(assembly, this);
            Meta.GetProcessorCore = () => this;

            List<LightAssemblyVisitor> visitors = new List<LightAssemblyVisitor>();

            foreach (var attr in assembly.GetCustomAttributes())
            {
                // Doing this little loop forces all attributes to be created,
                // and thus to globally register themselves, should they want to do so.
                // See: CometaryAttribute.cs
                Type attrType = attr.GetType();
                Assembly attrAssembly = attrType.GetTypeInfo().Assembly;

                if (Host.SharedAssemblies.ContainsKey(attrAssembly.GetName()))
                    continue;

                Host.SharedAssemblies.Add(attrAssembly.GetName(), attrAssembly);
                LoadVisitorsAndDispatcher(visitors, attrAssembly);
            }

            Host.Log("Successfully loaded emitted assembly.");

            // Load visitors, and let 'em visit the streams
            foreach (AssemblyName refAssemblyName in assembly.GetReferencedAssemblies())
            {
                if (Host.SharedAssemblies.ContainsKey(refAssemblyName))
                    continue;

                try
                {
                    Assembly refAssembly = Assembly.Load(refAssemblyName);

                    LoadVisitorsAndDispatcher(visitors, refAssembly);
                    Host.SharedAssemblies.Add(refAssemblyName, refAssembly);
                }
                catch
                {
                    // Whatever.
                }
            }

            LoadVisitorsAndDispatcher(visitors, assembly);

            if (visitors.Count == 0)
            {
                Host.Log("No visitor was loaded, cancelling.");
                return;
            }

            visitors.Sort();
            Visitors = visitors.AsReadOnly();

            foreach (LightAssemblyVisitor visitor in visitors)
                visitor.Visit(AssemblyStream, SymbolsStream, LightAssemblyVisitor.CompilationState.Loaded);


            // Process assembly
            Host.Log("Processing assembly...");

            int length = compilation.SyntaxTrees.Length;

            for (int i = 0; i < length; i++)
            {
                SyntaxTree syntaxTree = compilation.SyntaxTrees[i];

                compilation = compilation.ReplaceSyntaxTree(
                    syntaxTree, Dispatcher.Dispatch(syntaxTree as CSharpSyntaxTree, Visitors)
                );

                if (trackChanges)
                    changes[syntaxTree] = compilation.SyntaxTrees[i].GetChanges(syntaxTree).ToArray();

                Host.Log($"Processed {i + 1} of {length} syntax tree{(length > 1 ? "s" : "")}.");
            }

            foreach (LightAssemblyVisitor visitor in Visitors)
                compilation = visitor.Visit(assembly, compilation);

            Host.EmittedAssemblies.Remove(assembly);

            // Re-compile assembly
            succeeded = await EmitToMemoryAsync(compilation, token);

            if (!succeeded)
                throw new Exception("Unknown error encountered whilst compiling for the second time.");

            // Visit streams again
            foreach (LightAssemblyVisitor visitor in visitors)
                visitor.Visit(AssemblyStream, SymbolsStream, LightAssemblyVisitor.CompilationState.Visited);

            Host.Log("Saving to output file...");

            _hasBeenProcessed = _hasBeenEmitted = true;

            // ReSharper disable once AssignmentInConditionalExpression
            if (_trackedChanges = trackChanges)
                _changes = changes;

            Compilation = compilation;
        }

        /// <summary>
        /// Writes the processed assembly to the output file.
        /// </summary>
        /// <param name="outputFilePath">
        /// The path to the file in which the assembly will be written.
        /// If <see langword="null"/>, the file will be written to the default
        /// <see cref="Microsoft.CodeAnalysis.Project.OutputFilePath"/>.
        /// </param>
        /// <param name="symbolsFilePath">
        /// The path of the file in which the debugging symbols of the assembly will be written.
        /// If <see langword="null"/>, the file will have the same path as the <paramref name="outputFilePath"/>,
        /// but with the <c>.pdb</c> extension.
        /// </param>
        /// <param name="writeSymbols">
        /// Whether the debugging symbols should be written to a file.
        /// </param>
        /// <param name="token"></param>
        public async Task WriteAssemblyAsync(string outputFilePath = null, string symbolsFilePath = null, bool writeSymbols = true, CancellationToken token = default(CancellationToken))
        {
            if (!_hasBeenProcessed)
                await ProcessAsync(token);

            if (outputFilePath == null)
                outputFilePath = Project.OutputFilePath;
            if (symbolsFilePath == null)
                symbolsFilePath = Path.ChangeExtension(outputFilePath, ".pdb");

            AssemblyStream.Position = 0;
            SymbolsStream.Position = 0;

            using (FileStream assemblyFile = File.Open(outputFilePath, FileMode.Create, FileAccess.ReadWrite))
                await AssemblyStream.CopyToAsync(assemblyFile);

            if (!writeSymbols)
                return;

            using (FileStream symbolsFile = File.Open(symbolsFilePath, FileMode.Create, FileAccess.ReadWrite))
                await SymbolsStream.CopyToAsync(symbolsFile);
        }

        /// <summary>
        /// Returns the changes that occured during the process.
        /// </summary>
        public ILookup<SyntaxTree, TextChange> GetChanges()
        {
            if (!_trackedChanges)
                throw new InvalidOperationException("Changes weren't tracked.");

            return (from pair in _changes
                    from ch in pair.Value
                    select new KeyValuePair<SyntaxTree, TextChange>(pair.Key, ch))
                    .ToLookup(ch => ch.Key, ch => ch.Value);
        }

        /// <summary>
        /// Outputs the new syntax trees to new files, respecting their original path.
        /// </summary>
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute", Justification = "Bug in R# ignores Debug.Assert.")]
        public async Task OutputChangedSyntaxTreesAsync(Project project, string basePath = null, CancellationToken token = default(CancellationToken))
        {
            string projectDir = Path.GetDirectoryName(project.FilePath);

            Debug.Assert(projectDir != null);

            if (basePath == null)
            {
                basePath = Path.Combine(projectDir, "obj", "cometary-syntax");

                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);
            }
            else if (!Directory.Exists(basePath))
                throw new DirectoryNotFoundException("Output directory not found.");

            foreach (SyntaxTree syntaxTree in Compilation.SyntaxTrees)
            {
                string relPath = syntaxTree.FilePath;

                if (!relPath.StartsWith(projectDir, StringComparison.OrdinalIgnoreCase))
                    continue;

                relPath = Path.Combine(basePath, relPath.Substring(projectDir.Length + 1));

                // Make sure dir exists
                Directory.CreateDirectory(Path.GetDirectoryName(relPath));

                using (FileStream fs = File.Open(relPath, FileMode.Create, FileAccess.Write))
                using (TextWriter writer = new StreamWriter(fs, syntaxTree.Encoding))
                {
                    SourceText text = Formatter
                        .Format(await syntaxTree.GetRootAsync(token), Host.Workspace, null, token)
                        .GetText(syntaxTree.Encoding);

                    text.Write(writer, token);
                }
            }
        }
    }
}
