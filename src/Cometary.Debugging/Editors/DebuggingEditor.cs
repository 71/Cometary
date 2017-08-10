using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using F = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Cometary.Debugging
{
    /// <summary>
    ///   <see cref="CompilationEditor"/> that allows debugging the Cometary process.
    /// </summary>
    internal sealed class DebuggingEditor : CompilationEditor
    {
        private readonly DebugCometaryAttribute Attribute;

        internal DebuggingEditor(DebugCometaryAttribute attribute)
        {
            Attribute = attribute;
        }

        /// <inheritdoc />
        protected override void Initialize(CSharpCompilation oldCompilation, CancellationToken _)
        {
            OptimizationLevel compilationConfiguration = oldCompilation.Options.OptimizationLevel;
            DebugCometaryAttribute attribute = Attribute;

            if (compilationConfiguration == OptimizationLevel.Debug && !attribute.RunInDebug)
                return;
            if (compilationConfiguration == OptimizationLevel.Release && !attribute.RunInRelease)
                return;

            string typeName = attribute.MainClassName ?? DebugCometaryAttribute.DefaultMainClassName;

            if (Assembly.GetEntryAssembly().GetType(typeName) != null)
                return;

            CSharpCompilation EditCompilation(CSharpCompilation compilation, CancellationToken cancellationToken)
            {
                CSharpCompilationOptions options = compilation.Options;
                CSharpCompilationOptions newOptions = options
                    .WithOutputKind(OutputKind.ConsoleApplication)
                    .WithMainTypeName(typeName);

                // - Make the compilation an application, allowing its execution.
                // - Redirect the entry point to the automatically generated class.
                compilation = compilation.WithOptions(newOptions);

                // Create the entry point:
                string errorFile = Path.GetTempFileName();

                CSharpSyntaxTree generatedSyntaxTree = (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(GetSourceText(attribute.DisplayEndOfCompilationMessage, errorFile), cancellationToken: cancellationToken);
                CompilationUnitSyntax generatedRoot = generatedSyntaxTree.GetCompilationUnitRoot(cancellationToken);

                ClassDeclarationSyntax classSyntax = (ClassDeclarationSyntax)generatedRoot.Members.Last();
                ClassDeclarationSyntax originalClassSyntax = classSyntax;

                // Edit the generated syntax's name, if needed.
                if (typeName != DebugCometaryAttribute.DefaultMainClassName)
                {
                    classSyntax = classSyntax.WithIdentifier(F.Identifier(typeName));
                }

                // Change the filename and arguments.
                SyntaxList<MemberDeclarationSyntax> members = classSyntax.Members;

                FieldDeclarationSyntax WithValue(FieldDeclarationSyntax node, string value)
                {
                    VariableDeclaratorSyntax variableSyntax = node.Declaration.Variables[0];
                    LiteralExpressionSyntax valueSyntax = F.LiteralExpression(
                        SyntaxKind.StringLiteralExpression,
                        F.Literal(value)
                    );

                    return node.WithDeclaration(
                        node.Declaration.WithVariables(node.Declaration.Variables.Replace(
                            variableSyntax, variableSyntax.WithInitializer(F.EqualsValueClause(valueSyntax))
                        ))
                    );
                }

                FieldDeclarationSyntax WithBoolean(FieldDeclarationSyntax node, bool value)
                {
                    VariableDeclaratorSyntax variableSyntax = node.Declaration.Variables[0];
                    LiteralExpressionSyntax valueSyntax = F.LiteralExpression(value ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression);

                    return node.WithDeclaration(
                        node.Declaration.WithVariables(node.Declaration.Variables.Replace(
                            variableSyntax, variableSyntax.WithInitializer(F.EqualsValueClause(valueSyntax))
                        ))
                    );
                }

                for (int i = 0; i < members.Count; i++)
                {
                    FieldDeclarationSyntax field = members[i] as FieldDeclarationSyntax;

                    if (field == null)
                        continue;

                    string fieldName = field.Declaration.Variables[0].Identifier.Text;

                    switch (fieldName)
                    {
                        case "References":
                            field = WithValue(field, string.Join(";", compilation.References.OfType<PortableExecutableReference>().Select(x => x.FilePath)));
                            break;
                        case "Files":
                            field = WithValue(field, string.Join(";", compilation.SyntaxTrees.Select(x => x.FilePath)));
                            break;
                        case "AssemblyName":
                            field = WithValue(field, compilation.AssemblyName);
                            break;
                        case "ErrorFile":
                            field = WithValue(field, errorFile);
                            break;
                        case "Written":
                            field = WithBoolean(field, OutputAllTreesAttribute.Instance != null);
                            break;
                        case "BreakAtEnd":
                            field = WithBoolean(field, attribute.DisplayEndOfCompilationMessage);
                            break;
                        case "BreakAtStart":
                            field = WithBoolean(field, attribute.BreakDuringStart);
                            break;
                        default:
                            continue;
                    }

                    members = members.Replace(members[i], field);
                }

                // Return the modified compilation.
                return compilation.AddSyntaxTrees(
                    generatedSyntaxTree
                        .WithCometaryOptions(this)
                        .WithRoot(
                            generatedRoot.WithMembers(generatedRoot.Members.Replace(originalClassSyntax, classSyntax.WithMembers(members))
                        )
                    )
                );
            }

            CompilationPipeline += EditCompilation;
        }

        /// <summary>
        ///   Returns the <see cref="SourceText"/> of the template of the class to generate.
        /// </summary>
        private static SourceText GetSourceText(bool breaking, string errorFile)
        {
            using (Stream templateStream = typeof(DebuggingEditor).GetTypeInfo().Assembly.GetManifestResourceStream("Cometary.Debugging.DebugProgramTemplate.cs"))
            using (TextReader reader = new StreamReader(templateStream, Encoding.UTF8))
            {
                return breaking
                    ? SourceText.From(reader.ReadToEnd().Replace("%ERRORFILE%", errorFile), Encoding.UTF8)
                    : SourceText.From(reader.ReadToEnd(), Encoding.UTF8);
            }
        }
    }
}
