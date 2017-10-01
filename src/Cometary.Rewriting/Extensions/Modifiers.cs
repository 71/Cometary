using System;

namespace Cometary
{
    /// <summary>
    /// Represents a modifier that can be applied to
    /// a member.
    /// </summary>
    [Flags]
    public enum Modifiers
    {
#pragma warning disable CS1591
        None      = 0,
        Public    = 1 << 0,
        Private   = 1 << 1,
        Protected = 1 << 2,
        Internal  = 1 << 3,

        Static    = 1 << 4,
        Abstract  = 1 << 5,
        Virtual   = 1 << 6,
        Extern    = 1 << 7,
        Partial   = 1 << 8,

        Async     = 1 << 9
#pragma warning restore
    }
}
