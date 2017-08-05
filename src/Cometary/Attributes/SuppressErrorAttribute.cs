using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary
{
    /// <summary>
    ///   Suppresses all errors matching the given IDs.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class SuppressErrorsAttribute : CometaryAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string[] DiagnosticIds { get; }

        /// <summary>
        /// 
        /// </summary>
        public SuppressErrorsAttribute(params string[] diagnosticIds)
        {
            Requires.NoneNull(diagnosticIds, nameof(diagnosticIds));

            DiagnosticIds = diagnosticIds;
        }

        /// <inheritdoc />
        public override IEnumerable<CompilationEditor> Initialize()
        {
            return new CompilationEditor[] { new Editor(DiagnosticIds) };
        }

        private sealed class Editor : CompilationEditor
        {
            private readonly string[] CheckedIds;

            public Editor(string[] checkedIds)
            {
                CheckedIds = checkedIds;
            }

            /// <inheritdoc />
            public override void Initialize(CSharpCompilation compilation, CancellationToken cancellationToken)
            {
                SuppressDiagnostic(ShouldSuppress);
            }

            private bool ShouldSuppress(Diagnostic diagnostic)
            {
                return Array.IndexOf(CheckedIds, diagnostic.Id) != -1;
            }
        }
    }
}
