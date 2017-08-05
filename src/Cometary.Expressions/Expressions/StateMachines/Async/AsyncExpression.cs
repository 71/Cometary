using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents an <see langword="async"/> expression.
    /// </summary>
    public sealed class AsyncExpression : StateMachineExpression, IBodiedExpression
    {
        /// <inheritdoc />
        public Expression Body { get; }

        /// <inheritdoc />
        public override Type Type => LambdaType;

        internal AsyncExpression(Expression body)
        {
            Body = body;

            bool returnsVoid = body.Type == typeof(void);

            SetType(
                returnsVoid ? typeof(Task) : typeof(Task<>).MakeGenericType(body.Type), 
                returnsVoid ? typeof(AsyncStateMachine) : typeof(AsyncStateMachine<>).MakeGenericType(body.Type));
        }

        /// <inheritdoc cref="UnaryExpression.Update(Expression)" select="summary"/>
        public AsyncExpression Update(Expression body)
        {
            if (Body == body)
                return this;

            return Expressive.Async(body);
        }

        /// <inheritdoc />
        protected override Expression Project(Expression node)
        {
            if (node is AwaitExpression ae)
                node = Return(ae);

            return node;
        }

        /// <inheritdoc />
        protected override LambdaExpression ToLambda() => GenerateLambda(Body);

        /// <inheritdoc />
        protected override Expression VisitChildren(ExpressionVisitor visitor)
            => Update(visitor.Visit(Body));

        /// <inheritdoc />
        public override string ToString() => $"async {Body}";


        /// <summary>
        /// Compiles this <see cref="AsyncExpression"/> into a <see cref="Task{TResult}"/>.
        /// </summary>
        public new Task<T> Compile<T>() => ((AsyncStateMachine<T>)base.Compile()).Task;

        /// <summary>
        /// Compiles this <see cref="AsyncExpression"/> into a <see cref="Task"/>.
        /// </summary>
        public new Task Compile() => ((AsyncStateMachine)base.Compile()).Task;
    }

    partial class Expressive
    {
        /// <summary>
        /// Creates an <see cref="AsyncExpression"/> that represents
        /// a <see langword="async"/> lambda.
        /// </summary>
        public static AsyncExpression Async(Expression body)
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            return new AsyncExpression(body);
        }

        /// <summary>
        /// Creates an <see cref="AsyncExpression"/> that represents
        /// a <see langword="async"/> lambda.
        /// </summary>
        public static AsyncExpression Async(params Expression[] body)
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            return new AsyncExpression(Block(body));
        }
    }
}
