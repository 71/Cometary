using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Cometary
{
    public static partial class SourceSymbolFactory
    {
        private static readonly Assembly CSharpAssembly = ReflectionHelpers.CodeAnalysisCSharpAssembly;

        private static readonly Dictionary<int, Func<object[], object>> CachedFuncs = new Dictionary<int, Func<object[], object>>();
        private static readonly Dictionary<int, Func<object[], object>> CachedCtors = new Dictionary<int, Func<object[], object>>();

        private static object Invoke(int key, Type type, string methodName, params object[] args)
        {
            if (CachedFuncs.TryGetValue(key, out var func))
                return func(args);

            // ReSharper disable once CoVariantArrayConversion
            MethodInfo method = (MethodInfo)Proxy.FindMatchingMethod(type.GetMethods(Proxy.ALL), methodName, args);
            ParameterInfo[] parameters = method.GetParameters();

            func = Helpers.MakeDelegate<Func<object[], object>>(methodName, il =>
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    Type paramType = parameters[i].ParameterType;

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Ldelem_Ref);

                    if (paramType == typeof(object))
                        continue;

                    il.Emit(paramType.GetTypeInfo().IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, paramType);
                }

                il.Emit(OpCodes.Call, method);
                il.Emit(OpCodes.Castclass, method.ReturnType);
                il.Emit(OpCodes.Ret);
            }, type);

            CachedFuncs.Add(key, func);

            return func(args);
        }

        private static object Invoke(int key, Type type, params object[] args)
        {
            if (CachedCtors.TryGetValue(key, out var func))
                return func(args);

            // ReSharper disable once CoVariantArrayConversion
            ConstructorInfo ctor = (ConstructorInfo)Proxy.FindMatchingMethod(type.GetConstructors(Proxy.ALL), ".ctor", args);
            ParameterInfo[] parameters = ctor.GetParameters();

            func = Helpers.MakeDelegate<Func<object[], object>>("ctor", il =>
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    Type paramType = parameters[i].ParameterType;

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Ldelem_Ref);

                    if (paramType == typeof(object))
                        continue;

                    il.Emit(paramType.GetTypeInfo().IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, paramType);
                }

                il.Emit(OpCodes.Newobj, ctor);
                il.Emit(OpCodes.Ret);
            }, type);

            CachedCtors.Add(key, func);

            return func(args);
        }
    }
}
