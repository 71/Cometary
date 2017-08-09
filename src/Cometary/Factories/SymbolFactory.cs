using System;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class SymbolFactory
    {
        private static readonly Type underlyingType = ReflectionHelpers.CodeAnalysisCSharpAssembly.GetType("Microsoft.CodeAnalysis.CSharp.Binder");
        private static readonly PersistentProxyData data = new PersistentProxyData();

        private readonly object underlyingObject;
        private readonly Proxy proxy;

        internal SymbolFactory(CSharpCompilation compilation)
        {
            underlyingObject = Activator.CreateInstance(underlyingType, compilation);
            proxy = new Proxy(underlyingObject, underlyingType, data);
        }

        internal SymbolFactory(SymbolFactory next)
        {
            underlyingObject = Activator.CreateInstance(underlyingType, next.underlyingObject);
            proxy = new Proxy(underlyingObject, underlyingType, data);
        }
    }
}
