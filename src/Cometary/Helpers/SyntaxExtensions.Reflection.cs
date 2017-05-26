using System.Linq;
using System.Reflection;
using Cometary.Internal;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypeInfo = System.Reflection.TypeInfo;

namespace Cometary.Extensions
{
    partial class SyntaxExtensions
    {
        public static TypeInfo Info(this TypeDeclarationSyntax type)
        {
            return type.Symbol().Info();
        }

        public static MethodInfo Info(this MethodDeclarationSyntax method)
        {
            return method.DeclaringType().Info().GetDeclaredMethods(method.Identifier.Text).FirstOrDefault();
        }

        public static FieldInfo Info(this FieldDeclarationSyntax field)
        {
            return field.DeclaringType().Info().GetDeclaredField(field.Identifier().Text);
        }

        public static PropertyInfo Info(this PropertyDeclarationSyntax property)
        {
            return property.DeclaringType().Info().GetDeclaredProperty(property.Identifier.Text);
        }
    }
}
