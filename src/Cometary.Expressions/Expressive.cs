using System;
using System.Linq.Expressions;

namespace Cometary.Expressions
{
    /// <summary>
    /// Provides additional features to the system <see cref="Expression"/> class.
    /// </summary>
    public abstract partial class Expressive : Expression
    {
        protected Expressive()
        {
            throw new InvalidOperationException("The Expressionist cannot be inherited for instances.");
        }

        /// <summary>
        /// Returns the body of the given <see cref="LambdaExpression"/>.
        /// </summary>
        public static Expression Express<TDelegate>(Expression<TDelegate> expression) => expression.Body;

        /// <summary>
        /// Returns the body of the given <see cref="LambdaExpression"/>.
        /// </summary>
        public static Expression Express(Expression<Func<object>> expression) => (expression.Body as UnaryExpression)?.Operand ?? expression.Body;

        /// <summary>
        /// Creates a new <see cref="ExpressionPipeline"/>.
        /// </summary>
        public static ExpressionPipeline Pipeline(params ExpressionProjector[] projectors) => new ExpressionPipeline(projectors);
    }
}
