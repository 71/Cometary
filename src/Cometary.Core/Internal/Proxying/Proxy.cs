using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.CodeAnalysis;

namespace Cometary
{
    /// <summary>
    ///   Object that provides a proxy to an internal object.
    ///   It attempts to limit its use of Reflection to speed up its usage as much as possible.
    /// </summary>
    internal sealed class Proxy
    {
        #region Utils
        private const BindingFlags ALL = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.FlattenHierarchy;

        private static readonly Func<int, int, int> CombineHashes
            = ReflectionHelpers.CodeAnalysisAssembly
                               .GetType("Roslyn.Utilities.Hash")
                               .GetMethods(ALL)
                               .First(x => x.Name == "Combine" && x.GetParameters()[0].ParameterType == typeof(int))
                               .CreateDelegate(typeof(Func<int, int, int>)) as Func<int, int, int>;

        private static int Combine(int a, int b) => CombineHashes(a, b);

        #endregion

        /// <summary>
        ///   Persistent data used across proxies of the same type.
        /// </summary>
        private readonly PersistentProxyData data;

        /// <summary>
        ///   Gets the <see cref="object"/> on which calls will be made.
        /// </summary>
        public object Object { get; }

        /// <summary>
        ///   Gets the type of the object.
        /// </summary>
        public Type ObjectType { get; }

        /// <summary>
        ///   Gets the hash code of the type of the object.
        /// </summary>
        public int ObjectTypeHash { get; }

        internal Proxy(object obj, Type objType, PersistentProxyData data)
        {
            Object = obj;
            ObjectType = objType;
            ObjectTypeHash = obj.GetType().GetHashCode();

            this.data = data;
        }

        public object Invoke(string name, params object[] args) => TryInvoke(name, args, out var result)
            ? result
            : throw new InvalidOperationException();
        public T Invoke<T>(string name, params object[] args) => (T)Invoke(name, args);

        public object Get(string name) => TryGet(name, out var result) ? result : throw new InvalidOperationException();
        public T Get<T>(string name) => (T)Get(name);

        public void Set(string name, object value)
        {
            if (!TrySet(name, value))
                throw new InvalidOperationException();
        }

        /// <inheritdoc />
        public bool TryInvoke(string name, object[] args, out object result)
        {
            object obj = Object;

            // Compute key, and try to find an already computed delegate
            int key = Combine(Combine(ObjectTypeHash, name.GetHashCode()), args.Length.GetHashCode());
            var objType = obj.GetType();

            if (data.Invokers.TryGetValue(key, out var del))
            {
                result = del(obj, args);
                return true;
            }

            // Nothing already computed, compute it now
            MethodInfo[] possibleMethods = objType.GetMethods(ALL);

            for (int i = 0; i < possibleMethods.Length; i++)
            {
                MethodInfo mi = possibleMethods[i];

                if (mi.Name != name)
                    continue;

                // Same name, but do the parameters match?
                ParameterInfo[] parameters = mi.GetParameters();

                if (parameters.Length != args.Length)
                    continue;

                for (int j = 0; j < parameters.Length; j++)
                {
                    object arg = args[j];
                    ParameterInfo parameter = parameters[j];

                    if (arg == null)
                    {
                        if (parameter.ParameterType.GetTypeInfo().IsValueType)
                            goto Nope;

                        continue;
                    }

                    if (!parameter.ParameterType.IsInstanceOfType(arg))
                        goto Nope;
                }

                // The parameters do match, create a delegate and return its invocation result
                result = mi.Invoke(obj, args);
                return true;

                // TODO: Fix this. I can't get it to work.
                //data.Invokers[key] = del = Helpers.MakeDelegate<Func<object, object[], object>>(name, il =>
                //{
                //    bool isStatic = mi.IsStatic;

                //    if (!isStatic)
                //    {
                //        Type declaringType = mi.DeclaringType;

                //        il.Emit(OpCodes.Ldarg_0);

                //        if (declaringType.GetTypeInfo().IsValueType)
                //        {
                //            LocalBuilder loc = il.DeclareLocal(declaringType, false);

                //            il.Emit(OpCodes.Unbox_Any, declaringType);
                //            il.Emit(OpCodes.Stloc, loc);
                //            il.Emit(OpCodes.Ldloca, loc);
                //        }
                //        else // Who the f proxies object? if (declaringType != typeof(object))
                //        {
                //            il.Emit(OpCodes.Castclass, declaringType);
                //        }
                //    }

                //    for (int j = 0; j < parameters.Length; j++)
                //    {
                //        Type type = parameters[j].ParameterType;

                //        il.Emit(OpCodes.Ldarg_1);
                //        il.Emit(OpCodes.Ldc_I4, j);
                //        il.Emit(OpCodes.Ldelem_Ref);

                //        if (type.GetTypeInfo().IsValueType)
                //            il.Emit(OpCodes.Unbox_Any, type);
                //        else if (type != typeof(object))
                //            il.Emit(OpCodes.Castclass, type);
                //    }

                //    il.Emit(isStatic || mi.DeclaringType.GetTypeInfo().IsValueType ? OpCodes.Call : OpCodes.Callvirt, mi);

                //    if (mi.ReturnType.GetTypeInfo().IsValueType)
                //    {
                //        il.Emit(OpCodes.Box, mi.ReturnType);
                //    }
                //    else if (mi.ReturnType == typeof(void))
                //    {
                //        il.Emit(OpCodes.Ldnull);
                //    }

                //    il.Emit(OpCodes.Ret);
                //}, mi.DeclaringType);

                //result = del(obj, args);
                //return true;

                Nope:;
            }

            result = null;
            return false;
        }

