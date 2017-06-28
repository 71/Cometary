using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary
{
    /// <summary>
    ///   Dispatcher that visits all nodes in a <see cref="SyntaxTree"/>
    ///   by running every given <see cref="AssemblyRewriter"/>
    ///   on every node, one by one.
    /// </summary>
    internal sealed class DefaultDispatcher : IDispatcher
    {
        /// <summary>
        ///   <see cref="CSharpSyntaxRewriter"/> that dispatches all
        ///   given <see cref="Rewriters"/> to all nodes.
        /// </summary>
        private sealed class Rewriter : CSharpSyntaxRewriter
        {
            /// <summary>
            ///   Collection containing all the visitors to run on the syntax.
            /// </summary>
            private readonly ImmutableArray<AssemblyRewriter> Rewriters;

            /// <summary>
            ///   Initializes the <see cref="Rewriter"/>, given the visitors it'll use.
            /// </summary>
            public Rewriter(IEnumerable<AssemblyRewriter> rewriters)
            {
                Rewriters = rewriters.Where(x => x.RewritesTree).ToImmutableArray();
            }

            /// <summary>
            ///   Visits the given syntax tree.
            /// </summary>
            public CSharpSyntaxTree Visit(CSharpSyntaxTree syntaxTree)
            {
                return syntaxTree.WithRootAndOptions(Visit(syntaxTree.GetRoot()), syntaxTree.Options) as CSharpSyntaxTree;
            }

            /// <summary>
            ///   Runs all visitors on the given <paramref name="node"/>.
            /// </summary>
            public override SyntaxNode Visit(SyntaxNode node)
            {
                SyntaxNode rewrittenNode = node;
                ImmutableArray<AssemblyRewriter> rewriters = Rewriters;

                for (int i = 0; i < rewriters.Length; i++)
                {
                    rewrittenNode = rewriters[i].Visit(rewrittenNode);

                    if (rewrittenNode == null)
                        return null;
                }

                return base.Visit(rewrittenNode);
            }
        }

        /// <inheritdoc />
        public CSharpSyntaxTree Dispatch(CSharpSyntaxTree syntaxTree, IReadOnlyList<AssemblyRewriter> rewriters)
        {
            if (syntaxTree == null)
                throw new ArgumentNullException(nameof(syntaxTree));
            if (rewriters == null)
                throw new ArgumentNullException(nameof(rewriters));

            return new Rewriter(rewriters).Visit(syntaxTree);
        }
    }
}
