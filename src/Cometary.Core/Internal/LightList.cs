using System;
using System.Collections;
using System.Collections.Generic;

namespace Cometary
{
    /// <summary>
    ///   Represents a lightweight array with the ability to
    ///   add values, and clear all values.
    /// </summary>
    internal sealed class LightList<T> : IList<T>, IReadOnlyList<T>
    {
        private T[] underlyingArray = new T[0];

        public T this[int index]
        {
            get => underlyingArray[index];
            set => underlyingArray[index] = value;
        }

        T IList<T>.this[int index]
        {
            get => underlyingArray[index];
            set => underlyingArray[index] = value;
        }

        /// <summary>
        ///   Gets the underlying array in which items are stored.
        /// </summary>
        public T[] UnderlyingArray => underlyingArray;

        /// <inheritdoc />
        public int Count => ((IReadOnlyList<T>)this.UnderlyingArray).Count;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public void Add(T item)
        {
            T[] array = underlyingArray;
            int newSize = array.Length + 1;

            T[] newArray = new T[newSize--];
            Array.Copy(array, 0, newArray, 0, newSize);
            newArray[newSize] = item;

            underlyingArray = newArray;
        }

        /// <inheritdoc />
        public void Clear() => underlyingArray = new T[0];

        /// <inheritdoc />
        public bool Contains(T item) => Array.IndexOf(underlyingArray, item) != -1;

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex) => underlyingArray.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        public int IndexOf(T item) => Array.IndexOf(underlyingArray, item);

        /// <inheritdoc />
        public void Insert(int index, T item) => throw new NotSupportedException();

        /// <inheritdoc />
        public bool Remove(T item)
        {
            int index = Array.IndexOf(underlyingArray, item);

            if (index == -1)
                return false;

            RemoveAt(ref underlyingArray, index);

            return true;
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            if (index < Count)
                RemoveAt(ref underlyingArray, index);
        }

        private static void RemoveAt(ref T[] array, int index)
        {
            int newSize = array.Length - 1;

            T[] newArray = new T[newSize];
            Array.Copy(array, 0, newArray, 0, index);
            Array.Copy(array, index, newArray, index, newSize - index);
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => ((IList<T>)UnderlyingArray).GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => UnderlyingArray.GetEnumerator();

        public T[] ToArray() => underlyingArray;
    }
}
