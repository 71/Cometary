using System;
using System.Diagnostics.CodeAnalysis;

namespace Cometary
{
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
