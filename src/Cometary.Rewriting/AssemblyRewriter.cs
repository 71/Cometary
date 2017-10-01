using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary
{
    /// <summary>
    ///   Defines a class that rewrites an assembly, and its members.
    /// </summary>
    public abstract class AssemblyRewriter : CSharpSyntaxRewriter
    {
        /// <summary>
        /// <para>
        ///   Gets whether or not this visitor also rewrites the syntax tree.
        /// </para>
        /// <para>
        ///   Setting this to <see langword="false"/> will improve performances.
        /// </para>
        /// </summary>
        public virtual bool RewritesTree => false;

        /// <summary>
        /// <para>
        ///   Gets whether or not this visitor can update syntax trees incrementally,
        ///   in which case it cannot have side effects on other syntax trees.
        /// </para>
        /// <para>
        ///   If set to <see langword="true"/>, only the modified syntax trees will be modified by
        ///   this visitor when updating.
        /// </para>
        /// </summary>
        public virtual bool SupportsIncrementalUpdate => true;


        /// <summary>
        /// <para>
        ///   Transforms the specified <paramref name="compilation"/>.
        /// </para>
        /// <para>
        ///   This method is invoked after the original <see cref="CSharpSyntaxTree"/> transformation.
        /// </para>
        /// </summary>
        public virtual CSharpCompilation VisitCompilation(Assembly assembly, CSharpCompilation compilation) => compilation;

        /// <summary>
        /// <para>
        ///   Transforms the specified syntax tree.
        /// </para>
        /// <para>
        ///   Warning: during this process, symbols and operations cannot be
        ///   resolved, rendering many utilities useless.
        /// </para>
        /// <para>
        ///   As such, this method should only be overriden for pure
        ///   syntax tree manipulation.
        /// </para>
        /// </summary>
        public virtual CSharpSyntaxTree VisitSyntaxTree(CSharpSyntaxTree syntaxTree) => syntaxTree;
    }
}
