using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary.Attributes
{
    /// <summary>
    ///   Enables <see langword="methodof"/>, <see langword="fieldof"/>, and <see langword="propertyof"/>
    ///   syntax.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class ReflectionSyntaxAttribute : CometaryAttribute
    {
        /// <inheritdoc />
        public override IEnumerable<CompilationEditor> Initialize()
        {
            return new CompilationEditor[] { new Editor() };
        }

        private sealed class Editor : CompilationEditor
        {
            /// <inheritdoc />
            public override void Initialize(CSharpCompilation compilation, CancellationToken cancellationToken)
            {
                // Use the MethodKeyword (etc) to declare all this
            }
        }
    }
}
