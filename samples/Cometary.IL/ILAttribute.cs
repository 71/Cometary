using System;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Cometary
{
    using Attributes;

    /// <summary>
    /// Defines a method that can emit raw IL codes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
    public sealed class ILAttribute : Attribute, IMethodVisitor
    {
        // TODO: Two types of emitting possible:
        // - Emit all at once if the method is marked "IL", in which case
        //   it must be static and parameter-less ;
        // - Allow emitting constants only.

        /// <inheritdoc />
        MethodDeclarationSyntax IMethodVisitor.Visit(MethodInfo method, MethodDeclarationSyntax node)
        {
            if (method.IsGenericMethod)
                throw new ProcessingException(node, "The given node cannot emit IL code, because it has generic parameters.");

            // Populate arguments to pass
            ParameterInfo[] parameters = method.GetParameters();
            object[] args = new object[parameters.Length];

            for (int i = 0; i < args.Length; i++)
                args[i] = parameters[i].ParameterType.GetTypeInfo().IsValueType ? Activator.CreateInstance(parameters[i].ParameterType) : null;

            // Create type to add
            object obj = null;

            if (!method.IsStatic)
            {
                Type declaringType = method.DeclaringType;

                if (declaringType.GenericTypeArguments.Length > 0)
                    throw new ProcessingException(node, "The given node cannot emit IL code, because its declaring type has generic parameters.");
                if (declaringType.GetTypeInfo().IsAbstract)
                    throw new ProcessingException(node, "The given node cannot emit IL code, because its declaring type is abstract.");

                ParameterInfo[] ctorParams = new ParameterInfo[0];
                ConstructorInfo ctor = null;

                // Find best ctor: ctor with the least parameters
                foreach (ConstructorInfo constructor in declaringType.GetTypeInfo().DeclaredConstructors)
                {
                    if (ctor == null)
                    {
                        ctor = constructor;
                        ctorParams = constructor.GetParameters();
                        continue;
                    }

                    ParameterInfo[] cmpCtorParams = constructor.GetParameters();

                    if (cmpCtorParams.Length < ctorParams.Length)
                    {
                        ctor = constructor;
                        ctorParams = cmpCtorParams;
                    }
                }

                // Invoke ctor
                object[] ctorArgs = new object[ctorParams.Length];

                for (int i = 0; i < ctorArgs.Length; i++)
                    ctorArgs[i] = ctorParams[i].ParameterType.GetTypeInfo().IsValueType ? Activator.CreateInstance(ctorParams[i].ParameterType) : null;

                obj = ctor.Invoke(ctorArgs);
            }

            // Let the method emit some code.
            try
            {
                method.Invoke(obj, args);
            }
            catch (TargetInvocationException e)
            {
                throw new ProcessingException(node, "Error encountered whilst calling the given method.", e.InnerException);
            }

            // Retrieve emitted instructions, and save 'em for later.
            Instruction[] emitted = IL.EmittedInstructions.ToArray();

            IL.Do(method, def =>
            {
                def.Body.Instructions.Clear();

                for (int i = 0; i < emitted.Length; i++)
                    def.Body.Instructions.Add(emitted[i]);
            });

            IL.EmittedInstructions.Clear();

            return node;
        }

        /// <summary>
        /// 
        /// </summary>
        private static void VisitInlinedIL(MethodDefinition method)
        {
            var instructions = method.Body.Instructions;

            for (int i = 0; i < instructions.Count; i++)
            {
                var instruction = instructions[i];

                if (instruction.OpCode.Code != Code.Call)
                    continue;

                MethodReference target = instruction.Operand as MethodReference;

                if (target == null ||
                    target.DeclaringType.FullName != typeof(IL).FullName ||
                    target.Name != nameof(IL.Emit))
                    continue;

                // Call to Emit, let's emit something.
                // We're a bit restrictive here: the arguments *must* be constants.

                var objectIns = instruction.Previous ?? throw new InvalidOperationException();
                var opcodeIns = instruction.Previous.Previous ?? throw new InvalidOperationException();


            }
        }
    }
}
