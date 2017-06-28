using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Cometary
{
    /// <summary>
    ///   Class used to configure the global <see cref="CometaryOptions"/> instance.
    /// </summary>
    public sealed class CometaryConfigurator : CometaryOptions
    {
        internal CometaryConfigurator(ImmutableDictionary<string, object> data, ImmutableArray<Hook> hooks) : base(data, hooks)
        {
        }

        internal CometaryConfigurator() : base(ImmutableDictionary<string, object>.Empty, ImmutableArray<Hook>.Empty)
        {
        }

        #region Data
        /// <summary>
        ///   Adds the given <paramref name="key"/> / <paramref name="value"/>
        ///   pair to the options.
        /// </summary>
        public CometaryConfigurator AddData<T>(string key, T value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return new CometaryConfigurator(Properties.Add(key, value), Hooks);
        }

        /// <summary>
        ///   Adds the given key / value
        ///   <paramref name="pair"/> to the options.
        /// </summary>
        public CometaryConfigurator AddData<T>(KeyValuePair<string, T> pair)
        {
            if (pair.Key == null)
                throw new ArgumentException("The given pair's key cannot be null.", nameof(pair));

            return new CometaryConfigurator(Properties.Add(pair.Key, pair.Value), Hooks);
        }

        /// <summary>
        ///   Sets the given <paramref name="key"/> / <paramref name="value"/>
        ///   pair in the options.
        /// </summary>
        public CometaryConfigurator WithData<T>(string key, T value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return new CometaryConfigurator(Properties.SetItem(key, value), Hooks);
        }

        /// <summary>
        ///   Sets the given key / value
        ///   <paramref name="pair"/> in the options.
        /// </summary>
        public CometaryConfigurator WithData<T>(KeyValuePair<string, T> pair)
        {
            if (pair.Key == null)
                throw new ArgumentException("The given pair's key cannot be null.", nameof(pair));

            return new CometaryConfigurator(Properties.SetItem(pair.Key, pair.Value), Hooks);
        }

        /// <summary>
        /// <para>
        ///   Adds the given <paramref name="key"/> / <paramref name="value"/>
        ///   pair to the options.
        /// </para>
        /// <para>
        ///   If the <paramref name="key"/> already exists, the corresponding <paramref name="value"/>
        ///   will be replaced by calling <paramref name="updateFunc"/> on its previous value.
        /// </para>
        /// </summary>
        public CometaryConfigurator WithData<T>(string key, T value, Func<T, T> updateFunc)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (updateFunc == null)
                throw new ArgumentNullException(nameof(key));

            return new CometaryConfigurator(Properties.TryGetValue(key, out object val)
                    ? Properties.SetItem(key, updateFunc((T)val))
                    : Properties.Add(key, value),
                Hooks);
        }

        /// <summary>
        ///   Removes the pair identified by the given <see cref="key"/>
        ///   from the options.
        /// </summary>
        public CometaryConfigurator RemoveData(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return new CometaryConfigurator(Properties.Remove(key), Hooks);
        }

        /// <summary>
        ///   Gets whether or not the given <see cref="key"/> has already been used to save data.
        /// </summary>
        public bool ContainsData(string key) => Properties.ContainsKey(key);
        #endregion

        #region Hooks
        /// <summary>
        ///   Adds the given <paramref name="hook"/> to the registered hooks.
        /// </summary>
        public CometaryConfigurator AddHook(Hook hook)
        {
            if (hook == null)
                throw new ArgumentNullException(nameof(hook));

            return new CometaryConfigurator(Properties, Hooks.Add(hook));
        }

        /// <summary>
        ///   Removes the given <see cref="hook"/> from the registered hooks.
        /// </summary>
        public CometaryConfigurator RemoveHook(Hook hook)
        {
            if (hook == null)
                throw new ArgumentNullException(nameof(hook));

            return new CometaryConfigurator(Properties, Hooks.Remove(hook));
        }

        /// <summary>
        ///   Gets whether or not the given <see cref="hook"/> has already been registered.
        /// </summary>
        public bool ContainsHook(Hook hook) => Hooks.Contains(hook);
        #endregion

        /// <summary>
        ///   Sets the global <see cref="CometaryOptions"/> instance to <see langword="this"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Global Cometary options have already been set.</exception>
        internal CometaryOptions Build()
        {
            if (builtOptions != null)
                throw new InvalidOperationException("Options can only be built once.");

            return builtOptions = new CometaryOptions(Properties, Hooks);
        }
    }
}