using System.Collections.ObjectModel;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary.Common
{
    /// <summary>
    /// Defines a class used to dispatch a syntax tree
    /// to its visitors.
    /// </summary>
    public interface IDispatcher
    {
        /// <summary>
        /// Dispatches the given visitors on the given syntax tree.
        /// </summary>
        CSharpSyntaxTree Dispatch(CSharpSyntaxTree syntaxTree, ReadOnlyCollection<LightAssemblyVisitor> visitors);

        /// <summary>
        /// Returns whether or not this dispatcher should override
        /// the given <paramref name="dispatcher"/> as the default one.
        /// </summary>
        bool ShouldOverride(IDispatcher dispatcher);
    }
}
