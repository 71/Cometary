using System;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary.Common
{
    /// <summary>
    /// Lightweight assembly visitor that gets called on certain events.
    /// </summary>
    public abstract class LightAssemblyVisitor : CSharpSyntaxRewriter, IComparable<LightAssemblyVisitor>
    {
        /// <summary>
        /// Defines the current state of the compilation.
        /// </summary>
        public enum CompilationState
        {
            /// <summary>
            /// The assembly has been compiled, but not yet fully visited.
            /// </summary>
            Loaded,

            /// <summary>
            /// The assembly has been compiled and emitted.
            /// </summary>
            Visited
        }

        /// <summary>
        /// Compares one visitor to another. The result of this method
        /// will help decide what visitor should run first.
        /// <para>
        /// The lower this value, the faster to run.
        /// </para>
        /// </summary>
        public virtual int CompareTo(LightAssemblyVisitor other) => 0;

        /// <summary>
        /// Gets whether or not this visitor also rewrites the syntax tree.
        /// <para>
        /// Setting this to <see langword="false"/> will improve performances.
        /// </para>
        /// </summary>
        public virtual bool RewritesTree => false;

        /// <summary>
        /// Gets whether or not this visitor can update syntax trees incrementally,
        /// in which case it cannot have side effects on other syntax trees.
        /// <para>
        /// If set to <see langword="true"/>, only the modified syntax trees will be modified by
        /// this visitor when updating.
        /// </para>
        /// </summary>
        public virtual bool SupportsIncrementalUpdate => true;

        /// <summary>
        /// Transforms the specified compilation.
        /// <para>
        /// This method is invoked after the original <see cref="CSharpSyntaxTree"/> transformation.
        /// </para>
        /// </summary>
        public virtual CSharpCompilation Visit(Assembly assembly, CSharpCompilation compilation) => compilation;

        /// <summary>
        /// Transforms the given syntax tree.
        /// <para>
        /// **Warning:** during this process, symbols and operations cannot be
        /// resolved, rendering many utilities useless.
        /// </para>
        /// <para>
        /// As such, this method should only be overriden for pure
        /// syntax tree manipulation.
        /// </para>
        /// </summary>
        public virtual CSharpSyntaxTree VisitSyntaxTree(CSharpSyntaxTree syntaxTree) => syntaxTree;

        /// <summary>
        /// Transforms the given streams.
        /// </summary>
        /// <param name="assemblyStream">The <see cref="Stream"/> containing the emitted assembly.</param>
        /// <param name="symbolsStream">The <see cref="Stream"/> containing the symbols of the emitted assembly.</param>
        /// <param name="state">The state of the compilation.</param>
        public virtual void Visit(MemoryStream assemblyStream, MemoryStream symbolsStream, CompilationState state) { }
    }
}
