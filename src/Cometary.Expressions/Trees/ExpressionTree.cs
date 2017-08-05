using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Cometary.Expressions.Trees
{
    /// <summary>
    /// Provides utilities for manipulating expression trees.
    /// </summary>
    public static partial class ExpressionTree
    {
        /// <summary>
        /// Represents an equality comparer that compares expressions trees
        /// and their content, instead of comparing their reference.
        /// </summary>
        public sealed class Comparer : IEqualityComparer<Expression>
        {
            /// <inheritdoc />
            public bool Equals(Expression x, Expression y)
            {
                if (x == null)
                    return y == null;
                if (y == null)
                    return false;
                if (x.GetType() != y.GetType())
                    return false;

                if (x is ConstantExpression lc && y is ConstantExpression rc)
                    return lc.Value.Equals(rc.Value);

                return x.Children().SequenceEqual(y.Children(), this);
            }

            /// <inheritdoc />
            public int GetHashCode(Expression obj) => obj.GetHashCode();
        }

        /// <summary>
        /// Returns whether or not the given expression <paramref name="tree"/>
        /// contains the specified <paramref name="expression"/>.
        /// </summary>
        public static bool Contains(this Expression tree, Expression expression)
        {
            bool doesContain = false;
            ExpressionPipeline visitor = Expressive.Pipeline();

            visitor.Pipeline += expr =>
            {
                if (expr == expression)
                {
                    doesContain = true;
                    visitor.Stop();
                }

                return expr;
            };
            
            visitor.Visit(tree);

            return doesContain;
        }

        /// <summary>
        /// Returns whether or not an <see cref="Expression"/> inside the given <paramref name="tree"/>
        /// matches the given <paramref name="predicate"/>.
        /// </summary>
        public static bool Exists(this Expression tree, Predicate<Expression> predicate)
        {
            bool doesExist = false;
            ExpressionPipeline visitor = Expressive.Pipeline();

            visitor.Pipeline += expr =>
            {
                if (predicate(expr))
                {
                    doesExist = true;
                    visitor.Stop();
                }

                return expr;
            };

            visitor.Visit(tree);

            return doesExist;
        }

        /// <summary>
        /// Returns the first <see cref="Expression"/> that matches
        /// the given <paramref name="predicate"/>.
        /// </summary>
        public static Expression Find(this Expression tree, Predicate<Expression> predicate)
        {
            Expression result = null;
            ExpressionPipeline visitor = Expressive.Pipeline();

            visitor.Pipeline += expr =>
            {
                if (predicate(expr))
                {
                    result = expr;
                    visitor.Stop();
                }

                return expr;
            };

            visitor.Visit(tree);

            return result;
        }

        /// <summary>
        /// Returns all <see cref="Expression"/>s that matches
        /// the given <paramref name="predicate"/>.
        /// </summary>
        public static List<Expression> FindAll(this Expression tree, Predicate<Expression> predicate)
        {
            List<Expression> result = new List<Expression>();
            ExpressionPipeline visitor = Expressive.Pipeline();

            visitor.Pipeline += expr =>
            {
                if (predicate(expr))
                {
                    result.Add(expr);
                    visitor.Stop();
                }

                return expr;
            };

            visitor.Visit(tree);

            return result;
        }

        /// <summary>
        /// Returns the given expression <paramref name="tree"/>, replacing
        /// one expression by another.
        /// </summary>
        public static Expression Replace(this Expression tree, Expression replacee, Expression replacer)
        {
            return Expressive.Pipeline(expr => expr == replacee ? replacer : expr).Visit(tree);
        }

        /// <summary>
        /// Returns whether or not the given expressions are equal, even if
        /// they reference different objects.
        /// </summary>
        public static bool Matches(this Expression left, Expression right)
        {
            return new Comparer().Equals(left, right);
        }

        /// <summary>
        /// Returns whether or not both expression enumerables are equal.
        /// </summary>
        public static bool Match(this IEnumerable<Expression> expressions, IEnumerable<Expression> second)
        {
            return expressions.SequenceEqual(second, new Comparer());
        }
    }
}
