using System;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cometary.Attributes
{
    /// <summary>
    /// <para>
    ///   Indicates that the marked method will be invoked
    ///   during compilation.
    /// </para>
    /// <para>
    ///   The types can be used as parameters:
    ///   <see cref="MethodDeclarationSyntax"/>, <see cref="ClassDeclarationSyntax"/>,
    ///   <see cref="MethodInfo"/> and <see cref="TypeInfo"/>.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class InvokeAttribute : Attribute, IMethodVisitor
    {
        /// <summary>
        /// <para>
        ///   If <see langword="false"/>, the marked method will be removed
        ///   from the compiled <see cref="Assembly"/>.
        /// </para>
        /// <para>
        ///   Default: <see langword="false"/>.
        /// </para>
        /// </summary>
        public bool KeepMethod { get; set; }

        /// <summary>
        ///   Invokes the given <paramref name="method"/>.
        /// </summary>
        public MethodDeclarationSyntax Visit(MethodInfo method, MethodDeclarationSyntax node)
        {
            if (!method.IsStatic)
                throw new InvalidOperationException("A compile-time function must be static.");
            if (method.IsAbstract)
                throw new InvalidOperationException("A compile-time function cannot be abstract.");
            if (method.ReturnType != typeof(void))
                throw new InvalidOperationException("A compile-time function must return void.");

            ParameterInfo[] parameters = method.GetParameters();
            object[] arguments = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];
                TypeInfo parameterType  = parameter.ParameterType.GetTypeInfo();

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

            method.Invoke(null, arguments);

            return KeepMethod ? node : null;
        }
    }
}
