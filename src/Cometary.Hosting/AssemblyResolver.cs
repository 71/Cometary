using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

#if NET_CORE
using System.Runtime.Loader;
#endif

namespace Cometary
{
#if NET_CORE
    internal sealed class CometaryAssemblyLoadContext : AssemblyLoadContext, IDisposable
#else
    internal sealed class AssemblyResolver : IDisposable
#endif
    {
        private readonly Dictionary<string, Assembly> _loaded   = new Dictionary<string, Assembly>();
        private readonly Dictionary<string, string> _references = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets or sets the <see cref="Assembly"/> emitted by the compilation.
        /// </summary>
        public Assembly EmittedAssembly { get; internal set; }

#if NET_CORE
        /// <summary>
        /// 
        /// </summary>
        public CometaryAssemblyLoadContext()
        {
            KnownReferences = new List<string>();

            Resolving += ResolvingAssembly;
        }

        /// <summary>
        /// 
        /// </summary>
        private Assembly ResolvingAssembly(AssemblyLoadContext ctx, AssemblyName assemblyName)
        {
            string name = assemblyName.Name;

            // Maybe it's the emitted assembly?
            if (name == EmittedAssembly?.FullName)
                return EmittedAssembly;

            return LoadAssembly(name);
        }
#else
        public AssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
        }

        /// <summary>
        /// Resolves the assembly with the given name by looking up
        /// all registered references.
        /// </summary>
        public Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name == EmittedAssembly?.FullName)
                return EmittedAssembly;

            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName == args.Name)
                ?? LoadAssembly(args.Name);
        }
#endif
        /// <summary>
        /// 
        /// </summary>
        public void Register(string assemblyPath)
        {
            if (File.Exists(assemblyPath))
                _references[Path.GetFileNameWithoutExtension(assemblyPath)] = assemblyPath;
        }

        private Assembly LoadAssembly(string name)
        {
            int indexOfComma = name.IndexOf(',');
            if (indexOfComma != -1)
                name = name.Substring(0, indexOfComma);

            if (!_loaded.TryGetValue(name, out Assembly assembly))
#if NET_CORE
                _loaded[name] = assembly = LoadFromAssemblyPath(_references[name]);
#else
                _loaded[name] = assembly = Assembly.LoadFrom(_references[name]);
#endif


            return assembly;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _loaded.Clear();
            _references.Clear();

#if NET_CORE
            Resolving -= ResolvingAssembly;
#else
            AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolve;
#endif
        }
    }
}