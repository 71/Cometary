using System;
using System.ComponentModel;

namespace Cometary.Composition
{
    /// <summary>
    ///   Attribute used to save the content of a component.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ComponentAttribute : Attribute
    {
        /// <summary>
        ///   Gets the code (or content) from which the component was made.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string Content { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ComponentAttribute(string content)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        /// <summary>
        ///   Indicates that this class should be registered as a component.
        /// </summary>
        public ComponentAttribute()
        {
        }
    }
}
