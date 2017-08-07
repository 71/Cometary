using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary
{
    /// <summary>
    ///   Adds support for the use of the <see cref="IL"/> and <see cref="CodeGeneratorContext"/> classes in this assembly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class SupportILAttribute : CometaryAttribute
    {
        /// <inheritdoc />
        public override IEnumerable<CompilationEditor> Initialize()
        {
            return new CompilationEditor[] { new Editor() };
        }

        private sealed class Editor : CompilationEditor
        {
            /// <inheritdoc />
            protected override void Initialize(CSharpCompilation compilation, CancellationToken cancellationToken)
            {
                IL.ActiveEditors++;
            }

            /// <inheritdoc />
            public override void Dispose()
            {
                IL.ActiveEditors--;
            }
        }
    }
}
