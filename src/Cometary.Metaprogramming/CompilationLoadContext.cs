using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary
{
    /// <summary>
    ///   <see cref="AssemblyLoadContext"/> that uses a <see cref="CSharpCompilation"/>'s
    ///   <see cref="PortableExecutableReference"/>s for <see cref="Assembly"/> resolution.
    /// </summary>
    internal sealed class CompilationLoadContext : AssemblyLoadContext
    {
        /// <summary>
        ///   Gets the <see cref="CSharpCompilation"/> whose references are used for
        ///   <see cref="Assembly"/> resolution.
        /// </summary>
        public CSharpCompilation Compilation { get; }

        /// <summary>
        ///   Gets the produced assembly.
        /// </summary>
        public Assembly ProducedAssembly { get; set; }

        internal CompilationLoadContext(CSharpCompilation compilation)
        {
            Compilation = compilation;
        }

        /// <inheritdoc />
        protected override Assembly Load(AssemblyName assemblyName)
        {
            try
            {
                // If loading the current assembly, return it
                if (assemblyName.FullName == ProducedAssembly.GetName().FullName)
                    return ProducedAssembly;

                // Attempt to find a matching reference
                PortableExecutableReference reference =
                    Compilation.References.OfType<PortableExecutableReference>()
                               .FirstOrDefault(x => x.Display.Contains(assemblyName.Name));

                return reference != null && !reference.FilePath.Contains("\\ref\\")
                    ? LoadCore(reference.FilePath)
                    : LoadCore(assemblyName);
            }
            catch
            {
                return null;
            }
        }

        // Wrap the 'LoadFromAssemblyPath' logic in another call because of this:
        // https://github.com/dotnet/coreclr/blob/master/Documentation/botr/type-loader.md#2-type-loader-architecture
        [MethodImpl(MethodImplOptions.NoInlining)]
        private Assembly LoadCore(string path) => Default.LoadFromAssemblyPath(path);

        // Same deal
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Assembly LoadCore(AssemblyName assemblyName) => Assembly.Load(assemblyName);
    }
}
