using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace Cometary.Expressions
{
    /// <summary>
    /// <see cref="ExpressionVisitor"/> that stores all variables in an <see cref="object"/>
    /// array, allowing them to be saved between each run of the compiled expression.
    /// </summary>
    internal class TrackingExpressionVisitor : ExpressionVisitor
    {
        /// <summary>
        /// Gets the array in which variables will be stored.
        /// </summary>
        public IList<ParameterExpression> Variables { get; }

        /// <summary>
        /// Gets an optional <see cref="ExpressionProjector"/> used
        /// by this visitor.
        /// </summary>
        public ExpressionProjector Projector { get; }

        public TrackingExpressionVisitor(params ExpressionProjector[] projectors)
        {
            Variables = new LightList<ParameterExpression>();

            if (projectors.Length > 0)
                Projector = projectors.Length == 1 ? projectors[0] : (ExpressionProjector)Delegate.Combine(projectors);
        }

        /// <summary>
        /// Visits an expression.
        /// <para>
        /// If the expression represents a variable, it will be tracked.
        /// </para>
        /// </summary>
        public override Expression Visit(Expression node)
        {
            if (node == null)
                return null;

            if (Projector != null)
            {
                // Project things in here, to avoid traversing the tree multiple times
                // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
                foreach (ExpressionProjector projector in Projector.GetInvocationList())
                    node = projector(node);
            }

            node = node.ReduceExtensions();

            if (node is BlockExpression block)
                // Make sure we don't have smaller scopes that would break the variable assignements
                node = block.Update(Enumerable.Empty<ParameterExpression>(), block.Expressions);

            if (node is ParameterExpression variable && !Variables.Contains(variable) && !variable.IsAssignableTo<VirtualStateMachine>())
                // Keep track of variable
                Variables.Add(variable);

            if (node.NodeType == ExpressionType.Extension && !node.CanReduce)
                // In case we're returning a special expression (ie: YieldExpression)
                return node;

            return base.Visit(node);
        }

        /// <summary>
        /// Generates an <see cref="Expression"/> tree that starts
        /// by setting all previously visited variables to a certain value,
        /// and ends by saving their value in a <see cref="VirtualStateMachine"/>.
        /// </summary>
        /// <param name="body">The body to transform.</param>
        /// <param name="vsmExpression">The <see cref="VirtualStateMachine"/> in which the state will be saved.</param>
        public Expression GenerateBody(Expression body, Expression vsmExpression)
        {
            Debug.Assert(vsmExpression.Type.IsAssignableTo<VirtualStateMachine>());

            if (Variables.Count == 0)
                return body;

            Expression[] loadVariableExpressions = new Expression[Variables.Count];
            Expression[] saveVariableExpressions = new Expression[Variables.Count];

            Expression localsExpression = Expression.Field(vsmExpression, VirtualStateMachine.LocalsField);

            for (int i = 0; i < Variables.Count; i++)
            {
                ParameterExpression variable = Variables[i];

                loadVariableExpressions[i] = Expression.Assign(
                    variable, Expression.Convert(
                        Expression.ArrayAccess(localsExpression, Expression.Constant(i)),
                        variable.Type
                    )
                );

                saveVariableExpressions[i] = Expression.Assign(
                    Expression.ArrayAccess(localsExpression, Expression.Constant(i)),
                    Expression.Convert(variable, typeof(object))
                );
            }

            Expression init = loadVariableExpressions.Length == 0
                ? (Expression)Expression.Empty() : Expression.Block(typeof(void), loadVariableExpressions);
            Expression end = saveVariableExpressions.Length == 0
                ? (Expression)Expression.Empty() : Expression.Block(typeof(void), saveVariableExpressions);

            if (body.Type == typeof(void))
                return Expression.Block(Variables, init, body, end);

            ParameterExpression result = Expression.Variable(body.Type, "result");

            return Expression.Block(Variables.Prepend(result), init, Expression.Assign(result, body), end, result);
        }
    }
}
