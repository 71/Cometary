using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace Cometary
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AssemblyResolver : IDisposable
    {
        private readonly Dictionary<string, Assembly> _loaded = new Dictionary<string, Assembly>();
        private readonly List<string> _references = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyCollection<string> References { get; }

        /// <summary>
        /// 
        /// </summary>
        public Assembly EmittedAssembly { get; }

        /// <summary>
        /// 
        /// </summary>
        public AssemblyResolver(Assembly emitted)
        {
            References = _references.AsReadOnly();
            EmittedAssembly = emitted;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Register(string @ref)
        {
            _references.Add(@ref);
        }

        /// <summary>
        /// 
        /// </summary>
        public Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name == EmittedAssembly.FullName)
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
                _loaded[name] = assembly = Assembly.LoadFrom(_references.First(x => x.IndexOf(name, StringComparison.OrdinalIgnoreCase) != -1));

            return assembly;
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}
