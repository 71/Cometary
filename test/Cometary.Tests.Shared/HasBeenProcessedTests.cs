using System.Reflection;
using Shouldly;
using Xunit;

namespace Cometary.Tests
{
    using Attributes;

    public class HasBeenProcessedTests
    {
        [CTFI]
        public static void CreateTests(TypeInfo type)
        {
            Testing.DefinedTests = type.GetDeclaredMethods(nameof(ShouldHaveBeenProcessed));
            Testing.CreateTests();
        }

        public static void ProcessBody(Quote quote = null)
        {
            quote += "hasBeenProcessed = true";
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
