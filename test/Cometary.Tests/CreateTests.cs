using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Cometary.Attributes;
using Cometary.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace Cometary.Tests
{
    public static class Testing
    {
        public static IEnumerable<MethodInfo> DefinedTests = from type in typeof(Testing).GetTypeInfo().Assembly.DefinedTypes
                                                             from method in type.DeclaredMethods
                                                             where method.GetCustomAttribute<FactAttribute>() != null
                                                             select method;

        public static IEnumerable<StatementSyntax> Tests => from test in DefinedTests
                                                            let ccSyntax = "Console.ForegroundColor = ConsoleColor.White".Syntax<StatementSyntax>()
                                                            let msgSyntax = $"Console.WriteLine(\"[+] Running test {test.Name}...\")".Syntax<StatementSyntax>()
                                                            let rcSyntax = "Console.ResetColor()".Syntax<StatementSyntax>()
                                                            let testSyntax = $"new {test.DeclaringType}().{test.Name}()".Syntax<StatementSyntax>()
                                                            from line in new[] { ccSyntax, msgSyntax, rcSyntax, testSyntax }
                                                            select line;

        public static IEnumerable<StatementSyntax> WrappedTests
        {
            get
            {
                yield return "Console.WriteLine()".Syntax<StatementSyntax>();

                foreach (StatementSyntax stmt in Tests)
                    yield return stmt;

                yield return "Console.ForegroundColor = ConsoleColor.Green;".Syntax<StatementSyntax>();
                yield return "Console.WriteLine(\"[+] Tests all successfully passed!\");".Syntax<StatementSyntax>();
            }
        }

        [CTFE(KeepMethod = true)]
        public static void CreateTests()
        {
            IMethodSymbol method = Meta.Compilation
                                       .GetSymbolsWithName(name => name.Equals("Main"), SymbolFilter.Member)
                                       .OfType<IMethodSymbol>()
                                       .FirstOrDefault(x => x.IsStatic);

            if (method == null)
                return;

            MethodDeclarationSyntax methodSyntax = (MethodDeclarationSyntax)method.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();

            if (methodSyntax == null)
                return;

            methodSyntax.Replace(x => x.WithBody(SyntaxFactory.Block(WrappedTests)));

            Meta.LogMessage("Succesfully created test method.");
        }
    }
}
