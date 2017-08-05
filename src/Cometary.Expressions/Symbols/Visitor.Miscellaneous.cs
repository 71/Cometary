using System.Linq.Expressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Semantics;

namespace Cometary.Expressions
{
    partial class ExpressionParsingVisitor
    {
        #region Variables / Declarations
        /// <summary>
        ///   Transforms the given <see cref="ILocalSymbol"/> into a
        ///   <see cref="ParameterExpression"/>.
        /// </summary>
        public static ParameterExpression VisitLocal(ILocalSymbol localSymbol)
        {
            return Expression.Parameter(localSymbol.Type.GetCorrespondingType(), localSymbol.Name);
        }

        /// <inheritdoc />
        public override Expression VisitVariableDeclarationStatement(IVariableDeclarationStatement operation, LocalBinder argument)
        {
            return Expression.Block(operation.Variables.Select(x => x.Accept(this, argument)));
        }

        /// <inheritdoc />
        public override Expression VisitVariableDeclaration(IVariableDeclaration operation, LocalBinder argument)
        {
            return Expression.Assign(argument[operation.Variable], operation.InitialValue.Accept(this, argument));
        }
        #endregion

        #region Invalid
        /// <inheritdoc />
        public override Expression VisitInvalidStatement(IInvalidStatement operation, LocalBinder argument)
        {
            throw NotSupported(operation);
        }

        /// <inheritdoc />
        public override Expression VisitInvalidExpression(IInvalidExpression operation, LocalBinder argument)
        {
            throw NotSupported(operation);
        }
        #endregion
    }
}
