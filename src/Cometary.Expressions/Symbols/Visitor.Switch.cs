using System.Collections.Immutable;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis.Semantics;

namespace Cometary.Expressions
{
    partial class ExpressionParsingVisitor
    {
        /// <inheritdoc />
        public override Expression VisitSwitchStatement(ISwitchStatement operation, LocalBinder argument)
        {
            ImmutableArray<ISwitchCase> cases = operation.Cases;
            SwitchCase[] switchCases = new SwitchCase[cases.Length];
            Expression defaultBody = null;

            for (int i = 0; i < switchCases.Length; i++)
            {
                var @case = cases[i];
                var block = Expression.Block(@case.Body.Select(x => x.Accept(this, argument)));
                Expression[] values = new Expression[@case.Clauses.Length];

                for (int j = 0; j < values.Length; j++)
                {
                    var clause = @case.Clauses[j];

                    switch (clause.CaseKind)
                    {
                        case CaseKind.SingleValue:
                            values[i] = Expression.Constant(clause.ConstantValue.Value);
                            break;
                        case CaseKind.Default:
                            defaultBody = block;
                            values[i] = null;
                            break;
                        default:
                            throw NotSupported(operation);
                    }
                }

                switchCases[i] = Expression.SwitchCase(block, values);
            }

            return Expression.Switch(operation.Value.Accept(this, argument), defaultBody, switchCases);
        }

        /// <inheritdoc />
        public override Expression VisitSwitchCase(ISwitchCase operation, LocalBinder argument)
        {
            throw Unexpected(operation, nameof(VisitSwitchStatement));
        }

        /// <inheritdoc />
        public override Expression VisitSingleValueCaseClause(ISingleValueCaseClause operation, LocalBinder argument)
        {
            throw Unexpected(operation, nameof(VisitSwitchStatement));
        }

        /// <inheritdoc />
        public override Expression VisitRelationalCaseClause(IRelationalCaseClause operation, LocalBinder argument)
        {
            throw Unexpected(operation, nameof(VisitSwitchStatement));
        }

        /// <inheritdoc />
        public override Expression VisitRangeCaseClause(IRangeCaseClause operation, LocalBinder argument)
        {
            throw Unexpected(operation, nameof(VisitSwitchStatement));
        }
    }
}
