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
        /// <summary>
        ///   Describes an error encountered when emitting the produced compilation.
        /// </summary>
        public static readonly DiagnosticDescriptor EmitError
            = new DiagnosticDescriptor(nameof(EmitError), "Emit error", "Error encountered when emitting the assembly: {0}", Common.DiagnosticsCategory, DiagnosticSeverity.Error, true);

        /// <summary>
        ///   Describes an error encountered when loading the emitted assembly.
        /// </summary>
        public static readonly DiagnosticDescriptor LoadError
            = new DiagnosticDescriptor(nameof(LoadError), "Load error", "Error encountered when loading the emitted assembly: {0}", Common.DiagnosticsCategory, DiagnosticSeverity.Error, true);

        /// <summary>
        ///   Describes an error encountered when running the emitted assembly.
        /// </summary>
        public static readonly DiagnosticDescriptor ExecutionError
            = new DiagnosticDescriptor(nameof(ExecutionError), "Execution error", "Error encountered when running the emitted assembly: {0}", Common.DiagnosticsCategory, DiagnosticSeverity.Error, true);

        private CompilationEditor[] children;

        /// <inheritdoc />
        protected override void Initialize(CSharpCompilation compilation, CancellationToken cancellationToken)
        {
            using (MemoryStream assemblyStream = new MemoryStream())
            using (MemoryStream symbolsStream = new MemoryStream())
            {
                // Define the 'META' constant
                compilation = compilation.RecomputeCompilationWithOptions(opts => opts.WithPreprocessorSymbols("META"), cancellationToken);

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

                    return;
                }

                Assembly TryResolve(AssemblyLoadContext sender, AssemblyName assemblyName)
                {
                    if (assemblyName.Name == compilation.AssemblyName)
                        return null;
                    return null;
                }

                try
                {
                    assemblyStream.Position = 0;
                    symbolsStream.Position = 0;

                    AssemblyLoadContext.Default.Resolving += TryResolve;

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
                catch (TargetInvocationException e)
                {
                    Report(Diagnostic.Create(LoadError, Location.None, e.InnerException.Message));
                }
                finally
                {
                    AssemblyLoadContext.Default.Resolving -= TryResolve;
                }
            }
        }

        /// <inheritdoc />
        protected override CompilationEditor[] Children => children;

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
                if (Activator.CreateInstance(editorTypes[i].Value as Type, true) is CompilationEditor editor)
                {
                    editors[i] = editor;
                    continue;
                }

                Report(Diagnostic.Create(LoadError, Location.None, $"Could not create editor of type '{editorTypes[i].Value}'."));
                failed = true;
            }

            return failed ? null : editors;
        }
    }
}
