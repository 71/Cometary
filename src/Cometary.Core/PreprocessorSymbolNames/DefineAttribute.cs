using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary
{
    /// <summary>
    /// <para>
    ///   Indicates that the specified constants should be used as preprocessor symbol names
    ///   during compilation.
    /// </para>
    /// <para>
    ///   Setting this attribute on an assembly also ensures that all constants defined by
    ///   <see cref="CompilationEditor"/>s are used during compilation.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class DefineAttribute : CometaryAttribute
    {
        /// <summary>
        ///   Gets a list of all defined constants.
        /// </summary>
        public string[] DefinedConstants { get; }

        /// <inheritdoc cref="DefineAttribute" />
        public DefineAttribute(params string[] constants)
        {
            Requires.NoneNull(constants, nameof(constants));

            DefinedConstants = constants;
        }

        /// <inheritdoc />
        public override IEnumerable<CompilationEditor> Initialize()
        {
            return new CompilationEditor[] { new Editor(DefinedConstants) };
        }

        private sealed class Editor : CompilationEditor
        {
            private readonly string[] preprocessorSymbolNames;

            internal Editor(string[] preprocessorSymbolNames)
            {
                this.preprocessorSymbolNames = preprocessorSymbolNames;
            }

            /// <inheritdoc />
            protected override void Initialize(CSharpCompilation compilation, CancellationToken cancellationToken)
            {
                CompilationPipeline += EditCompilation;

                string[] symbolNames = preprocessorSymbolNames;

                for (int i = 0; i < symbolNames.Length; i++)
                {
                    string symbolName = symbolNames[i];

                    if (symbolName == null)
                        continue;

                    this.DefineConstant(symbolName);
                }
            }

            private CSharpCompilation EditCompilation(CSharpCompilation compilation, CancellationToken cancellationToken)
            {
                return PreprocessorSymbolNamesDefining.RecomputeCompilation(compilation, this, cancellationToken);
            }
        }
    }
}
