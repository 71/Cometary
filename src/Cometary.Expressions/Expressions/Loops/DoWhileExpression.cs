using System;
using System.Linq.Expressions;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents a <see langword="do"/> ... <see langword="while"/> expression.
    /// </summary>
	public sealed class DoWhileExpression : Expression, ILoopExpression, IConditionalExpression
    {
        /// <inheritdoc />
        public override bool CanReduce => true;

        /// <inheritdoc />
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <inheritdoc />
        public override Type Type => BreakLabel?.Type ?? typeof(void);

        /// <inheritdoc />
        public Expression Test { get; }

        /// <inheritdoc />
        public Expression Body { get; }

        /// <inheritdoc />
        public LabelTarget BreakLabel { get; }

        /// <inheritdoc />
        public LabelTarget ContinueLabel { get; }

        internal DoWhileExpression(Expression test, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
		{
			Test = test;
			Body = body;
			BreakLabel = breakTarget;
			ContinueLabel = continueTarget;
		}

        /// <inheritdoc cref="UnaryExpression.Update(Expression)" select="summary"/>
        public DoWhileExpression Update(Expression test, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
		{
            if (Test == test && Body == body && BreakLabel == breakTarget && ContinueLabel == continueTarget)
                return this;

			return Expressive.DoWhile(test, body, breakTarget, continueTarget);
		}

        /// <inheritdoc />
        public override Expression Reduce()
		{
			var inner_loop_break    = Label("inner_loop_break");
			var inner_loop_continue = Label("inner_loop_continue");

			var @continue = ContinueLabel ?? Label("continue");
			var @break    = BreakLabel ?? Label("break");

			return Block(
                Loop(
                    Block(
                        Label(@continue),
						Body,
						Condition(
                            Test,
                            Goto(inner_loop_continue),
                            Goto(inner_loop_break)
                        )
                    ),
					inner_loop_break,
					inner_loop_continue
                ),
                Label(@break)
           );
		}

        /// <inheritdoc />
        protected override Expression VisitChildren(ExpressionVisitor visitor)
            => Update(visitor.Visit(Test), visitor.Visit(Body), ContinueLabel, BreakLabel);

        /// <inheritdoc />
        public override string ToString() => $"do {{ ... }} while ({Test})";
    }

	partial class Expressive
    {
        /// <summary>
        /// Creates a <see cref="DoWhileExpression"/> that represents a
        /// <see langword="do"/> ... <see langword="while"/> statement.
        /// </summary>
		public static DoWhileExpression DoWhile(Expression test, Expression body)
		{
			return DoWhile(test, body, null);
		}

        /// <summary>
        /// Creates a <see cref="DoWhileExpression"/> that represents a
        /// <see langword="do"/> ... <see langword="while"/> statement.
        /// </summary>
		public static DoWhileExpression DoWhile(Expression test, Expression body, LabelTarget breakTarget)
		{
			return DoWhile(test, body, breakTarget, null);
		}

        /// <summary>
        /// Creates a <see cref="DoWhileExpression"/> that represents a
        /// <see langword="do"/> ... <see langword="while"/> statement.
        /// </summary>
		public static DoWhileExpression DoWhile(Expression test, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
		{
			if (test == null)
				throw new ArgumentNullException(nameof(test));
			if (body == null)
				throw new ArgumentNullException(nameof(body));

		    if (test.Type != typeof(bool))
		        throw Error.ArgumentMustBeBoolean(nameof(test));

		    if (continueTarget != null && continueTarget.Type != typeof(void))
		        throw Error.LabelTypeMustBeVoid(nameof(continueTarget));

			return new DoWhileExpression(test, body, breakTarget, continueTarget);
		}
	}
}
