using System;
using System.Linq.Expressions;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents a <see langword="for"/> expression.
    /// </summary>
	public sealed class ForExpression : Expression, ILoopExpression, IConditionalExpression
    {
        /// <inheritdoc />
        public override bool CanReduce => true;

        /// <inheritdoc />
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <inheritdoc />
        public override Type Type => BreakLabel?.Type ?? typeof(void);

        /// <summary>
        /// Gets the <see cref="ParameterExpression"/> in which
        /// the current index of the for loop is stored.
        /// </summary>
        public new ParameterExpression Variable { get; }

        /// <summary>
        /// Gets the <see cref="Expression"/> that is
        /// the first value of the index <see cref="Variable"/>.
        /// </summary>
        public Expression Initializer { get; }

        /// <summary>
        /// Gets the <see cref="Expression"/> that is
        /// invoked everytime a loop is over.
        /// </summary>
        public Expression Step { get; }

        /// <inheritdoc />
        public Expression Test { get; }

        /// <inheritdoc />
        public Expression Body { get; }

        /// <inheritdoc />
        public LabelTarget BreakLabel { get; }

        /// <inheritdoc />
        public LabelTarget ContinueLabel { get; }

		internal ForExpression(ParameterExpression variable, Expression initializer, Expression test, Expression step, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
		{
			Variable = variable;
			Initializer = initializer;
			Test = test;
			Step = step;
			Body = body;
			BreakLabel = breakTarget;
			ContinueLabel = continueTarget;
		}

        /// <inheritdoc cref="UnaryExpression.Update(Expression)" select="summary"/>
		public ForExpression Update(ParameterExpression variable, Expression initializer, Expression test, Expression step, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
		{
			if (Variable == variable && Initializer == initializer && Test == test && Step == step && Body == body && BreakLabel == breakTarget && ContinueLabel == continueTarget)
				return this;

			return Expressive.For(variable, initializer, test, step, body, breakTarget, continueTarget);
		}

        /// <inheritdoc />
        public override Expression Reduce()
		{
			var inner_loop_break    = Label("inner_loop_break");
			var inner_loop_continue = Label("inner_loop_continue");

			var @continue = ContinueLabel ?? Label("continue");
			var @break    = BreakLabel ?? Label("break");

			return Block(
				new [] { Variable },
                Assign(Variable, Initializer),
                Loop(
                    Block(
                        IfThen(
                            IsFalse(Test),
                            Break(inner_loop_break)),
						Body,
                        Label(@continue),
						Step),
					inner_loop_break,
					inner_loop_continue),
                Label(@break));
		}

        /// <inheritdoc />
        protected override Expression VisitChildren(ExpressionVisitor visitor)
            => Update(visitor.VisitAndConvert(Variable, nameof(VisitChildren)),
                      visitor.Visit(Initializer), visitor.Visit(Test),
                      visitor.Visit(Step), visitor.Visit(Body),
                      BreakLabel, ContinueLabel);

        /// <inheritdoc />
        public override string ToString() => $"for ({Initializer}; {Test}; {Step}) {{ ... }}";
    }

	partial class Expressive
    {
        /// <summary>
        /// Creates a new <see cref="ForExpression"/> that represents a <see langword="for"/> statement.
        /// </summary>
		public static ForExpression For(ParameterExpression variable, Expression initializer, Expression test, Expression step, Expression body)
		{
			return For(variable, initializer, test, step, body, null);
		}

        /// <summary>
        /// Creates a new <see cref="ForExpression"/> that represents a <see langword="for"/> statement.
        /// </summary>
        public static ForExpression For(ParameterExpression variable, Expression initializer, Expression test, Expression step, Expression body, LabelTarget breakTarget)
		{
			return For(variable, initializer, test, step, body, breakTarget, null);
		}

        /// <summary>
        /// Creates a new <see cref="ForExpression"/> that represents a <see langword="for"/> statement.
        /// </summary>
        public static ForExpression For(ParameterExpression variable, Expression initializer, Expression test, Expression step, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
		{
			if (variable == null)
				throw new ArgumentNullException(nameof(variable));
			if (initializer == null)
				throw new ArgumentNullException(nameof(initializer));
			if (test == null)
				throw new ArgumentNullException(nameof(test));
			if (step == null)
				throw new ArgumentNullException(nameof(step));
			if (body == null)
				throw new ArgumentNullException(nameof(body));

			if (!initializer.IsAssignableTo(variable.Type))
				throw new ArgumentException("Initializer must be assignable to variable", nameof(initializer));

		    if (test.Type != typeof(bool))
		        throw Error.ArgumentMustBeBoolean(nameof(test));

		    if (continueTarget != null && continueTarget.Type != typeof(void))
		        throw Error.LabelTypeMustBeVoid(nameof(continueTarget));

			return new ForExpression(variable, initializer, test, step, body, breakTarget, continueTarget);
		}
	}
}
