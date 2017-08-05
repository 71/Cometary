using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents an <see langword="await"/> expression, typically found in
    /// an <see cref="AsyncExpression"/>.
    /// </summary>
    /// <remarks>
    /// This expression can be lowered to an actual await expression,
    /// as if the C# compiler had generated it; or to a simpler
    /// <c>value.GetAwaiter().Result</c> expression.
    /// </remarks>
    public sealed class AwaitExpression : Expression
    {
        /// <inheritdoc />
        public override bool CanReduce => true;

        /// <inheritdoc />
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <inheritdoc />
        public override Type Type { get; }

        /// <summary>
        /// Gets the <see cref="System.Linq.Expressions.Expression"/> that is the task to await.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Gets the method used to get the expression's <see cref="TaskAwaiter"/>.
        /// </summary>
        public MethodInfo Method { get; }

        internal AwaitExpression(Expression value, MethodInfo awaiterGetter)
        {
            Expression = value;
            Type = value.Type.GenericTypeArguments.FirstOrDefault() ?? typeof(void);
            Method = awaiterGetter;
        }

        /// <inheritdoc cref="UnaryExpression.Update(System.Linq.Expressions.Expression)" select="summary"/>
        public AwaitExpression Update(Expression task, MethodInfo method)
        {
            if (Expression == task && Equals(Method, method))
                return this;

            return Expressive.Await(task, method);
        }

        /// <inheritdoc />
        public override Expression Reduce()
        {
            // Reduce this expression to a simple "task.Result" call.
            MethodCallExpression getAwaiterCall = Method.IsStatic ? Call(Method, Expression) : Call(Expression, Method);

            return Call(getAwaiterCall, nameof(TaskAwaiter.GetResult), null);
        }

        /// <inheritdoc />
        protected override Expression VisitChildren(ExpressionVisitor visitor)
            => Update(visitor.Visit(Expression), Method);

        /// <inheritdoc />
        public override string ToString() => $"await {Expression}";
    }

    partial class Expressive
    {
        /// <summary>
        /// Creates an <see cref="AwaitExpression"/> that represents
        /// an <see langword="await"/> expression.
        /// </summary>
        public static AwaitExpression Await(Expression task)
        {
            Requires.NotNull(task, nameof(task));

            MethodInfo method = task.Type.GetRuntimeMethods()
                                    .FirstOrDefault(x => x.ReturnType == typeof(TaskAwaiter) ||
                                                         x.ReturnType.IsConstructedGenericType &&
                                                         x.ReturnType.GetGenericTypeDefinition() == typeof(TaskAwaiter<>));

            if (method == null)
                throw new ArgumentException("The given argument must be awaitable.", nameof(task));

            return new AwaitExpression(task, method);
        }

        /// <summary>
        /// Creates an <see cref="AwaitExpression"/> that represents
        /// an <see langword="await"/> expression.
        /// </summary>
        public static AwaitExpression Await(Expression task, MethodInfo method)
        {
            Requires.NotNull(task, nameof(task));

            if (method == null)
                return Await(task);

            if (method.ReturnType != typeof(TaskAwaiter)
                && method.ReturnType.GetGenericTypeDefinition() != typeof(TaskAwaiter<>))
                throw new ArgumentException("Method does not return a TaskAwaiter.", nameof(method));

            ParameterInfo[] parameters = method.GetParameters();

            if (method.IsStatic)
            {
                if (parameters.Length != 1 || parameters[0].ParameterType.GetTypeInfo().IsAssignableFrom(task.Type.GetTypeInfo()))
                    throw new ArgumentException("Invalid method signature.", nameof(method));
            }
            else
            {
                if (parameters.Length != 0)
                    throw new ArgumentException("Invalid method signature.", nameof(method));
            }

            return new AwaitExpression(task, method);
        }
    }
}
