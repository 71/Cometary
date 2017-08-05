using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Shouldly;
using Xunit;

namespace Cometary.Tests
{
    /// <summary>
    ///   Class that provides tests related to the Cometary.Metaprogramming library.
    /// </summary>
    [SuppressMessage("ReSharper", "PartialTypeWithSinglePart")]
    public partial class Tests
    {
        [SuppressMessage("Compiler", "CS0626")] // Method, operator, or accessor is marked external and has no attributes on it
        public extern void DoSomething();

        /// <summary>
        ///   Ensures that extern methods are rewritten.
        /// </summary>
        [Fact]
        public void TestExterns()
        {
            Should.Throw<NotImplementedException>(() => DoSomething());
        }

        /// <summary>
        ///   Ensures that the custom editor is also executed.
        /// </summary>
        [Fact]
        public void TestExecution()
        {
            typeof(Tests).GetProperty("Answer").GetValue(null).ShouldBe(42);
        }

        /// <summary>
        ///   Ensures that the <see cref="InvokeAttribute"/> does lead to the execution of the marked method.
        /// </summary>
        [Invoke]
        public static void TestInvocation(IMethodSymbol currentMethod)
        {
            // When we can normally edit the assembly, do it instead.
            // For now, let's do some arbitrary stuff.
            File.WriteAllText("D:\\DidRun.txt", "It ran.");
        }
    }
}
