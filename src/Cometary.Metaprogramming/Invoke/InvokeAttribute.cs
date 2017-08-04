using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;

using TypeInfo = System.Reflection.TypeInfo;

namespace Cometary
{
    /// <summary>
    /// <para>
    ///   If set on an <see cref="Assembly"/>, enables the <see cref="InvokeAttribute"/> on methods.
    /// </para>
    /// <para>
    ///  If set on a <see cref="MethodInfo"/>, indicates that the marked method will be invoked during compilation.
    /// </para>
    /// <para>
    ///   The following types can be used as parameters:
    ///   <see cref="IMethodSymbol"/>, <see cref="INamedTypeSymbol"/>,
    ///   <see cref="MethodInfo"/> and <see cref="TypeInfo"/>.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = false)]
    public sealed class InvokeAttribute : CometaryAttribute
    {
        /// <inheritdoc />
        public override IEnumerable<CompilationEditor> Initialize()
        {
            return new CompilationEditor[] { new InvokeEditor() };
        }
    }
}