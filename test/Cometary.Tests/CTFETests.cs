using System.Diagnostics.CodeAnalysis;
using Cometary.Attributes;
using Cometary.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;
using Shouldly;

namespace Cometary.Tests
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    public class CTFETests
    {
        private static bool True => false;

        [CTFE]
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
        public void CtfeShouldNotBeTrue()
        {
            Meta.CTFE.ShouldBeFalse();
        }
    }
}
