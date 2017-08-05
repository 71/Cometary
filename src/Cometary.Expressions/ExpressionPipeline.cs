using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents a method that projects an <see cref="Expression"/>
    /// into a new one.
    /// </summary>
    /// <param name="node">The <see cref="Expression"/> to project.</param>
    /// <returns>An <see cref="Expression"/> that may be different from the input.</returns>
    public delegate Expression ExpressionProjector(Expression node);

    /// <summary>
    /// Represents an <see cref="ExpressionVisitor"/> that visits
    /// all <see cref="Expression">Expressions</see> in an expression tree.
    /// </summary>
    public sealed class ExpressionPipeline : ExpressionVisitor
    {
        private bool _isStopped;
        private IList<Expression> _visitedExpressions;

        /// <summary>
        /// Adds or removes a function that will
        /// modify any <see cref="Expression"/> visited by this
        /// visitor.
        /// </summary>
        public event ExpressionProjector Pipeline;

        /// <summary>
        /// Gets or sets whether or not each expression should only be visited once.
        /// </summary>
        public bool VisitOnce
        {
            get => _visitedExpressions != null;
            set
            {
                if (VisitOnce == value)
                    return;

                if (value)
                {
                    _visitedExpressions = new LightList<Expression>();
                }
                else
                {
                    _visitedExpressions.Clear();
                    _visitedExpressions = null;
                }
            }
        }

        /// <summary>
        /// Creates a new <see cref="ExpressionPipeline"/>, optionally
        /// giving it default projectors.
        /// </summary>
        public ExpressionPipeline(params ExpressionProjector[] pipeline)
        {
            if (pipeline == null)
                throw new ArgumentNullException(nameof(pipeline));
            if (Array.IndexOf(pipeline, null) != -1)
                throw new ArgumentNullException(nameof(pipeline), "The given array contains a null item.");

            foreach (ExpressionProjector projector in pipeline)
                Pipeline += projector;
        }

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public override Expression Visit(Expression node)
        {
            if (_isStopped)
                return node;
            if (_visitedExpressions != null && _visitedExpressions.Contains(node))
                return node;

            if (Pipeline != null)
            {
                Delegate[] projectors = Pipeline.GetInvocationList();

                for (int i = 0; i < projectors.Length; i++)
                {
                    node = (projectors[i] as ExpressionProjector).Invoke(node);

                    if (node == null)
                        return null;
                }
            }

            _visitedExpressions?.Add(node);

            return node.NodeType == ExpressionType.Extension && !node.CanReduce ? node : base.Visit(node);
        }

        /// <summary>
        /// Stops the <see cref="ExpressionPipeline"/> from visiting other
        /// expressions.
        /// </summary>
        public void Stop()
        {
            _isStopped = true;
        }

        #region Built-in projectors
        /// <summary>
        /// Replaces a <see cref="MemberExpression"/> to a field of a display class (compiler-generated class
        /// used to store local variables) by a direct <see cref="ConstantExpression"/>.
        /// 
        /// <para>
        /// For example, let's imagine the following method:
        ///   <c>Expression IsAnswer(int i) => Express(() => i == 42);</c>
        /// 
        /// To compile this, Rolsyn will generate a new class, with a field matching
        /// the "i" parameter. However, since "i" is a simple int, we'd like to print
        /// a Constant instead, making it much faster.
        /// </para>
        /// </summary>
        public static Expression ReplaceBoundVariableFields(Expression expr)
        {
            MemberExpression mx = expr as MemberExpression;
            FieldInfo accessedField = mx?.Member as FieldInfo;

            if (accessedField == null || !accessedField.DeclaringType.Name.StartsWith("<>c__"))
                return expr;

            ConstantExpression displayClassConstant = mx.Expression as ConstantExpression;

            if (displayClassConstant == null)
                return expr;

            return Expression.Constant(
                accessedField.GetValue(displayClassConstant.Value),
                displayClassConstant.Type);
        }

        /// <summary>
        /// Replaces every reducible <see cref="Expression"/> by its reduced form.
        /// </summary>
        public static Expression Reduce(Expression expr)
        {
            if (expr == null)
                return null;

            while (expr.CanReduce)
                expr = expr.Reduce();

            return expr;
        }
        #endregion
    }
}
