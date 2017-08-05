using System;
using System.Collections.Generic;

namespace Cometary
{
    /// <summary>
    ///   <see cref="Attribute"/> that instructs Cometary to delete
    ///   all references to itself in the marked assembly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class CleanUpAttribute : CometaryAttribute
    {
        /// <inheritdoc />
        public override IEnumerable<CompilationEditor> Initialize()
        {
            return new CompilationEditor[] { new CleaningEditor() };
        }
    }
}
