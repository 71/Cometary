using System;
using System.Collections.Generic;
using System.Linq;

namespace Cometary
{
    internal static class Requires
    {
        internal static void NotNull(object value, string paramName)
        {
            if (value == null)
                throw new ArgumentNullException(paramName);
        }

        internal static void NoneNull<T>(IEnumerable<T> values, string paramName) where T : class
        {
            NotNull(values, paramName);

            if (values.Contains(null))
                throw new ArgumentException("The given collection should not contain a null reference.", paramName);
        }

        internal static void NoneNull<T>(IReadOnlyList<T> values, string paramName) where T : class
        {
            NotNull(values, paramName);

            for (int i = 0; i < values.Count; i++)
            {
                if (ReferenceEquals(values[i], null))
                    throw new ArgumentException("The given collection should not contain a null reference.", paramName);
            }
        }

        internal static void NoneNull<T>(T[] values, string paramName) where T : class
        {
            NotNull(values, paramName);

            if (Array.IndexOf(values, null) != -1)
                throw new ArgumentException("The given collection should not contain a null reference.", paramName);
        }
    }
}
