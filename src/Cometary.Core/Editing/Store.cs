using System;
using System.Collections.Generic;

namespace Cometary
{
    /// <summary>
    ///   Provides the ability to store values, given a unique key represented
    ///   as a unique <see cref="object"/>.
    /// </summary>
    public sealed class Store
    {
        private readonly LightList<object> Keys;
        private readonly LightList<object> Values;

        internal Store()
        {
            Keys = new LightList<object>();
            Values = new LightList<object>();
        }

        /// <summary>
        ///   Returns the value corresponding to the given <paramref name="key"/>.
        /// </summary>
        public object Get(object key)
        {
            var keys = Keys.UnderlyingArray;

            for (int i = 0; i < keys.Length; i++)
            {
                if (ReferenceEquals(keys[i], key))
                    return Values[i];
            }

            throw new KeyNotFoundException();
        }

        /// <summary>
        ///   Returns the value of type <typeparamref name="T"/> corresponding to the given <paramref name="key"/>.
        /// </summary>
        public T Get<T>(object key) => (T)Get(key);

        /// <summary>
        ///   If it exists, returns the value corresponding to the given <paramref name="key"/>.
        /// </summary>
        public bool TryGet(object key, out object value)
        {
            var keys = Keys.UnderlyingArray;

            for (int i = 0; i < keys.Length; i++)
            {
                if (!ReferenceEquals(keys[i], key))
                    continue;

                value = Values[i];
                return true;
            }

            value = null;
            return false;
        }

        /// <summary>
        ///   If it exists and is of type <typeparamref name="T"/>, returns the value corresponding to the given <paramref name="key"/>.
        /// </summary>
        public bool TryGet<T>(object key, out T value)
        {
            if (TryGet(key, out object obj) && obj is T t)
            {
                value = t;
                return true;
            }

            value = default(T);
            return false;
        }

        /// <summary>
        ///   If a value corresponding to the given <paramref name="key"/> can be found, it will be returned.
        ///   Else, the given delegate will be used to compute its first value, that will also be added to the store, and returned.
        /// </summary>
        public T GetOrAdd<T>(object key, Func<T> value)
        {
            var keys = Keys.UnderlyingArray;

            for (int i = 0; i < keys.Length; i++)
            {
                if (!ReferenceEquals(keys[i], key))
                    continue;

                object obj = Values[i];

                if (obj is T t)
                    return t;

                throw new InvalidCastException();
            }

            T val = value();

            Keys.Add(key);
            Values.Add(val);

            return val;
        }

        /// <summary>
        ///   Adds the given value to the store, and returns the key used
        ///   to access it later on.
        /// </summary>
        public object Add<T>(T value)
        {
            object key = new object();

            Keys.Add(key);
            Values.Add(value);

            return key;
        }

        internal void Dispose(Action<Exception> logException)
        {
            var values = Values.UnderlyingArray;

            for (int i = 0; i < values.Length; i++)
            {
                if (!(values[i] is IDisposable disposable))
                    continue;

                try
                {
                    disposable.Dispose();
                }
                catch (Exception e)
                {
                    // Eh.
                    logException(e);
                }
            }
        }
    }
}
