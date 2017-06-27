using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

[assembly: InternalsVisibleTo("Cometary.Remote.Core, PublicKey=0024000004800000940000000602000000240000525341310004000001000100ddc50907e7882cd6af3432d5c3ba2f9a257e9ea6602df0e06098aba23eed5d650e4adb8aaefcee05afd5a70c43fe058b4d6dbecddf48a99ff9729f6a9968e8915677fa29a24a3a7293788c7de96040fb40d6eaf7f2b24320ec43624189d3a66250c5c0d31823343feb6e6fa9787f6e4961f8c84af6b59993c2e5d1c981b82bcb")]
[assembly: InternalsVisibleTo("Cometary.Remote.MSBuild, PublicKey=0024000004800000940000000602000000240000525341310004000001000100ddc50907e7882cd6af3432d5c3ba2f9a257e9ea6602df0e06098aba23eed5d650e4adb8aaefcee05afd5a70c43fe058b4d6dbecddf48a99ff9729f6a9968e8915677fa29a24a3a7293788c7de96040fb40d6eaf7f2b24320ec43624189d3a66250c5c0d31823343feb6e6fa9787f6e4961f8c84af6b59993c2e5d1c981b82bcb")]
[assembly: InternalsVisibleTo("Cometary.Remote.VisualStudio, PublicKey=0024000004800000940000000602000000240000525341310004000001000100ddc50907e7882cd6af3432d5c3ba2f9a257e9ea6602df0e06098aba23eed5d650e4adb8aaefcee05afd5a70c43fe058b4d6dbecddf48a99ff9729f6a9968e8915677fa29a24a3a7293788c7de96040fb40d6eaf7f2b24320ec43624189d3a66250c5c0d31823343feb6e6fa9787f6e4961f8c84af6b59993c2e5d1c981b82bcb")]

namespace Cometary
{
    using Core;

    /// <summary>
    /// Represents a host that delegates <see cref="Project"/> processing
    /// to a <see cref="Processor"/>.
    /// </summary>
    [DebuggerDisplay("Host for '{Workspace.CurrentSolution.FilePath}' (Contains {ProcessorMap.Count} processors)")]
    public sealed class ProcessorHost : IDisposable
    {
        [Conditional("DEBUG"), DebuggerStepThrough]
        internal void Log(string msg)
        {
            DebugMessageLogged?.Invoke(this, new ProcessingMessage(msg, sender: "Processor"));
        }

        /// <summary>
        /// The global instances for processor hosts.
        /// </summary>
        private static readonly List<ProcessorHost> Instances = new List<ProcessorHost>();

        /// <summary>
        /// A dictionary that contains all processors, identified by their <see cref="Processor.ID"/>.
        /// </summary>
        internal static readonly Dictionary<int, Processor> ProcessorMap = new Dictionary<int, Processor>();

        /// <summary>
        /// An <see cref="object"/> used to lock operations.
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// Gets the workspace for this processor.
        /// </summary>
        public Workspace Workspace { get; }

        /// <summary>
        /// Gets the <see cref="IDispatcher"/> used for the process.
        /// </summary>
        public IDispatcher Dispatcher { get; private set; }

        /// <summary>
        /// Gets a list of all processors that are currently active.
        /// </summary>
        public IReadOnlyCollection<Processor> Processors { get; }

        /// <summary>
        /// Gets a dictionary containing all loaded assemblies that
        /// are shared by different processors.
        /// </summary>
        internal Dictionary<AssemblyName, Assembly> SharedAssemblies { get; }

        /// <summary>
        /// Gets a dictionary containg all assemblies that have been or are
        /// being emitted by a <see cref="Processor"/>.
        /// </summary>
        internal Dictionary<Assembly, Processor> EmittedAssemblies { get; }

        /// <summary>
        /// Gets a dictionary containing all loaded visitors that
        /// are shared by different processors.
        /// </summary>
        internal Dictionary<Assembly, LightAssemblyVisitor> SharedVisitors { get; }

        /// <summary>
        /// Gets a list containing all references that
        /// are shared by different processors.
        /// </summary>
        internal List<string> SharedReferences { get; }

        /// <summary>
        /// Event invoked when a message is logged in a <see cref="Processor"/>.
        /// </summary>
        public event EventHandler<ProcessingMessage> MessageLogged;

        /// <summary>
        /// Event invoked when a warning is logged in a <see cref="Processor"/>.
        /// </summary>
        public event EventHandler<ProcessingMessage> WarningLogged;

        /// <summary>
        /// Event invoked when a debug message is logged in a <see cref="Processor"/>.
        /// </summary>
        public event EventHandler<ProcessingMessage> DebugMessageLogged;

