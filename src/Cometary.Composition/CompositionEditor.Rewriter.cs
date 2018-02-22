using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cometary.Composition
{
    using Extensions;

    internal sealed partial class CompositionEditor
    {
        private sealed class Rewriter : CSharpSyntaxRewriter
        {
            private readonly SemanticModel semanticModel;
            private readonly CancellationToken cancellationToken;

            internal Rewriter(CSharpCompilation compilation, SyntaxTree syntaxTree, CancellationToken cancellationToken)
            {
                this.semanticModel = compilation.GetSemanticModel(syntaxTree, true);
                this.cancellationToken = cancellationToken;
            }

            private static bool InheritsCompositionAttribute(INamedTypeSymbol type)
            {
                for (;;)
                {
                    if (type.MetadataName == nameof(CompositionAttribute))
                        return true;

                    type = type.BaseType;

                    if (type == null)
                        return false;
                }
            }

            /// <inheritdoc />
            public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
            {
                CancellationToken token = cancellationToken;
                INamedTypeSymbol symbol = semanticModel.GetDeclaredSymbol(node, token);

                if (symbol == null)
                    return node;

                int totalAttributes = 0;
                ImmutableArray<AttributeData> attrs = symbol.GetAttributes();

                if (attrs.IsDefaultOrEmpty)
                    return node;

                SyntaxList<AttributeListSyntax> attributeLists = node.AttributeLists;

                for (int i = 0; i < attributeLists.Count; i++)
                {
                    AttributeListSyntax attributeList = attributeLists[i];
                    SeparatedSyntaxList<AttributeSyntax> attributes = attributeList.Attributes;

                    for (int j = 0; j < attributes.Count; j++, totalAttributes++)
                    {
                        AttributeSyntax attr = attributes[j];
                        string attrName = attr.Name.ToString();

                        if (attrName == "Component" || attrName == nameof(ComponentAttribute))
                        {
                            // There is a 'Component' attribute: Specify its content.
                            string contentStr = node.ToString();

                            // TODO: Use b64 and serialization
                            // It works, except Roslyn <= 2.0.0 has a bug with serialization
                            //using (MemoryStream ms = new MemoryStream())
                            //{
                            //    node.SerializeTo(ms, cancellationToken);

                            //    contentStr = ms.TryGetBuffer(out ArraySegment<byte> buffer)
                            //        ? Convert.ToBase64String(buffer.Array, buffer.Offset, buffer.Count)
                            //        : Convert.ToBase64String(ms.ToArray());
                            //}

                            AttributeArgumentSyntax contentArg = SyntaxFactory.AttributeArgument(
                                SyntaxFactory.LiteralExpression(
                                    SyntaxKind.StringLiteralExpression,
                                    SyntaxFactory.Literal(contentStr)
                                )
                            );

                            node = node.WithAttributeLists(
                                attributeLists.Replace(
                                    attributeList,
                                    attributeList.WithAttributes(
                                        attributes.Replace(attr, attr.WithArgumentList(
                                            SyntaxFactory.AttributeArgumentList().AddArguments(contentArg)
                                        ))
                                    )
                                )
                            );
                        }

                        // Maybe a component?
                        AttributeData attrData = attrs[totalAttributes];

                        if (attrData.AttributeClass.MetadataName == nameof(CopyFromAttribute))
                        {
                            // CopyFrom component: copy members
                            node = CopyMembers(node, attrData, token);
                        }
                        else if (InheritsCompositionAttribute(attrData.AttributeClass))
                        {
                            // Component: apply it
                            CompositionAttribute builtAttr = attrData.Construct<CompositionAttribute>();

                            try
                            {
                                node = builtAttr.Component.Apply(node, symbol, token);

                                if (node == null)
                                    throw new NullReferenceException("A component cannot return null.");
                            }
                            catch (Exception e)
                            {
                                throw new DiagnosticException($"Error applying the {builtAttr.Component} component.", e, attr.GetLocation());
                            }
                            
                        }
                    }
                }

                return node;
            }

            private static ClassDeclarationSyntax CopyMembers(ClassDeclarationSyntax node, AttributeData attribute, CancellationToken cancellationToken)
            {
                // Copy all fields, properties, etc
                SyntaxList<MemberDeclarationSyntax> members = node.Members;

                // Verify argument, and get the content.
                ITypeSymbol componentType = attribute.ConstructorArguments[0].Value as ITypeSymbol;

                if (componentType == null)
                    throw new DiagnosticException("Invalid component: the component cannot be null.", attribute.ApplicationSyntaxReference.ToLocation());

                AttributeData componentAttrData = componentType.GetAttributes()
                    .FirstOrDefault(x => x.AttributeClass.MetadataName == nameof(ComponentAttribute));

                ClassDeclarationSyntax componentSyntax;

                if (componentAttrData == null)
                    throw new DiagnosticException($"Invalid component: the {componentType} component must have a component attribute.", attribute.ApplicationSyntaxReference.ToLocation());

                if (componentAttrData.ConstructorArguments.Length == 0)
                {
                    // Component with no argument, meaning it's (hopefully) in the current library
                    SyntaxReference componentSyntaxReference = componentType.DeclaringSyntaxReferences.FirstOrDefault();

                    componentSyntax = componentSyntaxReference?.GetSyntax(cancellationToken) as ClassDeclarationSyntax
                        ?? throw new DiagnosticException($"Invalid component: the {componentType} component must have a generated component attribute.", attribute.ApplicationSyntaxReference.ToLocation());

                    goto Merge;
                }

                string contentStr = componentAttrData.ConstructorArguments[0].Value as string;

                if (contentStr == null)
                    throw new DiagnosticException($"Invalid component: the {componentType} component must have a non-null string argument.", attribute.ApplicationSyntaxReference.ToLocation());

                //MemoryStream ms = null;

                try
                {
                    // Once again, we unfortunately cannot use serialization, and must pass the syntax tree directly
                    componentSyntax = SyntaxFactory.ParseCompilationUnit(contentStr).Members[0] as ClassDeclarationSyntax
                        ?? throw new DiagnosticException($"Invalid component: the {componentType} component is not a valid class.", attribute.ApplicationSyntaxReference.ToLocation());
                    //ms = new MemoryStream(Convert.FromBase64String(contentStr));

                    //componentSyntax = CSharpSyntaxNode.DeserializeFrom(ms, token) as ClassDeclarationSyntax
                    //               ?? throw new DiagnosticException($"Invalid component: the {componentType} component is not a ClassDeclarationSyntax.", attribute.ApplicationSyntaxReference.ToLocation());
                }
                catch (DiagnosticException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new DiagnosticException($"Invalid component: the {componentType} component cannot be deserialized.", e, attribute.ApplicationSyntaxReference.ToLocation());
                }

                Merge:
                return node.AddMembers(componentSyntax.Members.ToArray());
            }
        }
    }
}
