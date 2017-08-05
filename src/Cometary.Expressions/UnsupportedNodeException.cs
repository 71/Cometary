using System;
using Microsoft.CodeAnalysis;

namespace Cometary.Expressions
{
    /// <summary>
    /// Represents an error encountered by <see cref="Expressive"/>
    /// when trying to parse an unsupported <see cref="SyntaxNode"/>.
    /// </summary>
    public sealed class UnsupportedNodeException : DiagnosticException
    {
        public IOperation Operation { get; }

        internal UnsupportedNodeException(IOperation operation)
            : base($"The {operation.Kind} operation is not supported.", operation.Syntax.GetLocation())
        {
            Operation = operation;
        }

        internal UnsupportedNodeException(IOperation operation, string message)
            : base(message, operation.Syntax.GetLocation())
        {
            Operation = operation;
        }

        internal UnsupportedNodeException(IOperation operation, string message, Exception inner)
            : base(message, inner, operation.Syntax.GetLocation())
        {
            Operation = operation;
        }
    }
}
