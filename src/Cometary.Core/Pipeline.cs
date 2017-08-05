using System;
using System.Collections.Generic;
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

    /// <summary>
    /// 
    /// </summary>
    [SuppressMessage("Compiler", "CS0702", Justification = "That constraint is annoying.")]
    internal sealed class Pipeline<TDelegate> where TDelegate : Delegate
    {
        private readonly LightList<PipelineComponent<TDelegate>> Components;

        public Pipeline()
        {
            Components = new LightList<PipelineComponent<TDelegate>>();
        }

        public TDelegate MakeDelegate(TDelegate @default)
        {
            var components = Components.UnderlyingArray;
            var next = @default;

            for (int i = components.Length - 1; i >= 0; i--)
            {
                next = components[i](next);
            }

            return next;
        }

        public void Add(PipelineComponent<TDelegate> component)
        {
            Components.Add(component);
        }

        public void Remove(PipelineComponent<TDelegate> component)
        {
            Components.Remove(component);
        }
    }
}
