using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using TypeInfo = System.Reflection.TypeInfo;

namespace Cometary
{
    using Common;
    using Attributes;
    using Extensions;

    /// <summary>
    /// Defines a class that visits an assembly's members.
    /// </summary>
    public abstract class AssemblyVisitor : LightAssemblyVisitor,
        IClassVisitor, IDelegateVisitor, IEnumVisitor, IInterfaceVisitor,
        IMethodVisitor, IFieldVisitor, IPropertyVisitor,
        IAssemblyVisitor, IParameterVisitor
    {
        /// <inheritdoc />
        public virtual MethodDeclarationSyntax Visit(MethodInfo method, MethodDeclarationSyntax node) => node;

        /// <inheritdoc />
        public virtual ClassDeclarationSyntax Visit(TypeInfo type, ClassDeclarationSyntax node) => node;

        /// <inheritdoc />
        public virtual PropertyDeclarationSyntax Visit(PropertyInfo property, PropertyDeclarationSyntax node) => node;

        /// <inheritdoc />
        public virtual FieldDeclarationSyntax Visit(FieldInfo field, FieldDeclarationSyntax node) => node;

        /// <inheritdoc />
        public virtual DelegateDeclarationSyntax Visit(TypeInfo type, DelegateDeclarationSyntax node) => node;

        /// <inheritdoc />
        public virtual EventDeclarationSyntax Visit(EventInfo @event, EventDeclarationSyntax node) => node;

        /// <inheritdoc />
        public virtual MethodDeclarationSyntax Visit(ParameterInfo parameter, ParameterSyntax syntax, MethodDeclarationSyntax node1) => node1;

        /// <inheritdoc />
        public virtual EnumDeclarationSyntax Visit(TypeInfo @enum, EnumDeclarationSyntax node) => node;

        /// <inheritdoc />
        public virtual InterfaceDeclarationSyntax Visit(TypeInfo @interface, InterfaceDeclarationSyntax node) => node;

        /// <inheritdoc />
        public sealed override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node) => Visit(node.Info(), node);

        /// <inheritdoc />
        public sealed override SyntaxNode VisitEventDeclaration(EventDeclarationSyntax node) => Visit(node.Info(), node);

        /// <inheritdoc />
        public sealed override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node) => Visit(node.Info(), node);

        /// <inheritdoc />
        public sealed override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node) => Visit(node.Info(), node);

        /// <inheritdoc />
        public sealed override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node) => Visit(node.Info(), node);

        /// <inheritdoc />
        public sealed override SyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node) => Visit(node.Info(), node);

        /// <inheritdoc />
        public sealed override SyntaxNode VisitDelegateDeclaration(DelegateDeclarationSyntax node) => Visit(node.Info(), node);

        /// <inheritdoc />
        public sealed override SyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node) => Visit(node.Info(), node);
    }
}
