using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
#if !APPDOMAINS
using System.Runtime.Loader;
#endif
using Microsoft.CodeAnalysis;

namespace Cometary
{
    /// <summary>
    ///   Manages assembly resolution during the <see cref="CometaryAnalyzer"/> initialization.
    /// </summary>
    internal static class AssemblyLoading
    {
        public static Dictionary<string, Assembly> Loaded = new Dictionary<string, Assembly>();
        public static Compilation CurrentCompilation { get; set; }

        public static void EnableResolutionHelp()
        {
#if APPDOMAINS
            AppDomain.CurrentDomain.AssemblyResolve += Resolving;
#else
            AssemblyLoadContext.Default.Resolving += Resolving;
#endif
        }

        public static void DisableResolutionHelp()
        {
#if APPDOMAINS
            AppDomain.CurrentDomain.AssemblyResolve -= Resolving;
#else
            AssemblyLoadContext.Default.Resolving -= Resolving;
#endif
        }


#if APPDOMAINS
        private static Assembly Resolving(object sender, ResolveEventArgs args)
        {
            var compilation = CurrentCompilation;
            var assemblyName = new AssemblyName(args.Name);

            if (Loaded.TryGetValue(assemblyName.Name, out Assembly result))
                return result;

            if (compilation == null)
                return null;

            foreach (var reference in compilation.References)
            {
                if (!(reference is PortableExecutableReference pe) ||
                    !Path.GetFileNameWithoutExtension(pe.Display).Equals(assemblyName.Name, StringComparison.OrdinalIgnoreCase))
                    continue;

                try
                {
                    Assembly assembly = LoadCore(pe.FilePath);

                    Loaded[assemblyName.Name] = assembly;

                    return assembly;
                }
                catch
                {
                    // ReSharper disable once RedundantJumpStatement
                    continue;
                }
            }

            return AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(x => x.GetName().Name == assemblyName.Name && x.GetName().Version == assemblyName.Version);
        }

        // Wrap the 'Load' logic in another call because of this:
        // https://github.com/dotnet/coreclr/blob/master/Documentation/botr/type-loader.md#2-type-loader-architecture
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Assembly LoadCore(string path) => Assembly.LoadFrom(path);
#else
        private static Assembly Resolving(AssemblyLoadContext ctx, AssemblyName assemblyName)
        {
            var compilation = CurrentCompilation;

            if (Loaded.TryGetValue(assemblyName.Name, out Assembly result))
                return result;

            if (compilation == null)
                return null;

            foreach (var reference in compilation.References)
            {
                if (!(reference is PortableExecutableReference pe) ||
                    !Path.GetFileNameWithoutExtension(pe.Display).Equals(assemblyName.Name, StringComparison.OrdinalIgnoreCase))
                    continue;

                try
                {
                    Assembly assembly = LoadCore(ctx, pe.FilePath);

                    Loaded[assemblyName.Name] = assembly;

                    return assembly;
                }
                catch
                {
                    // ReSharper disable once RedundantJumpStatement
                    continue;
                }
            }

            return null;
        }

        // Wrap the 'LoadFromAssemblyPath' logic in another call because of this:
        // https://github.com/dotnet/coreclr/blob/master/Documentation/botr/type-loader.md#2-type-loader-architecture
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Assembly LoadCore(AssemblyLoadContext ctx, string path) => ctx.LoadFromAssemblyPath(path);
#endif
    }
}
