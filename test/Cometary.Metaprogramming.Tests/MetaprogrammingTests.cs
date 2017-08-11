using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Shouldly;
using Xunit;

namespace Cometary.Tests
{
    /// <summary>
    ///   Class that provides tests related to the Cometary.Metaprogramming library.
    /// </summary>
    [SuppressMessage("ReSharper", "PartialTypeWithSinglePart")]
    public partial class MetaprogrammingTests
    {
        [SuppressMessage("Compiler", "CS0626")] // Method, operator, or accessor is marked external and has no attributes on it
        public extern void DoSomething();

        /// <summary>
        ///   Ensures that extern methods are rewritten.
        /// </summary>
        [Fact]
        public void ShouldAddBodiesToExternMethods()
        {
            Should.Throw<NotImplementedException>(new Action(DoSomething));
        }

        /// <summary>
        ///   Ensures that the custom editor is also executed.
        /// </summary>
        [Fact]
        public void ShouldRunSelfModifyingEditor()
        {
            typeof(MetaprogrammingTests).GetProperty("Answer").GetValue(null).ShouldBe(42);
        }
    }
}
