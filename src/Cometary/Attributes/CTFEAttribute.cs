using System;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cometary.Attributes
{
    /// <summary>
    /// Indicates that the marked method will be called
    /// during compilation.
    /// <para>
    /// The following parameters can be defined:
    /// <see cref="MethodDeclarationSyntax"/>, <see cref="ClassDeclarationSyntax"/>,
    /// <see cref="MethodInfo"/> and <see cref="TypeInfo"/>.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class CTFEAttribute : Attribute, IMethodVisitor
    {
        /// <summary>
        /// Gets or sets whether or not the CTFE method should
        /// be kept after compilation.
        /// </summary>
        public bool KeepMethod { get; set; }

        /// <inheritdoc />
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
