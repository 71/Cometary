using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Cometary.Expressions.Trees
{
    partial class ExpressionTree
    {
        private static Expression[] MakeArray(params object[] expressions)
        {
            LightList<Expression> seq = new LightList<Expression>();

            for (int i = 0; i < expressions.Length; i++)
            {
                object expression = expressions[i];

                if (expression == null)
                    continue;

                if (expression is Expression expr)
                    seq.Add(expr);
                else if (expression is IEnumerable<Expression> exprs)
                    foreach (Expression ex in exprs)
                        seq.Add(ex);
                else
                    throw new ArgumentException();
            }

            return seq.ToArray();
        }

        /// <summary>
        /// Returns the given <see cref="Expression"/>'s direct children.
        /// </summary>
        public static IEnumerable<Expression> Children(this Expression expression, bool allChildren = false)
        {
            if (allChildren)
                return new ExpressionEnumerator(expression);

            switch (expression)
            {
                case BinaryExpression binary:
                    return new[] { binary.Left, binary.Right };
                case BlockExpression block:
                    return block.Expressions.ToArray();
                case ConditionalExpression conditional:
                    return new[] { conditional.Test, conditional.IfTrue, conditional.IfFalse };
                case GotoExpression @goto:
                    return new[] { @goto.Value };
                case IndexExpression index:
                    return MakeArray(index.Object, index.Arguments);
                case InvocationExpression invocation:
                    return MakeArray(invocation.Expression, invocation.Arguments);
                case LabelExpression label:
                    return new[] { label.DefaultValue };
                case LambdaExpression lambda:
                    return lambda.Body.Children();
                case ListInitExpression listInit:
                    return MakeArray(listInit.NewExpression, listInit.Initializers.SelectMany(x => x.Arguments));
                case LoopExpression loop:
                    return new[] { loop.Body };
                case MemberExpression member:
                    return new[] { member.Expression };
                case MemberInitExpression memberInit:
                    return new[] { memberInit.NewExpression };
                case MethodCallExpression call:
                    return MakeArray(call.Object, call.Arguments);
                case NewArrayExpression newArray:
                    return newArray.Expressions.ToArray();
                case NewExpression @new:
                    return @new.Arguments.ToArray();
                case RuntimeVariablesExpression runtimeVar:
                    return runtimeVar.Variables.Cast<Expression>().ToArray();
                case SwitchExpression @switch:
                    return MakeArray(@switch.SwitchValue, @switch.DefaultBody, @switch.Cases.SelectMany(x => x.TestValues.Prepend(x.Body)));
                case TryExpression @try:
                    return MakeArray(@try.Body, @try.Fault, @try.Handlers.Select(x => x.Body), @try.Finally);
                case TypeBinaryExpression typeBinary:
                    return new[] { typeBinary.Expression };
                case UnaryExpression unary:
                    return new[] { unary.Operand };

                default:
                    return new Expression[0];
            }
        }

        private sealed class ExpressionEnumerator : ExpressionVisitor, IEnumerator<Expression>, IEnumerable<Expression>
        {
            private readonly Queue<Expression> queue;
            private readonly Expression root;

            private Expression current;

            public ExpressionEnumerator(Expression root)
            {
                this.root = root;
                this.queue = new Queue<Expression>();
            }

            public Expression Current => current;

            object IEnumerator.Current => current;

            public void Dispose() => queue.Clear();

            public void Reset()
            {
                queue.Clear();
                Visit(root);
            }

            public bool MoveNext()
            {
                if (queue.Count == 0)
                    return false;

                current = queue.Dequeue();
                return true;
            }

            public override Expression Visit(Expression node)
            {
                queue.Enqueue(node);
                return node;
            }

            public IEnumerator<Expression> GetEnumerator() => this;
            IEnumerator IEnumerable.GetEnumerator() => this;
        }
    }
}
