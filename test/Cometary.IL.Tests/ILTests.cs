using System;
using Shouldly;
using Xunit;

namespace Cometary.Tests
{
    /// <summary>
    ///   Defines tests related to the <see cref="IL"/> class.
    /// </summary>
    public class ILTests
    {
        public static string ClassName()
        {
            IL.Ldtoken(typeof(ILTests));
            IL.Call(Type.GetTypeFromHandle(default(RuntimeTypeHandle)));
            IL.Callvirt(typeof(object).Name);
            IL.Ret();

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void ShouldPrintCustomIL() => ClassName().ShouldBe(nameof(ILTests));
    }
}
