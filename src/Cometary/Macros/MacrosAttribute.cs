using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Semantics;

namespace Cometary.Macros
{
    /// <summary>
    ///   Indicates that the marked assembly will have its macros lowered.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class MacrosAttribute : CometaryAttribute
    {
        /// <inheritdoc />
        public override IEnumerable<CompilationEditor> Initialize()
        {
            return new CompilationEditor[] { new Editor() };
        }

        private sealed class Editor : CompilationEditor
        {
            /// <inheritdoc />
            public override void Initialize(CSharpCompilation compilation, CancellationToken cancellationToken)
            {
                RegisterEdit(EditCall);
            }

            /// <summary>
            /// 
            /// </summary>
            private static IOperation EditCall(IOperation operation, CancellationToken cancellationToken)
            {
                IEnumerable<IOperation> Single(IOperation o)
                {
                    return new[] { o };
                }

                IEnumerable<IOperation> Variables(IVariableDeclarationStatement o)
                {
                    return o.Variables.Select(x => x.InitialValue);
                }

                // Find all expressions in the given statement.
                // If it's no statement, return directly.
                IEnumerable<IOperation> expressions;

                switch (operation.Kind)
                {
                    case OperationKind.VariableDeclarationStatement:
                        expressions = Variables((IVariableDeclarationStatement)operation);
                        break;
                    case OperationKind.SwitchStatement:
                        expressions = Single(((ISwitchStatement)operation).Value);
                        break;
                    case OperationKind.IfStatement:
                        expressions = Single(((IIfStatement)operation).Condition);
                        break;
                    case OperationKind.LoopStatement:
                        switch (((ILoopStatement)operation).LoopKind)
                        {
                            case LoopKind.For:
                                IForLoopStatement forLoop = (IForLoopStatement)operation;
                                expressions = forLoop.Before.Concat(forLoop.AtLoopBottom).Concat(new[] { forLoop.Condition });
                                break;
                            case LoopKind.ForEach:
                                expressions = Single(((IForEachLoopStatement)operation).Collection);
                                break;
                            case LoopKind.WhileUntil:
                                expressions = Single(((IWhileUntilLoopStatement)operation).Condition);
                                break;
                            default:
                                return operation;
                        }
                        break;
                    case OperationKind.ThrowStatement:
                        expressions = Single(((IThrowStatement)operation).ThrownObject);
                        break;
                    case OperationKind.ReturnStatement:
                    case OperationKind.YieldReturnStatement:
                        expressions = Single(((IReturnStatement)operation).ReturnedValue);
                        break;
                    case OperationKind.LockStatement:
                        expressions = Single(((ILockStatement)operation).LockedObject);
                        break;
                    case OperationKind.UsingStatement:
                        IUsingStatement usingStatement = (IUsingStatement)operation;
                        expressions = Single(usingStatement.Value) ?? Variables(usingStatement.Declaration);
                        break;
                    case OperationKind.ExpressionStatement:
                        expressions = Single(((IExpressionStatement)operation).Expression);
                        break;
                    case OperationKind.FixedStatement:
                        expressions = Variables(((IFixedStatement)operation).Variables);
                        break;
                    default:
                        return operation;
                }

                // We have the statement and the expressions, visit all calls
                foreach (var expression in expressions)
                {
                    if (expression.Kind != OperationKind.InvocationExpression)
                        continue;

                    bool IsMacroMethod(IMethodSymbol methodSymbol)
                    {
                        foreach (var attr in methodSymbol.GetAttributes())
                        {
                            if (attr.AttributeClass.MetadataName == nameof(ExpandAttribute))
                                return true;
                        }

                        return false;
                    }

                    IEnumerable<string> CheckErrors(IMethodSymbol methodSymbol)
                    {
                        if (!methodSymbol.IsStatic)
                            yield return "The target method must be static.";
                    }

                    // Check if it's a call to a macro method
                    var invocation = (IInvocationExpression)expression;
                    var target = invocation.TargetMethod;

                    if (!IsMacroMethod(target))
                        continue;

                    var errors = CheckErrors(target);
                    bool hasErrors = false;
                    LightList<Exception> exceptions = new LightList<Exception>();

                    foreach (string error in errors)
                    {
                        hasErrors = true;
                        exceptions.Add(new Exception(error));
                    }

                    if (hasErrors)
                        throw new DiagnosticException(
                            "Cannot call the specified method as a macro method.",
                            new AggregateException(exceptions.UnderlyingArray),
                            invocation.Syntax.GetLocation());

                    // It is a method, find it
                    MethodInfo method = target.GetCorrespondingMethod() as MethodInfo;

                    if (method == null)
                        throw new DiagnosticException("Cannot find corresponding method.", invocation.Syntax.GetLocation());

                    // Found it, make the arguments
                    ParameterInfo[] parameters = method.GetParameters();
                    object[] arguments = new object[parameters.Length];

                    for (int i = 0; i < arguments.Length; i++)
                    {
                        Type paramType = parameters[i].ParameterType;

                        arguments[i] = paramType.GetTypeInfo().IsValueType
                            ? Activator.CreateInstance(paramType)
                            : null;
                    }

                    // Set up the context
                    using (CallBinder.EnterContext(operation, invocation))
                    {
                        // Invoke the method
                        try
                        {
                            method.Invoke(null, arguments);
                        }
                        catch (Exception e)
                        {
                            throw new DiagnosticException("Exception thrown when invoking a macro.", e, invocation.Syntax.GetLocation());
                        }

                        // Return edited statement
                        // Actually not at all; we should build a new statement
                        // outta all the modified expressions, but also nobody got time for that.
                        return CallBinder.Statement;
                    }
                }

                return operation;
            }
        }
    }
}
