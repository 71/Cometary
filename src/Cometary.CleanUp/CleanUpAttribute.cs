using System;
using System.Collections.Generic;
using System.Text;

namespace Cometary
{
    /// <summary>
    ///   <see cref="Attribute"/> that instructs Cometary to delete
    ///   all references to itself in the marked assembly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class CleanUpAttribute : CometaryAttribute
    {
    }
}
