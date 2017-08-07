using System;
using System.Diagnostics.CodeAnalysis;

namespace Cometary
{
    /// <summary>
    ///   Represents a component in a pipeline.
    /// </summary>
    /// <typeparam name="TDelegate">The type of the <paramref name="next"/> <see cref="Delegate"/>.</typeparam>
    /// <param name="next">A <see cref="Delegate"/> that allows the continuation of the pipeline.</param>
    [SuppressMessage("Compiler", "CS0702", Justification = "That constraint is annoying.")]
    public delegate TDelegate PipelineComponent<TDelegate>(TDelegate next) where TDelegate : Delegate;
}