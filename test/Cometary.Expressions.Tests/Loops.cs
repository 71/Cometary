using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using Shouldly;

namespace Cometary.Tests
{
    using Expressions;
    using X = Expressions.Expressive;

    public class Loops
    {
        #region Foreach
        [Fact]
        public void ShouldIterateArray()
        {
            ParameterExpression output = Expression.Parameter(typeof(List<string>), "output");
            ParameterExpression item = Expression.Variable(typeof(string), "item");
            ConstantExpression items = Expression.Constant(new[] { "hello", "world" });

            List<string> addedItems = new List<string>();

            ForEachExpression loop = X.ForEach(item, items,
                Expression.Call(output, nameof(List<string>.Add), null, item)
            );

            Expression
                .Lambda<Action<List<string>>>(loop, output)
                .Compile()(addedItems);

            addedItems.Count.ShouldBe(2);
            addedItems[0].ShouldBe("hello");
            addedItems[1].ShouldBe("world");
        }

        [Fact]
        public void ShouldIterateEnumerable()
        {
            ParameterExpression output = Expression.Parameter(typeof(List<string>), "output");
            ParameterExpression item = Expression.Variable(typeof(string), "item");
            ConstantExpression items = Expression.Constant(Enumerable.Repeat("hello world", 10));

            List<string> addedItems = new List<string>();

            ForEachExpression loop = X.ForEach(item, items,
                Expression.Call(output, nameof(List<string>.Add), null, item)
            );

            Expression
                .Lambda<Action<List<string>>>(loop, output)
                .Compile()(addedItems);

            addedItems.Count.ShouldBe(10);
            addedItems[0].ShouldBe("hello world");
        }
        #endregion

        #region While
        [Fact]
        public void ShouldRespectWhile()
        {
            ParameterExpression nbr = Expression.Variable(typeof(int), nameof(nbr));
            ParameterExpression output = Expression.Parameter(typeof(List<int>), "output");

            List<int> addedItems = new List<int>();

            WhileExpression loop = X.While(
                Expression.LessThan(Expression.PostIncrementAssign(nbr), Expression.Constant(10)),
                Expression.Call(output, nameof(List<int>.Add), null, nbr)
            );

            Expression
                .Lambda<Action<List<int>>>(Expression.Block(new[] { nbr }, loop), output)
                .Compile()(addedItems);

            addedItems.Count.ShouldBe(10);

            int index = 0;
            while (index++ < 10)
                addedItems[index - 1].ShouldBe(index);
        }
        #endregion
    }
}
