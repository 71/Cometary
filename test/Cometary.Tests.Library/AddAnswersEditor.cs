using System.Threading;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary.Tests
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class AddAnswersEditor : CompilationEditor
    {
        /// <inheritdoc />
        protected override void Initialize(CSharpCompilation compilation, CancellationToken cancellationToken)
        {
            CompilationPipeline += EditCompilation;
        }

        /// <summary>
        ///   Edits the given <paramref name="compilation"/>, adding a <see cref="CSharpSyntaxTree"/>
        ///   defining the 'Answers' class.
        /// </summary>
        private static CSharpCompilation EditCompilation(CSharpCompilation compilation, CancellationToken cancellationToken)
        {
            var tree = SyntaxFactory.ParseSyntaxTree(@"
                namespace Cometary.Tests {
                    public static class Answers {
                        public static int LifeTheUniverseAndEverything => 42;
                    }
                }
            ");

            return compilation.AddSyntaxTrees(tree);
        }
    }
}
