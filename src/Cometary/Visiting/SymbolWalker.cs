using System;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Cometary.Visiting
{
    /// <summary>
    ///   <see cref="SymbolVisitor"/> that visits every symbol's children recursively.
    /// </summary>
    public abstract class SymbolWalker : SymbolVisitor
    {
        /// <summary>
        ///   Returns a new <see cref="SymbolWalker"/> that invokes the given <paramref name="callback"/>
        ///   for every child in a given symbol.
        /// </summary>
        public static SymbolWalker Create(Action<ISymbol> callback, CancellationToken cancellationToken = default(CancellationToken))
        {
            return new CallbackBasedWalker(callback, cancellationToken);
        }

        private readonly CancellationToken cancellationToken;

        /// <summary>
        ///   Creates a new <see cref="SymbolWalker"/>, given a delegate to call on every symbol.
        /// </summary>
        protected SymbolWalker(CancellationToken cancellationToken = default(CancellationToken))
        {
            this.cancellationToken = cancellationToken;
        }

        /// <summary>
        ///   Visits the given <paramref name="symbol"/>, if and only if
        ///   the previously given <see cref="CancellationToken"/> hasn't received
        ///   a cancellation request.
        /// </summary>
        public override void Visit(ISymbol symbol)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            DefaultVisit(symbol);
        }

        /// <summary>
        ///   Visits a <paramref name="symbol"/>.
        /// </summary>
        public abstract override void DefaultVisit(ISymbol symbol);

        /// <inheritdoc />
        public override void VisitAlias(IAliasSymbol symbol) => Visit(symbol);

        /// <inheritdoc />
        public override void VisitArrayType(IArrayTypeSymbol symbol) => Visit(symbol);

        /// <inheritdoc />
        public override void VisitDiscard(IDiscardSymbol symbol) => Visit(symbol);

        /// <inheritdoc />
        public override void VisitDynamicType(IDynamicTypeSymbol symbol) => Visit(symbol);

        /// <inheritdoc />
        public override void VisitLabel(ILabelSymbol symbol) => Visit(symbol);

        /// <inheritdoc />
        public override void VisitLocal(ILocalSymbol symbol) => Visit(symbol);

        /// <inheritdoc />
        public override void VisitParameter(IParameterSymbol symbol) => Visit(symbol);

        /// <inheritdoc />
        public override void VisitPointerType(IPointerTypeSymbol symbol) => Visit(symbol);

        /// <inheritdoc />
        public override void VisitRangeVariable(IRangeVariableSymbol symbol) => Visit(symbol);

        /// <inheritdoc />
        public override void VisitTypeParameter(ITypeParameterSymbol symbol) => Visit(symbol);

        /// <inheritdoc />
        public override void VisitAssembly(IAssemblySymbol symbol)
        {
            Visit(symbol);

            foreach (IModuleSymbol module in symbol.Modules)
            {
                module.Accept(this);
            }
        }

        /// <inheritdoc />
        public override void VisitModule(IModuleSymbol symbol)
        {
            Visit(symbol);

            symbol.GlobalNamespace.Accept(this);
        }

        /// <inheritdoc />
        public override void VisitEvent(IEventSymbol symbol)
        {
            Visit(symbol);

            symbol.AddMethod?.Accept(this);
            symbol.RemoveMethod?.Accept(this);
            symbol.RaiseMethod?.Accept(this);
        }

        /// <inheritdoc />
        public override void VisitField(IFieldSymbol symbol)
        {
            Visit(symbol);
        }

        /// <inheritdoc />
        public override void VisitMethod(IMethodSymbol symbol)
        {
            Visit(symbol);

            foreach (IParameterSymbol param in symbol.Parameters)
                param.Accept(this);
            foreach (ITypeParameterSymbol typeParam in symbol.TypeParameters)
                typeParam.Accept(this);
        }

        /// <inheritdoc />
        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            Visit(symbol);

            foreach (var member in symbol.GetMembers())
            {
                member.Accept(this);
            }
        }

        /// <inheritdoc />
        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            Visit(symbol);

            foreach (var member in symbol.GetMembers())
            {
                member.Accept(this);
            }
        }

        /// <inheritdoc />
        public override void VisitProperty(IPropertySymbol symbol)
        {
            Visit(symbol);

            symbol.GetMethod?.Accept(this);
            symbol.SetMethod?.Accept(this);
        }

        private sealed class CallbackBasedWalker : SymbolWalker
        {
            private readonly Action<ISymbol> callback;

            internal CallbackBasedWalker(Action<ISymbol> callback, CancellationToken cancellationToken) : base(cancellationToken)
            {
                this.callback = callback;
            }

            /// <inheritdoc />
            public override void DefaultVisit(ISymbol symbol) => callback(symbol);
        }
    }
}
