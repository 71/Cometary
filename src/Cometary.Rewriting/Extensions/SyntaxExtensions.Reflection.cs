using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using TypeInfo = System.Reflection.TypeInfo;

namespace Cometary.Extensions
{
    partial class SyntaxExtensions
    {
        /// <summary>
        /// Gets the <see cref="TypeInfo"/> associated with the given <see cref="TypeDeclarationSyntax"/>.
        /// </summary>
        public static TypeInfo Info(this TypeDeclarationSyntax type) => type.Symbol()?.Info();

        /// <summary>
        /// Gets the <see cref="TypeInfo"/> associated with the given <see cref="DelegateDeclarationSyntax"/>.
        /// </summary>
        public static TypeInfo Info(this DelegateDeclarationSyntax type) => type.Symbol<ITypeSymbol>()?.Info();

        /// <summary>
        /// Gets the <see cref="TypeInfo"/> associated with the given <see cref="EnumDeclarationSyntax"/>.
        /// </summary>
        public static TypeInfo Info(this EnumDeclarationSyntax type) => type.Symbol<ITypeSymbol>()?.Info();

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> associated with the given <see cref="MethodDeclarationSyntax"/>.
        /// </summary>
        public static MethodInfo Info(this MethodDeclarationSyntax method) => method.DeclaringType()?.Info()?.GetDeclaredMethods(method.Identifier.Text).FirstOrDefault();

        /// <summary>
        /// Gets the <see cref="FieldInfo"/> associated with the given <see cref="FieldDeclarationSyntax"/>.
        /// </summary>
        public static FieldInfo Info(this FieldDeclarationSyntax field) => field.DeclaringType()?.Info()?.GetDeclaredField(field.Identifier().Text);

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> associated with the given <see cref="PropertyDeclarationSyntax"/>.
        /// </summary>
        public static PropertyInfo Info(this PropertyDeclarationSyntax property) => property.DeclaringType()?.Info()?.GetDeclaredProperty(property.Identifier.Text);

        /// <summary>
        /// Gets the <see cref="EventInfo"/> associated with the given <see cref="EventDeclarationSyntax"/>.
        /// </summary>
        public static EventInfo Info(this EventDeclarationSyntax @event) => @event.DeclaringType()?.Info()?.GetDeclaredEvent(@event.Identifier.Text);
    }
}
