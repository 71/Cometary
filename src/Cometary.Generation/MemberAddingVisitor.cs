using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cometary
{
    /// <summary>
    ///   <see cref="AssemblyRewriter"/> that adds members to a <see cref="TypeDeclarationSyntax"/>.
    /// </summary>
    internal sealed class MemberAddingVisitor : AssemblyRewriter
    {
        internal static readonly Stack<MemberDeclarationSyntax> Members = new Stack<MemberDeclarationSyntax>();

        /// <inheritdoc />
        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            ClassDeclarationSyntax result = (ClassDeclarationSyntax)base.VisitClassDeclaration(node);

            if (Members.Count == 0)
                return result;

            MemberDeclarationSyntax[] membersToAdd = new MemberDeclarationSyntax[Members.Count];

            for (int i = 0; i < membersToAdd.Length; i++)
                membersToAdd[i] = Members.Pop();

            return result.AddMembers(membersToAdd);
        }

        /// <inheritdoc />
        public override SyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            InterfaceDeclarationSyntax result = (InterfaceDeclarationSyntax)base.VisitInterfaceDeclaration(node);

            if (Members.Count == 0)
                return result;

            MemberDeclarationSyntax[] membersToAdd = new MemberDeclarationSyntax[Members.Count];

            for (int i = 0; i < membersToAdd.Length; i++)
                membersToAdd[i] = Members.Pop();

            return result.AddMembers(membersToAdd);
        }

        /// <inheritdoc />
        public override SyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            EnumDeclarationSyntax result = (EnumDeclarationSyntax)base.VisitEnumDeclaration(node);

            if (Members.Count == 0)
                return result;

            EnumMemberDeclarationSyntax[] membersToAdd = new EnumMemberDeclarationSyntax[Members.Count];

            for (int i = 0; i < membersToAdd.Length; i++)
                membersToAdd[i] = Members.Pop() as EnumMemberDeclarationSyntax;

            return result.AddMembers(membersToAdd);
        }
    }
}
