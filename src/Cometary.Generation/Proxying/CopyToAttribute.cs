using System;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cometary
{
    using Attributes;

    /// <summary>
    /// <para>
    ///   Indicates that the marked member will be copied to one or many other members,
    ///   with the exact same signature and a new name.
    /// </para>
    /// <para>
    ///   This method should be used to provide multiple names for a single member.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    internal sealed class CopyToAttribute : Attribute, IMethodVisitor, IPropertyVisitor
    {
        /// <summary>
        ///   Gets the names of the member that'll be created.
        /// </summary>
        public string[] Names { get; }

        /// <inheritdoc cref="CopyToAttribute" />
        public CopyToAttribute(params string[] names)
        {
            Names = names;
        }

        /// <inheritdoc />
        public MethodDeclarationSyntax Visit(MethodInfo method, MethodDeclarationSyntax node)
        {
            foreach (string name in Names)
                Members.AddSibling(node, node.WithIdentifier(SyntaxFactory.Identifier(name)));

            return node;
        }

        /// <inheritdoc />
        public PropertyDeclarationSyntax Visit(PropertyInfo property, PropertyDeclarationSyntax node)
        {
            foreach (string name in Names)
                Members.AddSibling(node, node.WithIdentifier(SyntaxFactory.Identifier(name)));

            return node;
        }
    }
}
