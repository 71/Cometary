using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypeInfo = System.Reflection.TypeInfo;

namespace Cometary
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

        private static SyntaxList<AttributeListSyntax> RemoveAttribute(SyntaxList<AttributeListSyntax> attributes, IList<string> attrsToRemove)
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                var attrs = attributes[i].Attributes;

                for (int o = 0; o < attrs.Count; o++)
                {
                    AttributeSyntax attr = attrs[o];

                    if (attr.Name is SimpleNameSyntax name &&
                        (attrsToRemove.IndexOf(name.Identifier.Text) != -1 ||
                         attrsToRemove.IndexOf(name.Identifier.Text + nameof(Attribute)) != -1))
                        attrs = attrs.RemoveAt(o--);
                }

                if (attrs.Count == 0)
                    attributes = attributes.RemoveAt(i--);
            }

            return attributes;
        }

        /// <inheritdoc />
        public override MethodDeclarationSyntax Visit(MethodInfo method, MethodDeclarationSyntax node)
        {
            List<string> attrsToRemove = new List<string>();

            foreach (IMethodVisitor visitor in GetCustomAttributes<IMethodVisitor>(method))
            {
                node = visitor.Visit(method, node);

                if (node == null)
                    return null;

                attrsToRemove.Add(visitor.GetType().Name);
            }

            return node.WithAttributeLists(RemoveAttribute(node.AttributeLists, attrsToRemove));
        }

        /// <inheritdoc />
        public override ClassDeclarationSyntax Visit(TypeInfo type, ClassDeclarationSyntax node)
        {
            List<string> attrsToRemove = new List<string>();

            foreach (IClassVisitor visitor in GetCustomAttributes<IClassVisitor>(type))
            {
                node = visitor.Visit(type, node);

                if (node == null)
                    return null;

                attrsToRemove.Add(visitor.GetType().Name);
            }

            return node.WithAttributeLists(RemoveAttribute(node.AttributeLists, attrsToRemove));
        }

        /// <inheritdoc />
        public override PropertyDeclarationSyntax Visit(PropertyInfo property, PropertyDeclarationSyntax node)
        {
            List<string> attrsToRemove = new List<string>();

            foreach (IPropertyVisitor visitor in GetCustomAttributes<IPropertyVisitor>(property))
            {
                node = visitor.Visit(property, node);

                if (node == null)
                    return null;

                attrsToRemove.Add(visitor.GetType().Name);
            }

            return node.WithAttributeLists(RemoveAttribute(node.AttributeLists, attrsToRemove));
        }

        /// <inheritdoc />
        public override FieldDeclarationSyntax Visit(FieldInfo field, FieldDeclarationSyntax node)
        {
            List<string> attrsToRemove = new List<string>();

            foreach (IFieldVisitor visitor in GetCustomAttributes<IFieldVisitor>(field))
            {
                node = visitor.Visit(field, node);

                if (node == null)
                    return null;

                attrsToRemove.Add(visitor.GetType().Name);
            }

            return node.WithAttributeLists(RemoveAttribute(node.AttributeLists, attrsToRemove));
        }

        /// <inheritdoc />
        public override DelegateDeclarationSyntax Visit(TypeInfo type, DelegateDeclarationSyntax node)
        {
            List<string> attrsToRemove = new List<string>();

            foreach (IDelegateVisitor visitor in GetCustomAttributes<IDelegateVisitor>(type))
            {
                node = visitor.Visit(type, node);

                if (node == null)
                    return null;

                attrsToRemove.Add(visitor.GetType().Name);
            }

            return node.WithAttributeLists(RemoveAttribute(node.AttributeLists, attrsToRemove));
        }

        /// <inheritdoc />
        public override MethodDeclarationSyntax Visit(ParameterInfo parameter, ParameterSyntax syntax, MethodDeclarationSyntax node)
        {
            List<string> attrsToRemove = new List<string>();

            foreach (IParameterVisitor visitor in parameter.GetCustomAttributes().OfType<IParameterVisitor>())
            {
                node = visitor.Visit(parameter, syntax, node);

                if (node == null)
                    return null;

                attrsToRemove.Add(visitor.GetType().Name);
            }

            return node.WithAttributeLists(RemoveAttribute(node.AttributeLists, attrsToRemove));
        }

        /// <inheritdoc />
        public override EventDeclarationSyntax Visit(EventInfo @event, EventDeclarationSyntax node)
        {
            List<string> attrsToRemove = new List<string>();

            foreach (IEventVisitor visitor in GetCustomAttributes<IEventVisitor>(@event))
            {
                node = visitor.Visit(@event, node);

                if (node == null)
                    return null;

                attrsToRemove.Add(visitor.GetType().Name);
            }

            return node.WithAttributeLists(RemoveAttribute(node.AttributeLists, attrsToRemove));
        }

        /// <inheritdoc />
        public override EnumDeclarationSyntax Visit(TypeInfo @enum, EnumDeclarationSyntax node)
        {
            List<string> attrsToRemove = new List<string>();

            foreach (IEnumVisitor visitor in GetCustomAttributes<IEnumVisitor>(@enum))
            {
                node = visitor.Visit(@enum, node);

                if (node == null)
                    return null;

                attrsToRemove.Add(visitor.GetType().Name);
            }

            return node.WithAttributeLists(RemoveAttribute(node.AttributeLists, attrsToRemove));
        }

        /// <inheritdoc />
        public override InterfaceDeclarationSyntax Visit(TypeInfo @interface, InterfaceDeclarationSyntax node)
        {
            List<string> attrsToRemove = new List<string>();

            foreach (IInterfaceVisitor visitor in GetCustomAttributes<IInterfaceVisitor>(@interface))
            {
                node = visitor.Visit(@interface, node);

                if (node == null)
                    return null;

                attrsToRemove.Add(visitor.GetType().Name);
            }

            return node.WithAttributeLists(RemoveAttribute(node.AttributeLists, attrsToRemove));
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