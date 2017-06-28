using System;

namespace Cometary.Composition
{
    /// <summary>
    ///   Indicates that the marked type constitutes a mixin,
    ///   that can be included in a <see langword="class"/> using the 
    ///   <see cref="TraitAttribute"/> attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public sealed class MixinAttribute : Attribute
    {
    }
}
