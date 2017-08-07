using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Cometary
{
    /// <summary>
    ///   <see cref="CompilationEditor"/> that emits a compilation, loads it in memory,
    ///   and allows it to edit itself.
    /// </summary>
    internal sealed class SelfEditor : CompilationEditor
    {
        /// <summary>ID of the <see cref="EmitError"/> diagnostic.</summary>
        public const string EmitErrorId = Common.DiagnosticsPrefix + "S01";

        /// <summary>ID of the <see cref="LoadError"/> diagnostic.</summary>
        public const string LoadErrorId = Common.DiagnosticsPrefix + "S02";

        /// <summary>ID of the <see cref="RunError"/> diagnostic.</summary>
        public const string RunErrorId = Common.DiagnosticsPrefix + "S03";

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

        private CompilationEditor[] children;

        /// <inheritdoc />
        protected override void Initialize(CSharpCompilation compilation, CancellationToken cancellationToken)
        {
            SuppressDiagnostic(diagnostic => diagnostic.Id == "CS0116" || diagnostic.Id == "CS0658");

            using (MemoryStream assemblyStream = new MemoryStream())
            using (MemoryStream symbolsStream = new MemoryStream())
            {
                // Emit stream for the first time
                EmitResult result = compilation.Emit(
                    peStream: assemblyStream,
                    pdbStream: symbolsStream,
                    options: new EmitOptions(
                        tolerateErrors: true, includePrivateMembers: true,
                        debugInformationFormat: DebugInformationFormat.PortablePdb),
                    cancellationToken: cancellationToken);

                // Ensure everything is good
                if (!result.Success)
                {
                    foreach (Diagnostic diag in result.Diagnostics)
                    {
                        Report(diag);
                    }

                    Report(Diagnostic.Create(EmitError, Location.None, result.Diagnostics.Length.ToString()));
                }

                try
                {
                    assemblyStream.Position = 0;
                    symbolsStream.Position = 0;

                    // Set up custom load context for execution
                    // Note: This causes exceptions when loading already loaded assemblies (ie: System.Threading.Tasks),
                    // which is why I'm using the Default load context, already set up by the analyzer
                    // If no bugs show up, I'll simply remove CompilationLoadcontext at some point
                    //
                    //CompilationLoadContext compilationLoadContext = new CompilationLoadContext(compilation);
                    //
                    Assembly producedAssembly = AssemblyLoadContext.Default.LoadFromStream(assemblyStream, symbolsStream);
                    //
                    //compilationLoadContext.ProducedAssembly = producedAssembly;

                    children = GetAssemblyChildren(producedAssembly);
                }
                catch (TypeLoadException e)
                {
                    Report(Diagnostic.Create(LoadError, Location.None, e.TypeName));
                }
                catch (ReflectionTypeLoadException e)
                {
                    Report(Diagnostic.Create(LoadError, Location.None, e.Message));
                }
            }
        }

        /// <inheritdoc />
        protected override IEnumerable<CompilationEditor> GetChildren() => children;

        /// <summary>
        ///   Runs the emitted assembly.
        /// </summary>
        private CompilationEditor[] GetAssemblyChildren(Assembly assembly)
        {
            // Find the SelfEdit attribute on assembly
            string attrFullName = typeof(EditSelfAttribute).FullName;
            CustomAttributeData attrData = assembly.CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == attrFullName);

            if (attrData == null)
            {
                Report(Diagnostic.Create(LoadError, Location.None, "Could not find EditSelf attribute on compiled assembly."));
                return null;
            }

            // Construct its args
            var editorTypes = (IReadOnlyList<CustomAttributeTypedArgument>)attrData.ConstructorArguments[0].Value;
            CompilationEditor[] editors = new CompilationEditor[editorTypes.Count];

            bool failed = false;

            for (int i = 0; i < editors.Length; i++)
            {
                CompilationEditor editor = editors[i] = Activator.CreateInstance(editorTypes[i].Value as Type, true) as CompilationEditor;

                if (editor != null)
                    continue;

                Report(Diagnostic.Create(LoadError, Location.None, $"Could not create editor of type '{editorTypes[i].Value}'."));
                failed = true;
            }

            return failed ? null : editors;
        }
    }
}
