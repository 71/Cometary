using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary.Visiting
{
    /// <summary>
    /// <see cref="AssemblyVisitor"/> that cleans up references to
    /// assemblies related to Cometary.
    /// </summary>
    internal sealed class CleanUpVisitor : AssemblyVisitor
    {
        /// <inheritdoc />
        public override float Priority => float.MinValue;

        /// <inheritdoc />
        public override CSharpCompilation Visit(Assembly assembly, CSharpCompilation compilation)
        {
            return compilation;
        }
    }
}
