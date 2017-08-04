using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;

namespace Cometary
{
    /// <summary>
    ///   Manages assembly resolution.
    /// </summary>
    internal static class AssemblyLoading
    {
        public static Compilation CurrentCompilation { get; set; }

        public static void EnableResolutionHelp()
        {
            AssemblyLoadContext.Default.Resolving += Resolving;
        }

        public static void DisableResolutionHelp()
        {
            AssemblyLoadContext.Default.Resolving -= Resolving;
        }

        private static Assembly Resolving(AssemblyLoadContext ctx, AssemblyName assemblyName)
        {
            var compilation = CurrentCompilation;

            if (compilation == null)
                return null;

            foreach (var reference in compilation.References)
            {
                if (!(reference is PortableExecutableReference pe) ||
                    !Path.GetFileNameWithoutExtension(pe.Display).Equals(assemblyName.Name, StringComparison.OrdinalIgnoreCase))
                    continue;

                try
                {
                    return LoadCore(ctx, pe.FilePath);
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
    }
}
