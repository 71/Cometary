using Cometary;
using Shouldly;
using Xunit;
using Cometary.Tests;

[assembly: AddAnswers]
[assembly: Define("HELLO")]

namespace Cometary.Tests
{
    public class TestClass
    {
        /// <summary>
        ///   Ensures that the <see cref="Answers"/> class has been added to
        ///   the assembly.
        /// </summary>
        [Fact]
        public void TestLibrary()
        {
            // Note: Compiling this without the analyzer reports an error,
            // but with the analyzer, this compiles fine.
            Answers.LifeTheUniverseAndEverything.ShouldBe(42);
        }

        /// <summary>
        ///   Ensures that the <see cref="DefineAttribute"/> does define constants.
        /// </summary>
        [Fact]
        public void TestConstants()
        {
#if !HELLO
            throw new ShouldAssertException("Shouldn't have been compiled.");
#endif
        }
    }
}
