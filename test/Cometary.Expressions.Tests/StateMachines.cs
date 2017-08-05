using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Cometary.Tests
{
    using Expressions;
    using X = Expressions.Expressive;

    public class StateMachines
    {
        #region Iterator
        [Fact]
        public void ShouldCreateSimpleIterator()
        {
            IteratorExpression enumerable = X.Enumerable(
                X.Yield("hello".AsExpression()),
                X.Yield("world".AsExpression())
            );

            enumerable
                .Compile<string>()
                .SequenceEqual(new[] { "hello", "world" })
                .ShouldBeTrue();
        }

        [Fact]
        public void ShouldCreateComplexIterator()
        {
            ParameterExpression i = Expression.Parameter(typeof(int), "i");

            IteratorExpression enumerable = X.Enumerable(
                X.While(Expression.LessThan(i, 20.AsExpression()),
                    X.Yield(Expression.PostIncrementAssign(i))
                )
            );

            enumerable
                .Compile<int>()
                .SequenceEqual(Enumerable.Range(0, 20))
                .ShouldBeTrue();
        }
        #endregion

        #region Async
        [Fact]
        public void ShouldReduceAwaitToBlockingCall()
        {
            Task<string> sleepTask = new Task<string>(() =>
            {
                Thread.Sleep(1000);
                return "hey";
            });

            AwaitExpression expression = X.Await(X.Link(sleepTask));

            DateTime before = DateTime.Now;

            Action action = Expression.Lambda<Action>(expression).Compile();

            sleepTask.Start();
            action();

            TimeSpan timeTaken = DateTime.Now - before;

            timeTaken.ShouldBeGreaterThanOrEqualTo(TimeSpan.FromMilliseconds(1000));
        }

        [Fact]
        public async void ShouldCreateSimpleASM()
        {
            Task sleepTask = Task.Delay(1000);

            AsyncExpression async = X.Async(
                X.Await(X.Link(sleepTask))
            );

            DateTime before = DateTime.Now;

            await async.Compile();

            TimeSpan timeTaken = DateTime.Now - before;

            timeTaken.ShouldBeGreaterThanOrEqualTo(TimeSpan.FromMilliseconds(1000));
        }
        #endregion
    }
}
