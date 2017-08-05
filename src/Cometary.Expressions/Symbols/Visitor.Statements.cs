using System.Linq.Expressions;
using Microsoft.CodeAnalysis.Semantics;

namespace Cometary.Expressions
{
    partial class ExpressionParsingVisitor
    {
        /// <inheritdoc />
        public override Expression VisitLabelStatement(ILabelStatement operation, LocalBinder argument)
        {
            return Expression.Label(operation.LabeledStatement.Accept(this, argument));
        }

        /// <inheritdoc />
        public override Expression VisitBranchStatement(IBranchStatement operation, LocalBinder argument)
        {
            GotoExpressionKind kind;

            switch (operation.BranchKind)
            {
                case BranchKind.Break:
                    kind = GotoExpressionKind.Break;
                    break;
                case BranchKind.Continue:
                    kind = GotoExpressionKind.Continue;
                    break;
                default:
                    kind = GotoExpressionKind.Goto;
                    break;
            }

            return Expression.MakeGoto(kind, )
        }

        /// <inheritdoc />
        public override Expression VisitYieldBreakStatement(IReturnStatement operation, LocalBinder argument)
        {
            return Expression.Break()
        }

        /// <inheritdoc />
        public override Expression VisitEmptyStatement(IEmptyStatement operation, LocalBinder argument)
        {
            return Expression.Empty();
        }

        /// <inheritdoc />
        public override Expression VisitThrowStatement(IThrowStatement operation, LocalBinder argument)
        {
            return Expression.Throw(operation.ThrownObject.Accept(this, argument));
        }

        /// <inheritdoc />
        public override Expression VisitReturnStatement(IReturnStatement operation, LocalBinder argument)
        {
            return Expression.Return(ReturnTarget);
        }

        /// <inheritdoc />
        public override Expression VisitExpressionStatement(IExpressionStatement operation, LocalBinder argument)
        {
            return Expression.Convert(operation.Expression.Accept(this, argument), typeof(void));
        }

        /// <inheritdoc />
        public override Expression VisitWithStatement(IWithStatement operation, LocalBinder argument)
        {
            throw NotSupported(operation);
        }

        /// <inheritdoc />
        public override Expression VisitStopStatement(IStopStatement operation, LocalBinder argument)
        {
            throw NotSupported(operation);
        }

        /// <inheritdoc />
        public override Expression VisitEndStatement(IEndStatement operation, LocalBinder argument)
        {
            throw NotSupported(operation);
        }
    }
}
