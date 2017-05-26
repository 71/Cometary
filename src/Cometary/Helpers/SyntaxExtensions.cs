using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Cometary.Internal;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TypeInfo = System.Reflection.TypeInfo;

namespace Cometary.Extensions
{
    /// <summary>
    /// Provides utilities used to easily manipulate syntax.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static partial class SyntaxExtensions
    {
        /// <summary>
        /// Gets the identifier of the given <see cref="FieldDeclarationSyntax"/>.
        /// </summary>
        /// <remarks>
        /// This method is a shortcut for <c>field.Declaration.Variables.First().Identifier</c>.
        /// </remarks>
        public static SyntaxToken Identifier(this FieldDeclarationSyntax field)
        {
            return field.Declaration.Variables.First().Identifier;
        }

        /// <summary>
        /// Parses the given <see cref="string"/> to a <see cref="SyntaxNode"/>
        /// of type <typeparamref name="T"/>.
        /// </summary>
        public static T Syntax<T>(this string str) where T : SyntaxNode
        {
            TypeInfo typeOfT = typeof(T).GetTypeInfo();

            if (typeof(StatementSyntax).GetTypeInfo().IsAssignableFrom(typeOfT))
            {
                T result = SyntaxFactory.ParseStatement(str) as T;

                if (result is ExpressionStatementSyntax)
                    return (result as ExpressionStatementSyntax).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)) as T;

                return result;
            }

            if (typeof(ExpressionSyntax).GetTypeInfo().IsAssignableFrom(typeOfT))
                return SyntaxFactory.ParseExpression(str) as T;
            if (typeof(TypeSyntax).GetTypeInfo().IsAssignableFrom(typeOfT))
                return SyntaxFactory.ParseTypeName(str) as T;

            return CSharpSyntaxTree.ParseText(str)
                .GetRoot()
                .DescendantNodesAndSelf()
                .OfType<T>()
                .FirstOrDefault();
        }

        #region Type utils
        /// <summary>
        ///
        /// </summary>
        public static MethodDeclarationSyntax GetMethod(this TypeDeclarationSyntax type, string name)
        {
            return type.Members
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(x => x.Identifier.Text == name);
        }

        /// <summary>
        /// 
        /// </summary>
        public static MethodDeclarationSyntax GetMethod(this TypeDeclarationSyntax type, string name, params Type[] parameterTypes)
        {
            return type.Members
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(x => x.Identifier.Text == name
                    && x.ParameterList.Parameters.Select(y => y.Type.ToString()).SequenceEqual(parameterTypes.Select(z => z.Name)));
        }

        /// <summary>
        ///
        /// </summary>
        public static MethodDeclarationSyntax GetMethod(this TypeDeclarationSyntax type, Func<MethodDeclarationSyntax, bool> predicate)
        {
            return type.Members
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(predicate);
        }

        /// <summary>
        /// 
        /// </summary>
        public static FieldDeclarationSyntax GetField(this TypeDeclarationSyntax type, string name)
        {
            return type.Members
                .OfType<FieldDeclarationSyntax>()
                .FirstOrDefault(x => x.Identifier().Text == name);
        }

        /// <summary>
        /// 
        /// </summary>
        public static FieldDeclarationSyntax GetField(this TypeDeclarationSyntax type, Func<FieldDeclarationSyntax, bool> predicate)
        {
            return type.Members
                .OfType<FieldDeclarationSyntax>()
                .FirstOrDefault(predicate);
        }

        /// <summary>
        /// 
        /// </summary>
        public static PropertyDeclarationSyntax GetProperty(this TypeDeclarationSyntax type, string name)
        {
            return type.Members
                .OfType<PropertyDeclarationSyntax>()
                .FirstOrDefault(x => x.Identifier.Text == name);
        }

        /// <summary>
        /// 
        /// </summary>
        public static PropertyDeclarationSyntax GetProperty(this TypeDeclarationSyntax type, Func<PropertyDeclarationSyntax, bool> predicate)
        {
            return type.Members
                .OfType<PropertyDeclarationSyntax>()
                .FirstOrDefault(predicate);
        }

        /// <summary>
        /// Returns the given <see cref="TypeSyntax"/> as a <see cref="TypeInfo"/>.
        /// </summary>
        public static TypeInfo AsType(this TypeSyntax type)
        {
            return type.Symbol<ITypeSymbol>().Info();
        }
        #endregion

        /// <summary>
        /// Returns the declaring type of the given member.
        /// </summary>
        public static TypeDeclarationSyntax DeclaringType(this MemberDeclarationSyntax member)
        {
            return member.FirstAncestorOrSelf<TypeDeclarationSyntax>();
        }

        /// <summary>
        /// Returns the declaring method of the given parameter.
        /// </summary>
        public static MethodDeclarationSyntax DeclaringMethod(this ParameterSyntax parameter)
        {
            return parameter.FirstAncestorOrSelf<MethodDeclarationSyntax>();
        }

        /// <summary>
        /// Returns whether or not the given method is marked <see langword="extern"/>.
        /// </summary>
        public static bool IsExtern(this BaseMethodDeclarationSyntax method)
        {
            return method.Modifiers.Any(SyntaxKind.ExternKeyword);
        }

        /// <summary>
        /// Returns a new <see cref="MethodDeclarationSyntax"/> with
        /// a new body, and no <see langword="extern"/> keyword.
        /// </summary>
        public static MethodDeclarationSyntax NotExtern(this MethodDeclarationSyntax method, CSharpSyntaxNode body)
        {
            method = method.WithModifiers(method.Modifiers.Remove(method.Modifiers.First(x => x.IsKind(SyntaxKind.ExternKeyword))));

            if (body is BlockSyntax block)
                return method.WithBody(block);
            if (body is StatementSyntax stmt)
                return method.WithBody(SyntaxFactory.Block(stmt));
            if (body is ExpressionSyntax expr)
                return method.WithExpressionBody(SyntaxFactory.ArrowExpressionClause(expr));
            if (body is ArrowExpressionClauseSyntax arrow)
                return method.WithExpressionBody(arrow);

            throw new ArgumentException("Invalid body node.", nameof(body));
        }
    }
}
