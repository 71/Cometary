using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Semantics;

namespace Cometary.Expressions
{
    internal sealed partial class ExpressionParsingVisitor : OperationVisitor<LocalBinder, Expression>
    {
        private static Exception NotSupported(IOperation operation)
            => new UnsupportedNodeException(operation);
        private static Exception Unexpected(IOperation operation, string visitingMethod)
            => new UnsupportedNodeException(operation, $"Unexpected node visited. It should have been processed by {visitingMethod} earlier.");

        private readonly LocalBinder RootBinder;
        private readonly LabelTarget ReturnTarget;

        public ExpressionParsingVisitor()
        {
            RootBinder = LocalBinder.Empty;
            ReturnTarget = Expression.Label("End");
        }

        private Expression Convert(Expression node, Type target)
        {
            if (node.Type == target)
                return node;

            return Expression.Convert(node, target);
        }

        public override Expression DefaultVisit(IOperation operation, LocalBinder argument) => Expression.Empty();
        public override Expression Visit(IOperation operation, LocalBinder argument) => operation.Accept(this, argument);
    }
}
