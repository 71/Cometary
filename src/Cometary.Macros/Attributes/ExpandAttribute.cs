using System;

namespace Cometary.Macros
{
    /// <summary>
    /// <para>
    ///   Indicates that the marked method should be expanded when called.
    /// </para>
    /// <para>
    ///   In this method, access to the <see cref="CallBinder"/> class is allowed.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ExpandAttribute : Attribute
    {
    }
}
