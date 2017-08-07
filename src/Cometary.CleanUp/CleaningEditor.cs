using System;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class CleaningEditor : CompilationEditor
    {
        /// <inheritdoc />
        protected override void Initialize(CSharpCompilation compilation, CancellationToken cancellationToken)
        {
        }
    }
}
