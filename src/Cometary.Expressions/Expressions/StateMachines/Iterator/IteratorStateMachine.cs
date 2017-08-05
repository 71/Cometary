using System;
using System.Collections;
using System.Collections.Generic;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents an <see cref="IEnumerator{T}"/> that fetches
    /// new items using a <see langword="delegate"/>.
    /// </summary>
    /// <typeparam name="T">The type of the item to return.</typeparam>
    internal sealed class IteratorStateMachine<T> : VirtualStateMachine<IteratorStateMachine<T>>, IEnumerator<T>, IEnumerable<T>
    {
        private T current;

        /// <inheritdoc />
        public T Current
        {
            get
            {
                if (!IsStarted)
                    throw new InvalidOperationException("Cannot get the current value before its initialization.");

                return current;
            }
        }

        /// <inheritdoc />
        object IEnumerator.Current => Current;

        /// <inheritdoc />
        public bool MoveNext()
        {
            if (IsCompleted || IsFaulted)
                return false;

            current = Next<T>();
            return !IsCompleted && !IsFaulted;
        }

        /// <inheritdoc />
        public override void Reset()
        {
            current = default(T);

            base.Reset();
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            IteratorStateMachine<T> ism = (IteratorStateMachine<T>)this.MemberwiseClone();
            ism.Reset();

            return ism;
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