        private ProcessorHost(Workspace workspace)
        {
            Workspace  = workspace;
            Dispatcher = new CoreDispatcher();

            SharedAssemblies = new Dictionary<AssemblyName, Assembly>();
            SharedVisitors   = new Dictionary<Assembly, LightAssemblyVisitor>();
            SharedReferences = new List<string>();

            Processors = ProcessorMap.Values;
            EmittedAssemblies = new Dictionary<Assembly, Processor>();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Workspace.Dispose();

            foreach (Processor processor in Processors)
                processor.Dispose();

            foreach (IDisposable disposableVisitor in SharedVisitors.Values.OfType<IDisposable>())
                disposableVisitor.Dispose();
        }

        /// <summary>
        /// Gets a <see cref="ProcessorHost"/> for the given <paramref name="workspace"/>,
        /// optionally creating it.
        /// </summary>
        public static ProcessorHost GetHost(Workspace workspace)
        {
            lock (SyncRoot)
            {
                // Try to get an already existing host.
                ProcessorHost host = Instances.FirstOrDefault(x => x.Workspace == workspace);

                if (host != null)
                    return host;

                // No existing host found, create it.
                Instances.Add(host = new ProcessorHost(workspace));

                if (Instances.Count > 1)
                    return host;

                // First host that's created; initialize the environment.
                Meta.Compiling = true;

                Meta.GetCallingAssemblyCore = () => GetProcessor().Resolver.EmittedAssembly;
                Meta.GetAssemblyIDCore      = () => GetProcessor().ID;
                Meta.GetProcessorCore       = GetProcessor;

                Meta.WorkspaceCore   = processor => ((Processor)processor).Host.Workspace;
                Meta.ProjectCore     = processor => ((Processor)processor).Project;
                Meta.CompilationCore = processor => ((Processor)processor).Compilation;

                Meta.LogDebugCore =
                    (processor, sender, msg, node) => ((Processor)processor).OnDebugMessageLogged(new ProcessingMessage(msg, node, sender?.ToString()));
                Meta.LogMessageCore =
                    (processor, sender, msg, node) => ((Processor)processor).OnMessageLogged(new ProcessingMessage(msg, node, sender?.ToString()));
                Meta.LogWarningCore =
                    (processor, sender, msg, node) => ((Processor)processor).OnWarningLogged(new ProcessingMessage(msg, node, sender?.ToString()));

                return host;
            }
        }

        private static Processor GetProcessor()
        {
            StackTrace trace = new StackTrace();

            for (int i = trace.FrameCount - 1; i > 0; i--)
            {
                StackFrame frame = trace.GetFrame(i);
                Assembly assembly = frame.GetMethod()?.DeclaringType?.Assembly;

                if (assembly == null)
                    continue;

                foreach (Processor processor in ProcessorMap.Values)
                {
                    if (processor.Resolver.EmittedAssembly == assembly)
                        return processor;
                }
            }

            throw new InvalidOperationException("Couldn't find the calling assembly.");
        }

        /// <summary>
        /// <para>
        ///     Gets the <see cref="Processor"/> associated with the given <paramref name="project"/>.
        /// </para>
        /// <para>
        ///     If no matching processor exists, one will be created.
        /// </para>
        /// <para>
        ///     The created <see cref="Processor"/> will lazily initialize itself later.
        /// </para>
        /// </summary>
        public Processor GetProcessor(Project project)
        {
            // First, let's see if a processor already exists for the given project.
            foreach (Processor p in ProcessorMap.Values)
            {
                if (p.Project == project)
                    return p;
            }

            // No processor found for the given project, create it.
            Processor processor = new Processor(Processors.Count, this, project);

            processor.DebugMessageLogged += DebugMessageLogged;
            processor.MessageLogged += MessageLogged;
            processor.WarningLogged += WarningLogged;

            ProcessorMap.Add(ProcessorMap.Count, processor);

            return processor;
        }

        /// <summary>
        /// <para>
        ///     Gets the <see cref="Processor"/> associated with the given <paramref name="project"/>.
        /// </para>
        /// <para>
        ///     If no matching processor exists, one will be created.
        /// </para>
        /// </summary>
        public async Task<Processor> GetProcessorAsync(Project project, CancellationToken token = default(CancellationToken))
        {
            // First, let's see if a processor already exists for the given project.
            foreach (Processor p in ProcessorMap.Values)
            {
                if (p.Project == project)
                    return p;
            }

            // No processor found for the given project, create it.
            Processor processor = new Processor(Processors.Count, this, project);

            processor.DebugMessageLogged += DebugMessageLogged;
            processor.MessageLogged      += MessageLogged;
            processor.WarningLogged      += WarningLogged;

            ProcessorMap.Add(ProcessorMap.Count, processor);

            await processor.InitializeAsync(token);

            return processor;
        }
    }
}
