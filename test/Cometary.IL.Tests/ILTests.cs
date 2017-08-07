using Shouldly;
using Xunit;

namespace Cometary.Tests
{
    /// <summary>
    ///   Defines tests related to the <see cref="IL"/> class.
    /// </summary>
    public class ILTests
    {
        public static string Null()
        {
            IL.Ldstr(nameof(ILTests));
            IL.Ret();

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void TestInlineIL() => Null().ShouldBe(nameof(ILTests));
    }
}
