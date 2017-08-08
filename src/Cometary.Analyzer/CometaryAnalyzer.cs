using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cometary
{
    /// <summary>
    ///   <see cref="DiagnosticAnalyzer"/> that initializes a <see cref="CometaryManager"/>
    ///   in this context.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class CometaryAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        ///   Describes a <see cref="Diagnostic"/> informing the user that the emitting
        ///   process has been hooked.
        /// </summary>
        public static readonly DiagnosticDescriptor HookSuccess
#if DEBUG
            = new DiagnosticDescriptor("HookSuccess", "Hook successful", "The compiler hook has been successfully installed.", Common.DiagnosticsCategory, DiagnosticSeverity.Warning, true);
#else
            = new DiagnosticDescriptor("HookSuccess", "Hook successful", "The compiler hook has been successfully installed.", Common.DiagnosticsCategory, DiagnosticSeverity.Info, true);
#endif
        /// <summary>
        ///   Describes a <see cref="Diagnostic"/> informing the user that an error
        ///   has thrown.
        /// </summary>
        public static readonly DiagnosticDescriptor HookError
            = new DiagnosticDescriptor("HookFailure", "Hook unsuccessful", "{0}", Common.DiagnosticsCategory, DiagnosticSeverity.Error, true, "A registered edit has thrown an exception.");

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
            = ImmutableArray.Create(HookSuccess, HookError);

        public CometaryAnalyzer()
        {
            AssemblyLoading.EnableResolutionHelp();
        }

        /// <inheritdoc />
        [SuppressMessage("AnalyzerPerformance", "RS1012", Justification = "Yes, all those methods register no action. It's okay.")]
        public override void Initialize(AnalysisContext context)
        {
            // Initialize this exception here
            // When testing Cometary, I noticed that 'StartAction' often wasn't called at all,
            // and nothing indicated it (no error, or warning of the sort).
            // It hasn't happened since I fixed some other stuff (huh R# won't let me use the B u g word)
            // but I'm leaving this like this, just in case.

            // basically, we default to an error, and iff StartAction is successfully ran, we
            // remove this error. if it does run an throws, we replace this error with the exception.
            Exception encounteredException = new Exception("Could not initialize Cometary.");

            void RegisterAction(CompilationStartAnalysisContext ctx)
            {
                AssemblyLoading.CurrentCompilation = ctx.Compilation;
            }

            void StartAction(CompilationStartAnalysisContext ctx)
            {
                try
                {
                    Hooks.EnsureInitialized();

                    encounteredException = null;
                }
                catch (Exception e)
                {
                    encounteredException = e;
                }
            }

            void EndAction(CompilationAnalysisContext ctx)
            {
                ctx.ReportDiagnostic(encounteredException != null
                    ? Diagnostic.Create(CometaryAnalyzer.HookError, Location.None, encounteredException.ToString())
                    : Diagnostic.Create(CometaryAnalyzer.HookSuccess, Location.None));
            }

            // Make sure we Cometary.Core can be loaded
            context.RegisterCompilationStartAction(RegisterAction);

            // Create a matching CometaryManager
            context.RegisterCompilationStartAction(StartAction);

            // Logs a message on success
            context.RegisterCompilationAction(EndAction);
        }
    }
}
