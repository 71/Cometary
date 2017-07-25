using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;

using TypeInfo = System.Reflection.TypeInfo;

namespace Cometary
{
    /// <summary>
    /// 
    /// </summary>
    internal static class Helpers
    {
        /// <summary>
        /// 
        /// </summary>
        internal static Action<Diagnostic> MakeAddDiagnostic(object diagnosticBag)
        {
            return (Action<Diagnostic>)diagnosticBag.GetType()
                .GetRuntimeMethod("Add", new[] { typeof(Diagnostic) })
                .CreateDelegate(typeof(Action<Diagnostic>), diagnosticBag);
        }

        /// <summary>
        ///   Returns every single field declared by the specified <see cref="Type"/>,
        ///   and all of its inherites types.
        /// </summary>
        internal static IEnumerable<FieldInfo> GetAllFields(this Type type)
        {
            do
            {
                foreach (FieldInfo field in type.GetRuntimeFields())
                    yield return field;

                type = type.GetTypeInfo().BaseType;
            }
            while (type != null);
        }

        /// <summary>
        /// <para>
        ///   Returns the first <see cref="Type"/> common to both the given types.
        /// </para>
        /// <para>
        ///   Inheritance hierarchy is respected, and the closest ancestor to both
        ///   the specified types will be chosen.
        /// </para>
        /// </summary>
        internal static Type FindCommonType(Type a, Type b)
        {
            List<Type> aMap = new List<Type>();
            List<Type> bMap = new List<Type>();

            while (a != typeof(Compilation))
            {
                aMap.Insert(0, a);
                a = a.GetTypeInfo().BaseType;
            }

            while (b != typeof(Compilation))
            {
                bMap.Insert(0, b);
                b = b.GetTypeInfo().BaseType;
            }

            for (int i = 1; i < Math.Min(aMap.Count, bMap.Count); i++)
            {
                if (aMap[i] != bMap[i])
                    return aMap[i - 1];
            }

            return typeof(Compilation);
        }

        /// <summary>
        ///   Copies the value of all fields from one <see cref="Compilation"/> to another.
        /// </summary>
        internal static void CopyTo<TCompilation>(this TCompilation from, TCompilation to) where TCompilation : Compilation
        {
            // Find base type of both compilations
            TypeInfo fromType = from.GetType().GetTypeInfo();
            TypeInfo toType = to.GetType().GetTypeInfo();
            Type baseType;

            if (fromType.IsAssignableFrom(toType))
            {
                // ToCompilation inherits FromCompilation
                baseType = fromType.AsType();
            }
            else if (toType.IsAssignableFrom(fromType))
            {
                // FromCompilation inherits ToCompilation
                baseType = toType.AsType();
            }
            else
            {
                // No common type: find first common type
                baseType = Helpers.FindCommonType(fromType.AsType(), toType.AsType());
            }

            // Copy fields from one compilation to the other
            foreach (FieldInfo field in baseType.GetAllFields())
            {
                if (field.IsStatic)
                    continue;

                field.SetValue(to, field.GetValue(from));
            }
        }
    }
}
