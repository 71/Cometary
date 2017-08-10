using System.Threading;

namespace Cometary
{
    /// <summary>
    ///   Represents a <see langword="delegate"/> that returns a value
    ///   based on or equal to the specified <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value to edit.</typeparam>
    public delegate T Edit<T>(T value, CancellationToken token);
}