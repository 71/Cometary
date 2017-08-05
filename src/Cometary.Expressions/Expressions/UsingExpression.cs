using System;
using System.Linq.Expressions;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents a <see langword="using"/> expression.
    /// </summary>
	public sealed class UsingExpression : Expression
    {
        /// <inheritdoc />
        public override bool CanReduce => true;

        /// <inheritdoc />
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <inheritdoc />
        public override Type Type => Body.Type;

        /// <summary>
        /// Gets the <see cref="ParameterExpression"/> in which
        /// the disposable object will be stored.
        /// </summary>
        public new ParameterExpression Variable { get; }

        /// <summary>
        /// Gets the <see cref="Expression"/> that is
        /// the object to automatically dispose.
        /// </summary>
        public Expression Disposable { get; }

        /// <summary>
        /// Gets the <see cref="Expression"/> that is the body
        /// of the using statement.
        /// </summary>
        public Expression Body { get; }

		internal UsingExpression(ParameterExpression variable, Expression disposable, Expression body)
		{
			Variable = variable;
			Disposable = disposable;
			Body = body;
		}

        /// <inheritdoc cref="UnaryExpression.Update(Expression)" select="summary"/>
        public UsingExpression Update(ParameterExpression variable, Expression disposable, Expression body)
		{
			if (Variable == variable && Disposable == disposable && Body == body)
				return this;

			return Expressive.Using(variable, disposable, body);
		}

        /// <inheritdoc />
        public override Expression Reduce()
		{
			var end_finally = Label("end_finally");

			return Block (
				new [] { Variable },
                Assign(Variable, Disposable),
                TryFinally(
					Body,
                    Block(
                        Condition(
                            NotEqual(Variable, Constant(null)),
                            Block(
								Call(Convert(Variable, typeof(IDisposable)), Reflection.IDisposable_Dispose),
                                Goto(end_finally)
                            ),
                            Goto(end_finally)
                        ),
                        Label(end_finally)
                    )
                )
            );
		}

        /// <inheritdoc />
        protected override Expression VisitChildren(ExpressionVisitor visitor)
            => Update(visitor.VisitAndConvert(Variable, nameof(VisitChildren)), visitor.Visit(Disposable), visitor.Visit(Body));

        /// <inheritdoc />
        public override string ToString() => $"using ({Disposable}) {{ ... }}";
    }

	partial class Expressive
    {
        /// <summary>
        /// Creates a <see cref="UsingExpression"/> that represents a
        /// <see langword="using"/> statement.
        /// </summary>
		public static UsingExpression Using(Expression disposable, Expression body)
		{
			return Using(null, disposable, body);
		}

        /// <summary>
        /// Creates a <see cref="UsingExpression"/> that represents a
        /// <see langword="using"/> statement.
        /// </summary>
		public static UsingExpression Using(ParameterExpression variable, Expression disposable, Expression body)
		{
			if (disposable == null)
				throw new ArgumentNullException(nameof(disposable));
			if (body == null)
				throw new ArgumentNullException(nameof(body));

		    if (!disposable.IsAssignableTo<IDisposable>())
		        throw Error.ArgumentMustImplement<IDisposable>(nameof(disposable));

			if (variable == null)
				variable = Parameter(disposable.Type);

			return new UsingExpression(variable, disposable, body);
		}
	}
}