        /// <inheritdoc />
        public bool TryGet(string name, out object result)
        {
            object obj = Object;

            // Compute key, and try to find an already computed delegate
            int key = Combine(ObjectTypeHash, name.GetHashCode());

            if (data.Getters.TryGetValue(key, out var del))
            {
                result = del(obj);
                return true;
            }

            // Nothing already computed, compute it now
            PropertyInfo prop = obj.GetType().GetProperty(name, ALL);

            if (prop == null)
            {
                result = null;
                return false;
            }

            data.Getters[key] = del = Helpers.MakeDelegate<Func<object, object>>(name, il =>
            {
                if (!(prop.GetMethod ?? prop.SetMethod).IsStatic)
                    il.Emit(OpCodes.Ldarg_0);

                il.Emit(OpCodes.Call, prop.GetMethod);

                if (prop.PropertyType.GetTypeInfo().IsValueType)
                    il.Emit(OpCodes.Box, prop.PropertyType);

                il.Emit(OpCodes.Ret);
            });

            result = del(obj);
            return true;
        }

        /// <inheritdoc />
        public bool TrySet(string name, object value)
        {
            // Compute key, and try to find an already computed delegate
            int key = Combine(ObjectTypeHash, name.GetHashCode());

            if (data.Setters.TryGetValue(key, out var del))
            {
                del(Object, value);
                return true;
            }

            // Nothing already computed, compute it now
            PropertyInfo prop = ObjectType.GetProperty(name, ALL);

            if (prop == null)
                return false;

            data.Setters[key] = Helpers.MakeDelegate<Func<object, object, object>>(name, il =>
            {
                if ((prop.GetMethod ?? prop.SetMethod).IsStatic)
                {
                    il.Emit(OpCodes.Ldarg_0);
                }
                else
                {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_1);
                }

                if (prop.PropertyType.GetTypeInfo().IsValueType)
                    il.Emit(OpCodes.Unbox_Any);
                else if (prop.PropertyType != typeof(object))
                    il.Emit(OpCodes.Castclass, prop.PropertyType);

                il.Emit(OpCodes.Call, prop.SetMethod);

                if (prop.PropertyType.GetTypeInfo().IsValueType)
                    il.Emit(OpCodes.Box, prop.PropertyType);

                il.Emit(OpCodes.Ret);
            });

            return true;
        }
    }
}