using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cometary
{
    /// <summary>
    ///   Static class used to add members to types during a visit.
    /// </summary>
    public static class Members
    {
        /// <summary>
        ///   Adds the given <paramref name="member"/> to the <see langword="class"/>,
        ///   <see langword="interface"/> or <see langword="enum"/> currently being visited.
        /// </summary>
        public static void Add(MemberDeclarationSyntax member) => MemberAddingVisitor.Members.Push(member);
    }
}