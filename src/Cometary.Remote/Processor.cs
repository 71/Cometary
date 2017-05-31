using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;

[assembly: InternalsVisibleTo("Cometary.Remote.Core, PublicKey=0024000004800000940000000602000000240000525341310004000001000100ddc50907e7882cd6af3432d5c3ba2f9a257e9ea6602df0e06098aba23eed5d650e4adb8aaefcee05afd5a70c43fe058b4d6dbecddf48a99ff9729f6a9968e8915677fa29a24a3a7293788c7de96040fb40d6eaf7f2b24320ec43624189d3a66250c5c0d31823343feb6e6fa9787f6e4961f8c84af6b59993c2e5d1c981b82bcb")]
[assembly: InternalsVisibleTo("Cometary.Remote.MSBuild, PublicKey=0024000004800000940000000602000000240000525341310004000001000100ddc50907e7882cd6af3432d5c3ba2f9a257e9ea6602df0e06098aba23eed5d650e4adb8aaefcee05afd5a70c43fe058b4d6dbecddf48a99ff9729f6a9968e8915677fa29a24a3a7293788c7de96040fb40d6eaf7f2b24320ec43624189d3a66250c5c0d31823343feb6e6fa9787f6e4961f8c84af6b59993c2e5d1c981b82bcb")]
[assembly: InternalsVisibleTo("Cometary.Remote.VisualStudio, PublicKey=0024000004800000940000000602000000240000525341310004000001000100ddc50907e7882cd6af3432d5c3ba2f9a257e9ea6602df0e06098aba23eed5d650e4adb8aaefcee05afd5a70c43fe058b4d6dbecddf48a99ff9729f6a9968e8915677fa29a24a3a7293788c7de96040fb40d6eaf7f2b24320ec43624189d3a66250c5c0d31823343feb6e6fa9787f6e4961f8c84af6b59993c2e5d1c981b82bcb")]

namespace Cometary
{
    using Core;

    /// <summary>
    /// Represents the Cometary processor.
    /// </summary>
    public sealed class Processor : IDisposable
    {
        [Conditional("DEBUG"), DebuggerStepThrough]
        private void Log(string msg)
        {
            DebugMessageLogged?.Invoke(this, new ProcessingMessage(msg, sender: "Processor"));
        }

        #region Properties
        /// <summary>
        /// Gets the workspace for this processor.
        /// </summary>
        public Workspace Workspace { get; }

        /// <summary>
        /// Gets the project for this processor.
        /// </summary>
        public Project Project { get; private set; }

        /// <summary>
        /// Gets the current compilation.
        /// </summary>
        public CSharpCompilation Compilation
        {
            get => Meta.Compilation;
            set => Meta.Compilation = value;
        }

        /// <summary>
        /// Gets the compilation first gotten by this processor.
        /// </summary>
        public CSharpCompilation OriginalCompilation { get; }

        /// <summary>
        /// Gets the <see cref="AppDomain"/> in which code will run.
        /// </summary>
        public AppDomain ExecutionDomain { get; }

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
        /// Gets the visitors loaded by the processor.
        /// </summary>
        public ReadOnlyCollection<LightAssemblyVisitor> Visitors { get; private set; }

        /// <summary>
        /// Gets the <see cref="IDispatcher"/> used for the process.
        /// </summary>
        public IDispatcher Dispatcher { get; private set; }

        /// <summary>
        /// Gets the <see cref="AssemblyResolver"/> used for assembly resolution.
        /// </summary>
        private AssemblyResolver Resolver { get; }

        /// <summary>
        /// Gets a list of referenced assemblies that have been loaded.
        /// </summary>
        private List<Assembly> LoadedReferencedAssemblies { get; }
        #endregion

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
        #endregion

