using System;
using System.Collections.Generic;

namespace Cometary.Debugging
{
    /// <summary>
    ///   Generates a new entry point that launches the Cometary process.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class DebugCometaryAttribute : ConfigurationDependantAttribute
    {
        /// <summary>
        ///   Default value of the <see cref="MainClassName"/> property.
        /// </summary>
        public const string DefaultMainClassName = "DebugProgram";

        /// <summary>
        /// <para>
        ///   Gets or sets the name of the class that will define the automatically
        ///   generated debug entry point.
        /// </para>
        /// <para>
        ///   A class with this name cannot already exist.
        /// </para>
        /// <para>
        ///   Default value is equal to <see cref="DefaultMainClassName"/>.
        /// </para>
        /// </summary>
        public string MainClassName { get; set; } = DefaultMainClassName;

        /// <summary>
        /// <para>
        ///   Gets or sets a value indicating whether or not a message should be displayed
        ///   at the end of the compilation via a <see cref="System.Diagnostics.Debugger.Break"/> call.
        /// </para>
        /// <para>
        ///   Default: <see langword="true"/>.
        /// </para>
        /// </summary>
        public bool DisplayEndOfCompilationMessage { get; set; } = true;

        /// <summary>
        /// <para>
        ///   Gets or sets a value indicating whether or not the created process should break
        ///   as soon as it starts.
        /// </para>
        /// <para>
        ///   Default: <see langword="false"/>.
        /// </para>
        /// </summary>
        public bool BreakDuringStart { get; set; } = false;

        /// <inheritdoc cref="DebugCometaryAttribute" />
        public DebugCometaryAttribute()
        {
            LastInstance = this;
        }

        /// <inheritdoc />
        public override IEnumerable<CompilationEditor> Initialize()
        {
            return new CompilationEditor[] { new DebuggingEditor(this) };
        }

        internal static DebugCometaryAttribute LastInstance;
    }
}
