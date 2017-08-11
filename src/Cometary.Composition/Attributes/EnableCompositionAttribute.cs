using System;
using System.Collections.Generic;

namespace Cometary.Composition
{
    /// <summary>
    ///   Indicates that this assembly uses composition.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class EnableCompositionAttribute : CometaryAttribute
    {
        /// <inheritdoc />
        public override IEnumerable<CompilationEditor> Initialize()
        {
            return new CompilationEditor[] { new CompositionEditor() };
        }
    }
}
