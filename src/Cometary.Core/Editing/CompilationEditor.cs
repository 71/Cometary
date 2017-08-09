using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary
{
    /// <summary>
    ///   Defines a set of members used to analyze and modify a
    ///   <see cref="CSharpCompilation"/>.
    /// </summary>
    /// <seealso href="https://github.com/6A/Cometary" />
    public abstract partial class CompilationEditor : IDisposable
    {
        #region Diagnostics
        /// <summary>
        ///   Represents an error reported by a <see cref="CompilationEditor"/>.
        /// </summary>
        public static readonly DiagnosticDescriptor EditorError
            = new DiagnosticDescriptor(nameof(EditorError), "Cometary error", "{0}: \"{1}\"", Common.DiagnosticsCategory, DiagnosticSeverity.Error, true);

        /// <summary>
        ///   Represents a warning reported by a <see cref="CompilationEditor"/>.
        /// </summary>
        public static readonly DiagnosticDescriptor EditorWarning
            = new DiagnosticDescriptor(nameof(EditorWarning), "Cometary warning", "{0}: \"{1}\"", Common.DiagnosticsCategory, DiagnosticSeverity.Warning, true);

        /// <summary>
        ///   Represents an information reported by a <see cref="CompilationEditor"/>.
        /// </summary>
        public static readonly DiagnosticDescriptor EditorInfo
            = new DiagnosticDescriptor(nameof(EditorInfo), "Cometary information", "{0}: \"{1}\"", Common.DiagnosticsCategory, DiagnosticSeverity.Info, true);
        #endregion


        /// <summary>
        ///   <see cref="Action"/> used to report errors, warnings and messages.
        /// </summary>
        private Action<Diagnostic> ReportDiagnostic;

        /// <summary>
        ///   Gets a <see cref="Store"/> used to store values
        ///   specific to this <see cref="CompilationEditor"/>.
        /// </summary>
        public Store Storage { get; } = new Store();

        /// <summary>
        ///   Gets a <see cref="Store"/> used to store values
        ///   specific to the processed <see cref="CSharpCompilation"/>.
        /// </summary>
        public Store SharedStorage { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public event Action<CompilationEditor, CSharpCompilation> CompilationStart;

        /// <summary>
        /// 
        /// </summary>
        public event Action<CompilationEditor, CSharpCompilation> CompilationEnd;

        /// <summary>
        /// 
        /// </summary>
        public event Action<CompilationEditor> EmissionStart;

        /// <summary>
        /// 
        /// </summary>
        public event Action<CompilationEditor> EmissionEnd;

        internal void TriggerCompilationStart(CSharpCompilation compilation) => CompilationStart?.Invoke(this, compilation);
        internal void TriggerCompilationEnd(CSharpCompilation compilation) => CompilationEnd?.Invoke(this, compilation);
        internal void TriggerEmissionStart() => EmissionStart?.Invoke(this);
        internal void TriggerEmissionEnd() => EmissionEnd?.Invoke(this);

        /// <summary>
        ///   Initializes this <see cref="CompilationEditor"/>.
        /// </summary>
        protected abstract void Initialize(CSharpCompilation compilation, CancellationToken cancellationToken);

        /// <summary>
        /// <para>
        ///   Gets an array of editors considered children of this <see cref="CompilationEditor"/>.
        /// </para>
        /// <para>
        ///   This children will be inserted in order, right after their parent, and
        ///   can be computed in the <see cref="Initialize"/> method.
        /// </para>
        /// </summary>
        protected virtual CompilationEditor[] Children => null;

        /// <summary>
        ///   Suppresses all reported diagnostics that match the given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">
        ///   A predicate that takes a <see cref="Diagnostic"/> as input.
        ///   If it returns <see langword="true"/>, the <see cref="Diagnostic"/> will be suppressed.
        /// </param>
        protected void SuppressDiagnostic(Predicate<Diagnostic> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var predicates = Hooks.DiagnosticPredicates;

            if (!predicates.Contains(predicate))
                 predicates.Add(predicate);
        }

        #region Diagnostic reporting or suppression
        /// <summary>
        ///   Reports an <paramref name="error"/>, optionally specifying its <paramref name="location"/>.
        /// </summary>
        protected void ReportError(string error, Location location = null)
        {
            Requires.NotNull(error, nameof(error));

            ReportDiagnostic(Diagnostic.Create(EditorError, location ?? Location.None, this, error));
        }

        /// <summary>
        ///   Reports a <paramref name="warning"/>, optionally specifying its <paramref name="location"/>.
        /// </summary>
        protected void ReportWarning(string warning, Location location = null)
        {
            Requires.NotNull(warning, nameof(warning));

            ReportDiagnostic(Diagnostic.Create(EditorWarning, location ?? Location.None, this, warning));
        }

        /// <summary>
        ///   Reports a <paramref name="message"/>, optionally specifying its <paramref name="location"/>.
        /// </summary>
        protected void ReportInfo(string message, Location location = null)
        {
            Requires.NotNull(message, nameof(message));

            ReportDiagnostic(Diagnostic.Create(EditorInfo, location ?? Location.None, this, message));
        }

        /// <summary>
        ///   Reports the specified <paramref name="diagnostic"/>.
        /// </summary>
        protected void Report(Diagnostic diagnostic)
        {
            ReportDiagnostic(diagnostic ?? throw new ArgumentNullException(nameof(diagnostic)));
        }
        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return GetType().Name;
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
        }
    }
}
