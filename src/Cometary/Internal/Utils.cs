using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypeInfo = System.Reflection.TypeInfo;

namespace Cometary.Internal
{
    /// <summary>
    /// Provides internal static utilities.
    /// </summary>
    internal static class Utils
    {
        internal static TypeInfo Info(this ITypeSymbol type)
        {
            StringBuilder builder = new StringBuilder();

            if (type.ContainingNamespace != null && !type.ContainingNamespace.IsGlobalNamespace)
            {
                builder.Append(type.ContainingNamespace);
                builder.Append('.');
            }

            builder.Append(type.Name);

            return Type.GetType($"{builder}")?.GetTypeInfo()
                ?? Type.GetType($"{builder}, {type.ContainingAssembly}")?.GetTypeInfo();
        }

        internal static TypeDeclarationSyntax WithMembers(this TypeDeclarationSyntax type, IEnumerable<MemberDeclarationSyntax> members)
        {
            SyntaxList<MemberDeclarationSyntax> _members = SyntaxFactory.List(members);

            if (type is ClassDeclarationSyntax _class)
                return _class.WithMembers(_members);
            if (type is StructDeclarationSyntax _struct)
                return _struct.WithMembers(_members);
            if (type is InterfaceDeclarationSyntax _interface)
                return _interface.WithMembers(_members);

            return SyntaxFactory.TypeDeclaration(type.Kind(), type.AttributeLists,
                type.Modifiers, type.Keyword, type.Identifier,
                type.TypeParameterList, type.BaseList, type.ConstraintClauses,
                type.OpenBraceToken, _members, type.CloseBraceToken, type.SemicolonToken);
        }

        internal static bool IsAssignableTo(this Type from, Type to)
        {
            return to.GetTypeInfo().IsAssignableFrom(from.GetTypeInfo());
        }
    }
}
