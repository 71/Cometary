using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cometary
{
    using Core;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// <see cref="LightAssemblyVisitor"/> that adds members to a <see cref="TypeDeclarationSyntax"/>.
    /// </summary>
    internal sealed class MemberAddingVisitor : LightAssemblyVisitor
    {
        internal static readonly Stack<MemberDeclarationSyntax> Members = new Stack<MemberDeclarationSyntax>();

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

    /// <summary>
    /// Static class used to add members to types during a visit.
    /// </summary>
    public static class Members
    {
        /// <summary>
        /// Adds the given <paramref name="member"/> to the <see langword="class"/>,
        /// <see langword="interface"/> or <see langword="enum"/> currently being visited.
        /// </summary>
        public static void Add(MemberDeclarationSyntax member) => MemberAddingVisitor.Members.Push(member);
    }
}
