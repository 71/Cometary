using System;

namespace Cometary.Composition
{
    /// <summary>
    ///   Defines the base class for all composition attributes.
    /// </summary>
    public abstract class CompositionAttribute : Attribute
    {
        /// <summary>
        ///   Gets the <see cref="Component"/> to apply to the marked class.
        /// </summary>
        public Component Component { get; }

        /// <summary>
        ///   Creates a new <see cref="CompositionAttribute"/>, specifying
        ///   the component to apply.
        /// </summary>
        protected CompositionAttribute(Component component)
        {
            Component = component;
        }
    }
}
