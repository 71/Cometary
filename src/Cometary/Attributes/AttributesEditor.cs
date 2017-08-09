using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class AttributesEditor : CompilationEditor
    {
        /// <inheritdoc />
        protected override void Initialize(CSharpCompilation compilation, CancellationToken cancellationToken)
        {
            RegisterEdit(new Edit<ISymbol>(EditSymbol));
        }

        /// <summary>
        /// 
        /// </summary>
        private static ISymbol EditSymbol(ISymbol symbol, CancellationToken cancellationToken)
        {
            ImmutableArray<AttributeData> attributes = symbol.GetAttributes();

            switch (symbol.Kind)
            {
                case SymbolKind.Field:
                    IFieldSymbol fieldSymbol = (IFieldSymbol)symbol;
                    foreach (var editor in attributes.FindAttributesOfInterface<IFieldVisitor>())
                        fieldSymbol = editor.Visit(fieldSymbol);
                    symbol = fieldSymbol;
                    break;
                case SymbolKind.Property:
                    IPropertySymbol propSymbol = (IPropertySymbol)symbol;
                    foreach (var editor in attributes.FindAttributesOfInterface<IPropertyVisitor>())
                        propSymbol = editor.Visit(propSymbol);
                    symbol = propSymbol;
                    break;
                case SymbolKind.Event:
                    IEventSymbol eventSymbol = (IEventSymbol)symbol;
                    foreach (var editor in attributes.FindAttributesOfInterface<IEventVisitor>())
                        eventSymbol = editor.Visit(eventSymbol);
                    symbol = eventSymbol;
                    break;
                case SymbolKind.Method:
                    IMethodSymbol methodSymbol = (IMethodSymbol)symbol;
                    foreach (var editor in attributes.FindAttributesOfInterface<IMethodVisitor>())
                        methodSymbol = editor.Visit(methodSymbol);
                    symbol = methodSymbol;
                    break;
                case SymbolKind.NamedType:
                    INamedTypeSymbol typeSymbol = (INamedTypeSymbol)symbol;
                    foreach (var editor in attributes.FindAttributesOfInterface<ITypeVisitor>())
                        typeSymbol = editor.Visit(typeSymbol);
                    symbol = typeSymbol;
                    break;
                case SymbolKind.Parameter:
                    IParameterSymbol parameterSymbol = (IParameterSymbol)symbol;
                    foreach (var editor in attributes.FindAttributesOfInterface<IParameterVisitor>())
                        parameterSymbol = editor.Visit(parameterSymbol);
                    symbol = parameterSymbol;
                    break;
                case SymbolKind.TypeParameter:
                    ITypeParameterSymbol typeParameterSymbol = (ITypeParameterSymbol)symbol;
                    foreach (var editor in attributes.FindAttributesOfInterface<ITypeParameterVisitor>())
                        typeParameterSymbol = editor.Visit(typeParameterSymbol);
                    symbol = typeParameterSymbol;
                    break;
            }

            return symbol;
        }
    }
}
