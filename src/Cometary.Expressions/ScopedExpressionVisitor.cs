using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents an <see cref="System.Linq.Expressions.Expression"/> whose parent is known.
    /// </summary>
    public sealed class ScopedExpression : Expression
    {
        private readonly IScopedExpressionVisitor _visitor;
        private readonly int _depth;

        /// <inheritdoc />
        public override bool CanReduce => true;

        /// <inheritdoc />
        public override ExpressionType NodeType => Expression.NodeType;

        /// <inheritdoc />
        public override Type Type => Expression.Type;

        /// <inheritdoc />
        public override Expression Reduce() => Expression;

        internal ScopedExpression(IScopedExpressionVisitor visitor, int depth)
        {
            _visitor = visitor;
            _depth = depth;
        }

        /// <summary>
        /// Gets whether or not this <see cref="System.Linq.Expressions.Expression"/> is at the
        /// top of its <see cref="System.Linq.Expressions.Expression"/> tree.
        /// </summary>
        public bool IsRoot => _depth == 0;

        /// <summary>
        /// Gets the parent of this <see cref="System.Linq.Expressions.Expression"/>, or <see langword="null"/>
        /// if it is has none.
        /// </summary>
        public ScopedExpression Parent => _depth == 0 ? null : new ScopedExpression(_visitor, _depth - 1);

        /// <summary>
        /// Gets the <see cref="System.Linq.Expressions.Expression"/> wrapped by this <see cref="ScopedExpression"/>.
        /// </summary>
        public Expression Expression => _visitor.Scope[_depth];

        /// <inheritdoc />
        public override string ToString() => Expression.ToString();
    }

    /// <summary>
    /// Represents an <see cref="ExpressionVisitor"/> that keeps track of
    /// its depth in the expression tree.
    /// </summary>
    public abstract class ScopedExpressionVisitor : ExpressionVisitor, IScopedExpressionVisitor
    {
        internal Expression[] _scope;
        internal int _depth;

        ReadOnlyCollection<Expression> IScopedExpressionVisitor.Scope => new ReadOnlyCollection<Expression>(_scope);

        private void EnsureCapacity(int capacity)
        {
            if (_scope.Length != capacity - 1)
                return;

            Expression[] tmp = new Expression[capacity];

            _scope.CopyTo(tmp, 0);
            _scope = tmp;
        }

        /// <summary>
        /// Gets the <see cref="ScopedExpression"/> that was last visited.
        /// </summary>
        public ScopedExpression Top => new ScopedExpression(this, _depth);

        protected ScopedExpressionVisitor()
        {
            _scope = new Expression[0];
        }

        /// <inheritdoc />
        public override Expression Visit(Expression node)
        {
            if (node == null)
                return null;

            EnsureCapacity(_depth + 1);
            _scope[_depth++] = node;

            base.Visit(node);

            node = Visit(new ScopedExpression(this, --_depth));

            _scope[_depth] = null;
            return node;
        }

        /// <summary>
        /// Transforms the given node.
        /// </summary>
        protected abstract Expression Visit(ScopedExpression node);
    }
}