        #region Initialization, Destruction
        private Processor(Workspace workspace, CSharpCompilation compilation)
        {
            Workspace = workspace;
            Resolver  = new AssemblyResolver();
            LoadedReferencedAssemblies = new List<Assembly>();

            OriginalCompilation = compilation;
            Compilation = compilation;

            AssemblyStream = new MemoryStream();
            SymbolsStream  = new MemoryStream();

            ExecutionDomain = AppDomain.CurrentDomain;
            Dispatcher = new CoreDispatcher();
        }

        /// <summary>
        /// Creates a new <see cref="Processor"/>, given its workspace and project.
        /// </summary>
        public static async Task<Processor> CreateAsync(Workspace workspace, Project project, CancellationToken token = default(CancellationToken))
        {
            project = project.WithParseOptions(
                project.ParseOptions.WithFeatures(
                    project.ParseOptions.Features.Concat(new[] { new KeyValuePair<string, string>(nameof(IOperation), "True") })
                )
            );

            CSharpCompilation compilation = await project.GetCompilationAsync(token) as CSharpCompilation;

            Processor processor = new Processor(workspace, compilation)
            {
                Project = project
            };

            processor.LoadReferences();

            // Set up environment
            Meta.Compilation = processor.Compilation;
            Meta.CTFE = true;
            Meta.GetWorkspace = () => workspace;
            Meta.GetProject = () => project;

            Meta.LogDebugInternal =
                (sender, msg, node) => processor.DebugMessageLogged?.Invoke(sender ?? "Unknown", new ProcessingMessage(msg, node, sender?.ToString()));
            Meta.LogMessageInternal =
                (sender, msg, node) => processor.MessageLogged?.Invoke(sender ?? "Unknown", new ProcessingMessage(msg, node, sender?.ToString()));
            Meta.LogWarningInternal =
                (sender, msg, node) => processor.WarningLogged?.Invoke(sender ?? "Unknown", new ProcessingMessage(msg, node, sender?.ToString()));

            processor.Log("Environment ready.");

            return processor;
        }

        /// <summary>
        /// Creates a new <see cref="Processor"/> given its project file.
        /// </summary>
        public static async Task<Processor> CreateAsync(string projectFile, CancellationToken token = default(CancellationToken))
        {
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();
            Project project = await workspace.OpenProjectAsync(projectFile, token);

            return await CreateAsync(workspace, project, token);
        }

        /// <summary>
        /// Creates a new <see cref="Processor"/> associated with a single source.
        /// </summary>
        public static async Task<Processor> CreateAsync(SourceText source, CancellationToken token = default(CancellationToken))
        {
            AdhocWorkspace workspace = new AdhocWorkspace();
            Project project = workspace.AddProject("Temp", "C#")
                                       .AddDocument("default.cs", source)
                                       .Project;

            return await CreateAsync(workspace, project, token);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Workspace.Dispose();
            AssemblyStream.Dispose();
            SymbolsStream.Dispose();
            Resolver.Dispose();

            if (Visitors == null)
                return;

            foreach (IDisposable disposableVisitor in Visitors.OfType<IDisposable>())
                disposableVisitor.Dispose();
        }
        #endregion

