using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cometary
{
    /// <summary>
    /// <para>
    ///   Static class used to add members to types.
    /// </para>
    /// <para>
    ///   Manipulated types must be <see langword="partial"/>.
    /// </para>
    /// </summary>
    public static class Members
    {
        #region Hook
        public const string OptionsKey = "GENERATION";

        /// <summary>
        ///   Dictionary that contains all generated namespaces.
        /// </summary>
        private static readonly Dictionary<string, NamespaceDeclarationSyntax> Namespaces = new Dictionary<string, NamespaceDeclarationSyntax>();

        /// <summary>
        ///   Indicates Cometary that an additional <see cref="CSharpSyntaxTree"/> will be
        ///   imported into the project. The syntax tree will be automatically modified by Cometary
        ///   and its extensions.
        /// </summary>
        public static CometaryConfigurator GenerateTree(this CometaryConfigurator config, string newTreeName = "Cometary.g.cs")
        {
            return config
                .AddData(OptionsKey, true)
                .AddHook(Hook);

            void Hook(CometaryState state)
            {
                // Generate root
                CompilationUnitSyntax root = SyntaxFactory
                    .CompilationUnit()
                    .WithMembers(SyntaxFactory.List(Namespaces.OfType<MemberDeclarationSyntax>()));

                // Generate tree, and add it
                SyntaxTree generatedTree = CSharpSyntaxTree.Create(root);

                state.Compilation = state.Compilation.AddSyntaxTrees(generatedTree);
            }
        }
        #endregion

        /// <summary>
        ///   Adds the given <paramref name="member"/> to the declaring type
        ///   of the specified <paramref name="sibling"/>.
        /// </summary>
        public static void AddSibling(MemberDeclarationSyntax sibling, MemberDeclarationSyntax member)
        {
            if (sibling == null)
                throw new ArgumentNullException(nameof(sibling));

            AddCore((TypeDeclarationSyntax)sibling.Parent, member);
        }

        /// <summary>
        ///   Adds the given <paramref name="member"/> to the specified <see langword="class"/>.
        /// </summary>
        public static void Add(this ClassDeclarationSyntax type, MemberDeclarationSyntax member) => AddCore(type, member);

        /// <summary>
        ///   Adds the given <paramref name="member"/> to the specified <see langword="struct"/>.
        /// </summary>
        public static void Add(this StructDeclarationSyntax type, MemberDeclarationSyntax member) => AddCore(type, member);

        /// <summary>
        ///   Adds the given <paramref name="member"/> to the specified <see langword="interface"/>.
        /// </summary>
        public static void Add(this InterfaceDeclarationSyntax type, MemberDeclarationSyntax member) => AddCore(type, member);


        /// <summary>
        ///   Internal implementation of all Add methods.
        /// </summary>
        private static void AddCore(this TypeDeclarationSyntax type, MemberDeclarationSyntax member)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            // Find user namespace
            NamespaceDeclarationSyntax nsSyntax = type.FirstAncestorOrSelf<NamespaceDeclarationSyntax>();

            if (nsSyntax == null)
                throw new ArgumentException("The given type must be within a namespace.", nameof(type));

            string nsName = nsSyntax.Name.ToString();

            // Find or create generated namespace
            if (!Namespaces.TryGetValue(nsName, out NamespaceDeclarationSyntax ns))
                ns = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(nsName));

            // Find or create generated type
            TypeDeclarationSyntax generatedType = ns.Members
                .OfType<TypeDeclarationSyntax>()
                .FirstOrDefault(x => x.Identifier.Text == type.Identifier.Text);

            bool addedType = generatedType == null;

            // Add member to type
            if (addedType)
            {
                SyntaxToken partialModifier = SyntaxFactory.Token(SyntaxKind.PartialKeyword);

                switch (type.Kind())
                {
                    case SyntaxKind.ClassDeclaration:
                        generatedType = SyntaxFactory.ClassDeclaration(type.Identifier)
                                                     .AddModifiers(partialModifier)
                                                     .AddMembers(member);
                        break;

                    case SyntaxKind.InterfaceDeclaration:
                        generatedType = SyntaxFactory.InterfaceDeclaration(type.Identifier)
                                                     .AddModifiers(partialModifier)
                                                     .AddMembers(member);
                        break;

                    case SyntaxKind.StructDeclaration:
                        generatedType = SyntaxFactory.StructDeclaration(type.Identifier)
                                                     .AddModifiers(partialModifier)
                                                     .AddMembers(member);
                        break;

                    default:
                        throw new IndexOutOfRangeException();
                }

                // Add generated type to namespace
                ns = ns.AddMembers(generatedType);
            }
            else
            {
                TypeDeclarationSyntax oldGeneratedType = generatedType;

                switch (type)
                {
                    case ClassDeclarationSyntax c:
                        generatedType = c.AddMembers(member);
                        break;

                    case InterfaceDeclarationSyntax i:
                        generatedType = i.AddMembers(member);
                        break;

                    case StructDeclarationSyntax s:
                        generatedType = s.AddMembers(member);
                        break;

                    default:
                        throw new IndexOutOfRangeException();
                }

                // Replace generated type in namespace
                ns = ns.WithMembers(ns.Members.Replace(oldGeneratedType, generatedType));
            }

            // Update namespace
            Namespaces[nsName] = ns;
        }
    }
}