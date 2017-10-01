using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypeInfo = System.Reflection.TypeInfo;

namespace Cometary
{
    using Attributes;
    using Extensions;

    /// <summary>
    ///   <see cref="AssemblyRewriter"/> that visits attributes set on members.
    /// </summary>
    internal sealed class AttributesVisitor : AssemblyRewriter
    {
        /// <inheritdoc />
        public override bool RewritesTree => true;

        private static IEnumerable<T> GetCustomAttributes<T>(MemberInfo member) where T : class
        {
            return member == null ? Enumerable.Empty<T>() : member.GetCustomAttributes().OfType<T>();
        }

        private static SyntaxList<AttributeListSyntax> RemoveAttribute(SyntaxList<AttributeListSyntax> attributes, IList<string> attrsToRemove)
        {
            if (!CleanUp.ShouldCleanUp)
                return attributes;

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

        private static MethodDeclarationSyntax VisitParameter(ParameterSyntax parameterSyntax, MethodDeclarationSyntax node, MethodInfo method, ParameterInfo parameter)
        {
            List<string> attrsToRemove = new List<string>();

            foreach (IParameterVisitor visitor in GetCustomAttributes<IParameterVisitor>(method))
            {
                node = visitor.Visit(parameter, parameterSyntax, node);

                if (node == null)
                    return null;

                attrsToRemove.Add(visitor.GetType().Name);
            }

            ParameterSyntax newParameterSyntax = parameterSyntax.WithAttributeLists(RemoveAttribute(parameterSyntax.AttributeLists, attrsToRemove));

            return node.WithParameterList(node.ParameterList.WithParameters(node.ParameterList.Parameters.Replace(parameterSyntax, newParameterSyntax)));
        }

        /// <inheritdoc />
        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            MethodInfo method = node.Info();

            if (method == null)
                return node;

            ParameterInfo[] parameters = method.GetParameters();

            for (int i = 0; i < parameters.Length; i++)
            {
                node = VisitParameter(node.ParameterList.Parameters[i], node, method, parameters[i]);

                if (node == null)
                    return null;
            }

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
        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            TypeInfo type = node.Info();

            if (type == null)
                return node;

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
        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            PropertyInfo property = node.Info();

            if (property == null)
                return node;

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
        public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            FieldInfo field = node.Info();

            if (field == null)
                return node;

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
        public override SyntaxNode VisitDelegateDeclaration(DelegateDeclarationSyntax node)
        {
            TypeInfo type = node.Info();

            if (type == null)
                return node;

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
        public override SyntaxNode VisitEventDeclaration(EventDeclarationSyntax node)
        {
            EventInfo @event = node.Info();

            if (@event == null)
                return node;

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
        public override SyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            TypeInfo @enum = node.Info();

            if (@enum == null)
                return node;

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
        public override SyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            TypeInfo @interface = node.Info();

            if (@interface == null)
                return node;

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
    }
}