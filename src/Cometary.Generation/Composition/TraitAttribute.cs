using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypeInfo = System.Reflection.TypeInfo;

namespace Cometary.Composition
{
    using Attributes;

    /// <summary>
    /// <para>
    ///   Indicates that the marked type will include a mixin.
    /// </para>
    /// <para>
    ///   The included <see langword="class"/> must be marked with the
    ///   <see cref="MixinAttribute"/> attribute.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public sealed class TraitAttribute : Attribute, IClassVisitor, IStructVisitor
    {
        /// <summary>
        ///   Gets the <see cref="Type"/> of the <see langword="class"/> to include.
        /// </summary>
        public Type MixinType { get; }

        /// <summary>
        ///   Indicates that the marked type will include the mixin
        ///   defined by the given type.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="mixinType"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="mixinType"/> is invalid.</exception>
        public TraitAttribute(Type mixinType)
        {
            if (mixinType == null)
                throw new ArgumentNullException(nameof(mixinType));

            // Ensures the given type is valid
            TypeInfo info = mixinType.GetTypeInfo();

            if (info.IsInterface)
                throw new ArgumentException("The given type must be a class.", nameof(mixinType));
            if (info.IsAbstract)
                throw new ArgumentException("The given type cannot be abstract.", nameof(mixinType));
            if (info.GetCustomAttribute<MixinAttribute>() == null)
                throw new ArgumentException("The given type must be marked with the Mixin attribute.", nameof(mixinType));
            if (Array.Exists(info.GenericTypeArguments, x => x.IsGenericParameter))
                throw new ArgumentException("The given type cannot have unresolved generic parameters.", nameof(mixinType));

            MixinType = mixinType;
        }

        /// <summary>
        ///   Includes the mixin in the marked class.
        /// </summary>
        ClassDeclarationSyntax IClassVisitor.Visit(TypeInfo type, ClassDeclarationSyntax node)
        {
            TypeDeclarationSyntax mixinSyntax = FindTypeDeclarationSyntax(MixinType);

            foreach (MemberDeclarationSyntax member in mixinSyntax.Members.Where(ShouldInclude))
                node.Add(member);

            return node;
        }

        /// <summary>
        ///   Includes the mixin in the marked struct.
        /// </summary>
        StructDeclarationSyntax IStructVisitor.Visit(TypeInfo @struct, StructDeclarationSyntax node)
        {
            TypeDeclarationSyntax mixinSyntax = FindTypeDeclarationSyntax(MixinType);

            foreach (MemberDeclarationSyntax member in mixinSyntax.Members.Where(ShouldInclude))
                node.Add(member);

            return node;
        }

        private static TypeDeclarationSyntax FindTypeDeclarationSyntax(Type type)
        {
            INamedTypeSymbol symbol = Meta.Compilation.GetTypeByMetadataName(type.AssemblyQualifiedName);

            return (TypeDeclarationSyntax)symbol.DeclaringSyntaxReferences.First().GetSyntax();
        }

        private static bool ShouldInclude(MemberDeclarationSyntax member)
        {
            // TODO: Add ability to keep some members from being included.
            return !member.IsKind(SyntaxKind.ConstructorDeclaration);
        }
    }
}
