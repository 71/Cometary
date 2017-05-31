using Shouldly;
using Xunit;

namespace Cometary.Tests
{
    using Extensions;

    public class MixinTests
    {
        [Fact]
        public void ShouldProduceValidExpressions()
        {
            "true".Mixin<bool>().ShouldBeTrue();
            "(1 + 1)".Mixin<int>().ShouldBe(2);
        }

        [Fact]
        public void ShouldProduceValidStatements()
        {
            int x = 0;

            "x++".Mixin();
            "x.ShouldBe(1)".Mixin();
        }
    }
}
