using System.ComponentModel;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Cometary
{
    /// <summary>
    ///   Provides methods used to register edits made to the IL written according to an <see cref="IOperation"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static class MethodBodyRewriting
    {
        /// <summary>
        ///   Key used by the various methods declared in this class for storage.
        /// </summary>
        private static readonly object Key = new object();

        /// <summary>
        ///   Registers a new <paramref name="edit"/> that is to be applied to all operations
        ///   that are about to be emitted.
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="edit"></param>
        public static void EditOperation(this CompilationEditor editor, Edit<IOperation> edit)
        {
            editor.EditIL(next => (context, operation, used) => next(context, edit(operation, default(CancellationToken)), used));
        }

        /// <summary>
        ///   Adds the given <paramref name="component"/> to the IL emission pipeline.
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="component"></param>
        public static void EditIL(this CompilationEditor editor, AlternateEmitDelegate component)
        {
            editor.EditIL(CodeGeneratorContext.ToComponent(component));
        }

        /// <summary>
        ///   Adds the given <paramref name="component"/> to the IL emission pipeline.
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="component"></param>
        public static void EditIL(this CompilationEditor editor, PipelineComponent<EmitDelegate> component)
        {
            Pipeline<EmitDelegate> GetDefaultValue()
            {
                EmitDelegate Emit(EmitDelegate next)
                {
                    var pipeline = editor.Storage.Get<Pipeline<EmitDelegate>>(Key);
                    var del = pipeline.MakeDelegate(next);

                    return del;
                }

                CodeGeneratorContext.EmitPipeline += Emit;
                //editor.EmissionStart += ce => CodeGeneratorContext.EmitPipeline += Emit;
                //editor.EmissionEnd += ce => CodeGeneratorContext.EmitPipeline -= Emit;

                return new Pipeline<EmitDelegate>();
            }

            editor.Storage.GetOrAdd(Key, GetDefaultValue).Add(component);
        }
    }
}
