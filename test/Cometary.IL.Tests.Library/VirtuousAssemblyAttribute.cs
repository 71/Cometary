using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp;
using Cometary;

/// <summary>
///   Indicates that this assembly is virtuous, and shall never knowingly return an evil number.
/// </summary>
public sealed class VirtuousAssemblyAttribute : CometaryAttribute
{
    public override IEnumerable<CompilationEditor> Initialize()
    {
        return new CompilationEditor[] { new Editor() };
    }

    private sealed class Editor : CompilationEditor
    {
        protected override void Initialize(CSharpCompilation compilation, CancellationToken cancellationToken)
        {
            CodeGeneratorContext.EmitPipeline += next =>
            {
                return (context, expression, used) =>
                {
                    var constant = expression.ConstantValue;

                    if (constant.HasValue && constant.Value is int nbr && nbr == 666)
                    {
                        context.EmitConstant(0);
                    }
                    else
                    {
                        next(context, expression, used);
                    }
                };
            };
        }
    }
}
