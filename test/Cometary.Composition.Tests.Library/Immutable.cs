using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using F = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using K = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

namespace Cometary.Tests
{
    /// <summary>
    ///   <see cref="Component"/> that adds With*() members to a class.
    /// </summary>
    public sealed class Immutable : Component
    {
        /// <inheritdoc />
        public override ClassDeclarationSyntax Apply(ClassDeclarationSyntax node, INamedTypeSymbol symbol, CancellationToken cancellationToken)
        {
            var parameters = ImmutableArray.CreateBuilder<ParameterSyntax>();
            var arguments  = ImmutableArray.CreateBuilder<ArgumentSyntax>();
            var ctorStmts  = ImmutableArray.CreateBuilder<StatementSyntax>();

            // Generate all parameters and statements
            var members = symbol.GetMembers();

            for (int i = 0; i < members.Length; i++)
            {
                IPropertySymbol member = members[i] as IPropertySymbol;

                if (member == null || !member.IsReadOnly || !member.CanBeReferencedByName)
                    continue;

                // Read-only prop, we good
                string propName = member.Name;
                string paramName = $"{char.ToLower(propName[0]).ToString()}{propName.Substring(1)}";
                TypeSyntax paramType = ((PropertyDeclarationSyntax)member.DeclaringSyntaxReferences[0]
                    .GetSyntax(cancellationToken)).Type;

                MemberAccessExpressionSyntax propAccess = F.MemberAccessExpression(
                    K.SimpleMemberAccessExpression,
                    F.ThisExpression(),
                    F.IdentifierName(propName)
                );

                // Make parameter & argument
                parameters.Add(F.Parameter(F.Identifier(paramName)).WithType(paramType));
                arguments.Add(F.Argument(propAccess));

                // Make ctor stmt
                ctorStmts.Add(F.ExpressionStatement(
                    F.AssignmentExpression(K.SimpleAssignmentExpression,
                        propAccess,
                        F.IdentifierName(paramName)
                    )
                ));
            }

            // The ctor is full, make all the 'with' methods
            TypeSyntax returnType = F.IdentifierName(symbol.Name);
            MemberDeclarationSyntax[] additionalMethods = new MemberDeclarationSyntax[parameters.Count + 1];

            arguments.Capacity = arguments.Count;
            ImmutableArray<ArgumentSyntax> args = arguments.MoveToImmutable();

            for (int i = 0; i < parameters.Count; i++)
            {
                ParameterSyntax parameter = parameters[i];
                string parameterName = parameter.Identifier.Text;

                ArgumentSyntax name = F.Argument(F.IdentifierName(parameterName));
                SeparatedSyntaxList<ArgumentSyntax> allArguments = F.SeparatedList(args.Replace(args[i], name));

                StatementSyntax returnStmt = F.ReturnStatement(
                    F.ObjectCreationExpression(returnType).WithArgumentList(F.ArgumentList(allArguments))
                );

                additionalMethods[i] = F.MethodDeclaration(returnType, $"With{char.ToUpper(parameterName[0]).ToString()}{parameterName.Substring(1)}")
                    .AddModifiers(F.Token(K.PublicKeyword), F.Token(K.NewKeyword))
                    .AddParameterListParameters(parameter)
                    .AddBodyStatements(returnStmt);
            }

            // Add the private ctor
            additionalMethods[parameters.Count] = F.ConstructorDeclaration(symbol.Name)
                .AddModifiers(F.Token(K.PrivateKeyword))
                .AddParameterListParameters(parameters.ToArray())
                .AddBodyStatements(ctorStmts.ToArray());

            return node.AddMembers(additionalMethods);
        }
    }
}
