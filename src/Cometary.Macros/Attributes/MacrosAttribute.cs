using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary
{
    using Macros;

    /// <summary>
    ///   Indicates that the marked assembly will have its macro methods lowered.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class MacrosAttribute : CometaryAttribute
    {
        /// <inheritdoc />
        public override IEnumerable<CompilationEditor> Initialize()
        {
            return new CompilationEditor[] { new Editor() };
        }

        private sealed class Editor : CompilationEditor
        {
            /// <inheritdoc />
            protected override void Initialize(CSharpCompilation compilation, CancellationToken cancellationToken)
            {
                CompilationPipeline += EditCompilation;

                this.DefineFeatureDependency("IOperation");
            }

            private static CSharpCompilation EditCompilation(CSharpCompilation compilation, CancellationToken cancellationToken)
            {
                int syntaxTreesLength = compilation.SyntaxTrees.Length;

                for (int i = 0; i < syntaxTreesLength; i++)
                {
                    CSharpSyntaxTree tree = compilation.SyntaxTrees[i] as CSharpSyntaxTree;

                    if (tree == null)
                        continue;

                    MacroExpander expander = new MacroExpander(tree, compilation, cancellationToken);

                    SyntaxNode root = tree.GetCompilationUnitRoot(cancellationToken);
                    SyntaxNode newRoot = expander.Visit(root);

                    if (root != newRoot)
                        compilation = compilation.ReplaceSyntaxTree(tree, tree.WithRootAndOptions(newRoot, tree.Options));
                }

                return compilation;
            }
        }
    }
}
