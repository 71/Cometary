using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary.SelfAnalysis.Tests.Analyzers
{
    public sealed class TestEditor : CompilationEditor
    {
        public override Task<CSharpCompilation> EditAsync(CSharpCompilation compilation, CancellationToken cancellationToken)
        {
            var tree = SyntaxFactory.ParseSyntaxTree(@"
            namespace Cometary.Tests {
                public class TestClass {
                    public static void Do() { }
                }
            }
            ");

            ReportWarning($"Nb of trees before: {compilation.SyntaxTrees.Length}");

            compilation = compilation.AddSyntaxTrees(tree);

            ReportWarning($"Nb of trees after: {compilation.SyntaxTrees.Length}");

            return Task.FromResult(compilation);
        }
    }
}
