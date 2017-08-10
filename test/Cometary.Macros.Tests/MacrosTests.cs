using System;
using Shouldly;
using Xunit;

namespace Cometary.Tests
{
    using Macros;

    public class MacrosTests
    {
        // Gotta use properties & comparisons because the C# compiler automatically
        // removes or inserts statements if it can compute every expression easily.

        public object Null => null;
        public object NonNull => new object();

        public int One => 1;
        public int Two => 2;

        [Fact]
        public void TestRequireClass()
        {
            void TestMethod(int number, string str, object obj)
            {
                Require.NoArgumentNull();
            }

            Should.Throw<AssertionException>(() => { Require.NotNull(Null); });
            Should.Throw<AssertionException>(() => { Require.Null(NonNull); });
            Should.Throw<AssertionException>(() => { Require.True(1 == Two); });
            Should.Throw<AssertionException>(() => { Require.False(2 == Two); });

            Should.Throw<ArgumentNullException>(() => TestMethod(0, "hello", null));
            Should.Throw<ArgumentNullException>(() => TestMethod(0, null, new object()));

            Should.NotThrow(() => TestMethod(0, "hello", NonNull));

            Require.NotNull(NonNull);
            Require.Null(Null);
            Require.True(1 == One);
            Require.False(2 == One);
        }

        [Fact]
        public void TestMixins()
        {
            int i = 0;

            "i++".Mixin();

            i.ShouldBe(1);
        }

        [Fact]
        public void TestLateBoundObjects()
        {
            Literal.One.ShouldBe(1);
            Literal.Twelve.ShouldBe(12);
        }
    }
}
