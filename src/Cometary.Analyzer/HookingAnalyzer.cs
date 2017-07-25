using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Emit;

namespace Cometary
{
    /// <summary>
    ///   Represents an edit on a <see cref="CSharpCompilation"/>.
    /// </summary>
    public delegate CSharpCompilation Edit(CSharpCompilation compilation, Action<Diagnostic> addDiagnostic, CancellationToken token);

    /// <summary>
    ///   <see cref="DiagnosticAnalyzer"/> that hooks into the emitting process
    ///   to replace the original <see cref="CSharpCompilation"/> by another one.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class HookingAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// 
        /// </summary>
        [SuppressMessage("AnalyzerPerformance", "RS1008")]
        private static readonly List<Edit> pipelineFuncs = new List<Edit>();

        /// <summary>
        ///   Event corresponding to the edits to apply on the analyzed <see cref="CSharpCompilation"/>.
        /// </summary>
        public static event Edit EditPipeline
        {
            add
            {
                if (!pipelineFuncs.Contains(value))
                    pipelineFuncs.Add(value);
            }
            remove => pipelineFuncs.Remove(value);
        }

        #region Hooking
        /// <summary>
        ///   Object that manages the <see cref="Redirection"/> to <see cref="CheckOptionsAndCreateModuleBuilder"/>.
        /// </summary>
        private static Redirection Hook;

        /// <summary>
        /// 
        /// </summary>
        private static bool HasBeenModified;

        /// <summary>
        ///   Registers the <see cref="Hook"/>.
        /// </summary>
        internal static void Register()
        {
            if (Hook != null)
            {
                // Maybe we have unregistered everything?
                // In that case, just restart it.
                Hook.Start();
                return;
            }

            // Hooking the internal CheckOptionsAndCreateModuleBuilder method with Ryder:
            // - Method: http://source.roslyn.io/#Microsoft.CodeAnalysis/Compilation/Compilation.cs,42341c66e909e676
            // - Ryder: https://github.com/6A/Ryder

            MethodInfo originalMethod = typeof(Compilation)
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .First(x => x.Name == nameof(CheckOptionsAndCreateModuleBuilder) && x.IsAssembly);

            MethodInfo replacementMethod = typeof(HookingAnalyzer)
                .GetMethod(nameof(CheckOptionsAndCreateModuleBuilder), BindingFlags.Static | BindingFlags.NonPublic);

            try
            {
                // Invoke it once to ensure the method is jitted
                typeof(CSharpCompilation).GetMethod(nameof(CheckOptionsAndCreateModuleBuilder), BindingFlags.Instance | BindingFlags.NonPublic)
                                         .Invoke(CSharpCompilation.Create(string.Empty), new object[] { null, null, null, null, null, null, null, default(CancellationToken) });
            }
            catch
            {
                // Did this on purpose
            }

            Hook = new Redirection(originalMethod, replacementMethod, true);
        }

        /// <summary>
        ///   Unregisters the <see cref="Hook"/>.
        /// </summary>
        internal static void Unregister()
        {
            Hook.Stop();
        }

        /// <summary>
        ///   Injects the custom <see cref="CSharpCompilation"/> to the emitting process,
        ///   and resumes it.
        /// </summary>
        /// <seealso href="http://source.roslyn.io/#Microsoft.CodeAnalysis/Compilation/Compilation.cs,42341c66e909e676"/>
        private static object CheckOptionsAndCreateModuleBuilder(
            CSharpCompilation self,
            object diagnostics,
            IEnumerable<ResourceDescription> manifestResources,
            EmitOptions options,
            IMethodSymbol debugEntryPoint,
            Stream sourceLinkStream,
            IEnumerable<EmbeddedText> embeddedTexts,
            object testData,
            CancellationToken cancellationToken)
        {
            if (!HasBeenModified)
            {
                var addDiagnostic = Helpers.MakeAddDiagnostic(diagnostics);

                CSharpCompilation modified = self;

                foreach (var editor in pipelineFuncs)
                {
                    modified = editor(modified, addDiagnostic, cancellationToken);
                }

                // Note: we can't pass the modified compilation here for some unknown reason,
                // so we copy every single field from our compilation to the original compilation.
                modified.CopyTo(self);

                HasBeenModified = true;
            }

            return Hook.InvokeOriginal(self,
                diagnostics, manifestResources, options, debugEntryPoint, sourceLinkStream,
                embeddedTexts, testData, cancellationToken);
        }

        #endregion

        /// <summary>ID of the <see cref="HookingInfo"/> diagnostic.</summary>
        public const string HookingInfoId = "COMETH01";

        /// <summary>
        ///   Describes a <see cref="Diagnostic"/> informing the user that the emitting
        ///   process has been hooked.
        /// </summary>
        public static readonly DiagnosticDescriptor HookingInfo
            = new DiagnosticDescriptor(HookingInfoId, "Hook successful", "The compiler hook has been successfully installed.", "Redirection", DiagnosticSeverity.Info, true);

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(HookingInfo);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            Register();

            void Action(CompilationAnalysisContext ctx)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(HookingInfo, Location.None));
            }

            context.RegisterCompilationAction(Action);
        }
    }
}
