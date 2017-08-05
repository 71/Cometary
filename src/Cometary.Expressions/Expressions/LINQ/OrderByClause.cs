using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents an <see cref="Expression"/> that either
    /// orders an enumerable ascendingly or descendingly.
    /// </summary>
    public sealed class OrderingExpression
    {
        /// <summary>
        /// Gets the <see cref="Expression"/> that will
        /// return the key used for sorting.
        /// </summary>
        public Expression Key { get; }

        /// <summary>
        /// Gets whether or not the sorting is done in an ascending manner.
        /// </summary>
        public bool IsAscending { get; }

        /// <summary>
        /// Gets whether or not the sorting is done in a descending manner.
        /// </summary>
        public bool IsDescending => !IsAscending;

        internal OrderingExpression(Expression key, bool isAscending)
        {
            Key = key;
            IsAscending = isAscending;
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("{0} {1}", Key, IsAscending ? "ascending": "descending");
    }

    /// <summary>
    /// Represents a LINQ <see langword="orderby"/> clause.
    /// </summary>
    public sealed class OrderByClause : QueryClause
    {
        /// <inheritdoc />
        public override Type Type => typeof(bool);

        /// <inheritdoc />
        public override QueryClauseType ClauseType => QueryClauseType.OrderBy;

        /// <summary>
        /// Gets the <see cref="Expression"/> that will
        /// return the key used for sorting.
        /// </summary>
        public ReadOnlyCollection<OrderingExpression> Orderings { get; }

        internal OrderByClause(IList<OrderingExpression> orderings)
        {
            Orderings = new ReadOnlyCollection<OrderingExpression>(orderings);
        }

        /// <inheritdoc cref="UnaryExpression.Update(Expression)" select="summary"/>
        public OrderByClause Update(IEnumerable<OrderingExpression> orderings)
        {
            if (orderings.SequenceEqual(Orderings))
                return this;

            return Expressive.OrderBy(orderings);
        }

        /// <inheritdoc />
        protected override Expression Reduce(QueryClause previous, QueryClause next)
        {
            return next.Reduce();
        }

        /// <inheritdoc />
        protected internal override QueryClause VisitChildren(ExpressionVisitor visitor) => this;

        /// <inheritdoc />
        public override string ToString() => $"orderby {string.Join(", ", Orderings)}";
    }

    partial class Expressive
    {
        /// <summary>
        /// Creates a <see cref="OrderByClause"/> that represents a
        /// LINQ <see langword="orderby"/> clause.
        /// </summary>
		public static OrderByClause OrderBy(Expression key, bool isAscending = true)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return OrderBy(new OrderingExpression(key, isAscending));
        }

        /// <summary>
        /// Creates a <see cref="OrderByClause"/> that represents a
        /// LINQ <see langword="orderby"/> clause.
        /// </summary>
		public static OrderByClause OrderBy(params OrderingExpression[] orderings)
        {
            if (orderings == null)
                throw new ArgumentNullException(nameof(orderings));
            if (Array.IndexOf(orderings, null) != -1)
                throw new ArgumentNullException(nameof(orderings));

            return new OrderByClause(orderings);
        }

        /// <summary>
        /// Creates a <see cref="OrderByClause"/> that represents a
        /// LINQ <see langword="orderby"/> clause.
        /// </summary>
		public static OrderByClause OrderBy(IEnumerable<OrderingExpression> orderings)
        {
            if (orderings == null)
                throw new ArgumentNullException(nameof(orderings));
            if (orderings.Contains(null))
                throw new ArgumentNullException(nameof(orderings));

            return new OrderByClause(orderings.ToArray());
        }

        /// <summary>
        /// Creates a <see cref="OrderingExpression"/>, used in a <see cref="OrderByClause"/>.
        /// </summary>
        public static OrderingExpression Ordering(Expression key, bool isAscending)
        {
            Requires.NotNull(key, nameof(key));

            return new OrderingExpression(key, isAscending);
        }

        /// <summary>
        /// Creates a <see cref="OrderingExpression"/> representing
        /// an <see langword="orderby"/> ... <see langword="ascending"/>
        /// expression.
        /// </summary>
        public static OrderingExpression Ascending(Expression key) => Ordering(key, true);

        /// <summary>
        /// Creates a <see cref="OrderingExpression"/> representing
        /// an <see langword="orderby"/> ... <see langword="descending"/>
        /// expression.
        /// </summary>
        public static OrderingExpression Descending(Expression key) => Ordering(key, false);
    }
}
