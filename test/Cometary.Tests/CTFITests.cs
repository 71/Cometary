using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;
using Shouldly;

namespace Cometary.Tests
{
    using Attributes;
    using Extensions;

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    public class CTFITests
    {
        private static bool True => false;

        [Invoke]
        private static void ModifyProperty(TypeDeclarationSyntax type)
        {
#pragma warning disable RS1014 // Do not ignore values returned by methods on immutable objects.
            type.GetProperty(nameof(True))
                .Replace(x => x.WithExpressionBody(
                    x.ExpressionBody.WithExpression("true".Syntax<ExpressionSyntax>()))
                );
#pragma warning restore RS1014 // Do not ignore values returned by methods on immutable objects.
        }

        [Fact]
        public void PropertyShouldBeTrue()
        {
            True.ShouldBeTrue();
        }

        [Fact]
        public void CompilingShouldNotBeTrue()
        {
            Meta.Compiling.ShouldBeFalse();
        }
    }
}
