using System;

namespace Cometary.Composition
{
    /// <summary>
    ///   Indicates that this class should implement one or more components.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ComposeAttribute : Attribute
    {
        /// <summary>
        ///   Gets the types of every <see cref="Component"/> that composes the marked class.
        /// </summary>
        public Type[] ComponentTypes { get; }

        /// <summary>
        ///   Creates a new <see cref="ComposeAttribute"/>, specifying
        ///   the types of every <see cref="Component"/> that composes this class.
        /// </summary>
        public ComposeAttribute(params Type[] componentsTypes)
        {
            ComponentTypes = componentsTypes;
        }
    }
}
