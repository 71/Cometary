using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents an iterator.
    /// </summary>
    public sealed class IteratorExpression : StateMachineExpression
    {
        private readonly IList<MutableExpression<Expression>> _yields;

        /// <summary>
        /// Gets the type of the item to return.
        /// </summary>
        public Type ItemType { get; }

        /// <summary>
        /// Gets the <see cref="Expression"/> that provides
        /// values to iterate over, using <see cref="YieldExpression"/>.
        /// </summary>
        public Expression Iterator { get; }

        /// <inheritdoc />
        public override Type Type => typeof(IEnumerable<>).MakeGenericType(ItemType);

        internal IteratorExpression(Expression iterator)
        {
            Iterator     = iterator;

            _yields = new LightList<MutableExpression<Expression>>();

            CountYields();

            ItemType = _yields.Count == 0 ? typeof(object) : DeduceItemType();

            SetType(ItemType, typeof(IteratorStateMachine<>).MakeGenericType(ItemType));
        }

        private Type DeduceItemType()
        {
            Type commonType = _yields[0].Type;

            for (int i = 1; i < _yields.Count; i++)
            {
                Type yieldType = _yields[i].Type;

                while (!yieldType.IsAssignableTo(commonType))
                    yieldType = commonType.GetTypeInfo().BaseType;
            }

            return commonType;
        }

        /// <inheritdoc cref="UnaryExpression.Update(Expression)" select="summary"/>
        public IteratorExpression Update(Expression iterator)
        {
            if (Iterator == iterator)
                return this;

            return Expressive.Iterator(iterator);
        }

        /// <inheritdoc />
        protected override LambdaExpression ToLambda() => GenerateLambda(Iterator.ReduceExtensions());

        /// <inheritdoc />
        protected override Expression Project(Expression node)
        {
            // If it yields something, reduce it.
            if (node is YieldExpression yield)
                return Return(yield.Expression);

            return node;
        }

        private void CountYields()
        {
            _yields.Clear();

            Expressive.Pipeline(CountYield).Visit(Iterator);
        }

        private Expression CountYield(Expression node)
        {
            if (node is YieldExpression yield)
                _yields.Add(yield);

            return node;
        }

        /// <inheritdoc />
        protected override Expression VisitChildren(ExpressionVisitor visitor)
            => Update(visitor.Visit(Iterator));

        /// <summary>
        /// Compiles this <see cref="IteratorExpression"/> into an <see cref="IEnumerable{T}"/>.
        /// </summary>
        public new IEnumerable<T> Compile<T>() => base.Compile<IEnumerable<T>>();

        /// <summary>
        /// Compiles this <see cref="IteratorExpression"/> into an <see cref="IEnumerable"/>.
        /// </summary>
        public new IEnumerable Compile() => base.Compile<IEnumerable>();
    }

    partial class Expressive
    {
        /// <summary>
        /// Creates a <see cref="IteratorExpression"/> that represents a
        /// <see cref="LambdaExpression"/> in which <see langword="yield"/> statements
        /// can be used through <see cref="YieldExpression"/>.
        /// </summary>
        public static IteratorExpression Enumerator(Expression iterator)
        {
            return Iterator(iterator);
        }

        /// <inheritdoc cref="Enumerator(Expression)"/>
        public static IteratorExpression Enumerator(params Expression[] expressions)
        {
            return Iterator(Block(expressions));
        }

        /// <summary>
        /// Creates a <see cref="IteratorExpression"/> that represents a
        /// <see cref="LambdaExpression"/> in which <see langword="yield"/> statements
        /// can be used through <see cref="YieldExpression"/>.
        /// </summary>
        public static IteratorExpression Enumerable(Expression iterator)
        {
            return Iterator(iterator);
        }

        /// <inheritdoc cref="Enumerable(Expression)"/>
        public static IteratorExpression Enumerable(params Expression[] expressions)
        {
            return Iterator(Block(expressions));
        }

        /// <summary>
        /// Creates a <see cref="IteratorExpression"/> that represents a
        /// <see cref="LambdaExpression"/> in which <see langword="yield"/> statements
        /// can be used through <see cref="YieldExpression"/>.
        /// </summary>
        public static IteratorExpression Iterator(Expression iterator)
        {
            if (iterator == null)
                throw new ArgumentNullException(nameof(iterator));

            return new IteratorExpression(iterator);
        }

        /// <inheritdoc cref="Iterator(Expression)"/>
        public static IteratorExpression Iterator(params Expression[] expressions)
        {
            return Iterator(Block(expressions));
        }
    }
}
