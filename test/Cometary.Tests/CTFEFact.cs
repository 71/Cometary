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
    public sealed class CTFEFactAttribute : Attribute, IMethodVisitor
    {
        MethodDeclarationSyntax IMethodVisitor.Visit(MethodInfo method, MethodDeclarationSyntax node)
        {
            object obj = method.IsStatic ? null : Activator.CreateInstance(method.DeclaringType);

            try
            {
                method.Invoke(obj, null);
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
