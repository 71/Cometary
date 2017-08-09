using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        private struct Disposable : IDisposable
        {
            public void Dispose()
            {
                InvocationSymbol = null;
                StatementSymbol = null;

                InvocationSyntax = null;
                StatementSyntax = null;

                Monitor.Exit(syncObject);
            }
        }

        private static readonly object syncObject = new object();

        internal static IDisposable EnterContext(IInvocationExpression invocation, Lazy<IOperation> statement,
            InvocationExpressionSyntax invocationSyntax, StatementSyntax statementSyntax)
        {
            InvocationSymbol = new Lazy<IInvocationExpression>(() => invocation); // Lazy for consistency.
            StatementSymbol = statement;

            InvocationSyntax = invocationSyntax;
            StatementSyntax = statementSyntax;

            Monitor.Enter(syncObject);

            return new Disposable();
        }

        internal static (ExpressionSyntax Expr, StatementSyntax Stmt) Result => (InvocationSyntax, StatementSyntax);
        #endregion

        /// <summary>
        ///   Gets the expression representing the call to the currently
        ///   executing method.
        /// </summary>
        public static Lazy<IInvocationExpression> InvocationSymbol { get; private set; }

        /// <summary>
        ///   Gets the statement in which the <see cref="InvocationSymbol"/> is.
        /// </summary>
        public static Lazy<IOperation> StatementSymbol { get; private set; }

        /// <summary>
        ///   Gets or sets the <see cref="ExpressionSyntax"/> representing the call to the current method.
        /// </summary>
        public static ExpressionSyntax InvocationSyntax { get; set; }

        /// <summary>
        /// <para>
        ///   Gets or sets the syntax of the statement in which the <see cref="InvocationSyntax"/> is.
        /// </para>
        /// <para>
        ///   If this value is changed, then the value of the <see cref="InvocationSyntax"/> property will
        ///   be ignored, and the whole statement replaced.
        /// </para>
        /// </summary>
        public static StatementSyntax StatementSyntax { get; set; }
    }
}
