using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents an <see cref="IAsyncStateMachine"/> that advances
    /// using a <see langword="delegate"/>.
    /// </summary>
    internal sealed class AsyncStateMachine : VirtualStateMachine<AsyncStateMachine>, IAsyncStateMachine
    {
        private AsyncTaskMethodBuilder builder;

        /// <summary>
        /// Gets the task associated with this state machine.
        /// </summary>
        public Task Task { get; }

        private AsyncStateMachine()
        {
            builder = AsyncTaskMethodBuilder.Create();

            AsyncStateMachine asm = this;
            builder.Start(ref asm);

            Task = asm.builder.Task;
        }

        /// <inheritdoc />
        public void MoveNext()
        {
            try
            {
                Next();

                if (IsCompleted)
                    builder.SetResult();
            }
            catch (Exception e)
            {
                builder.SetException(e);
            }
        }

        /// <inheritdoc />
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            builder.SetStateMachine(stateMachine);
        }
    }

    /// <summary>
    /// Represents an <see cref="IAsyncStateMachine"/> that advances
    /// using a <see langword="delegate"/>.
    /// </summary>
    /// <typeparam name="T">The type of the item to return.</typeparam>
    internal sealed class AsyncStateMachine<T> : VirtualStateMachine<AsyncStateMachine<T>>, IAsyncStateMachine
    {
        private AsyncTaskMethodBuilder<T> builder;

        /// <summary>
        /// Gets the task associated with this state machine.
        /// </summary>
        public Task<T> Task { get; }

        private AsyncStateMachine()
        {
            builder = AsyncTaskMethodBuilder<T>.Create();

            AsyncStateMachine<T> asm = this;
            builder.Start(ref asm);

            Task = asm.builder.Task;
        }

        /// <inheritdoc />
        public void MoveNext()
        {
            try
            {
                T result = Next<T>();

                if (IsCompleted)
                    builder.SetResult(result);
            }
            catch (Exception e)
            {
                builder.SetException(e);
            }
        }

        /// <inheritdoc />
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            builder.SetStateMachine(stateMachine);
        }
    }
}
