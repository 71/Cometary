using System;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cometary.Tests
{
    using Attributes;
    using Extensions;

    /// <summary>
    /// Executes this method during compilation, and if it fails, sets its body to
    /// "throw new Exception()".
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class CTFIFactAttribute : Attribute, IMethodVisitor
    {
        MethodDeclarationSyntax IMethodVisitor.Visit(MethodInfo method, MethodDeclarationSyntax node)
        {
            object obj = method.IsStatic ? null : Activator.CreateInstance(method.DeclaringType);

            ParameterInfo[] parameters = method.GetParameters();
            object[] arguments = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];
                TypeInfo parameterType = parameter.ParameterType.GetTypeInfo();

                if (parameterType.IsAssignableFrom(typeof(MethodDeclarationSyntax).GetTypeInfo()))
                    arguments[i] = node;
                else if (parameterType.IsAssignableFrom(typeof(MethodInfo).GetTypeInfo()))
                    arguments[i] = method;
                else if (parameterType.IsAssignableFrom(typeof(ClassDeclarationSyntax).GetTypeInfo()))
                    arguments[i] = node.Parent as ClassDeclarationSyntax;
                else if (parameter.ParameterType == typeof(TypeInfo))
                    arguments[i] = method.DeclaringType.GetTypeInfo();
                else if (parameter.ParameterType == typeof(Type))
                    arguments[i] = method.DeclaringType;
            }

            node = node.WithParameterList(SyntaxFactory.ParameterList());

            try
            {
                method.Invoke(obj, arguments);
            }
            catch (TargetInvocationException e)
            {
                return node.WithExpressionBody(
                    SyntaxFactory.ArrowExpressionClause(
                        $"throw new Exception(\"Compile-time error: {e.InnerException}\")".Syntax<ExpressionSyntax>()));
            }

            return node.WithBody(SyntaxFactory.Block());
        }
    }
}
