using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Cometary.Attributes;
using Cometary.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using F = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Cometary.Tests
{
    /// <summary>
    /// Ensures the marked parameter is never <see langword="null"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class NotNullAttribute : Attribute, IParameterVisitor
    {
        private static StatementSyntax GetCheckStatement(string parameterName)
            => F.IfStatement($"{parameterName} == null".Syntax<BinaryExpressionSyntax>(),
                             $"throw new ArgumentNullException(nameof({parameterName}));".Syntax<StatementSyntax>());

        /// <inheritdoc />
        public ParameterSyntax Visit(ParameterInfo parameter, ParameterSyntax node)
        {
            node.DeclaringMethod()
                .Replace(x =>
                    x.WithBody(
                        x.Body.WithStatements(
                            x.Body.Statements.Insert(0, GetCheckStatement(parameter.Name)))));

            return node;
        }
    }

    class AttributesTests
    {

    }
}
