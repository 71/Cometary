using System.Threading;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary.Composition
{
    /// <summary>
    ///   <see cref="CompilationEditor"/> that enables composition.
    /// </summary>
    internal sealed partial class CompositionEditor : CompilationEditor
    {
        // The composition editor does two things:
        //  - Save the content of every component in the "component" attribute.
        //  - Load the content of every component in elements that implement them.

        protected override void Initialize(CSharpCompilation compilation, CancellationToken cancellationToken)
        {
            this.EditSyntaxTree(EditSyntaxTree);
        }

        private static CSharpSyntaxTree EditSyntaxTree(CSharpSyntaxTree syntaxTree, CSharpCompilation compilation, CancellationToken cancellationToken)
        {
            Rewriter rewriter = new Rewriter(compilation, syntaxTree, cancellationToken);
            CSharpSyntaxNode root = syntaxTree.GetRoot(cancellationToken);

            return syntaxTree.WithRoot(rewriter.Visit(root)) as CSharpSyntaxTree;
        }
    }
}
