using System;
using System.Collections.Generic;

namespace Cometary
{
    /// <summary>
    ///   Represents an <see cref="Attribute"/> that allows modifying an assebly through
    ///   the Cometary analyzer.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public abstract class CometaryAttribute : Attribute
    {
        /// <summary>
        ///   Initializes this plugin, returning 
        /// </summary>
        public abstract IEnumerable<CompilationEditor> Initialize();
    }
}
