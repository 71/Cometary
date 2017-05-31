using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cometary.Extensions
{
    partial class SyntaxExtensions
    {
        /// <summary>
        /// Gets the <see cref="IOperation"/> associated with the given <see cref="SyntaxNode"/>.
        /// </summary>
        public static IOperation Operation(this SyntaxNode node) => node.SyntaxTree.Model()?.GetOperation(node);

        /// <summary>
        /// Gets the <see cref="IOperation"/> associated with the given <see cref="SyntaxNode"/>.
        /// </summary>
        public static TOperation Operation<TOperation>(this SyntaxNode node) where TOperation : class, IOperation => node.Operation() as TOperation;

        /// <summary>
        /// Gets the <see cref="ISymbol"/> associated with the given declaration <see cref="SyntaxNode"/>.
        /// </summary>
        public static ISymbol Symbol(this SyntaxNode declaration) => declaration.SyntaxTree.Model()?.GetDeclaredSymbol(declaration);

        /// <summary>
        /// Gets the <see cref="ISymbol"/> associated with the given declaration <see cref="SyntaxNode"/>.
        /// </summary>
        public static TSymbol Symbol<TSymbol>(this SyntaxNode declaration) where TSymbol : class, ISymbol => declaration.Symbol() as TSymbol;

        /// <summary>
        /// Gets the <see cref="ITypeSymbol"/> associated with the given <see cref="TypeDeclarationSyntax"/>.
        /// </summary>
        public static ITypeSymbol Symbol(this TypeDeclarationSyntax type) => type.Symbol<ITypeSymbol>();

        /// <summary>
        /// Gets the <see cref="IMethodSymbol"/> associated with the given <see cref="MethodDeclarationSyntax"/>.
        /// </summary>
        public static IMethodSymbol Symbol(this MethodDeclarationSyntax method)
            => method.DeclaringType()
                     .Symbol()?
                     .GetMembers(method.Identifier.Text)
                     .OfType<IMethodSymbol>()
                     .FirstOrDefault();

        /// <summary>
        /// Gets the <see cref="IFieldSymbol"/> associated with the given <see cref="FieldDeclarationSyntax"/>.
        /// </summary>
        public static IFieldSymbol Symbol(this FieldDeclarationSyntax field)
            => field.DeclaringType()
                    .Symbol()?
                    .GetMembers(field.Identifier().Text)
                    .OfType<IFieldSymbol>()
                    .FirstOrDefault();

        /// <summary>
        /// Gets the <see cref="IPropertySymbol"/> associated with the given <see cref="PropertyDeclarationSyntax"/>.
        /// </summary>
        public static IPropertySymbol Symbol(this PropertyDeclarationSyntax property)
            => property.DeclaringType()
                       .Symbol()?
                       .GetMembers(property.Identifier.Text)
                       .OfType<IPropertySymbol>()
                       .FirstOrDefault();

        /// <summary>
        /// Gets the <see cref="IParameterSymbol"/> associated with the given <see cref="ParameterSyntax"/>.
        /// </summary>
        public static IParameterSymbol Symbol(this ParameterSyntax parameter)
        {
            MethodDeclarationSyntax method = parameter.DeclaringMethod();
            int index = method.ParameterList.Parameters.IndexOf(parameter);

            return method.Symbol().Parameters[index];
        }
    }
}
