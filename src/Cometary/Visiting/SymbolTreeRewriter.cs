using System;
using Microsoft.CodeAnalysis;

namespace Cometary.Visiting
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class SymbolTreeRewriter : SymbolVisitor<ISymbol>
    {
        private readonly CompilationEditor editor;
        private readonly Func<ISymbol, ISymbol> symbolRewriter;

        internal SymbolTreeRewriter(CompilationEditor editor, Func<ISymbol, ISymbol> symbolRewriter)
        {
            this.editor = editor;
            this.symbolRewriter = symbolRewriter;
        }

        internal IAssemblySymbol VisitRoot(IAssemblySymbol assemblySymbol)
        {
            ISymbol result = assemblySymbol.Accept(this);

            if (result.GetType().Name != "SourceAssemblySymbol")
                throw new InvalidOperationException($"Cannot convert {result.GetType()} to 'SourceAssemblySymbol'.");

            return (IAssemblySymbol)result;
        }

        public override ISymbol Visit(ISymbol symbol)
        {
            return symbolRewriter?.Invoke(symbol);
        }

        public override ISymbol DefaultVisit(ISymbol symbol)
        {
            return symbol;
        }

        public override ISymbol VisitMethod(IMethodSymbol symbol)
        {
        }
    }
}
