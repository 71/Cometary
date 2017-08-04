using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Semantics;

namespace Cometary.Visiting
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class OperationTreeRewriter : OperationVisitor<object, IOperation>
    {
        private readonly CompilationEditor editor;

        public override IOperation Visit(IOperation operation, object argument)
        {
            //return editor.
            return operation;
        }
    }
}
