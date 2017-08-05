using System.Collections.Immutable;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Semantics;

namespace Cometary.Expressions
{
    partial class ExpressionParsingVisitor
    {
        /// <inheritdoc />
        public override Expression VisitLambdaExpression(ILambdaExpression operation, LocalBinder argument)
        {
            // Make parameters
            ImmutableArray<IParameterSymbol> sigParameters = operation.Signature.Parameters;
            ParameterExpression[] parameters = new ParameterExpression[sigParameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                IParameterSymbol sigParameter = sigParameters[i];

                parameters[i] = Expression.Parameter(sigParameter.Type.GetCorrespondingType(), sigParameter.Name);
            }

            // Return lambda
            return Expression.Lambda(operation.Body.Accept(this, argument), operation.Signature.Name, parameters);
        }

        /// <inheritdoc />
        public override Expression VisitUnboundLambdaExpression(IUnboundLambdaExpression operation, LocalBinder argument)
        {
            throw NotSupported(operation);
        }

        /// <inheritdoc />
        public override Expression VisitLocalFunctionStatement(IOperation operation, LocalBinder argument)
        {
            throw NotSupported(operation);
        }
    }
}
