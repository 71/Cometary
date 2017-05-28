using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Cometary
{
    /// <summary>
    /// Internal assembly resolver that uses Project references for
    /// assembly resolution.
    /// </summary>
    internal sealed class AssemblyResolver : IDisposable
    {
        private readonly Dictionary<string, Assembly> _loaded   = new Dictionary<string, Assembly>();
        private readonly Dictionary<string, string> _references = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets or sets the <see cref="Assembly"/> emitted by the compilation.
        /// </summary>
        public Assembly EmittedAssembly { get; internal set; }

        /// <summary>
        /// Registers the given reference (a full path to a .dll).
        /// </summary>
        public void Register(string @ref)
        {
            _references.Add(Path.GetFileNameWithoutExtension(@ref), @ref);
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

        private Assembly LoadAssembly(string name)
        {
            int indexOfComma = name.IndexOf(',');
            if (indexOfComma != -1)
                name = name.Substring(0, indexOfComma);

            if (!_loaded.TryGetValue(name, out Assembly assembly))
                _loaded[name] = assembly = Assembly.LoadFrom(_references[name]);

            return assembly;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _loaded.Clear();
            _references.Clear();
        }
    }
}
