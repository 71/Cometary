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

[assembly: InternalsVisibleTo("Cometary.Remote.Core")]
[assembly: InternalsVisibleTo("Cometary.Remote.MSBuild")]
[assembly: InternalsVisibleTo("Cometary.Remote.VisualStudio")]

namespace Cometary
{
    using Rewriting;
    using Visiting;

    /// <summary>
    /// Represents the Voltaire processor.
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
        public CSharpCompilation Compilation { get; private set; }

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
        public ReadOnlyCollection<AssemblyVisitor> Visitors { get; private set; }
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
        private Processor(Workspace workspace)
        {
            Workspace = workspace;

            AssemblyStream = new MemoryStream();
            SymbolsStream  = new MemoryStream();

            ExecutionDomain = AppDomain.CurrentDomain;
        }

        /// <summary>
        /// Creates a new <see cref="Processor"/>, given its workspace and project.
        /// </summary>
        public static async Task<Processor> CreateAsync(Workspace workspace, Project project, CancellationToken token = default(CancellationToken))
        {
            Processor processor = new Processor(workspace)
            {
                Project = project
            };

            processor.Project = processor.Project.WithParseOptions(
                processor.Project.ParseOptions.WithFeatures(
                    processor.Project.ParseOptions.Features.Concat(new[] { new KeyValuePair<string, string>(nameof(IOperation), "True") })
                )
            );

            processor.Compilation = await processor.Project.GetCompilationAsync(token) as CSharpCompilation;

            // Set up environment
            Meta.Compilation = processor.Compilation;
            Meta.CTFE = true;

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
                SyntaxTree tree = Compilation.SyntaxTrees[i];

                noExternCompilation = noExternCompilation.ReplaceSyntaxTree(
                    tree, tree.WithRootAndOptions(externRewriter.Visit(tree.GetRoot()), tree.Options));
            }

            // Compile to stream
            bool succeeded = await Compile(noExternCompilation, token);

            if (!succeeded)
                throw new Exception("Unknown error encountered whilst compiling for the first time.");

            // Retrieve assembly from stream
            Log("Loading emitted assembly.");

            Assembly assembly = LoadAssembly();

            Log("Successfully loaded emitted assembly.");

            // Load visitors, and let 'em visit the streams
            LoadVisitors();

            foreach (AssemblyVisitor visitor in Visitors)
                visitor.Visit(AssemblyStream, SymbolsStream, AssemblyVisitor.CompilationState.Loaded);

            // Get ready to process the assembly
            using (AssemblyResolver resolver = new AssemblyResolver(assembly))
            {
                foreach (var @ref in Compilation.References)
                {
                    if (@ref is PortableExecutableReference peRef)
                    {
                        resolver.Register(peRef.FilePath);
                    }
                    else if (@ref is CompilationReference cRef)
                    {
                        Project project = Project.Solution?.GetProject(cRef.Compilation.Assembly);

                        if (project != null)
                            resolver.Register(project.OutputFilePath);
                    }
                }

                ExecutionDomain.AssemblyResolve += resolver.AssemblyResolve;

                // Process assembly
                Log("Processing assembly...");

                ProcessAssembly(assembly);

                // Re-compile assembly
                succeeded = await Compile(Compilation, token);

                if (!succeeded)
                    throw new Exception("Unknown error encountered whilst compiling for the second time.");

                // Visit streams again
                foreach (AssemblyVisitor visitor in Visitors)
                    visitor.Visit(AssemblyStream, SymbolsStream, AssemblyVisitor.CompilationState.Visited);

                ExecutionDomain.AssemblyResolve -= resolver.AssemblyResolve;

                Log("Saving to output file...");

                // Save to file
                AssemblyStream.Position = 0;
                SymbolsStream.Position = 0;

                using (FileStream assemblyFile = File.Open(Project.OutputFilePath, FileMode.Create, FileAccess.ReadWrite))
                    await AssemblyStream.CopyToAsync(assemblyFile);

                using (FileStream symbolsFile = File.Open(Path.ChangeExtension(Project.OutputFilePath, ".pdb"), FileMode.Create, FileAccess.ReadWrite))
                    await SymbolsStream.CopyToAsync(symbolsFile);
            }
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
                switch (diagnostic.Severity)
                {
                    case DiagnosticSeverity.Error:
                        exceptions.Add(new ProcessingException(diagnostic.Location.SourceSpan, diagnostic.GetMessage()));
                        break;
                    case DiagnosticSeverity.Warning:
                    case DiagnosticSeverity.Info:
                        (diagnostic.Severity == DiagnosticSeverity.Warning ? WarningLogged : MessageLogged)?.Invoke(
                            this, new ProcessingMessage(diagnostic.GetMessage(), diagnostic.Location.SourceSpan)
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
        /// Finds and loads all visitors declared by this assembly,
        /// and all referenced assemblies.
        /// </summary>
        private void LoadVisitors()
        {
            // Find visitors
            List<AssemblyVisitor> builder = new List<AssemblyVisitor>(1);

            try
            {
                foreach (Type type in from a in ExecutionDomain.GetAssemblies()
                                      from t in a.GetTypes()
                                      where typeof(AssemblyVisitor).IsAssignableFrom(t) && !t.IsAbstract
                                      select t)
                {
                    try
                    {
                        AssemblyVisitor visitor = Activator.CreateInstance(type) as AssemblyVisitor;

                        builder.Add(visitor);

                        Log($"Loaded visitor {type.FullName}.");
                    }
                    catch
                    {
                        // Alright, skipping this one
                        Log($"Could not load visitor {type.FullName}.");
                    }
                }
            }
            catch (ReflectionTypeLoadException e)
            {
                WarningLogged?.Invoke(this,
                    new ProcessingMessage($"Could not load some types from {e.LoaderExceptions.OfType<BadImageFormatException>().FirstOrDefault()?.FileName ?? "an unknown assembly"}, continuing..."));
            }

            builder.Sort((a, b) => a.Priority.CompareTo(b.Priority));

            Visitors = builder.AsReadOnly();
        }

        /// <summary>
        /// Processes an assembly.
        /// </summary>
        private void ProcessAssembly(Assembly assembly)
        {
            // Execute visitors
            AssemblyRewriter rewriter = new AssemblyRewriter();
            TemplateRewriter templateRewriter = new TemplateRewriter();

            for (int i = 0; i < Compilation.SyntaxTrees.Length; i++)
            {
                SyntaxTree syntaxTree = Compilation.SyntaxTrees[i];

                AssemblyVisitor.Visit(syntaxTree, Visitors);

                Compilation = Compilation.ReplaceSyntaxTree(
                    Compilation.SyntaxTrees[i],
                    syntaxTree.WithRootAndOptions(
                        rewriter.Visit(
                            templateRewriter.Visit(syntaxTree.GetRoot())
                        ), syntaxTree.Options
                    )
                );

                Log($"Processed {i + 1} of {Compilation.SyntaxTrees.Length} syntax tree(s).");
            }

            foreach (AssemblyVisitor visitor in Visitors)
                Compilation = visitor.Visit(assembly, Compilation);
        }
#endregion
    }
}
