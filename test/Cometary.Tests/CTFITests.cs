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

        [CTFI]
        private static void ModifyProperty(TypeDeclarationSyntax type)
        {
            type.GetProperty(nameof(True))
                .Replace(x => x.WithExpressionBody(
                    x.ExpressionBody.WithExpression("true".Syntax<ExpressionSyntax>()))
                );
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
