using System;
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
            private readonly Func<SyntaxNode, SyntaxNode> callback;

            internal SyntaxTreeRewriter(Func<SyntaxNode, SyntaxNode> callback)
            {
                this.callback = callback;
            }

            /// <summary>
            ///   Invokes the previously given <see cref="callback"/>, and returns its result.
            /// </summary>
            public override SyntaxNode Visit(SyntaxNode node) => base.Visit(callback(node));
        }

        /// <summary>
        ///   Registers a new <paramref name="edit"/> that is to be applied to any <see cref="SyntaxNode"/>
        ///   found on the <see cref="CSharpCompilation"/>.
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="edit"></param>
        public static void EditSyntax(this CompilationEditor editor, Edit<SyntaxNode> edit)
        {
            IList<Edit<SyntaxNode>> GetDefaultValue()
            {
                editor.CompilationPipeline += EditCompilation;

                return new LightList<Edit<SyntaxNode>>();
            }

            CSharpCompilation EditCompilation(CSharpCompilation compilation, CancellationToken cancellationToken)
            {
                SyntaxTreeRewriter syntaxRewriter = new SyntaxTreeRewriter(node =>
                {
                    foreach (var syntaxEdit in editor.Storage.Get<IList<Edit<SyntaxNode>>>(SyntaxKey))
                    {
                        node = syntaxEdit(node, cancellationToken);

                        if (node == null)
                            return null;
                    }

                    return node;
                });

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
        /// <param name="editor"></param>
        /// <param name="edit"></param>
        public static void EditSyntaxTree(this CompilationEditor editor, Edit<CSharpSyntaxTree> edit)
        {
            IList<Edit<CSharpSyntaxTree>> GetDefaultValue()
            {
                editor.CompilationPipeline += EditCompilation;

                return new LightList<Edit<CSharpSyntaxTree>>();
            }

            CSharpCompilation EditCompilation(CSharpCompilation compilation, CancellationToken cancellationToken)
            {
                var edits = editor.Storage.Get<IList<Edit<CSharpSyntaxTree>>>(SyntaxTreeKey);

                for (int i = 0; i < compilation.SyntaxTrees.Length; i++)
                {
                    CSharpSyntaxTree tree = compilation.SyntaxTrees[i] as CSharpSyntaxTree;
                    CSharpSyntaxTree newTree = tree;

                    if (tree == null)
                        continue;

                    foreach (var treeEdit in edits)
                    {
                        newTree = treeEdit(newTree, cancellationToken);

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
