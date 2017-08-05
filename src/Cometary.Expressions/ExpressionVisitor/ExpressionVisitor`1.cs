using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Cometary.Expressions
{
    internal interface IScopedExpressionVisitor
    {
        /// <summary>
        /// Gets the collection representing the scope of this visitor.
        /// <para>
        /// The larger the index, the higher the <see cref="Expression"/> in its tree.
        /// </para>
        /// </summary>
        ReadOnlyCollection<Expression> Scope { get; }
    }

    /// <summary>
    /// Represents a visitor that transforms an expression tree
    /// to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object output by this visitor.</typeparam>
    public abstract partial class ExpressionVisitor<T> : IScopedExpressionVisitor
    {
        private Expression[] _scope = new Expression[0];
        private Func<Expression, T> dynamicVisit;
        private int _depth;

        private void EnsureCapacity(int capacity)
        {
            if (_scope.Length != capacity - 1)
                return;

            Expression[] tmp = new Expression[capacity];

            _scope.CopyTo(tmp, 0);
            _scope = tmp;
        }

        /// <summary>
        /// Gets the <see cref="ScopedExpression"/> at the bottom of the scope.
        /// </summary>
        public ScopedExpression Bottom => new ScopedExpression(this, _depth - 1);

        /// <summary>
        /// Gets the <see cref="ScopedExpression"/> at the top of the scope.
        /// </summary>
        public ScopedExpression Root => new ScopedExpression(this, 0);

        /// <inheritdoc />
        public ReadOnlyCollection<Expression> Scope => new ReadOnlyCollection<Expression>(_scope);

        /// <summary>
        /// Transforms an <see cref="Expression"/> into an object of type <see cref="T"/>.
        /// </summary>
        public virtual T Visit(Expression node)
        {
            if (node == null)
                return default(T);

            EnsureCapacity(_depth + 1);
            _scope[_depth++] = node;

            T result = PrivateVisit(node);

            _scope[--_depth] = null;
            return result;
        }

        /// <summary>
        /// Attempts to visit an <see cref="Expression"/> even
        /// if <see cref="Visit(Expression)"/> didn't find a match.
        /// <para>
        /// This method checks if the node can be reduced,
        /// or if it is a known manually reducible node.
        /// </para>
        /// </summary>
        protected bool TryVisitUnknown(Expression node, out T result)
        {
            // If reducible, reduce it and visit the reduced node
            if (node.CanReduce)
            {
                result = Visit(node.Reduce());
                return true;
            }

            // If known node, individually modify them
            // ReSharper disable PossibleNullReferenceException
            switch (node.NodeType)
            {
                case ExpressionType.AddChecked:
                    {
                        BinaryExpression binary = node as BinaryExpression;
                        result = Visit(Expression.Add(binary.Left, binary.Right));
                        return true;
                    }
                case ExpressionType.SubtractChecked:
                    {
                        BinaryExpression binary = node as BinaryExpression;
                        result = Visit(Expression.Subtract(binary.Left, binary.Right));
                        return true;
                    }
                case ExpressionType.MultiplyChecked:
                    {
                        BinaryExpression binary = node as BinaryExpression;
                        result = Visit(Expression.Multiply(binary.Left, binary.Right));
                        return true;
                    }
                case ExpressionType.ConvertChecked:
                    {
                        UnaryExpression unary = node as UnaryExpression;
                        result = Visit(Expression.Convert(unary.Operand, unary.Type));
                        return true;
                    }
                case ExpressionType.NegateChecked:
                    {
                        UnaryExpression unary = node as UnaryExpression;
                        result = Visit(Expression.Negate(unary.Operand, unary.Method));
                        return true;
                    }
            }
            // ReSharper restore PossibleNullReferenceException

            result = default(T);
            return false;
        }

        /// <summary>
        /// Returns the default result.
        /// </summary>
        public virtual T DefaultVisit(Expression node) => default(T);

        #region Special cases
        /// <summary>
        /// Transforms an extension <see cref="Expression"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitExtension(Expression node) => Visit(node.Reduce());

        /// <summary>
        /// Transforms a dynamic <see cref="Expression"/> into an object of type <see cref="T"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">The <paramref name="node"/> does not inherit <see cref="Expression"/> or cannot be reduced.</exception>
        protected virtual T VisitDynamic(IDynamicExpression node)
        {
            if (node is Expression expr && expr.CanReduce)
                return Visit(expr.Reduce());

            throw new NotSupportedException();
        }
        #endregion

        #region Built-in
        /// <summary>
        /// Transforms a <see cref="BlockExpression"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitBlock(BlockExpression node)
        {
            var expressions = node.Expressions;
            int countMinusOne = expressions.Count - 1;

            for (int i = 0; i < countMinusOne; i++)
                Visit(expressions[i]);

            return Visit(expressions[countMinusOne]);
        }

        /// <summary>
        /// Transforms a <see cref="TryExpression"/> into an object of type <see cref="T"/>.
        /// </summary>
        protected virtual T VisitTry(TryExpression node)
        {
            try
            {
                return Visit(node);
            }
            catch
            {
                return Visit(node.Fault);
            }
            finally
            {
                Visit(node.Finally);
            }
        }
        #endregion

        #region Dynamic
        /// <summary>
        /// Enables dynamic visiting -- that is, using derived Visit methods built for extensions by
        /// constructing a <see langword="delegate"/> that tests equality on all of them.
        /// </summary>
        /// <example>
        /// If a derived class has a <code>VisitForEach(<see cref="ForEachExpression"/> node)</code>
        /// method, it will automatically called when calling <see cref="Visit(Expression)"/>.
        /// </example>
        protected void EnableDynamicVisiting()
        {
            if (dynamicVisit != null)
                return;

            Type returnType = typeof(T);
            ParameterExpression exprParam = Expression.Parameter(typeof(Expression), "node");
            Expression body = Expression.Call(Expression.Constant(this), nameof(DefaultVisit), null, exprParam);

            foreach (MethodInfo method in this.GetType().GetTypeInfo().DeclaredMethods)
            {
                if (method.ReturnType != returnType || !method.Name.StartsWith("Visit"))
                    continue;

                ParameterInfo[] parameters = method.GetParameters();

                if (parameters.Length != 1)
                    continue;

                ParameterInfo parameter = parameters[0];

                if (!parameter.ParameterType.IsAssignableTo(typeof(Expression)))
                    continue;

                // We got this far, it's a valid Visit*() method.
                ParameterExpression exprVar = Expression.Parameter(parameter.ParameterType, parameter.Name);

                // expr is TExpr newExpr ? newExpr : body
                body = Expression.Block(new[] { exprVar },
                    Expression.Assign(exprVar, Expression.TypeAs(exprParam, parameter.ParameterType)),
                    Expression.Condition(
                        Expression.ReferenceNotEqual(exprVar, Expression.Constant(null)),
                        exprVar,
                        body
                    )
                );
            }

            dynamicVisit = Expression.Lambda<Func<Expression, T>>(body, exprParam).Compile();
        }
        #endregion
    }
}
