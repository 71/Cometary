using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Cometary
{
    /// <summary>
    ///   <see cref="DiagnosticAnalyzer"/> that can edit a <see cref="CSharpCompilation"/>.
    /// </summary>
    public abstract class EditingAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        ///   Gets the <see cref="CompilationEditor"/> associated with this <see cref="EditingAnalyzer"/>.
        /// </summary>
        public abstract CompilationEditor Editor { get; }

        /// <summary>
        ///   Initializes this <see cref="EditingAnalyzer"/> by registering
        ///   the provided <see cref="Editor"/> on the corresponding <see cref="CometaryManager"/>.
        /// </summary>
        public override void Initialize(AnalysisContext context)
        {
            // TODO: Make this work or remove this class
            context.RegisterCompilationAction(ctx =>
            {
                CSharpCompilation compilation = ctx.Compilation as CSharpCompilation;
                CometaryManager manager = CometaryManager.Create();

                manager.Register(Editor);
            });
        }
    }
}