        #region Processing
        /// <summary>
        /// Processes the target project.
        /// </summary>
        /// <exception cref="ProcessingException">Invalid syntax.</exception>
        /// <exception cref="Exception">Unknown error.</exception>
        /// <exception cref="AggregateException">Multiple issues.</exception>
        public async Task ProcessAsync(CancellationToken token = default(CancellationToken))
        {
            // Take care of extern methods
            ExternRewriter externRewriter = new ExternRewriter();
            CSharpCompilation noExternCompilation = Compilation;

            for (int i = 0; i < Compilation.SyntaxTrees.Length; i++)
            {
                CSharpSyntaxTree tree = (CSharpSyntaxTree)Compilation.SyntaxTrees[i];

                noExternCompilation = noExternCompilation.ReplaceSyntaxTree(
                    tree, tree.WithRootAndOptions(externRewriter.Visit(tree.GetRoot()), tree.Options)
                );
            }

            // Compile to stream
            bool succeeded = await Compile(noExternCompilation, token);

            if (!succeeded)
                throw new Exception("Unknown error encountered whilst compiling for the first time.");

            // Use special resolver context
            ExecutionDomain.AssemblyResolve += Resolver.AssemblyResolve;

            // Retrieve assembly from stream
            Log("Loading emitted assembly.");

            Assembly assembly = Resolver.EmittedAssembly = LoadAssembly();
            List<LightAssemblyVisitor> visitors = new List<LightAssemblyVisitor>();

            foreach (var attr in assembly.GetCustomAttributes())
            {
                // Doing this little loop forces all attributes to be created,
                // and thus to globally register themselves, should they want to do so.
                // See: CometaryAttribute.cs
                Type attrType = attr.GetType();

                if (LoadedReferencedAssemblies.Contains(attrType.Assembly))
                    continue;

                LoadedReferencedAssemblies.Add(attrType.Assembly);
                LoadVisitorsAndDispatcher(visitors, attrType.Assembly);
            }

            Log("Successfully loaded emitted assembly.");

            // Load visitors, and let 'em visit the streams
            foreach (AssemblyName refAssemblyName in assembly.GetReferencedAssemblies())
            {
                if (LoadedReferencedAssemblies.Any(x => x.GetName() == refAssemblyName))
                    continue;

                try
                {
                    Assembly refAssembly = Assembly.Load(refAssemblyName);

                    LoadVisitorsAndDispatcher(visitors, refAssembly);
                    LoadedReferencedAssemblies.Add(refAssembly);
                }
                catch
                {
                    // Whatever.
                }
            }

            LoadVisitorsAndDispatcher(visitors, assembly);

            if (visitors.Count == 0)
            {
                Log("No visitor was loaded, cancelling.");
                return;
            }

            visitors.Sort();
            Visitors = visitors.AsReadOnly();

            foreach (LightAssemblyVisitor visitor in visitors)
                visitor.Visit(AssemblyStream, SymbolsStream, LightAssemblyVisitor.CompilationState.Loaded);


            // Process assembly
            Log("Processing assembly...");

            ProcessAssembly(assembly);

            // Re-compile assembly
            succeeded = await Compile(Compilation, token);

            if (!succeeded)
                throw new Exception("Unknown error encountered whilst compiling for the second time.");

            // Visit streams again
            foreach (LightAssemblyVisitor visitor in visitors)
                visitor.Visit(AssemblyStream, SymbolsStream, LightAssemblyVisitor.CompilationState.Visited);

            Log("Saving to output file...");

            // Save to file
            AssemblyStream.Position = 0;
            SymbolsStream.Position = 0;

            using (FileStream assemblyFile = File.Open(Project.OutputFilePath, FileMode.Create, FileAccess.ReadWrite))
                await AssemblyStream.CopyToAsync(assemblyFile);

            using (FileStream symbolsFile = File.Open(Path.ChangeExtension(Project.OutputFilePath, ".pdb"), FileMode.Create, FileAccess.ReadWrite))
                await SymbolsStream.CopyToAsync(symbolsFile);

            ExecutionDomain.AssemblyResolve -= Resolver.AssemblyResolve;
        }

        /// <summary>
        /// Emits the compilation to the assembly stream, and the symbols stream.
        /// </summary>
        private async Task<bool> Compile(Compilation compilation, CancellationToken token)
        {
            AssemblyStream.SetLength(0);
            SymbolsStream.SetLength(0);

            List<ProcessingException> exceptions = new List<ProcessingException>();
            EmitResult result = await Task.Run(() => compilation.Emit(AssemblyStream, SymbolsStream, cancellationToken: token), token);

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
                    return result.Success;
                case 1:
                    throw exceptions[0];
                default:
                    throw new AggregateException(exceptions);
            }
        }

        /// <summary>
        /// Loads the emitted assembly in memory.
        /// </summary>
        private Assembly LoadAssembly()
        {
            byte[] assemblyBytes = AssemblyStream.GetBuffer();
            byte[] symbolsBytes  = SymbolsStream.GetBuffer();

            return Assembly.Load(assemblyBytes, symbolsBytes);
        }

