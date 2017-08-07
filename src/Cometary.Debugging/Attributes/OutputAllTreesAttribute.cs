using System;
using System.Collections.Generic;

namespace Cometary.Debugging
{
    /// <summary>
    ///   Ensures all syntax trees of the compilation are written to a new temporary file
    ///   if they have been modified, in order to ease debugging of the changed code.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class OutputAllTreesAttribute : ConfigurationDependantAttribute
    {
        internal static OutputAllTreesAttribute Instance;

        public OutputAllTreesAttribute()
        {
            Instance = this;
        }

        /// <inheritdoc />
        public override IEnumerable<CompilationEditor> Initialize()
        {
            return new CompilationEditor[] { new SyntaxTreeFixerEditor(RunInRelease, RunInDebug) };
        }
    }
}
