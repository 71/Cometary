using System;
using System.Collections;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Cometary.Expressions
{
    internal static class Requires
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void AssignableTo<T>(Expression node, string paramName)
        {
            if (!node.IsAssignableTo<T>())
                throw Error.ArgumentMustBeAssignableTo<T>(paramName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void NotNull(object value, string paramName)
        {
            if (value == null)
                throw new ArgumentNullException(paramName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void NotNull(IEnumerable values, string paramName)
        {
            if (values == null)
                throw new ArgumentNullException(paramName);

            int index = 0;
            foreach (object value in values)
            {
                if (value == null)
                    throw new ArgumentNullException($"{paramName}[{index}]");

                index++;
            }
        }
    }
}
