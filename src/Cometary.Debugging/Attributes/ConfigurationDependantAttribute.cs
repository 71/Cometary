using Microsoft.CodeAnalysis;

namespace Cometary.Debugging
{
    /// <summary>
    ///   <see cref="CometaryAttribute"/> that can be configured to run
    ///   only when the <see cref="CompilationOptions.OptimizationLevel"/>
    ///   matches the <see cref="RunInRelease"/> and <see cref="RunInDebug"/>
    ///   properties.
    /// </summary>
    public abstract class ConfigurationDependantAttribute : CometaryAttribute
    {
        /// <summary>
        /// <para>
        ///   Gets or sets whether or not the <see cref="CompilationEditor"/>
        ///   associated with this attribute should run in Release.
        /// </para>
        /// <para>
        ///   Default: <see langword="false"/>.
        /// </para>
        /// </summary>
        public bool RunInRelease { get; set; } = false;

        /// <summary>
        /// <para>
        ///   Gets or sets whether or not the <see cref="CompilationEditor"/>
        ///   associated with this attribute should run in Debug.
        /// </para>
        /// <para>
        ///   Default: <see langword="true"/>.
        /// </para>
        /// </summary>
        public bool RunInDebug { get; set; } = true;

        internal ConfigurationDependantAttribute()
        {
        }
    }
}
