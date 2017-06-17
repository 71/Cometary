using System;
using System.IO;
using Cometary.Internal;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Cometary
{
    using Visiting;

    /// <summary>
    /// <see cref="AssemblyVisitor"/> that allows the use of
    /// Jean-Baptiste Evain's Mono.Cecil library.
    /// </summary>
    /// <seealso href="https://github.com/jbevain/cecil"/>
    internal sealed class ILVisitor : AssemblyVisitor
    {
        internal static readonly ISymbolWriterProvider WriterProvider = new PortablePdbWriterProvider();
        internal static readonly ISymbolReaderProvider ReaderProvider = new PortablePdbReaderProvider();

        internal static readonly AssemblyResolver AssemblyResolver = new AssemblyResolver();
        internal static readonly MetadataResolver MetadataResolver = new MetadataResolver(AssemblyResolver);

        /// <summary>
        /// Loads or saves the assembly being processed.
        /// </summary>
        public override void Visit(MemoryStream assemblyStream, MemoryStream symbolsStream, CompilationState state)
        {
            AssemblyDefinition def = IL.Assembly = LoadAssembly(assemblyStream, symbolsStream);

            switch (state)
            {
                case CompilationState.Loaded:
                    OnAssemblyLoaded(def);
                    break;
                case CompilationState.Visited:
                    OnAssemblyVisited(def);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state));
            }

            SaveAssembly(assemblyStream, symbolsStream, def);
        }

        private void OnAssemblyLoaded(AssemblyDefinition assembly)
        {
            
        }

        private void OnAssemblyVisited(AssemblyDefinition assembly)
        {
            foreach (var action in IL.Actions)
            {
                try
                {
                    action(assembly);
                }
                catch (Exception e)
                {
                    Meta.LogWarning(this, e.Message);
                }
            }
        }

        #region Static IO
        /// <summary>
        /// Loads the given streams to an <see cref="AssemblyDefinition"/>.
        /// </summary>
        private static AssemblyDefinition LoadAssembly(Stream assembly, Stream symbols)
        {
            assembly.Position = 0;

            if (symbols != null)
            {
                try
                {
                    return AssemblyDefinition.ReadAssembly(assembly, new ReaderParameters
                    {
                        InMemory = true,
                        ReadWrite = true,
                        AssemblyResolver = AssemblyResolver,
                        MetadataResolver = MetadataResolver,
                        SymbolStream = symbols,
                        SymbolReaderProvider = ReaderProvider
                    });
                }
                catch
                {
                    // Fallback to loading no symbols.
                    assembly.Position = 0;
                }
            }

            return AssemblyDefinition.ReadAssembly(assembly, new ReaderParameters
            {
                InMemory = true,
                ReadWrite = true,
                AssemblyResolver = AssemblyResolver,
                MetadataResolver = MetadataResolver
            });
        }

        /// <summary>
        /// Saves the given <see cref="AssemblyDefinition"/> to the given streams.
        /// </summary>
        private static void SaveAssembly(Stream assembly, Stream symbols, AssemblyDefinition definition)
        {
            assembly.Position = 0;

            if (symbols != null && definition.MainModule.HasSymbols)
            {
                symbols.Position = 0;

                definition.Write(assembly, new WriterParameters
                {
                    SymbolStream = symbols,
                    SymbolWriterProvider = WriterProvider,
                    WriteSymbols = true
                });
            }
            else
            {
                definition.Write(assembly, new WriterParameters());
            }
        }
        #endregion
    }
}
