using System;
using System.Linq.Expressions;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents a <see langword="while"/> expression.
    /// </summary>
	public sealed class WhileExpression : Expression, IConditionalExpression, ILoopExpression
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

        internal WhileExpression(Expression test, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
		{
            Test = test;
		    Body = body;
		    BreakLabel = breakTarget;
		    ContinueLabel = continueTarget;
		}

        /// <inheritdoc cref="UnaryExpression.Update(Expression)" select="summary"/>
		public WhileExpression Update(Expression test, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
		{
            if (test == Test && body == Body && breakTarget == BreakLabel && continueTarget == ContinueLabel)
				return this;

			return Expressive.While(test, body, breakTarget, continueTarget);
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
						Condition(
                            Test,
                            Block(
                                Body,
                                Goto(inner_loop_continue)
                            ),
                            Goto(inner_loop_break)
                        )
                    ),
					inner_loop_break,
					inner_loop_continue),
                Label(@break)
            );
		}

        /// <inheritdoc />
        protected override Expression VisitChildren(ExpressionVisitor visitor)
            => Update(visitor.Visit(Test), visitor.Visit(Body), ContinueLabel, BreakLabel);

        /// <inheritdoc />
        public override string ToString() => $"while ({Test}) {{ ... }}";
    }

	partial class Expressive
    {
        /// <summary>
        /// Creates a new <see cref="WhileExpression"/> that represents a <see langword="while"/> statement.
        /// </summary>
        public static WhileExpression While(Expression test, Expression body)
		{
			return While(test, body, null);
		}

        /// <summary>
        /// Creates a new <see cref="WhileExpression"/> that represents a <see langword="while"/> statement.
        /// </summary>
        public static WhileExpression While(Expression test, Expression body, LabelTarget breakTarget)
		{
			return While(test, body, breakTarget, null);
		}

        /// <summary>
        /// Creates a new <see cref="WhileExpression"/> that represents a <see langword="while"/> statement.
        /// </summary>
        public static WhileExpression While(Expression test, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
		{
			if (test == null)
				throw new ArgumentNullException(nameof(test));
			if (body == null)
				throw new ArgumentNullException(nameof(body));

			if (test.Type != typeof(bool))
				throw Error.ArgumentMustBeBoolean(nameof(test));

		    if (continueTarget != null && continueTarget.Type != typeof(void))
		        throw Error.LabelTypeMustBeVoid(nameof(continueTarget));

			return new WhileExpression(test, body, breakTarget, continueTarget);
		}
	}
}
