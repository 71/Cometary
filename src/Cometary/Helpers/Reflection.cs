using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypeInfo = System.Reflection.TypeInfo;

namespace Cometary.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public static class ReflectionHelpers
    {
        public static TypeInfo AsType(this BaseTypeDeclarationSyntax type)
        {
            return Meta.Compilation
                          .GetSymbolsWithName(x => x == type.Identifier.Text, SymbolFilter.Type)
                          .OfType<INamedTypeSymbol>()
                          .FirstOrDefault()?.Info();
        }

        public static TypeInfo AsType(this DelegateDeclarationSyntax type)
        {
            return Meta.Compilation
                          .GetSymbolsWithName(x => x == type.Identifier.Text, SymbolFilter.Member)
                          .OfType<ITypeSymbol>()
                          .FirstOrDefault()?.Info();
        }

        public static MethodInfo AsMethod(this MethodDeclarationSyntax method)
        {
            return method.FirstAncestorOrSelf<BaseTypeDeclarationSyntax>()
                .AsType()?.DeclaredMethods.FirstOrDefault(x => x.Name == method.Identifier.Text);
        }

        public static FieldInfo AsField(this FieldDeclarationSyntax field)
        {
            return field.FirstAncestorOrSelf<BaseTypeDeclarationSyntax>()
                .AsType()?.DeclaredFields.FirstOrDefault(x => x.Name == field.Declaration.Variables[0].Identifier.Text);
        }

        public static PropertyInfo AsProperty(this PropertyDeclarationSyntax property)
        {
            return property.FirstAncestorOrSelf<BaseTypeDeclarationSyntax>()
                .AsType()?.DeclaredProperties.FirstOrDefault(x => x.Name == property.Identifier.Text);
        }
    }
}
