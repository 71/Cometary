using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents a <see langword="foreach"/> expression.
    /// </summary>
	public sealed class ForEachExpression : Expression, ILoopExpression
    {
        /// <inheritdoc />
        public override bool CanReduce => true;

        /// <inheritdoc />
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <inheritdoc />
        public override Type Type => BreakLabel?.Type ?? typeof(void);

        /// <summary>
        /// Gets the <see cref="ParameterExpression"/> in which
        /// the current item will be stored.
        /// </summary>
        public new ParameterExpression Variable { get; }

        /// <summary>
        /// Gets the <see cref="Expression"/> that is
        /// the enumerable to iterate over.
        /// </summary>
        public Expression Enumerable { get; }

        /// <inheritdoc />
        public Expression Body { get; }

        /// <inheritdoc />
        public LabelTarget BreakLabel { get; }

        /// <inheritdoc />
        public LabelTarget ContinueLabel { get; }

		internal ForEachExpression (ParameterExpression variable, Expression enumerable, Expression body, LabelTarget break_target, LabelTarget continue_target)
		{
			Variable = variable;
			Enumerable = enumerable;
			Body = body;
			BreakLabel = break_target;
			ContinueLabel = continue_target;
		}

        /// <inheritdoc cref="UnaryExpression.Update(Expression)" select="summary"/>
		public ForEachExpression Update(ParameterExpression variable, Expression enumerable, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
		{
			if (this.Variable == variable && this.Enumerable == enumerable && this.Body == body && BreakLabel == breakTarget && ContinueLabel == continueTarget)
				return this;

			return Expressive.ForEach(variable, enumerable, body, breakTarget, continueTarget);
		}

        /// <inheritdoc />
        public override Expression Reduce ()
        {
            return Enumerable.Type.IsArray
                ? ReduceForArray()
                : ReduceForEnumerable();
        }

		private Expression ReduceForArray()
		{
			LabelTarget inner_loop_break    = Label("inner_loop_break");
			LabelTarget inner_loop_continue = Label("inner_loop_continue");

			LabelTarget @continue = ContinueLabel ?? Label("continue");
			LabelTarget @break    = BreakLabel ?? Label("break");

			ParameterExpression index = Variable(typeof(int), "i");

			return Block(
				new [] { index, Variable },
                Assign(index, Constant(0)),
                Loop(
                    Block(
                        IfThen(
                            IsFalse(LessThan(index, ArrayLength(Enumerable))),
                            Break(inner_loop_break)
                        ),
                        Assign(Variable, Convert(ArrayIndex(Enumerable, index), Variable.Type)),
                        Body,
                        Label(@continue),
                        PreIncrementAssign(index)
                    ),
					inner_loop_break,
					inner_loop_continue
                ),
                Label(@break)
            );
		}

		private Expression ReduceForEnumerable()
		{
			MethodInfo get_enumerator;
			MethodInfo get_current;

		    ResolveEnumerationMembers(out get_enumerator, out get_current);

			Type enumerator_type = get_enumerator.ReturnType;

			ParameterExpression enumerator = Variable(enumerator_type);

			LabelTarget inner_loop_continue = Label("inner_loop_continue");
			LabelTarget inner_loop_break    = Label("inner_loop_break");

			LabelTarget @continue = ContinueLabel ?? Label("continue");
			LabelTarget @break    = BreakLabel ?? Label("break");

			Expression variable_initializer = Property(enumerator, get_current);

		    if (!Variable.Type.GetTypeInfo().IsAssignableFrom(get_current.ReturnType.GetTypeInfo()))
				variable_initializer = Convert(variable_initializer, Variable.Type);

			Expression loop = Block(
				new [] { Variable },
                Goto(@continue),
                Loop(
                    Block(
                        Assign(Variable, variable_initializer),
                        Body,
                        Label(@continue),
                        Condition(
                            Call(enumerator, Reflection.IEnumerator_MoveNext),
                            Goto(inner_loop_continue),
                            Goto(inner_loop_break)
                        )
                    ),
					inner_loop_break,
					inner_loop_continue
                ),
                Label(@break)
            );

			Expression dispose = CreateDisposeOperation(enumerator_type.GetTypeInfo(), enumerator);

			return Block(
				new [] { enumerator },
                Assign(enumerator, Call(Enumerable, get_enumerator)),
				dispose != null ? TryFinally(loop, dispose) : loop
            );
		}

		private void ResolveEnumerationMembers (
			out MethodInfo get_enumerator,
			out MethodInfo get_current)
		{
			Type item_type;
			Type enumerable_type;
			Type enumerator_type;

			if (TryGetGenericEnumerableArgument(out item_type))
            {
				enumerable_type = typeof(IEnumerable<>).MakeGenericType(item_type);
				enumerator_type = typeof(IEnumerator<>).MakeGenericType(item_type);
			}
            else
            {
				enumerable_type = typeof(IEnumerable);
				enumerator_type = typeof(IEnumerator);
			}

			get_current = enumerator_type.GetRuntimeProperty(nameof(IEnumerator<object>.Current)).GetMethod;
			get_enumerator = Enumerable.Type.GetRuntimeMethod("GetEnumerator", Type.EmptyTypes);

			if (get_enumerator == null || !enumerator_type.GetTypeInfo().IsAssignableFrom(get_enumerator.ReturnType.GetTypeInfo()))
                get_enumerator = enumerable_type.GetRuntimeMethod("GetEnumerator", Type.EmptyTypes);
		}

		private static Expression CreateDisposeOperation (TypeInfo enumerator_type, ParameterExpression enumerator)
		{
		    if (Reflection.IDisposable.IsAssignableFrom(enumerator_type))
		        return Call(enumerator, Reflection.IDisposable_Dispose);

		    if (enumerator_type.IsValueType)
                return null;

			ParameterExpression disposable = Variable(typeof(IDisposable));

			return Block(
				new [] { disposable },
                Assign(disposable, TypeAs(enumerator, typeof(IDisposable))),
                IfThen(ReferenceNotEqual(disposable, Constant(null)),
                    Call(disposable, Reflection.IDisposable_Dispose)
                )
            );
		}

		private bool TryGetGenericEnumerableArgument (out Type argument)
		{
			argument = null;

			foreach (var iface in Enumerable.Type.GetTypeInfo().ImplementedInterfaces)
            {
				if (iface.GenericTypeArguments.Length == 0)
					continue;

				var definition = iface.GetGenericTypeDefinition();
				if (definition != typeof(IEnumerable<>))
					continue;

				argument = iface.GenericTypeArguments[0];

				if (Variable.Type.GetTypeInfo().IsAssignableFrom(argument.GetTypeInfo()))
					return true;
			}

			return false;
		}

        /// <inheritdoc />
        protected override Expression VisitChildren(ExpressionVisitor visitor)
            => Update(visitor.VisitAndConvert(Variable, nameof(ForEachExpression.VisitChildren)),
                      visitor.Visit(Enumerable), visitor.Visit(Body),
                      BreakLabel, ContinueLabel);

        /// <inheritdoc />
        public override string ToString() => $"foreach ({Variable.Type} in {Enumerable}) {{ ... }}";
    }

	partial class Expressive
    {
        /// <summary>
        /// Creates a new <see cref="ForEachExpression"/> that represents a <see langword="foreach"/> statement.
        /// </summary>
		public static ForEachExpression ForEach(ParameterExpression variable, Expression enumerable, Expression body)
		{
			return ForEach(variable, enumerable, body, null);
		}

        /// <summary>
        /// Creates a new <see cref="ForEachExpression"/> that represents a <see langword="foreach"/> statement.
        /// </summary>
		public static ForEachExpression ForEach(ParameterExpression variable, Expression enumerable, Expression body, LabelTarget breakTarget)
		{
			return ForEach(variable, enumerable, body, breakTarget, null);
		}

        /// <summary>
        /// Creates a new <see cref="ForEachExpression"/> that represents a <see langword="foreach"/> statement.
        /// </summary>
        public static ForEachExpression ForEach(ParameterExpression variable, Expression enumerable, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
		{
			if (variable == null)
				throw new ArgumentNullException(nameof(variable));
			if (enumerable == null)
				throw new ArgumentNullException(nameof(enumerable));
			if (body == null)
				throw new ArgumentNullException(nameof(body));

		    if (!enumerable.IsAssignableTo<IEnumerable>())
		        throw Error.ArgumentMustImplement<IEnumerable>(nameof(enumerable));

		    if (continueTarget != null && continueTarget.Type != typeof(void))
		        throw Error.LabelTypeMustBeVoid(nameof(continueTarget));

			return new ForEachExpression(variable, enumerable, body, breakTarget, continueTarget);
		}
	}
}
