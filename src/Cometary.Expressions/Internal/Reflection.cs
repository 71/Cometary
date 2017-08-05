using System;
using System.Collections;
using System.Reflection;

namespace Cometary.Expressions
{
    internal static class Reflection
    {
        public static readonly TypeInfo IDisposable = typeof(IDisposable).GetTypeInfo();
        public static readonly MethodInfo IDisposable_Dispose = IDisposable.GetDeclaredMethod("Dispose");

        public static readonly TypeInfo IEnumerator = typeof(IEnumerator).GetTypeInfo();
        public static readonly MethodInfo IEnumerator_MoveNext = IEnumerator.GetDeclaredMethod("MoveNext");
    }
}