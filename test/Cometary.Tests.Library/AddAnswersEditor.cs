using System;
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
        public override void Initialize(CSharpCompilation compilation, CancellationToken cancellationToken)
        {
            SuppressDiagnostic(diagnostic => diagnostic.Id == "CS0103" && diagnostic.GetMessage().Contains("Answers"));

            RegisterEdit(EditCompilation);
        }

        /// <summary>
        ///   Edits the given <paramref name="compilation"/>, adding a <see cref="CSharpSyntaxTree"/>
        ///   defining the 'Answers' class.
        /// </summary>
        private CSharpCompilation EditCompilation(CSharpCompilation compilation, CancellationToken cancellationToken)
        {
            if (State == CompilationState.End)
                return compilation;

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
