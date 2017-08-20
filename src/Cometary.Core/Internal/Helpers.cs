using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using TypeInfo = System.Reflection.TypeInfo;

namespace Cometary
{
    /// <summary>
    /// 
    /// </summary>
    internal static class Helpers
    {
        internal static readonly object RecomputeKey = new object();

        /// <summary>
        /// 
        /// </summary>
        private static readonly Lazy<Action<CSharpCompilation, IAssemblySymbol>> LazySetAssembly = MakeLazyDelegate<Action<CSharpCompilation, IAssemblySymbol>>("SetAssembly", il =>
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, typeof(CSharpCompilation)
                .GetField("_lazyAssemblySymbol", BindingFlags.Instance | BindingFlags.NonPublic)
            );
        });

        /// <summary>
        /// 
        /// </summary>
        internal static Lazy<TDelegate> MakeLazyDelegate<TDelegate>(string name, Action<ILGenerator> ilgen)
            where TDelegate : Delegate
        {
            Debug.Assert(typeof(TDelegate).GetMethod("Invoke") != null);

            return new Lazy<TDelegate>(() => MakeDelegate<TDelegate>(name, ilgen));
        }

        /// <summary>
        /// 
        /// </summary>
        internal static TDelegate MakeDelegate<TDelegate>(string name, Action<ILGenerator> ilgen, Type owner = null)
            where TDelegate : Delegate
        {
            MethodInfo invokeMethod = typeof(TDelegate).GetMethod("Invoke");

            Debug.Assert(invokeMethod != null);

            ParameterInfo[] parameters = invokeMethod.GetParameters();
            Type[] parameterTypes = new Type[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                parameterTypes[i] = parameters[i].ParameterType;
            }

            DynamicMethod method = owner == null
                ? new DynamicMethod(name, invokeMethod.ReturnType, parameterTypes, true)
                : new DynamicMethod(name, invokeMethod.ReturnType, parameterTypes, owner, true);

            ILGenerator il = method.GetILGenerator();

            ilgen(il);

            return (TDelegate)method.CreateDelegate(typeof(TDelegate));
        }

        /// <summary>
        ///   Creates a <see langword="delegate"/> that adds diagnostics
        ///   to the given <paramref name="diagnosticBag"/>.
        /// </summary>
        internal static Action<Diagnostic> MakeAddDiagnostic(object diagnosticBag)
        {
            return (Action<Diagnostic>)diagnosticBag.GetType()
                .GetRuntimeMethod("Add", new[] { typeof(Diagnostic) })
                .CreateDelegate(typeof(Action<Diagnostic>), diagnosticBag);
        }

        /// <summary>
        ///   Creates a <see langword="delegate"/> that returns all diagnostics
        ///   in a given <paramref name="diagnosticBag"/>.
        /// </summary>
        internal static Func<IEnumerable<Diagnostic>> MakeGetDiagnostics(object diagnosticBag)
        {
            return (Func<IEnumerable<Diagnostic>>)diagnosticBag.GetType()
                .GetRuntimeMethod("AsEnumerable", Type.EmptyTypes)
                .CreateDelegate(typeof(Func<IEnumerable<Diagnostic>>), diagnosticBag);
        }

        /// <summary>
        ///   Returns whether or not the described method is an override of the base type.
        /// </summary>
        internal static bool IsMethodOverriden(this Type type, Type baseType, string methodName, params Type[] parameters)
        {
            Debug.Assert(type != null);
            Debug.Assert(baseType != null);
            Debug.Assert(methodName != null);

            return type.GetMethod(methodName, parameters)?.DeclaringType == baseType;
        }

        /// <summary>
        ///   Sets the <see cref="Compilation.Assembly"/> of the specified <paramref name="compilation"/>
        ///   to the given <paramref name="assembly"/>.
        /// </summary>
        internal static void SetAssembly(this CSharpCompilation compilation, IAssemblySymbol assembly)
        {
            Debug.Assert(assembly.GetType().Name == "SourceAssemblySymbol");

            LazySetAssembly.Value(compilation, assembly);
        }

        internal static string Filter(this string str) => str.Replace('\r', ' ').Replace('\n', ' ');

        internal static CSharpCompilation RecomputeCompilationWithOptions(this CSharpCompilation compilation,
            Func<CSharpParseOptions, CSharpParseOptions> selector, CancellationToken cancellationToken)
        {
            SyntaxTree[] syntaxTrees = new SyntaxTree[compilation.SyntaxTrees.Length];

            for (int i = 0; i < syntaxTrees.Length; i++)
            {
                CSharpSyntaxTree tree = compilation.SyntaxTrees[i] as CSharpSyntaxTree;

                if (tree == null)
                {
                    syntaxTrees[i] = compilation.SyntaxTrees[i];
                    continue;
                }

                SourceText source = tree.GetText(cancellationToken);
                CSharpParseOptions options = selector(tree.Options);

                SyntaxTree newTree = CSharpSyntaxTree.ParseText(source, options, tree.FilePath, cancellationToken);

                syntaxTrees[i] = newTree;
            }

            return compilation.RemoveAllSyntaxTrees().AddSyntaxTrees(syntaxTrees);
        }

        internal static Pipeline<Func<CSharpParseOptions, CSharpParseOptions>> GetRecomputationPipeline(this CompilationEditor editor)
        {
            return editor.SharedStorage.GetOrAdd(RecomputeKey, () => new Pipeline<Func<CSharpParseOptions, CSharpParseOptions>>());
        }

        #region CopyTo & cie
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
        ///   Copies the value of all fields from one <see cref="object"/> to another.
        /// </summary>
        internal static void CopyTo<T>(this T from, T to) where T : class
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
                baseType = FindCommonType(fromType.AsType(), toType.AsType());
            }

            // Copy fields from one compilation to the other
            foreach (FieldInfo field in baseType.GetAllFields())
            {
                if (field.IsStatic)
                    continue;

                field.SetValue(to, field.GetValue(from));
            }
        }
        #endregion
    }
}
