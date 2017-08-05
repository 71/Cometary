using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Shouldly;
using Xunit;

namespace Cometary.Tests
{
    using Expressions;
    using X = Expressions.Expressive;

    public class LINQ
    {
        [Fact]
        public void ShouldCreateValidQuery()
        {
            ParameterExpression item = Expression.Variable(typeof(int), "item");
            ConstantExpression items = Expression.Constant(new[] { 0, 1, 1, 3 });

            X.Query(
                X.From(item, items),
                X.Where(Expression.Equal(item, 1.AsExpression())),
                X.Select(item)
            ).Compile<Func<IEnumerable<int>>>()().SequenceEqual(new[]{1, 1}).ShouldBeTrue();
        }

        [Fact]
        public void ShouldTransformQuery()
        {
            IEnumerable<int> items = Enumerable.Range(0, 100);

            Expression body = X.Express(() => from item in items
                                              where item > 0
                                              let cube = item * item * item
                                              from ch in cube.ToString().ToCharArray()
                                              select $"{cube}{ch}");

            QueryExpression query = X.Query(body);
        }

        [Fact]
        public void ShouldTransformComplexQuery()
        {
            IEnumerable<int> positives = Enumerable.Range(0, 100);
            IEnumerable<int> negatives = Enumerable.Range(-100, 0);

            Expression body = X.Express(() => from p in positives
                                              from n in negatives
                                              let sum = p + n
                                              where sum == 0
                                              let product = p * n
                                              orderby product descending
                                              select sum.ToString());

            QueryExpression query = X.Query(body);
            string[] result = query.Compile<Func<IEnumerable<string>>>()().ToArray();
        }
    }
}
