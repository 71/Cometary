using System.Linq.Expressions;
using Microsoft.CodeAnalysis.Semantics;

namespace Cometary.Expressions
{
    partial class ExpressionParsingVisitor
    {
        /// <inheritdoc />
        public override Expression VisitLiteralExpression(ILiteralExpression operation, LocalBinder argument)
        {
            return Expression.Constant(operation.ConstantValue.Value);
        }
    }
}
