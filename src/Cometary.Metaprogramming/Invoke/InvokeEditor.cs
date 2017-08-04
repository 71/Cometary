using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using TypeInfo = System.Reflection.TypeInfo;

namespace Cometary
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class InvokeEditor : CompilationEditor
    {
        /// <inheritdoc />
        public override void Initialize(CSharpCompilation compilation, CancellationToken cancellationToken)
        {
            RegisterEdit(new Edit<ISymbol>(EditSymbol));
        }

        private ISymbol EditSymbol(ISymbol symbol, CancellationToken cancellationToken)
        {
            // Only check methods
            if (!(symbol is IMethodSymbol method))
                return symbol;

            // Find Invoke attribute
            var attr = method.GetAttributes().FirstOrDefault(x => x.AttributeClass.Name == nameof(InvokeAttribute));

            if (attr == null)
                // No attribute found, return
                return symbol;

            // We have the attribute, find its matching method
            var info = method.GetCorrespondingMethod() as MethodInfo;

            if (info == null)
            {
                ReportWarning("Cannot invoke given method", symbol.Locations[0]);
                return symbol;
            }

            // Check method
            if (!method.IsStatic)
                throw new DiagnosticException("A compile-time function must be static.", symbol.Locations[0]);
            if (method.IsAbstract)
                throw new DiagnosticException("A compile-time function cannot be abstract.", symbol.Locations[0]);
            if (method.ReturnType.MetadataName != "Void")
                ReportWarning("A compile-time function should return void.", symbol.Locations[0]);

            // Populate args
            ParameterInfo[] parameters = info.GetParameters();
            object[] arguments = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];
                TypeInfo parameterType = parameter.ParameterType.GetTypeInfo();

                if (parameterType.IsAssignableFrom(typeof(IMethodSymbol).GetTypeInfo()))
                    arguments[i] = method;
                else if (parameterType.IsAssignableFrom(typeof(MethodInfo).GetTypeInfo()))
                    arguments[i] = info;
                else if (parameter.ParameterType.IsAssignableFrom(typeof(ITypeSymbol)))
                    arguments[i] = symbol.ContainingType;
                else if (parameter.ParameterType == typeof(TypeInfo))
                    arguments[i] = info.DeclaringType.GetTypeInfo();
                else if (parameter.ParameterType == typeof(Type))
                    arguments[i] = info.DeclaringType;
            }

            // Invoke and return
            try
            {
                info.Invoke(null, arguments);
            }
            catch (Exception e)
            {
                ReportError(e.Message, symbol.Locations[0]);
            }

            return symbol;
        }
    }
}
