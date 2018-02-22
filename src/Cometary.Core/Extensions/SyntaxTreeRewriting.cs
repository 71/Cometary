using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary
{
    /// <summary>
    ///   Provides methods used to register edits made to a <see cref="CSharpCompilation"/>,
    ///   and more specifically its syntax.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static class SyntaxTreeRewriting
    {
        /// <summary>
        ///   Represents an edit made to a <see cref="SyntaxNode"/>.
        /// </summary>
        public delegate SyntaxNode SyntaxEdit(SyntaxNode node, CSharpCompilation compilation, CancellationToken cancellationToken);

        /// <summary>
        ///   Represents an edit made to a <see cref="SyntaxTree"/>.
        /// </summary>
        public delegate CSharpSyntaxTree SyntaxTreeEdit(CSharpSyntaxTree syntaxTree, CSharpCompilation compilation, CancellationToken cancellationToken);

        /// <summary>
        ///   Key used by the <see cref="EditSyntax"/> method for storage.
        /// </summary>
        private static readonly object SyntaxKey = new object();

        /// <summary>
        ///   Key used by the <see cref="EditSyntaxTree"/> method for storage.
        /// </summary>
        private static readonly object SyntaxTreeKey = new object();

        /// <summary>
        ///   <see cref="CSharpSyntaxRewriter"/> that invokes a callback
        ///   when it visits a <see cref="SyntaxNode"/>.
        /// </summary>
        private sealed class SyntaxTreeRewriter : CSharpSyntaxRewriter
        {
            private readonly CSharpCompilation compilation;
            private readonly IList<SyntaxEdit> edits;
            private readonly CancellationToken cancellationToken;

            internal SyntaxTreeRewriter(CSharpCompilation compilation, IList<SyntaxEdit> edits, CancellationToken cancellationToken)
            {
                this.edits = edits;
                this.compilation = compilation;
                this.cancellationToken = cancellationToken;
            }

            /// <summary>
            ///   Passes the given <see cref="SyntaxNode"/> to all the given edits.
            /// </summary>
            public override SyntaxNode Visit(SyntaxNode node)
            {
                var syntaxEdits = edits;
                var token = cancellationToken;

                foreach (var syntaxEdit in syntaxEdits)
                {
                    node = syntaxEdit(node, compilation, token);

                    if (node == null)
                        return null;
                }

                return base.Visit(node);
            }
        }

        /// <summary>
        ///   Registers a new <paramref name="edit"/> that is to be applied to any <see cref="SyntaxNode"/>
        ///   found on the <see cref="CSharpCompilation"/>.
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="edit"></param>
        public static void EditSyntax(this CompilationEditor editor, SyntaxEdit edit)
        {
            IList<SyntaxEdit> GetDefaultValue()
            {
                editor.CompilationPipeline += EditCompilation;

                return new LightList<SyntaxEdit>();
            }

            CSharpCompilation EditCompilation(CSharpCompilation compilation, CancellationToken cancellationToken)
            {
                IList<SyntaxEdit> edits = editor.Storage.Get<IList<SyntaxEdit>>(SyntaxKey);
                SyntaxTreeRewriter syntaxRewriter = new SyntaxTreeRewriter(compilation, edits, cancellationToken);

                for (int i = 0; i < compilation.SyntaxTrees.Length; i++)
                {
                    CSharpSyntaxTree tree = compilation.SyntaxTrees[i] as CSharpSyntaxTree;

                    if (tree == null)
                        continue;

                    CSharpSyntaxNode root = tree.GetRoot(cancellationToken);
                    SyntaxNode newRoot = syntaxRewriter.Visit(root);

                    if (root != newRoot)
                        compilation = compilation.ReplaceSyntaxTree(tree, tree.WithRootAndOptions(newRoot, tree.Options));
                }

                return compilation;
            }

            editor.Storage.GetOrAdd(SyntaxKey, GetDefaultValue).Add(edit);
        }

        /// <summary>
        ///   Registers a new <paramref name="edit"/> that is to be applied to any <see cref="CSharpSyntaxTree"/>
        ///   found on the <see cref="CSharpCompilation"/>.
        /// </summary>
        public static void EditSyntaxTree(this CompilationEditor editor, SyntaxTreeEdit edit)
        {
            IList<SyntaxTreeEdit> GetDefaultValue()
            {
                editor.CompilationPipeline += EditCompilation;

                return new LightList<SyntaxTreeEdit>();
            }

            CSharpCompilation EditCompilation(CSharpCompilation compilation, CancellationToken cancellationToken)
            {
                var edits = editor.Storage.Get<IList<SyntaxTreeEdit>>(SyntaxTreeKey);

                for (int i = 0; i < compilation.SyntaxTrees.Length; i++)
                {
                    CSharpSyntaxTree tree = compilation.SyntaxTrees[i] as CSharpSyntaxTree;
                    CSharpSyntaxTree newTree = tree;

                    if (tree == null)
                        continue;

                    foreach (var treeEdit in edits)
                    {
                        newTree = treeEdit(newTree, compilation, cancellationToken);

                        if (newTree != null)
                            continue;

                        i--;
                        compilation = compilation.RemoveSyntaxTrees(tree);

                        goto NextTree;
                    }

                    compilation = compilation.ReplaceSyntaxTree(tree, newTree);

                    NextTree:;
                }

                return compilation;
            }

            editor.Storage.GetOrAdd(SyntaxTreeKey, GetDefaultValue).Add(edit);
        }
    }
}
