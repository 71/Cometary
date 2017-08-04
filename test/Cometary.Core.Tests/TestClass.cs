using Shouldly;
using Xunit;
using Cometary.Tests;

[assembly: AddAnswers]

namespace Cometary.Tests
{
    public class TestClass
    {
        [Fact]
        public void Test()
        {
            Answers.LifeTheUniverseAndEverything.ShouldBe(42);
        }
    }
}
