using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Emit;

using TypeInfo = System.Reflection.TypeInfo;

namespace Cometary.Analyzers
{
    /// <summary>
    ///   <see cref="DiagnosticAnalyzer"/> that allows an assembly to analyze itself.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class SelfAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>ID of the <see cref="EmitError"/> diagnostic.</summary>
        public const string EmitErrorId = Common.DiagnosticsPrefix + "S01";

        /// <summary>ID of the <see cref="LoadError"/> diagnostic.</summary>
        public const string LoadErrorId = Common.DiagnosticsPrefix + "S02";

        /// <summary>ID of the <see cref="RunError"/> diagnostic.</summary>
        public const string RunErrorId  = Common.DiagnosticsPrefix + "S03";

        /// <summary>
        ///   Describes an error encountered when emitting the produced compilation.
        /// </summary>
        public static readonly DiagnosticDescriptor EmitError
            = new DiagnosticDescriptor(EmitErrorId, "Emit error", "Error encountered when emitting the assembly: {0}", "Execution", DiagnosticSeverity.Error, true);

        /// <summary>
        ///   Describes an error encountered when loading the emitted assembly.
        /// </summary>
        public static readonly DiagnosticDescriptor LoadError
            = new DiagnosticDescriptor(LoadErrorId, "Load error", "Error encountered when loading the emitted assembly: {0}", "Execution", DiagnosticSeverity.Error, true);

        /// <summary>
        ///   Describes an error encountered when running the emitted assembly.
        /// </summary>
        public static readonly DiagnosticDescriptor RunError
            = new DiagnosticDescriptor(RunErrorId, "Run error", "Error encountered when running the emitted assembly: {0}", "Execution", DiagnosticSeverity.Error, true);

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
            = ImmutableArray.Create(EmitError, LoadError, RunError);

        private volatile bool hasEdittedPipeline;

        /// <summary>
        ///   Initializes this analyzer by registering an end-of-compilation action.
        /// </summary>
        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxTreeAction(Analyze);

        /// <summary>
        ///   Emits, loads and executes the <paramref name="context"/> <see cref="Compilation"/>.
        /// </summary>
        private void Analyze(SyntaxTreeAnalysisContext context)
        {
            if (hasEdittedPipeline)
                return;

            hasEdittedPipeline = true;

            HookingAnalyzer.EditPipeline += (compilation, addDiagnostic, token) =>
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    // Emit stream for the first time
                    HookingAnalyzer.Unregister();

                    EmitResult result = compilation.Emit(stream,
                        options: new EmitOptions(tolerateErrors: true, includePrivateMembers: true, debugInformationFormat: DebugInformationFormat.Embedded),
                        cancellationToken: token);

                    HookingAnalyzer.Register();

                    // Ensure everything is good
                    if (!result.Success)
                    {
                        foreach (Diagnostic diag in result.Diagnostics)
                        {
                            addDiagnostic(diag);
                        }

                        addDiagnostic(Diagnostic.Create(EmitError, Location.None, result.Diagnostics.Length));
                        return compilation;
                    }

                    stream.Position = 0;

                    Assembly AssemblyResolve(AssemblyLoadContext ctx, AssemblyName name)
                    {
                        var reference = compilation.References
                            .OfType<PortableExecutableReference>()
                            .FirstOrDefault(x => x.Display.Contains(name.Name));

                        return reference == null ? null : ctx.LoadFromAssemblyPath(reference.FilePath);
                    }

                    AssemblyLoadContext.Default.Resolving += AssemblyResolve;

                    try
                    {
                        RunAssembly(AssemblyLoadContext.Default.LoadFromStream(stream), ref compilation, addDiagnostic, token);
                    }
                    catch (TypeLoadException e)
                    {
                        addDiagnostic(Diagnostic.Create(LoadError, Location.None, e.TypeName));
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        addDiagnostic(Diagnostic.Create(LoadError, Location.None, e.Message));
                    }
                    catch (Exception e)
                    {
                        // TODO: Find source of error.
                        do
                            addDiagnostic(Diagnostic.Create(RunError, Location.None, e.Message));
                        while ((e = e.InnerException) != null);
                    }

                    AssemblyLoadContext.Default.Resolving -= AssemblyResolve;
                }

                return compilation;
            };
        }

        /// <summary>
        ///   Runs the emitted assembly.
        /// </summary>
        private static void RunAssembly(Assembly assembly, ref CSharpCompilation compilation, Action<Diagnostic> addDiagnostic, CancellationToken token)
        {
            // Let every editor modify the assembly
            foreach (TypeInfo type in assembly.DefinedTypes)
            {
                if (!type.IsAbstract && type.BaseType == typeof(CompilationEditor))
                {
                    var analyzer = (CompilationEditor)Activator.CreateInstance(type.AsType(), true);

                    analyzer.ReportDiagnostic = addDiagnostic;

                    compilation = analyzer.EditAsync(compilation, token).Result;
                }
            }
        }
    }
}
