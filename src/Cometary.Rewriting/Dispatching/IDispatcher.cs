using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary
{
    /// <summary>
    ///   Defines a class used to dispatch a list of <see cref="AssemblyRewriter"/>s
    ///   on a <see cref="CSharpSyntaxTree"/>.
    /// </summary>
    public interface IDispatcher
    {
        /// <summary>
        ///   Dispatches the given rewriters on the given syntax tree.
        /// </summary>
        CSharpSyntaxTree Dispatch(CSharpSyntaxTree syntaxTree, IReadOnlyList<AssemblyRewriter> rewriters);
    }
}
