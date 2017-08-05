using System;
using System.Collections.Generic;
using System.Linq;

namespace Cometary
{
    /// <summary>
    ///   Adds support for the use of the <see cref="IL"/> class in this assembly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class SupportILAttribute : CometaryAttribute
    {
        /// <inheritdoc />
        public override IEnumerable<CompilationEditor> Initialize()
        {
            CodeGeneratorContext.EnsureInitialized();
            IL.EnsurePipelineComponentIsActive();

            return Enumerable.Empty<CompilationEditor>();
        }
    }
}
