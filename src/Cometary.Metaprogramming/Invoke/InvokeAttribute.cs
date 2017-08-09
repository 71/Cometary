using System;
using System.Collections.Generic;

namespace Cometary
{
    /// <summary>
    /// <para>
    ///   If set on an assembly, enables the <see cref="InvokeAttribute"/> on methods.
    /// </para>
    /// <para>
    ///  If set on a method, indicates that the marked method will be invoked during compilation.
    /// </para>
    /// <para>
    ///   The following types can be used as parameters:
    ///   <see cref="T:Microsoft.CodeAnalysis.IMethodSymbol"/>, <see cref="T:Microsoft.CodeAnalysis.INamedTypeSymbol"/>,
    ///   <see cref="T:System.Reflection.MethodInfo"/> and <see cref="T:System.Reflection.TypeInfo"/>.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = false)]
    [Obsolete("Not implemented yet, please do not use.")]
    public sealed class InvokeAttribute : CometaryAttribute
    {
        /// <inheritdoc />
        public override IEnumerable<CompilationEditor> Initialize()
        {
            return new CompilationEditor[] { new InvokeEditor() };
        }
    }
}