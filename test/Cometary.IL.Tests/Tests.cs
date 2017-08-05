using System;
using Shouldly;
using Xunit;

namespace Cometary.Tests
{
    /// <summary>
    /// 
    /// </summary>
    public class Tests
    {
        public static object Null()
        {
            IL.Ldnull();
            IL.Ret();

            return new object();
        }

        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void TestInlineIL() => Null().ShouldBeNull();
    }
}
