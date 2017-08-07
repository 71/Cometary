using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Semantics;

namespace Cometary
{
    /// <summary>
    ///   Provides the ability to print CIL instructions inline.
    /// </summary>
    public static partial class IL
    {
        private static readonly PipelineComponent<EmitDelegate> _component = CodeGeneratorContext.ToComponent(Emit);
        private static int _activeEditors;

        private static void Emit(CodeGeneratorContext context, IOperation expression, bool used, Action next)
        {
            // Check if we're emitting a method call matching an IL call signature.
            if (expression is IInvocationExpression invocation)
            {
                var target = invocation.TargetMethod;

                if (!target.IsStatic ||
                    target.ReturnType.MetadataName != "Void" ||
                    target.ContainingType?.MetadataName != nameof(IL))
                    goto Default;

                // We got this far, so we're emitting raw IL
                // Find the opcode
                int start = 0;
                var invocationArgs = invocation.ArgumentsInSourceOrder;

                if (!Enum.TryParse(target.Name, out ILOpCode opcode))
                {
                    if (target.Name != nameof(Emit))
                        goto Default;

                    opcode = (ILOpCode)invocationArgs[0].Value.ConstantValue.Value;
                    start = 1;
                }

                // Find the (optional) argument
                if (invocationArgs.Length == start)
                {
                    // No argument, emit the opcode and return
                    context.EmitOpCode(opcode);

                    // Or there is a type parameter
                    if (target.IsGenericMethod)
                        context.EmitSymbolToken(target.TypeArguments[0], invocation.Syntax);

                    return;
                }

                // There is an argument, check it and emit it.
                var arg = invocationArgs[start];
                var val = arg.Value;

                if (val.ConstantValue.HasValue)
                {
                    // Constant value (not including typeof)
                    context.EmitOpCode(opcode);

                    object constantValue = val.ConstantValue.Value;

                    switch (opcode)
                    {
                        //case ILOpCode.Br:
                        //case ILOpCode.Brfalse:
                        //case ILOpCode.Brfalse_s:
                        //case ILOpCode.Brtrue:
                        //case ILOpCode.Brtrue_s:
                        //case ILOpCode.Br_s:
                            // Would be nice to allow the user to specify a label by string
                            //if (constantValue is string str)
                            //    context.EmitConstant(context.)
                            //break;
                        default:
                            context.EmitRawConstant(constantValue);
                            break;
                    }

                    return;
                }

                if (val is IInvocationExpression call)
                {
                    // Call: emit method token
                    context.EmitOpCode(opcode);
                    context.EmitSymbolToken(call.TargetMethod, call.Syntax);

                    return;
                }

                if (val is IFieldReferenceExpression field)
                {
                    // Field reference: emit field token
                    context.EmitOpCode(opcode);
                    context.EmitSymbolToken(field.Field, field.Syntax);

                    return;
                }

                if (val is IPropertyReferenceExpression property)
                {
                    // Property reference: emit property getter
                    context.EmitOpCode(opcode);
                    context.EmitSymbolToken(property.Property.GetMethod, property.Syntax);

                    return;
                }

                if (val is IAssignmentExpression assignement && assignement.Target is IPropertyReferenceExpression prop)
                {
                    // Property reference: emit property getter
                    context.EmitOpCode(opcode);
                    context.EmitSymbolToken(prop.Property.SetMethod, prop.Syntax);

                    return;
                }

                if (val is ITypeOfExpression type)
                {
                    // Type reference: emit type
                    context.EmitOpCode(opcode);
                    context.EmitSymbolToken(type.TypeOperand, type.Syntax);

                    return;
                }

                // Throwing here is bad, but we don't have much of a choice.
                // Instead, Emit() throws, so we'll have a runtime exception.
            }

            Default: next();
        }

        /// <summary>
        /// 
        /// </summary>
        internal static int ActiveEditors
        {
            get => _activeEditors;

            set
            {
                int active = _activeEditors;

                Debug.Assert(value == active + 1 || value == active - 1);

                if (value == active + 1)
                {
                    Interlocked.Increment(ref _activeEditors);
                }
                else if (value == active - 1)
                {
                    Interlocked.Decrement(ref _activeEditors);
                }

                switch (value)
                {
                    case 0:
                        CodeGeneratorContext.EmitPipeline -= _component;
                        break;
                    case 1:
                        CodeGeneratorContext.EmitPipeline += _component;
                        break;
                }
            }
        }

        /// <summary>
        ///   Emits the given <paramref name="opcode"/>, and its (optional) <paramref name="operand"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">An IL method can only be invoked during compilation.</exception>
        public static void Emit(ILOpCode opcode, object operand = null)
        {
            throw new InvalidOperationException("An IL method can only be invoked during compilation.");
        }
    }
}