        /// <summary>
        /// Loads all references for later.
        /// </summary>
        private void LoadReferences()
        {
            foreach (var @ref in Compilation.References)
            {
                if (@ref is PortableExecutableReference peRef)
                {
                    Resolver.Register(peRef.FilePath);
                }
                else if (@ref is CompilationReference cRef)
                {
                    Project project = Project.Solution?.GetProject(cRef.Compilation.Assembly);

                    if (project != null)
                        Resolver.Register(project.OutputFilePath);
                }
            }
        }

        /// <summary>
        /// Static methods are faster, duh.
        /// </summary>
        private static Type[] GetTypesInAssembly(Assembly assembly) => assembly.GetTypes();

        private IEnumerable<Assembly> GetLoadedAssemblies(Assembly assembly)
        {
            return ExecutionDomain.GetAssemblies().Concat(LoadedReferencedAssemblies).Concat(new[] { assembly });
        }

        /// <summary>
        /// Finds and loads all visitors declared by this assembly,
        /// and all referenced assemblies.
        /// </summary>
        private void LoadVisitorsAndDispatcher(List<LightAssemblyVisitor> visitors, Assembly assembly)
        {
            // Find visitors
            Type lavType = typeof(LightAssemblyVisitor);
            Type dispatcherType = typeof(IDispatcher);

            void LoadDispatcher(IDispatcher dispatcher, Type type)
            {
                if (dispatcher.ShouldOverride(Dispatcher))
                    Dispatcher = dispatcher;

                Log($"Loaded dispatcher {type.FullName}.");
            }

            try
            {
                foreach (Type type in GetTypesInAssembly(assembly))
                {
                    if (type.IsAbstract)
                        continue;

                    if (lavType.IsAssignableFrom(type))
                    {
                        try
                        {
                            LightAssemblyVisitor visitor = Activator.CreateInstance(type) as LightAssemblyVisitor;

                            visitors.Add(visitor);

                            // ReSharper disable once SuspiciousTypeConversion.Global
                            if (visitor is IDispatcher dispatcher)
                                LoadDispatcher(dispatcher, type);

                            Log($"Loaded visitor {type.FullName}.");
                        }
                        catch
                        {
                            // Alright, skipping this one
                            Log($"Could not load visitor {type.FullName}.");
                        }
                    }
                    else if (type.GetInterfaces().Contains(dispatcherType))
                    {
                        try
                        {
                            LoadDispatcher(Activator.CreateInstance(type) as IDispatcher, type);
                        }
                        catch
                        {
                            // Ditto.
                            Log($"Could not load dispatcher {type.FullName}.");
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
        /// Processes an assembly.
        /// </summary>
        private void ProcessAssembly(Assembly assembly)
        {
            // Execute visitors
            int length = Compilation.SyntaxTrees.Length;

            for (int i = 0; i < length; i++)
            {
                SyntaxTree syntaxTree = Compilation.SyntaxTrees[i];

                Compilation = Compilation.ReplaceSyntaxTree(
                    syntaxTree, Dispatcher.Dispatch(syntaxTree as CSharpSyntaxTree, Visitors)
                );

                Log($"Processed {i + 1} of {length} syntax tree{(length > 1 ? "s" : "")}.");
            }

            foreach (LightAssemblyVisitor visitor in Visitors)
                Compilation = visitor.Visit(assembly, Compilation);
        }
#endregion

        /// <summary>
        /// Outputs the changed syntax trees.
        /// </summary>
        public async Task OutputChangedSyntaxTreesAsync(string basePath = null, CancellationToken token = default(CancellationToken))
        {
            string projectDir = Path.GetDirectoryName(Project.FilePath);

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
                    SourceText text = await syntaxTree.GetTextAsync(token);

                    text.Write(writer, token);
                }
            }
        }
    }
}
