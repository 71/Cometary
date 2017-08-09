using System.Threading;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary.Tests
{
    /// <summary>
    ///   <see cref="CompilationEditor"/> that reports an error
    ///   if it is initialized.
    /// </summary>
    public sealed class UnusedEditor : CompilationEditor
    {
        /// <inheritdoc />
        protected override void Initialize(CSharpCompilation compilation, CancellationToken cancellationToken)
        {
            ReportError("Well, that shouldn't happen.");
        }
    }
}
