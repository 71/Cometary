using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cometary.Debugging
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class BreakingEditor : CompilationEditor
    {
        private readonly string[] possibleNames;

        internal BreakingEditor(Type[] attributeTypes)
        {
            possibleNames = new string[attributeTypes.Length * 2];

            for (int i = 0; i < attributeTypes.Length; i++)
            {
                string attrName = attributeTypes[i].Name;

                possibleNames[i] = attrName.Replace(nameof(Attribute), string.Empty);
                possibleNames[i + attributeTypes.Length] = attrName;
            }
        }

        /// <inheritdoc />
        protected override void Initialize(CSharpCompilation compilation, CancellationToken cancellationToken)
        {
            this.EditSyntax(EditSyntaxNode);
        }

        private static readonly StatementSyntax BreakStatement
            = SyntaxFactory.ParseStatement($"{typeof(Debugger).FullName}.Break();");

        private SyntaxNode EditSyntaxNode(SyntaxNode node, CancellationToken cancellationToken)
        {
            bool HasBreakingAttribute(SyntaxList<AttributeListSyntax> attrLists)
            {
                for (int i = 0; i < attrLists.Count; i++)
                {
                    SeparatedSyntaxList<AttributeSyntax> attrs = attrLists[i].Attributes;

                    for (int j = 0; j < attrs.Count; j++)
                    {
                        NameSyntax attrName = attrs[i].Name;
                        string attrNameString;

                        switch (attrName.Kind())
                        {
                            case SyntaxKind.IdentifierName:
                                attrNameString = ((IdentifierNameSyntax)attrName).Identifier.Text;
                                break;
                            case SyntaxKind.QualifiedName:
                                attrNameString = ((QualifiedNameSyntax)attrName).Right.Identifier.Text;
                                break;
                            default:
                                continue;
                        }

                        if (Array.IndexOf(possibleNames, attrNameString) != -1)
                            return true;
                    }
                }

                return false;
            }

            if (node is MethodDeclarationSyntax method && HasBreakingAttribute(method.AttributeLists))
            {
                var statements = method.Body.Statements.Insert(0, BreakStatement.NormalizeWhitespace());

                return method.WithBody(method.Body.WithStatements(statements));
            }

            return node;
        }
    }
}
