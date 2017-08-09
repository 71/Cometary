using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Cometary
{
    public sealed partial class SyntheticSymbolFactory
    {
        private static readonly Type underlyingType = ReflectionHelpers.CodeAnalysisCSharpAssembly.GetType("Microsoft.CodeAnalysis.CSharp.SyntheticBoundNodeFactory");
        private static readonly PersistentProxyData data = new PersistentProxyData();

        private readonly object underlyingObject;
        private readonly Proxy proxy;

        internal SyntheticSymbolFactory(IMethodSymbol topLevelMethod, SyntaxNode node, object compilationState, object diagnosticBag)
        {
            Debug.Assert(compilationState.GetType().Name == "TypeCompilationState");
            Debug.Assert(diagnosticBag.GetType().Name == "DiagnosticBag");

            underlyingObject = Activator.CreateInstance(underlyingType, topLevelMethod, node, compilationState, diagnosticBag);
            proxy = new Proxy(underlyingObject, underlyingType, data);
        }
    }
}
