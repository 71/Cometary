using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Cometary
{
    /// <summary>
    ///   Provides methods used to define features that are needed for execution.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static class FeaturesDependencyDefining
    {
        private static readonly object Key = new object();

        /// <summary>
        ///   Defines a feature on which the calling <see cref="CompilationEditor"/> is dependant.
        /// </summary>
        public static void DefineFeatureDependency(this CompilationEditor editor, string feature, string value = "True")
        {
            IDictionary<string, string> GetDefaultValue()
            {
                editor.GetRecomputationPipeline().Add(next =>
                {
                    return opts =>
                    {
                        if (editor.SharedStorage.TryGet(Key, out IDictionary<string, string> features))
                            opts = opts.WithFeatures(opts.Features.Concat(features));

                        return next(opts);
                    };
                });

                return new Dictionary<string, string>();
            }

            editor.SharedStorage.GetOrAdd(Key, GetDefaultValue)[feature] = value;
        }
    }
}
