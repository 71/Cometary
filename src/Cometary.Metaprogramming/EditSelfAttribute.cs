using System;
using System.Collections.Generic;

namespace Cometary
{
    /// <summary>
    ///   Indicates that this assembly should edit itself on compilation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public sealed class EditSelfAttribute : CometaryAttribute
    {
        internal readonly Type[] editorTypes;

        /// <summary>
        ///   Initializes a new <see cref="EditSelfAttribute"/>.
        /// </summary>
        /// <param name="editorTypes">
        ///   A list of types, each representing a <see cref="CompilationEditor"/>
        ///   that is to edit the current assembly.
        /// </param>
        public EditSelfAttribute(params Type[] editorTypes)
        {
            // That's tricky:
            // In order to load the editor types, the assembly needs to be loaded
            // in memory. However, in order to load the assembly, we need to construct the self
            // editor. So what are we gonna do?
            // When we construct this, simply ask for some null types. When we load the assembly in memory,
            // load the actual types
            this.editorTypes = editorTypes;
        }

        /// <inheritdoc />
        public override IEnumerable<CompilationEditor> Initialize()
        {
            return new CompilationEditor[] { new SelfEditor() };
        }
    }
}
