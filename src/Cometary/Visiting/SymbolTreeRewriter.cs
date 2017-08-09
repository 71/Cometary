using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary.Visiting
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class SymbolTreeRewriter : SymbolVisitor<ISymbol>
    {
        // TODO: Find a way to rewriter a symbol tree.
        private readonly CompilationEditor editor;
        private readonly CSharpCompilation compilation;

        private readonly object diagnostics;
        private readonly SymbolFactory binder;
        private readonly Func<ISymbol, ISymbol> symbolRewriter;

        internal SymbolTreeRewriter(CompilationEditor editor, CSharpCompilation compilation, Func<ISymbol, ISymbol> symbolRewriter)
        {
            this.editor = editor;
            this.compilation = compilation;
            this.symbolRewriter = symbolRewriter;

            this.binder = new SymbolFactory(compilation);
        }

        internal IAssemblySymbol VisitRoot(IAssemblySymbol assemblySymbol)
        {
            ISymbol result = assemblySymbol.Accept(this);

            if (result.GetType().Name != "SourceAssemblySymbol")
                throw new InvalidOperationException($"Cannot convert {result.GetType()} to 'SourceAssemblySymbol'.");

            return (IAssemblySymbol)result;
        }

        public override ISymbol Visit(ISymbol symbol) => symbol.Accept(this);

        public override ISymbol DefaultVisit(ISymbol symbol) => symbolRewriter(symbol)?.Accept(this);

        public override ISymbol VisitNamedType(INamedTypeSymbol symbol)
        {
            return SourceSymbolFactory.CreateSourceNamedTypeSymbol(symbol.ContainingType,, diagnostics);
        }

        public override ISymbol VisitEvent(IEventSymbol symbol)
        {
        }
    }
}
