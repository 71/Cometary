using Shouldly;
using Xunit;
using Cometary.Tests;

[assembly: AddAnswers]

namespace Cometary.Tests
{
    public class TestClass
    {
        /// <summary>
        ///   Ensures that the <see cref="Answers"/> class has been added to
        ///   the assembly.
        /// </summary>
        [Fact]
        public void Test()
        {
            // Note: Compiling this without the analyzer reports an error,
            // but with the analyzer, this compiles fine.
            Answers.LifeTheUniverseAndEverything.ShouldBe(42);
        }
    }
}
