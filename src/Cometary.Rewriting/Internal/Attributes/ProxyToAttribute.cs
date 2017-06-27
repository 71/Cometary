using System;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using F = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Cometary
{
    using Attributes;
    using System.Linq;

    /// <summary>
    /// Indicates that new members that match the marked member's signature
    /// will be created, and proxied to the marked member.
    /// <para>
    /// This method should be used to provide multiple names for a single member.
    /// </para>
    /// <para>
    /// Unlike the <see cref="CopyToAttribute"/>, the body of the method
    /// is not copied.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class ProxyToAttribute : Attribute, IMethodVisitor
    {
        /// <summary>
        /// Gets the names of the member that'll be created.
        /// </summary>
        public string[] Names { get; }

        /// <inheritdoc cref="CopyToAttribute" />
        public ProxyToAttribute(params string[] names)
        {
            Names = names;
        }

        private static TypeSyntax Convert(TypeParameterSyntax node)
        {
            return F.ParseTypeName(node.ToFullString());
        }

        private static ArgumentSyntax Convert(ParameterSyntax node)
        {
            return F.Argument(F.IdentifierName(node.Identifier));
        }

        /// <inheritdoc />
        public MethodDeclarationSyntax Visit(MethodInfo method, MethodDeclarationSyntax node)
        {
            ExpressionSyntax expression = method.IsGenericMethod
                ? F.GenericName(node.Identifier).AddTypeArgumentListArguments(node.TypeParameterList.Parameters.Select(Convert).ToArray()) as ExpressionSyntax
                : F.IdentifierName(node.Identifier);

            ArgumentSyntax[] arguments = node.ParameterList.Parameters.Select(Convert).ToArray();

            MethodDeclarationSyntax proxy = node.WithExpressionBody(F.ArrowExpressionClause(F.InvocationExpression(expression).AddArgumentListArguments(arguments)));

            foreach (string name in Names)
                Members.Add(proxy.WithIdentifier(F.Identifier(name)));

            return node;
        }
    }
}
