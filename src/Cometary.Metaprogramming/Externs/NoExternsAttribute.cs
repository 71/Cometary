using System;
using System.Collections.Generic;

namespace Cometary
{
    /// <summary>
    /// <para>
    ///   Indicates that all <see langword="extern"/> method should now declare
    ///   a simple body that throws an <see cref="NotImplementedException"/>.
    /// </para>
    /// <para>
    ///   This attribute can be used by assemblies that edit themselves while declaring
    ///   extern methods, in order to be able to run them flawlessly, despite the presence of
    ///   methods with "no implementation".
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class NoExternsAttribute : CometaryAttribute
    {
        /// <inheritdoc />
        public override IEnumerable<CompilationEditor> Initialize()
        {
            return new CompilationEditor[] { new ExternsReplacingEditor() };
        }
    }
}
