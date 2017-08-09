using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Cometary
{
    /// <summary>
    ///   Provides methods used to define new preprocessor symbol names.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static class PreprocessorSymbolNamesDefining
    {
        private static readonly object Key = new object();

        /// <summary>
        ///   Defines a constant to be used as a preprocessor symbol name when parsing syntax trees.
        /// </summary>
        public static void DefineConstant(this CompilationEditor editor, string constant)
        {
            IList<string> GetDefaultValue()
            {
                editor.GetRecomputationPipeline().Add(next =>
                {
                    return opts =>
                    {
                        if (editor.SharedStorage.TryGet(Key, out IList<string> preprocessorSymbolNames))
                            opts = opts.WithPreprocessorSymbols(opts.PreprocessorSymbolNames.Concat(preprocessorSymbolNames));

                        return next(opts);
                    };
                });

                return new LightList<string>();
            }

            editor.SharedStorage.GetOrAdd(Key, GetDefaultValue).Add(constant);
        }
    }
}
