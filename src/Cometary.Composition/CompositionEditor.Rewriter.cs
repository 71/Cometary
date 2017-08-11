using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cometary.Composition
{
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

            /// <inheritdoc />
            public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
            {
                SyntaxList<AttributeListSyntax> attributeLists = node.AttributeLists;

                for (int i = 0; i < attributeLists.Count; i++)
                {
                    AttributeListSyntax attributeList = attributeLists[i];
                    SeparatedSyntaxList<AttributeSyntax> attributes = attributeList.Attributes;

                    for (int j = 0; j < attributes.Count; j++)
                    {
                        AttributeSyntax attr = attributes[j];
                        string attrName = attr.Name is SimpleNameSyntax simpleName
                            ? simpleName.Identifier.Text
                            : ((QualifiedNameSyntax)attr.Name).Right.Identifier.Text;

                        switch (attrName)
                        {
                            case nameof(ComposeAttribute):
                            case "Compose":
                                // There is a 'Compose' attribute: Modify the whole class.
                                {
                                    CancellationToken token = cancellationToken;

                                    return Compose(node, semanticModel.GetDeclaredSymbol(node, token), token);
                                }


                            case nameof(ComponentAttribute):
                            case "Component":
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

                                return node.WithAttributeLists(
                                    attributeLists.Replace(
                                        attributeList,
                                        attributeList.WithAttributes(
                                            attributes.Replace(attr, attr.WithArgumentList(
                                                SyntaxFactory.AttributeArgumentList().AddArguments(contentArg)
                                            ))
                                        )
                                    )
                                );

                            default:
                                continue;
                        }
                    }
                }

                return node;

                ClassDeclarationSyntax Compose(ClassDeclarationSyntax decl, INamedTypeSymbol symbol, CancellationToken token)
                {
                    // Find 'Compose' attribute, and its arguments
                    AttributeData attribute = symbol.GetAttributes()
                        .FirstOrDefault(x => x.AttributeClass.MetadataName == nameof(ComposeAttribute));
                    ImmutableArray<TypedConstant> args = attribute.ConstructorArguments[0].Values;

                    // Create pipeline for dynamic components
                    List<Component> dynamicComponents = new List<Component>();

                    // Copy all fields, properties, etc
                    SyntaxList<MemberDeclarationSyntax> members = decl.Members;

                    for (int i = 0; i < args.Length; i++)
                    {
                        // Verify argument, and get the content.
                        ITypeSymbol componentType = args[i].Value as ITypeSymbol;

                        if (componentType == null)
                            throw new DiagnosticException("Invalid component: no component cannot be null.", attribute.ApplicationSyntaxReference.ToLocation());

                        if (componentType.BaseType.MetadataName == nameof(Component))
                        {
                            // Dynamic component
                            Component component;

                            try
                            {
                                component = (Component)Activator.CreateInstance(componentType.GetCorrespondingType());
                            }
                            catch (Exception e)
                            {
                                throw new DiagnosticException($"Invalid component: the {componentType} component cannot be instantiated.", e, attribute.ApplicationSyntaxReference.ToLocation());
                            }

                            dynamicComponents.Add(component);

                            continue;
                        }

                        AttributeData componentAttrData = componentType.GetAttributes()
                            .FirstOrDefault(x => x.AttributeClass.MetadataName == nameof(ComponentAttribute));

                        if (componentAttrData == null)
                            throw new DiagnosticException($"Invalid component: the {componentType} component must have a component attribute.", attribute.ApplicationSyntaxReference.ToLocation());
                        if (componentAttrData.ConstructorArguments.Length == 0)
                            throw new DiagnosticException($"Invalid component: the {componentType} component must have a generated component attribute.", attribute.ApplicationSyntaxReference.ToLocation());

                        string contentStr = componentAttrData.ConstructorArguments[0].Value as string;

                        if (contentStr == null)
                            throw new DiagnosticException($"Invalid component: the {componentType} component must have a non-null string argument.", attribute.ApplicationSyntaxReference.ToLocation());

                        ClassDeclarationSyntax componentSyntax;
                        //MemoryStream ms = null;

                        try
                        {
                            // Once again, we unfortunately cannot use serialization, and must pass the syntax tree directly
                            componentSyntax = SyntaxFactory.ParseCompilationUnit(contentStr).Members[0] as ClassDeclarationSyntax;
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
                        finally
                        {
                            //ms?.Dispose();
                        }

                        // We now have the syntax of the original component: merge members.
                        members = members.AddRange(componentSyntax.Members);
                    }

                    decl = decl.WithMembers(members);

                    // Apply dynamic components
                    for (int i = 0; i < dynamicComponents.Count; i++)
                    {
                        try
                        {
                            decl = dynamicComponents[i].Apply(decl, symbol, token);

                            if (decl == null)
                                throw new NullReferenceException("A component cannot return null.");
                        }
                        catch (Exception e)
                        {
                            throw new DiagnosticException($"Error applying the {dynamicComponents[i]} component.", e, attribute.ApplicationSyntaxReference.ToLocation());
                        }
                    }

                    return decl;
                }
            }
        }
    }
}
