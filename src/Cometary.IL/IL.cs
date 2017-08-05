using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Semantics;

namespace Cometary
{
    /// <summary>
    ///   Provides the ability to print CIL instructions inline.
    /// </summary>
    public static partial class IL
    {
        /// <summary>
        /// 
        /// </summary>
        internal static bool HasBeenAdded;

        /// <summary>
        /// 
        /// </summary>
        internal static void EnsurePipelineComponentIsActive()
        {
            if (HasBeenAdded)
                return;

            HasBeenAdded = true;

            void Emit(CodeGeneratorContext context, IOperation expression, bool used, Action next)
            {
                // Check if we're emitting a method call matching an IL call signature.
                if (expression is IInvocationExpression invocation)
                {
                    var target = invocation.TargetMethod;

                    if (!target.IsStatic ||
                        target.ReturnType.MetadataName != "Void" ||
                        target.ContainingType?.MetadataName != nameof(IL))
                        goto Default;

                    MethodBase correspondingMethod = target.GetCorrespondingMethod();

                    if (correspondingMethod == null)
                        goto Default;

                    // We got this far, so we're emitting raw IL
                    // Compute every parameter
                    var invocationArgs = invocation.ArgumentsInSourceOrder;
                    object[] args = new object[invocationArgs.Length];

                    for (int i = 0; i < args.Length; i++)
                    {
                        var invocationArg = invocationArgs[i];

                        if (!invocationArg.ConstantValue.HasValue)
                            throw new DiagnosticException("The given value must be a constant.", invocationArg.Syntax.GetLocation());

                        args[i] = invocationArg.ConstantValue.Value;
                    }

                    // Get the emitted opcode
                    OpCode opcode = OpCodes.Nop;
                    object operand = null;

                    EmitCore = (oc, op) => {
                        opcode = oc;
                        operand = op;
                    };

                    correspondingMethod.Invoke(null, args);

                    // Emit opcode
                    context.EmitOpCode((ILOpCode)opcode.Value);

                    return;
                }

                Default: next();
            }

            CodeGeneratorContext.EmitPipeline += CodeGeneratorContext.ToComponent(Emit);
        }

        private static Action<OpCode, object> EmitCore;

        /// <summary>
        ///   Emits the given <see cref="Instruction"/>. If no handler is here to
        /// </summary>
        private static void Emit(OpCode opCode, object operand = null)
        {
            if (EmitCore == null)
                throw new InvalidOperationException("An IL method can only be invoked during compilation.");

            EmitCore(opCode, operand);
        }
    }
}
