using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Semantics;

namespace Cometary.Macros
{
    /// <summary>
    ///   Provides access to the <see cref="IOperation"/> that called
    ///   the currently executing method.
    /// </summary>
    public static class CallBinder
    {
        #region Internal
        internal struct Context
        {
            
        }

        private struct Disposable : IDisposable
        {
            public void Dispose()
            {
                Monitor.Exit(syncObject);
            }
        }

        //
        private static readonly object syncObject = new object();

        //
        private static Context context;

        /// <summary>
        /// 
        /// </summary>
        internal static IDisposable EnterContext(IOperation statement, IInvocationExpression expression)
        {
            Expression = expression;
            Statement = statement;

            context = new Context();

            Monitor.Enter(syncObject);

            return new Disposable();
        }

        /// <summary>
        /// 
        /// </summary>
        internal static Context GetContext() => context;
        #endregion


        /// <summary>
        ///   Gets or sets the expression representing the call to the currently
        ///   executing method.
        /// </summary>
        public static IInvocationExpression Expression { get; set; }

        /// <summary>
        ///   Gets or sets the statement in which the <see cref="Expression"/> is.
        /// </summary>
        public static IOperation Statement { get; set; }
    }
}
