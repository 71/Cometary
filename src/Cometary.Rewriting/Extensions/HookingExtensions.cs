using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary
{
    using Attributes;

    /// <summary>
    ///   Defines a set of static methods used to add various hooks
    ///   to the Cometary process.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static class HookingExtensions
    {
        public const string DispatcherOptionsKey = "DISPATCHER_REWRITING";
        public const string AttributesOptionsKey = "ATTRIBUTES_REWRITING";

        /// <summary>
        /// <para>
        ///   Sets the given <paramref name="dispatcher"/> to handle rewriting.
        /// </para>
        /// <para>
        ///   Unless specified, a default internal <see cref="IDispatcher"/> will be used.
        /// </para>
        /// </summary>
        public static CometaryConfigurator SetDispatcher(this CometaryConfigurator config, IDispatcher dispatcher)
        {
            if (dispatcher == null)
                throw new ArgumentNullException(nameof(dispatcher));

            return config.AddData(DispatcherOptionsKey, dispatcher);
        }

        /// <summary>
        ///   Adds the given <paramref name="rewriters"/> to the rewriting pipeline.
        /// </summary>
        public static CometaryConfigurator Rewrite(this CometaryConfigurator config, params AssemblyRewriter[] rewriters)
        {
            return config.AddHook(Hook);

            void Hook(CometaryState state)
            {
                IDispatcher dispatcher = state.Options.GetOrDefault(DispatcherOptionsKey, new DefaultDispatcher());

                state.ChangeSyntaxTrees(syntaxTree => dispatcher.Dispatch(syntaxTree, rewriters));
            }
        }

        /// <summary>
        ///   Ensures all attributes that implement <see cref="ICometaryVisitor"/> are
        ///   called accordingly.
        /// </summary>
        public static CometaryConfigurator HandleAttributes(this CometaryConfigurator config)
        {
            return config
                .Rewrite(new AttributesVisitor())
                .AddData(AttributesOptionsKey, true)
                .AddHook(Hook);

            void Hook(CometaryState state)
            {
                Assembly assembly = state.Assembly;
                CSharpCompilation compilation = state.Compilation;

                // ReSharper disable once SuspiciousTypeConversion.Global
                foreach (IAssemblyVisitor visitor in assembly.GetCustomAttributes().OfType<IAssemblyVisitor>())
                {
                    CSharpCompilation newCompilation = visitor.Visit(assembly, compilation);

                    if (newCompilation != null)
                        compilation = newCompilation;
                }

                state.Compilation = compilation;
            }
        }
    }
}
