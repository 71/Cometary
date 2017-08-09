using System;
using Shouldly;
using Xunit;

namespace Cometary.Tests
{
    public class UnitTests
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
            Should.Throw<AssertionException>(() => { Require.NotNull(Null); });
            Should.Throw<AssertionException>(() => { Require.Null(NonNull); });
            Should.Throw<AssertionException>(() => { Require.True(1 == Two); });
            Should.Throw<AssertionException>(() => { Require.False(2 == Two); });

            Require.NotNull(NonNull);
            Require.Null(Null);
            Require.True(1 == One);
            Require.False(2 == One);
        }
    }
}
