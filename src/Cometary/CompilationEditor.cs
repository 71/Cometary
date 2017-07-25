using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CompilationEditor
    {
        // Right now diagnostics have to exist both in CompilationEditor
        // and SelfAnalyzer, because CompilationEditor cannot use SelfAnalyzer,
        // and when SelfAnalyzer accesses CompilationEditor the latter is not initialized,
        // and thus SelfAnalyzer crashes.

        /// <summary>ID of the <see cref="EditorError"/> diagnostic.</summary>
        public const string EditorErrorId   = Common.DiagnosticsPrefix + "E0";

        /// <summary>ID of the <see cref="EditorWarning"/> diagnostic.</summary>
        public const string EditorWarningId = Common.DiagnosticsPrefix + "W0";

        /// <summary>ID of the <see cref="EditorInfo"/> diagnostic.</summary>
        public const string EditorInfoId    = Common.DiagnosticsPrefix + "I0";

        /// <summary>
        ///   Represents an error reported by a <see cref="CompilationEditor"/>.
        /// </summary>
        public static readonly DiagnosticDescriptor EditorError
            = new DiagnosticDescriptor(EditorErrorId, "Cometary error", "{0}", "Compilation editing", DiagnosticSeverity.Error, true);

        /// <summary>
        ///   Represents a warning reported by a <see cref="CompilationEditor"/>.
        /// </summary>
        public static readonly DiagnosticDescriptor EditorWarning
            = new DiagnosticDescriptor(EditorWarningId, "Cometary warning", "{0}", "Compilation editing", DiagnosticSeverity.Warning, true);

        /// <summary>
        ///   Represents an information reported by a <see cref="CompilationEditor"/>.
        /// </summary>
        public static readonly DiagnosticDescriptor EditorInfo
            = new DiagnosticDescriptor(EditorInfoId, "Cometary information", "{0}", "Compilation editing", DiagnosticSeverity.Info, true);

        /// <summary>
        ///   <see cref="Action"/> used to report errors, warnings and messages.
        /// </summary>
        internal Action<Diagnostic> ReportDiagnostic;

        /// <summary>
        ///   Reports an <paramref name="error"/>, optionally specifying its <paramref name="location"/>.
        /// </summary>
        protected void ReportError(string error, Location location = null)
        {
            ReportDiagnostic(Diagnostic.Create(EditorError, location ?? Location.None, error));
        }

        /// <summary>
        ///   Reports a <paramref name="warning"/>, optionally specifying its <paramref name="location"/>.
        /// </summary>
        protected void ReportWarning(string warning, Location location = null)
        {
            ReportDiagnostic(Diagnostic.Create(EditorWarning, location ?? Location.None, warning));
        }

        /// <summary>
        ///   Reports a <paramref name="message"/>, optionally specifying its <paramref name="location"/>.
        /// </summary>
        protected void ReportInfo(string message, Location location = null)
        {
            ReportDiagnostic(Diagnostic.Create(EditorInfo, location ?? Location.None, message));
        }

        /// <summary>
        ///   Returns a modified <see cref="CSharpCompilation"/> based on the given <paramref name="compilation"/>.
        /// </summary>
        public abstract Task<CSharpCompilation> EditAsync(CSharpCompilation compilation, CancellationToken cancellationToken);
    }
}
