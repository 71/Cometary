using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cometary
{
    /// <summary>
    ///   Represents a component that generates its members dynamically.
    /// </summary>
    public abstract class Component
    {
        /// <summary>
        ///   Applies the given component to the given class.
        /// </summary>
        public abstract ClassDeclarationSyntax Apply(ClassDeclarationSyntax node, INamedTypeSymbol symbol, CancellationToken cancellationToken);
    }
}
