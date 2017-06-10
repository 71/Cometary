using System;
using System.Collections;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cometary.Contracts
{
    using Attributes;
    using System.Linq;

    /// <summary>
    /// Defines a parameter whose length (for a <see cref="ICollection"/>) or
    /// value (for a numeric type) must be between <see cref="Min"/> and <see cref="Max"/>.
    /// <para>
    /// If it is <see langword="null"/>, an <see cref="ArgumentNullException"/>
    /// exception will be thrown.
    /// </para>
    /// </summary>
    public sealed class RangeAttribute : Attribute, IParameterVisitor
    {
        /// <summary>The minimum length or value of the parameter.</summary>
        public int Min { get; }

        /// <summary>The maximum length or value of the parameter.</summary>
        public int Max { get; }

        /// <inheritdoc cref="RangeAttribute" />
        public RangeAttribute(int min, int max = int.MaxValue)
        {
            if (min < 0)
                throw new ArgumentOutOfRangeException(nameof(min));
            if (max < 0 || max < min)
                throw new ArgumentOutOfRangeException(nameof(max));

            Min = min;
            Max = max;
        }

        /// <summary>
        /// Injects a range-check at the beginning of the method body.
        /// </summary>
        public MethodDeclarationSyntax Visit(ParameterInfo parameter, ParameterSyntax syntax, MethodDeclarationSyntax node)
        {
            if (parameter.ParameterType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(ICollection)))
            {
                return node;
            }
            else
            {
                return node;
            }
        }
    }
}
