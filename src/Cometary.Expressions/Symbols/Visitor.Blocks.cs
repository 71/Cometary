using System.Collections.Immutable;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Semantics;

namespace Cometary.Expressions
{
    partial class ExpressionParsingVisitor
    {
        /// <inheritdoc />
        public override Expression VisitBlockStatement(IBlockStatement operation, LocalBinder argument)
        {
            // Create corresponding variables
            var locals = operation.Locals;
            var variables = new ParameterExpression[locals.Length];

            for (int i = 0; i < locals.Length; i++)
            {
                variables[i] = VisitLocal(locals[i]);
            }

            // Transform statements
            var binder = argument.Child(locals, variables);
            var statements = operation.Statements;
            var expressions = new Expression[statements.Length];

            for (int i = 0; i < expressions.Length; i++)
            {
                expressions[i] = statements[i].Accept(this, binder);
            }

            // Return created block
            return Expression.Block(variables, expressions);
        }

        /// <inheritdoc />
        public override Expression VisitIfStatement(IIfStatement operation, LocalBinder argument)
        {
            if (operation.IfFalseStatement != null)
            {
                return Expression.IfThenElse(
                    operation.Condition.Accept(this, argument),
                    operation.IfTrueStatement.Accept(this, argument),
                    operation.IfFalseStatement.Accept(this, argument)
                );
            }

            return Expression.IfThen(
                operation.Condition.Accept(this, argument),
                operation.IfTrueStatement.Accept(this, argument)
            );
        }

        /// <inheritdoc />
        public override Expression VisitWhileUntilLoopStatement(IWhileUntilLoopStatement operation, LocalBinder argument)
        {
            Expression condition = operation.Condition.Accept(this, argument);
            Expression body = operation.Body.Accept(this, argument);

            return operation.IsTopTest
                ? Expressive.While(condition, body) as Expression
                : Expressive.DoWhile(condition, body);
        }

        /// <inheritdoc />
        public override Expression VisitForLoopStatement(IForLoopStatement operation, LocalBinder argument)
        {
            LocalBinder binder = argument.Child();
            ImmutableArray<IOperation> before = operation.Before;
            Expression[] expressions = new Expression[before.Length + 1];

            for (int i = 0; i < before.Length; i++)
            {
                expressions[i] = before[i].Accept(this, argument);
            }

            expressions[before.Length] = Expressive.For(


            );

            return Expression.Block(

                expressions
            );
        }

        /// <inheritdoc />
        public override Expression VisitForEachLoopStatement(IForEachLoopStatement operation, LocalBinder argument)
        {
            ParameterExpression variable = VisitLocal(operation.IterationVariable);
            LocalBinder binder = argument.Child(operation.IterationVariable, variable);

            return Expressive.ForEach(variable,
                operation.Collection.Accept(this, argument),
                operation.Body.Accept(this, binder)
            );
        }

        /// <inheritdoc />
        public override Expression VisitLockStatement(ILockStatement operation, LocalBinder argument)
        {
            return Expressive.Lock(
                operation.LockedObject.Accept(this, argument),
                operation.Body.Accept(this, argument)
            );
        }

        /// <inheritdoc />
        public override Expression VisitUsingStatement(IUsingStatement operation, LocalBinder argument)
        {
            // TODO: Declarations in new binder
            return Expressive.Using(
                Visit(operation.Declaration ?? operation.Value, argument),
                operation.Body.Accept(this, argument)
            );
        }

        /// <inheritdoc />
        public override Expression VisitFixedStatement(IFixedStatement operation, LocalBinder argument)
        {
            throw NotSupported(operation);
        }

        /// <inheritdoc />
        public override Expression VisitTryStatement(ITryStatement operation, LocalBinder argument)
        {
            CatchBlock MakeCatch(ICatchClause catchClause)
            {
                ParameterExpression variable = VisitLocal(catchClause.ExceptionLocal);
                LocalBinder binder = argument.Child(catchClause.ExceptionLocal, variable);

                return Expression.MakeCatchBlock(
                    catchClause.CaughtType.GetCorrespondingType(),
                    variable,
                    catchClause.Handler.Accept(this, binder),
                    catchClause.Filter?.Accept(this, binder)
                );
            }

            return Expression.MakeTry(typeof(void),
                body: operation.Body.Accept(this, argument),
                @finally: operation.FinallyHandler?.Accept(this, argument),
                fault: null,
                handlers: operation.Catches.Select(MakeCatch)
            );
        }

        /// <inheritdoc />
        public override Expression VisitCatch(ICatchClause operation, LocalBinder argument)
        {
            throw Unexpected(operation, nameof(VisitTryStatement));
        }
    }
}
