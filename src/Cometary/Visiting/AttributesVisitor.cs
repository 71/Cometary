using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypeInfo = System.Reflection.TypeInfo;

namespace Cometary.Visiting
{
    using Internal;
    using Attributes;

    /// <summary>
    /// <see cref="AssemblyVisitor"/> that visits attributes set on members.
    /// </summary>
    internal sealed class AttributesVisitor : AssemblyVisitor
    {
        /// <inheritdoc />
        public override float Priority => 500.0f;

        private static IEnumerable<T> GetCustomAttributes<T>(MemberInfo member) where T : class
        {
            return member.GetCustomAttributes().OfType<T>();
        }

        /// <inheritdoc />
        public override MethodDeclarationSyntax Visit(MethodInfo method, MethodDeclarationSyntax node)
        {
            foreach (IMethodVisitor visitor in AttributesVisitor.GetCustomAttributes<IMethodVisitor>(method))
            {
                node = visitor.Visit(method, node);

                if (node == null)
                    return null;
            }

            return node;
        }

        /// <inheritdoc />
        public override TypeDeclarationSyntax Visit(TypeInfo type, TypeDeclarationSyntax node)
        {
            if (type == null)
                type = node.SyntaxTree.Model().GetDeclaredSymbol(node).Info();

            foreach (ITypeVisitor visitor in AttributesVisitor.GetCustomAttributes<ITypeVisitor>(type))
            {
                node = visitor.Visit(type, node);

                if (node == null)
                    return null;
            }

            return node;
        }

        /// <inheritdoc />
        public override PropertyDeclarationSyntax Visit(PropertyInfo property, PropertyDeclarationSyntax node)
        {
            foreach (IPropertyVisitor visitor in AttributesVisitor.GetCustomAttributes<IPropertyVisitor>(property))
            {
                node = visitor.Visit(property, node);

                if (node == null)
                    return null;
            }

            return node;
        }

        /// <inheritdoc />
        public override FieldDeclarationSyntax Visit(FieldInfo field, FieldDeclarationSyntax node)
        {
            foreach (IFieldVisitor visitor in AttributesVisitor.GetCustomAttributes<IFieldVisitor>(field))
            {
                node = visitor.Visit(field, node);

                if (node == null)
                    return null;
            }

            return node;
        }

        /// <inheritdoc />
        public override DelegateDeclarationSyntax Visit(TypeInfo type, DelegateDeclarationSyntax node)
        {
            foreach (IDelegateVisitor visitor in AttributesVisitor.GetCustomAttributes<IDelegateVisitor>(type))
            {
                node = visitor.Visit(type, node);

                if (node == null)
                    return null;
            }

            return node;
        }
    }
}
