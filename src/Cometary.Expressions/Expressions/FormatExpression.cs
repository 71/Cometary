using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents an expression that represents a formatted <see cref="string"/>.
    /// </summary>
    public sealed class FormatExpression : Expression
    {
        private static readonly MethodInfo FormatMethod = typeof(string).GetRuntimeMethod(nameof(string.Format), new[] { typeof(string), typeof(object[]) });

        /// <inheritdoc />
        public override bool CanReduce => true;

        /// <inheritdoc />
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <inheritdoc />
        public override Type Type => typeof(string);

        /// <summary>
        /// Gets the array containing the expressions that will be
        /// used to form the formatted <see cref="string"/>.
        /// </summary>
        public ReadOnlyCollection<Expression> Expressions { get; }

        /// <summary>
        /// Gets the format <see cref="string"/> used.
        /// </summary>
        public string Format { get; }

        internal FormatExpression(string format, IList<Expression> expressions)
        {
            Format = format;
            Expressions = new ReadOnlyCollection<Expression>(expressions);
        }

        /// <inheritdoc />
        public override Expression Reduce()
        {
            return Call(FormatMethod, Constant(Format), NewArrayInit(typeof(object), Expressions));
        }

        /// <inheritdoc cref="UnaryExpression.Update(Expression)" select="summary"/>
        public FormatExpression Update(string format, IEnumerable<Expression> expressions)
        {
            Requires.NotNull(expressions, nameof(expressions));

            if (format == Format && expressions.SequenceEqual(Expressions))
                return this;

            return Expressive.Format(format, expressions);
        }

        /// <inheritdoc />
        protected override Expression VisitChildren(ExpressionVisitor visitor)
            => Update(Format, visitor.Visit(Expressions));

        /// <inheritdoc />
        public override string ToString() => $"$\"{string.Format(Format, Expressions.Select(x => (object)('{' + x.ToString() + '}')).ToArray())}\"";
    }

    partial class Expressive
    {
        /// <summary>
        /// Creates a <see cref="FormatExpression"/> that represents a formatted <see cref="string"/>.
        /// </summary>
        public static FormatExpression Format(string format, IEnumerable<Expression> expressions)
        {
            Requires.NotNull(format, nameof(format));
            Requires.NotNull(expressions, nameof(expressions));

            return new FormatExpression(format, expressions.ToArray());
        }

        /// <inheritdoc cref="Format(string, IEnumerable{Expression})" />
        public static FormatExpression Format(string format, params Expression[] expressions)
        {
            return Format(format, (IEnumerable<Expression>)expressions);
        }

        /// <summary>
        /// Creates a <see cref="FormatExpression"/> that represents a formatted <see cref="string"/>,
        /// using either the given arguments, or the value they represent (if they inherit <see cref="Expression"/>).
        /// </summary>
        public static FormatExpression Format(string format, IEnumerable<object> args)
        {
            Requires.NotNull(format, nameof(format));
            Requires.NotNull(args, nameof(args));

            return new FormatExpression(format, args.Select(x => x is Expression node ? node : Constant(x)).ToArray());
        }

        /// <inheritdoc cref="Format(string, IEnumerable{object})" />
        public static FormatExpression Format(string format, params object[] args)
        {
            return Format(format, (IEnumerable<object>)args);
        }
    }
}
