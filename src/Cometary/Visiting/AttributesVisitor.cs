using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypeInfo = System.Reflection.TypeInfo;

namespace Cometary.Visiting
{
    using Attributes;

    /// <summary>
    /// <see cref="AssemblyVisitor"/> that visits attributes set on members.
    /// </summary>
    internal sealed class AttributesVisitor : AssemblyVisitor
    {
        /// <inheritdoc />
        public override bool RewritesTree => true;

        private static IEnumerable<T> GetCustomAttributes<T>(MemberInfo member) where T : class
        {
            return member == null ? Enumerable.Empty<T>() : member.GetCustomAttributes().OfType<T>();
        }

        /// <inheritdoc />
        public override MethodDeclarationSyntax Visit(MethodInfo method, MethodDeclarationSyntax node)
        {
            foreach (IMethodVisitor visitor in GetCustomAttributes<IMethodVisitor>(method))
            {
                node = visitor.Visit(method, node);

                if (node == null)
                    return null;
            }

            return node;
        }

        /// <inheritdoc />
        public override ClassDeclarationSyntax Visit(TypeInfo type, ClassDeclarationSyntax node)
        {
            foreach (ITypeVisitor visitor in GetCustomAttributes<ITypeVisitor>(type))
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
            foreach (IPropertyVisitor visitor in GetCustomAttributes<IPropertyVisitor>(property))
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
            foreach (IFieldVisitor visitor in GetCustomAttributes<IFieldVisitor>(field))
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
            foreach (IDelegateVisitor visitor in GetCustomAttributes<IDelegateVisitor>(type))
            {
                node = visitor.Visit(type, node);

                if (node == null)
                    return null;
            }

            return node;
        }

        /// <inheritdoc />
        public override MethodDeclarationSyntax Visit(ParameterInfo parameter, ParameterSyntax syntax, MethodDeclarationSyntax node1)
        {
            foreach (IParameterVisitor visitor in parameter.GetCustomAttributes().OfType<IParameterVisitor>())
            {
                node1 = visitor.Visit(parameter, syntax, node1);

                if (node1 == null)
                    return null;
            }

            return node1;
        }

        /// <inheritdoc />
        public override EventDeclarationSyntax Visit(EventInfo @event, EventDeclarationSyntax node)
        {
            foreach (IEventVisitor visitor in GetCustomAttributes<IEventVisitor>(@event))
            {
                node = visitor.Visit(@event, node);

                if (node == null)
                    return null;
            }

            return node;
        }

        /// <inheritdoc />
        public override EnumDeclarationSyntax Visit(TypeInfo @enum, EnumDeclarationSyntax node)
        {
            foreach (IEnumVisitor visitor in GetCustomAttributes<IEnumVisitor>(@enum))
            {
                node = visitor.Visit(@enum, node);

                if (node == null)
                    return null;
            }

            return node;
        }

        /// <inheritdoc />
        public override InterfaceDeclarationSyntax Visit(TypeInfo @interface, InterfaceDeclarationSyntax node)
        {
            foreach (IInterfaceVisitor visitor in GetCustomAttributes<IInterfaceVisitor>(@interface))
            {
                node = visitor.Visit(@interface, node);

                if (node == null)
                    return null;
            }

            return node;
        }

        /// <inheritdoc />
        public override CSharpCompilation Visit(Assembly assembly, CSharpCompilation compilation)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            foreach (IAssemblyVisitor visitor in assembly.GetCustomAttributes().OfType<IAssemblyVisitor>())
            {
                CSharpCompilation newCompilation = visitor.Visit(assembly, compilation);

                if (newCompilation != null)
                    compilation = newCompilation;
            }

            return compilation;
        }
    }
}
