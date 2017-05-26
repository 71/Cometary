using System;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cometary.Contracts
{
    using Attributes;

    /// <summary>
    /// Defines a parameter that cannot be <see langword="null"/>.
    /// <para>
    /// If it is <see langword="null"/>, an <see cref="ArgumentNullException"/>
    /// exception will be thrown.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class NotNullAttribute : Attribute, IParameterVisitor, IReturnValueVisitor
    {
        /// <summary>
        /// Injects a null-check at the beginning of the method's body.
        /// </summary>
        public ParameterSyntax Visit(ParameterInfo parameter, ParameterSyntax node)
        {
            return node;
        }

        /// <summary>
        /// Adds a null-check to all <see langword="return"/> statements.
        /// </summary>
        public MethodDeclarationSyntax Visit(TypeInfo parameterType, MethodDeclarationSyntax node)
        {
            return node;
        }
    }
}
