#if false
using System;
using System.Linq.Expressions;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents an inline <see cref="System.Type"/> expression.
    /// <para>
    /// This <see cref="Expression"/> cannot be reduced, and must be
    /// replaced before compilation.
    /// </para>
    /// </summary>
	public sealed class TypeExpression : Expression
    {
        /// <inheritdoc />
        public override bool CanReduce => false;

        /// <inheritdoc />
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <inheritdoc />
        public override Type Type { get; }

        internal TypeExpression(Type type)
        {
            Type = type;
        }

        /// <inheritdoc cref="UnaryExpression.Update(Expression)" select="summary"/>
        public TypeExpression Update(Type type)
        {
            if (Type == type)
                return this;

            return Expressive.Type(type);
        }
    }

    partial class Expressive
    {
        /// <summary>
        /// Creates a <see cref="TypeExpression"/> that represents an
        /// inline type expression.
        /// </summary>
		public new static TypeExpression Type(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return new TypeExpression(type);
        }

        /// <summary>
        /// Creates a <see cref="TypeExpression"/> that represents an
        /// inline type expression.
        /// </summary>
		public new static TypeExpression Type<T>()
        {
            return new TypeExpression(typeof(T));
        }
    }
}

#endif