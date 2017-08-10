using System;
using System.Collections.Generic;

namespace Cometary
{
    /// <summary>
    ///   Defines an <see cref="Attribute"/> that configures one or many
    ///   <see cref="CompilationEditor"/>s, in order to edit the marked assembly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public abstract class CometaryAttribute : Attribute
    {
        /// <summary>
        /// <para>
        ///   Gets or sets the optional order in which this attribute should be applied
        ///   to the assembly.
        /// </para>
        /// <para>
        ///   By default, the order is <c>0</c> ; and if multiple attributes have the same order,
        ///   they'll be executed in the order in which they're declared in code.
        /// </para>
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        ///   Initializes this plugin, returning a collection of non-<see langword="null"/>
        ///   <see cref="CompilationEditor"/>s that are to edit the assembly marked by this attribute.
        /// </summary>
        public abstract IEnumerable<CompilationEditor> Initialize();
    }
}
