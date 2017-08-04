using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary
{
    /// <summary>
    ///   Provides static helpers and fields for faster Reflection.
    /// </summary>
    internal static class ReflectionHelpers
    {
        public static readonly Assembly CodeAnalysisCSharpAssembly
            = typeof(CSharpCompilation).GetTypeInfo().Assembly;

        public static readonly Assembly CodeAnalysisAssembly
            = typeof(Compilation).GetTypeInfo().Assembly;
    }
}
