using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cometary
{
    /// <summary>
    /// <para>
    ///   Represents a read-only list whose content is that of multiple
    ///   other read-only lists flattened.
    /// </para>
    /// <para>
    ///   For enumeration, consider using <see cref="GetEnumerator"/> instead of a
    ///   <see langword="for"/> loop, the former being much faster.
    /// </para>
    /// </summary>
    internal sealed class FlatteningList<T> : IReadOnlyList<T>
    {
        private readonly LightList<IReadOnlyList<T>> lists = new LightList<IReadOnlyList<T>>();

        /// <summary>
        /// 
        /// </summary>
        public void AddRange(IReadOnlyList<T> items) => lists.Add(items);

        /// <summary>
        /// 
        /// </summary>
        public void RemoveRange(IReadOnlyList<T> items) => lists.Remove(items);

        /// <inheritdoc />
        public T this[int index]
        {
            get
            {
                IReadOnlyList<T>[] lists = this.lists.UnderlyingArray;
                int total = 0;

                for (int i = 0; i < lists.Length; i++)
                {
                    var list = lists[i];

                    if (index <= total + list.Count)
                        return list[total + index];

                    total += list.Count;
                }

                throw new IndexOutOfRangeException();
            }
        }

        /// <inheritdoc />
        public int Count
        {
            get
            {
                int count = 0;
                IReadOnlyList<T>[] lists = this.lists.UnderlyingArray;

                for (int i = 0; i < lists.Length; i++)
                {
                    count += lists[i].Count;
                }

                return count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Contains(T item)
        {
            var lists = this.lists.UnderlyingArray;

            if (item is IEquatable<T> equatable)
                return ContainsEquatable(lists, equatable);

            for (int i = 0; i < lists.Length; i++)
            {
                var list = lists[i];

                for (int j = 0; j < list.Count; j++)
                {
                    if (item.Equals(list[j]))
                        return true;
                }
            }

            return false;
        }

        private static bool ContainsEquatable(IReadOnlyList<T>[] lists, IEquatable<T> item)
        {
            for (int i = 0; i < lists.Length; i++)
            {
                var list = lists[i];

                for (int j = 0; j < list.Count; j++)
                {
                    if (item.Equals(list[j]))
                        return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => lists.SelectMany(x => x).GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private struct Enumerator : IEnumerator<T>
        {
            private readonly LightList<IReadOnlyList<T>> innerList;
            private int pos;
            private int posInList;

            /// <inheritdoc />
            public T Current => innerList[pos][posInList];

            /// <inheritdoc />
            object IEnumerator.Current => Current;

            internal Enumerator(FlatteningList<T> list)
            {
                this.innerList = list.lists;
                this.posInList = -1;
                this.pos = 0;
            }

            /// <inheritdoc />
            public bool MoveNext()
            {
                int pos = this.pos;
                var lists = this.innerList;

                Console.WriteLine($"Pos: {pos} / {lists.Count}");

                TryAgain:

                if (lists.Count == pos)
                    return false;

                IReadOnlyList<T> list = lists[pos];
                int posInList = this.posInList;

                Console.WriteLine($"Pos in list: {posInList} / {list.Count}");

                // TODO: Fix this
                if (++posInList >= list.Count)
                {
                    pos = ++this.pos;
                    posInList = this.posInList = -1;

                    goto TryAgain;
                }

                return true;
            }

            /// <inheritdoc />
            public void Reset()
            {
                posInList = -1;
                pos = 0;
            }

            /// <inheritdoc />
            public void Dispose()
            {
            }
        }
    }
}
