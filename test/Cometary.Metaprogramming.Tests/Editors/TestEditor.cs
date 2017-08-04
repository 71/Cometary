using System.IO;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary.Tests
{
    /// <summary>
    ///   <see cref="CompilationEditor"/> that adds a simple syntax tree to the output compilation.
    /// </summary>
    public sealed class TestEditor : CompilationEditor
    {
        /// <inheritdoc />
        public override void Initialize(CSharpCompilation compilation, CancellationToken cancellationToken)
        {
            RegisterEdit(EditCompilation, CompilationState.Start);
        }

        /// <summary>
        ///   Edits the given <paramref name="compilation"/>, adding a <see cref="CSharpSyntaxTree"/>
        ///   defining the 'Answers' class.
        /// </summary>
        private CSharpCompilation EditCompilation(CSharpCompilation compilation, CancellationToken cancellationToken)
        {
            ReportWarning("The test editor successfully ran.");

            var tree = SyntaxFactory.ParseSyntaxTree(@"
                namespace Cometary.Tests {
                    partial class Tests {
                        public static int Answer => 42;
                    }
                }
            ");

            return compilation.AddSyntaxTrees(tree);
        }
    }
}
