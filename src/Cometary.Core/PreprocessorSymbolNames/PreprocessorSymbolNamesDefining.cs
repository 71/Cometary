using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Cometary
{
    /// <summary>
    ///   Provides methods used to define new preprocessor symbol names.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static class PreprocessorSymbolNamesDefining
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly object Key = new object();

        /// <summary>
        /// <para>
        ///   Defines a constant to be used as a preprocessor symbol name when parsing syntax trees.
        /// </para>
        /// <para>
        ///   Since this operation can be quite expensive, the user must opt-in with the
        ///   <see cref="DefineAttribute"/>.
        /// </para>
        /// </summary>
        /// <remarks>
        ///   All already parsed syntax trees will be parsed again.
        /// </remarks>
        public static void DefineConstant(this CompilationEditor editor, string constant)
        {
            IList<string> GetDefaultValue() => new LightList<string>();

            editor.SharedStorage.GetOrAdd(Key, GetDefaultValue).Add(constant);
        }

        internal static CSharpCompilation RecomputeCompilation(
            CSharpCompilation compilation, CompilationEditor editor, CancellationToken cancellationToken)
        {
            return editor.SharedStorage.TryGet(Key, out IList<string> preprocessorSymbolNames)
                ? RecomputeCompilation(compilation, preprocessorSymbolNames, cancellationToken)
                : compilation;
        }

        internal static CSharpCompilation RecomputeCompilation(
            CSharpCompilation compilation, IList<string> preprocessorSymbols, CancellationToken cancellationToken)
        {
            for (int i = 0; i < compilation.SyntaxTrees.Length; i++)
            {
                CSharpSyntaxTree tree = compilation.SyntaxTrees[i] as CSharpSyntaxTree;

                if (tree == null)
                    continue;

                SourceText source = tree.GetText(cancellationToken);
                CSharpParseOptions options = tree.Options.WithPreprocessorSymbols(
                    tree.Options.PreprocessorSymbolNames.Concat(preprocessorSymbols)
                );

                SyntaxTree newTree = CSharpSyntaxTree.ParseText(source, options, tree.FilePath, cancellationToken);

                compilation = compilation.ReplaceSyntaxTree(tree, newTree);
            }

            return compilation;
        }
    }
}
