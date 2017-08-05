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

    public class OtherExpressions
    {
        #region Using
        private class TrackingDisposable : IDisposable
        {
            public bool HasBeenDisposed { get; private set; }

            public void Dispose() => HasBeenDisposed = true;
        }

        [Fact]
        public void ShouldCallDispose()
        {
            TrackingDisposable disposable = new TrackingDisposable();
            ConstantExpression disposableEx = Expression.Constant(disposable);

            disposable.HasBeenDisposed.ShouldBeFalse();

            Expression
                .Lambda<Action>(X.Using(disposableEx, Expression.Empty()))
                .Compile()();

            disposable.HasBeenDisposed.ShouldBeTrue();
        }

        [Fact]
        public void ShouldCallDisposeAfterException()
        {
            TrackingDisposable disposable = new TrackingDisposable();
            ConstantExpression disposableEx = Expression.Constant(disposable);

            ConstantExpression exceptionEx = Expression.Constant(new Exception());

            disposable.HasBeenDisposed.ShouldBeFalse();

            Should.Throw<Exception>(() => Expression
                .Lambda<Action>(X.Using(disposableEx, Expression.Throw(exceptionEx)))
                .Compile()());

            disposable.HasBeenDisposed.ShouldBeTrue();
        }
        #endregion

        #region TypeOf
        [Fact]
        public void ShouldReturnType()
        {
            Expression
                .Lambda<Func<Type>>(X.TypeOf<string>())
                .Compile()()
                .ShouldBe(typeof(string));

            Expression
                .Lambda<Func<Type>>(X.TypeOf(typeof(void)))
                .Compile()()
                .ShouldBe(typeof(void));
        }
        #endregion

        #region Lock
        [Fact]
        public void ShouldLockExecution()
        {
            object syncRoot = new object();
            Expression syncRootEx = syncRoot.AsExpression();

            Expression sleep = X.Express<Action>(() => Thread.Sleep(2000));

            Action compiledAction = Expression
                .Lambda<Action>(X.Lock(syncRootEx, sleep))
                .Compile();

            Task task = Task.Run(compiledAction);

            // Sleep to make sure the task has the time to start running
            while (task.Status != TaskStatus.Running)
                Thread.Sleep(50);

            DateTime beforeEntering = DateTime.Now;

            lock (syncRoot)
                DateTime.Now.ShouldBeGreaterThan(beforeEntering.AddMilliseconds(1000));
        }
        #endregion

        #region Tuple
        [Fact]
        public void ShouldCheckTupleArguments()
        {
            Should.Throw<ArgumentOutOfRangeException>(() => X.Tuple());

            Should.Throw<InvalidOperationException>(() => X.Tuple(Enumerable.Repeat(Expression.Variable(typeof(string)), 10)));
            Should.NotThrow(() => X.Tuple(Enumerable.Repeat(Expression.Variable(typeof(string)), 7)));

            Should.Throw<ArgumentException>(() => X.Tuple(null));
        }
        #endregion

        #region Linked
        [Fact]
        public void LinkShouldKeepVariableInSync()
        {
            int value = 0;
            LinkedExpression<int> link = X.Link(() => value);

            Func<int> increment = Expression
                .PreIncrementAssign(link.Reduce())
                .Compile<Func<int>>();

            increment().ShouldBe(1);
            link.Value.ShouldBe(1);
            value.ShouldBe(1);

            increment().ShouldBe(2);
            link.Value.ShouldBe(2);
            value.ShouldBe(2);
        }

        [Fact]
        public void LinkShouldKeepValueInSync()
        {
            int value = 0;
            LinkedExpression<int> link = X.Link(value);

            Func<int> increment = Expression
                .PreIncrementAssign(link.Reduce())
                .Compile<Func<int>>();

            increment().ShouldBe(1);
            link.Value.ShouldBe(1);
            value.ShouldBe(0);

            increment().ShouldBe(2);
            link.Value.ShouldBe(2);
            value.ShouldBe(0);
        }
        #endregion
    }
}
