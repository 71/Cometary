using System;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using TypeInfo = System.Reflection.TypeInfo;
using F = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Cometary.Contracts
{
    using Attributes;
    using Extensions;

    /// <summary>
    /// Defines a parameter that cannot be <see langword="null"/>.
    /// <para>
    /// If it is <see langword="null"/>, an <see cref="ArgumentNullException"/>
    /// exception will be thrown.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class NotNullAttribute : Attribute, IParameterVisitor, IReturnValueVisitor
    {
        private static StatementSyntax GetCheckStatement(string parameterName)
            => F.IfStatement($"{parameterName} == null".Syntax<BinaryExpressionSyntax>(),
                             $"throw new ArgumentNullException(nameof({parameterName}));".Syntax<StatementSyntax>());

        private static ExpressionSyntax GetCheckExpression(ExpressionSyntax retExpr)
            => F.BinaryExpression(SyntaxKind.CoalesceExpression,
                                  retExpr, "throw new ArgumentNullException(\"return value\")".Syntax<ExpressionSyntax>());

        /// <summary>
        /// Injects a null-check at the beginning of the method's body.
        /// </summary>
        public MethodDeclarationSyntax Visit(ParameterInfo parameter, ParameterSyntax syntax, MethodDeclarationSyntax node)
        {
            StatementSyntax checkStmt = GetCheckStatement(parameter.Name);

            if (node.ExpressionBody == null)
                return node.WithBody(node.Body.WithStatements(node.Body.Statements.Insert(0, checkStmt)));

            return node.WithBody(F.Block(
                checkStmt,
                node.Symbol().ReturnsVoid
                    ? F.ExpressionStatement(node.ExpressionBody.Expression) as StatementSyntax
                    : F.ReturnStatement(node.ExpressionBody.Expression)
           ));
        }

        /// <summary>
        /// Adds a null-check to all <see langword="return"/> statements.
        /// </summary>
        public MethodDeclarationSyntax Visit(TypeInfo parameterType, MethodDeclarationSyntax node)
        {
            if (node.ExpressionBody != null)
                return node.WithExpressionBody(node.ExpressionBody.WithExpression(GetCheckExpression(node.ExpressionBody.Expression)));

            return node.ReplaceNodes(node.Body.Statements, (_, n) =>
            {
                if (!(n is ReturnStatementSyntax ret))
                    return n;

                return ret.WithExpression(GetCheckExpression(ret.Expression));
            });
        }
    }
}
