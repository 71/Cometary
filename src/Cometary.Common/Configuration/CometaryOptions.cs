using System;
using System.Collections.Immutable;

namespace Cometary
{
    /// <summary>
    ///   Represents a function invoked when certain Cometary-related events are triggered.
    /// </summary>
    public delegate void Hook(CometaryState state);

    /// <summary>
    ///   Class used to store options set by Cometary and its extensions.
    /// </summary>
    public class CometaryOptions
    {
        internal static CometaryOptions builtOptions;

        /// <summary>
        ///   Gets the global <see cref="CometaryOptions"/>.
        /// </summary>
        public static CometaryOptions Options
            => builtOptions ?? (builtOptions = new CometaryOptions(ImmutableDictionary.Create<string, object>(), ImmutableArray.Create<Hook>()));

        /// <summary>
        ///   Gets the key / value store that contains all data
        ///   set by extensions.
        /// </summary>
        public ImmutableDictionary<string, object> Properties { get; }

        /// <summary>
        ///   Gets the list that contains all hooks that will be executed automatically.
        /// </summary>
        public ImmutableArray<Hook> Hooks { get; }

        /// <summary>
        ///   Gets whether or not this instance of <see cref="CometaryOptions"/> is the default one.
        /// </summary>
        public bool IsDefault => Properties.IsEmpty && Hooks.IsEmpty;

        internal CometaryOptions(ImmutableDictionary<string, object> props, ImmutableArray<Hook> hooks)
        {
            Properties = props;
            Hooks = hooks;
        }

        /// <summary>
        ///   Gets a key that can be used to store
        ///   data in the key / value store associated with this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> that's certified to be available.</returns>
        public string GetRandomKey()
        {
            string key;
            Random random = new Random();

            do
            {
                key = random.Next(1, int.MaxValue).ToString();
            }
            while (Properties.ContainsKey(key));

            return key;
        }

        /// <summary>
        ///   Gets the value of type <typeparamref name="T"/> associated with the given
        /// </summary>
        public T Get<T>(string key)
        {
            return (T)Properties[key];
        }

        /// <summary>
        /// 
        /// </summary>
        public T GetOrDefault<T>(string key, T def = default(T))
        {
            return Properties.TryGetValue(key, out object result) && result is T t ? t : def;
        }

        /// <summary>
        /// 
        /// </summary>
        public (bool, T) TryGet<T>(string key)
        {
            return Properties.TryGetValue(key, out object result) && result is T t ? (true, t) : (false, default(T));
        }

        /// <summary>
        /// 
        /// </summary>
        public bool TryGet<T>(string key, out T value)
        {
            if (Properties.TryGetValue(key, out object result) && result is T t)
            {
                value = t;
                return true;
            }

            value = default(T);
            return false;
        }
    }
}