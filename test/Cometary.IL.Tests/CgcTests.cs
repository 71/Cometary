using Shouldly;
using Xunit;

[assembly: VirtuousAssembly]

namespace Cometary.Tests
{
    /// <summary>
    ///   Defines tests related to the <see cref="CodeGeneratorContext"/> class.
    /// </summary>
    public class CgcTests
    {
        public int EvilNumber => 666;

        [Fact]
        public void ShouldReplaceAllInstancesOfThe666ConstantValue()
        {
            EvilNumber.ShouldBe(0);
        }
    }
}
