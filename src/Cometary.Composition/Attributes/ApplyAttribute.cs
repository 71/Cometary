using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cometary.Composition
{
    /// <summary>
    ///   Indicates that this class should implement one or more components.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ApplyAttribute : CompositionAttribute
    {
        /// <summary>
        ///   Creates a new <see cref="ApplyAttribute"/>, specifying
        ///   the types of the <see cref="Component"/> that is to be applied.
        /// </summary>
        public ApplyAttribute(Type componentType) : base(new ApplyingComponent(componentType))
        {
        }

        private sealed class ApplyingComponent : Component
        {
            private readonly Type componentType;

            public ApplyingComponent(Type componentType)
            {
                this.componentType = componentType;
            }

            public override ClassDeclarationSyntax Apply(ClassDeclarationSyntax node, INamedTypeSymbol symbol, CancellationToken cancellationToken)
            {
                return Activator.CreateInstance(componentType) is Component component
                    ? component.Apply(node, symbol, cancellationToken)
                    : node;
            }
        }
    }
}
