using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Xunit;

namespace Cometary.Tests
{
    using Extensions;
    using Rewriting;
    using Attributes;

    public class HasBeenProcessedTests
    {
        [CTFE]
        public static void CreateTests(TypeInfo type)
        {
            Testing.DefinedTests = type.GetDeclaredMethods(nameof(ShouldHaveBeenProcessed));
            Testing.CreateTests();
        }

        public static void ProcessBody(Quote quote = null)
        {
            quote.Unquote(
                "hasBeenProcessed = true".Syntax<StatementSyntax>()
            );
        }

        [Fact]
        public void ShouldHaveBeenProcessed()
        {
            bool hasBeenProcessed = false;

            ProcessBody();

            hasBeenProcessed.ShouldBeTrue();
        }
    }
}
