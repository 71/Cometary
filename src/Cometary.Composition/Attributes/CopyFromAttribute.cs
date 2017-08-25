using System;

namespace Cometary.Composition
{
    /// <summary>
    ///   Indicates that the specified <see cref="Type"/> will have its members
    ///   copied to the current type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class CopyFromAttribute : Attribute
    {
        /// <summary>
        ///   Gets the <see cref="Type"/> whose members will be copied to the marked type.
        /// </summary>
        public Type SourceType { get; }

        /// <summary>
        ///   Creates a new <see cref="CopyFromAttribute"/>, given the <see cref="Type"/>
        ///   of the class whose members shall be copied to this instance.
        /// </summary>
        public CopyFromAttribute(Type type)
        {
            SourceType = type;
        }
    }
}
