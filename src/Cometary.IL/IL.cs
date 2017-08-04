using System;
using System.Reflection;
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
        internal static void AddPipelineComponent()
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
                    var invocationArgs = invocation.ArgumentsInEvaluationOrder;
                    object[] args = new object[invocationArgs.Length];

                    for (int i = 0; i < args.Length; i++)
                    {
                        var invocationArg = invocationArgs[i];

                        if (!invocationArg.ConstantValue.HasValue)
                            throw new DiagnosticException("", invocationArg.Syntax.GetLocation());

                        args[i] = invocationArg.ConstantValue.Value;
                    }

                    return;
                }

                Default: next();
            }

            CodeGeneratorContext.EmitPipeline += CodeGeneratorContext.ToComponent(Emit);
        }
    }
}
