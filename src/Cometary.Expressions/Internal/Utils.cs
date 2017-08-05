using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Cometary.Expressions
{
    internal static class Utils
    {
        internal static IEnumerable<MemberInfo> GetMembers(this Type type)
        {
            return type.GetTypeInfo().DeclaredMembers;
        }

        internal static MemberInfo GetMember(this Type type, string name)
        {
            return type.GetTypeInfo().DeclaredMembers.First(x => x.Name == name);
        }

        internal static IEnumerable<MemberInfo> GetMembers(this TypeInfo type)
        {
            return type.DeclaredMembers;
        }

        internal static MemberInfo GetMember(this TypeInfo type, string name)
        {
            return type.DeclaredMembers.First(x => x.Name == name);
        }

        internal static IEnumerable<MethodInfo> GetMethods(this TypeInfo type, string name)
        {
            return type.BaseType == null
                ? type.GetDeclaredMethods(name)
                : type.GetDeclaredMethods(name).Concat(type.BaseType.GetTypeInfo().GetMethods(name));
        }

        internal static bool IsAssignableTo<T>(this Expression expr)
        {
            return typeof(T).GetTypeInfo().IsAssignableFrom(expr.Type.GetTypeInfo());
        }

        internal static bool IsAssignableTo(this Expression expr, Type type)
        {
            return type.GetTypeInfo().IsAssignableFrom(expr.Type.GetTypeInfo());
        }

        internal static bool IsAssignableTo<T>(this Type self)
        {
            return typeof(T).GetTypeInfo().IsAssignableFrom(self.GetTypeInfo());
        }

        internal static bool IsAssignableTo(this Type self, Type type)
        {
            return type.GetTypeInfo().IsAssignableFrom(self.GetTypeInfo());
        }

        internal static Type GetItemType(Expression enumerable)
        {
            Type type = enumerable.Type;

            if (type.IsArray)
                return type.GetElementType();

            TypeInfo enumerableTypeInfo = typeof(IEnumerable<>).GetTypeInfo();
            TypeInfo typeInfo = type.GetTypeInfo();

            foreach (var iface in typeInfo.ImplementedInterfaces)
            {
                if (enumerableTypeInfo.IsAssignableFrom(iface.GetTypeInfo()))
                {
                    return iface.GenericTypeArguments[0];
                }
            }

            return typeof(object);
        }

        internal static IEnumerable<T> Append<T>(this IEnumerable<T> enumerable, T item)
        {
            foreach (T obj in enumerable)
                yield return obj;

            yield return item;
        }

        internal static IEnumerable<T> Prepend<T>(this IEnumerable<T> enumerable, T item)
        {
            yield return item;

            foreach (T obj in enumerable)
                yield return obj;
        }

        internal static TypeCode GetTypeCode(this Type type)
        {
            return Enum.TryParse(type.Name, out TypeCode code) ? code : TypeCode.Object;
        }

        internal static bool IsDelegateType(this Type type)
        {
            return type.IsAssignableTo<Delegate>();
        }

        internal static bool IsCompilerGenerated(this Type type)
        {
            return type.GetTypeInfo().IsCompilerGenerated();
        }

        internal static bool IsCompilerGenerated(this TypeInfo type)
        {
            return type.GetCustomAttribute<CompilerGeneratedAttribute>(false) != null;
        }

        internal static Expression Convert(this Expression node, Type target)
        {
            if (node.Type == target)
                return node;

            return Expression.Convert(node, target);
        }

        internal static U[] Select<T, U>(this IReadOnlyList<T> list, Func<T, U> selector)
        {
            U[] result = new U[list.Count];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = selector(list[i]);
            }

            return result;
        }
    }
}
