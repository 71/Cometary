using System;
using System.Linq.Expressions;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents a <see langword="typeof"/> expression.
    /// </summary>
	public sealed class TypeOfExpression : Expression
    {
        /// <inheritdoc />
        public override bool CanReduce => true;

        /// <inheritdoc />
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <inheritdoc />
        public override Type Type => typeof(Type);

        /// <summary>
        /// Gets the <see cref="System.Type"/> referenced by this expression.
        /// </summary>
        public Type TargetType { get; }

        internal TypeOfExpression(Type type)
        {
            TargetType = type;
        }

        /// <inheritdoc cref="UnaryExpression.Update(Expression)" select="summary"/>
        public TypeOfExpression Update(Type type)
        {
            if (TargetType == type)
                return this;

            return Expressive.TypeOf(type);
        }

        /// <inheritdoc />
        public override Expression Reduce() => Constant(TargetType);

        /// <inheritdoc />
        public override string ToString() => $"typeof({TargetType})";
    }

    partial class Expressive
    {
        /// <summary>
        /// Creates a <see cref="TypeOfExpression"/> that represents a
        /// <see langword="typeof"/> expression.
        /// </summary>
		public static TypeOfExpression TypeOf(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return new TypeOfExpression(type);
        }

        /// <summary>
        /// Creates a <see cref="TypeOfExpression"/> that represents a
        /// <see langword="typeof"/> expression.
        /// </summary>
		public static TypeOfExpression TypeOf<T>()
        {
            return new TypeOfExpression(typeof(T));
        }
    }
}
