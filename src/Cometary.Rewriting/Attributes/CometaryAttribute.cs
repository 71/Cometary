using System;

namespace Cometary
{
    /// <summary>
    /// Attribute exposing various settings related to Cometary.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class CometaryAttribute : Attribute
    {
        private static CometaryAttribute instance;

        /// <summary>
        /// Gets the global <see cref="CometaryAttribute"/> set on the currently executing assembly.
        /// <para>
        /// Only accessible during compilation.
        /// </para>
        /// </summary>
        public static CometaryAttribute Instance => instance ?? new CometaryAttribute();

        /// <summary>
        /// Sets whether or not all references to cometary should be removed.
        /// </summary>
        public bool CleanUp { internal get; set; } = false;

        /// <summary>
        /// Initializes a new <see cref="CometaryAttribute"/>, and sets the global
        /// instance.
        /// </summary>
        public CometaryAttribute()
        {
            instance = this;

            Meta.LogDebug("Initialized Cometary attribute.");
        }
    }
}
