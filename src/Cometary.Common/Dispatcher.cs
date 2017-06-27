using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary.Core
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

    /// <summary>
    /// Dispatcher that visits all nodes in a <see cref="SyntaxTree"/>
    /// by running every given <see cref="LightAssemblyVisitor"/>
    /// on every node, one by one.
    /// </summary>
    internal sealed class CoreDispatcher : IDispatcher
    {
        /// <summary>
        /// <see cref="CSharpSyntaxRewriter"/> that dispatches all
        /// given <see cref="Visitors"/> to all nodes.
        /// </summary>
        private sealed class Rewriter : CSharpSyntaxRewriter
        {
            /// <summary>
            /// Gets the collection containing all the visitors to run on the syntax.
            /// </summary>
            private readonly LightAssemblyVisitor[] Visitors;

            /// <summary>
            /// Initializes the <see cref="Rewriter"/>, given the visitors it'll use.
            /// </summary>
            public Rewriter(IEnumerable<LightAssemblyVisitor> visitors)
            {
                Visitors = visitors.Where(x => x.RewritesTree).ToArray();
            }

            /// <summary>
            /// Visits the given syntax tree.
            /// </summary>
            public CSharpSyntaxTree Visit(CSharpSyntaxTree syntaxTree)
            {
                return syntaxTree.WithRootAndOptions(Visit(syntaxTree.GetRoot()), syntaxTree.Options) as CSharpSyntaxTree;
            }

            /// <summary>
            /// Runs all visitors on the given <paramref name="node"/>.
            /// </summary>
            public override SyntaxNode Visit(SyntaxNode node)
            {
                SyntaxNode rewrittenNode = node;
                LightAssemblyVisitor[] visitors = Visitors;

                for (int i = 0; i < visitors.Length; i++)
                {
                    rewrittenNode = visitors[i].Visit(rewrittenNode);

                    if (rewrittenNode == null)
                        return null;
                }

                return base.Visit(rewrittenNode);
            }
        }

        /// <inheritdoc />
        public CSharpSyntaxTree Dispatch(CSharpSyntaxTree syntaxTree, ReadOnlyCollection<LightAssemblyVisitor> visitors) => new Rewriter(visitors).Visit(syntaxTree);

        /// <inheritdoc />
        public bool ShouldOverride(IDispatcher dispatcher) => false;
    }
}
