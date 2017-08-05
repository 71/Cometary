using System.Linq.Expressions;
using Xunit;

namespace Cometary.Tests
{
    using Expressions;
    using X = Expressions.Expressive;

    public class ScopedVisitor
    {
        // TODO: Make automatic unit tests, instead of debugging everything manually by setting breakpoints.

        public class Visitor : ScopedExpressionVisitor
        {
            protected override Expression Visit(ScopedExpression node) => node.Expression;
        }

        [Fact]
        public void ShouldKeepScope()
        {
            Expression body = X.Express(() => $"{1} + {3} is {4}");

            new Visitor().Visit(body);
        }
    }
}
