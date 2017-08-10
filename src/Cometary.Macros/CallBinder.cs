using System;
using System.Reflection;
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
                inContext = false;

                invocationSymbol = null;
                statementSymbol = null;

                expressionSyntax = null;
                statementSyntax = null;

                callerInfo = null;
                callerSymbol = null;

                methodInfo = null;
                methodSymbol = null;

                Monitor.Exit(syncObject);
            }
        }

        private static bool inContext;
        private static readonly object syncObject = new object();

        internal static IDisposable EnterContext(IInvocationExpression invocation, Lazy<IOperation> statement,
            InvocationExpressionSyntax invocationSyntax, StatementSyntax stmtSyntax,
            MethodInfo mi, IMethodSymbol ms, Lazy<MethodInfo> callerMi, Lazy<IMethodSymbol> callerMs)
        {
            inContext = true;

            invocationSymbol = invocation;
            statementSymbol = statement;

            expressionSyntax = invocationSyntax;
            statementSyntax = stmtSyntax;

            callerInfo = callerMi;
            callerSymbol = callerMs;

            methodInfo = mi;
            methodSymbol = ms;

            Monitor.Enter(syncObject);

            return new Disposable();
        }

        internal static (ExpressionSyntax Expr, StatementSyntax Stmt) Result => (InvocationSyntax, StatementSyntax);
        #endregion

        private static Lazy<IOperation> statementSymbol;
        private static Lazy<MethodInfo> callerInfo;
        private static Lazy<IMethodSymbol> callerSymbol;
        private static IInvocationExpression invocationSymbol;
        private static ExpressionSyntax expressionSyntax;
        private static StatementSyntax statementSyntax;
        private static MethodInfo methodInfo;
        private static IMethodSymbol methodSymbol;

        private static InvalidOperationException NotInContext => new InvalidOperationException("Cannot call CallBinder methods outside of an Expand method during compilation.");


        /// <summary>
        ///   Gets the expression representing the call to the currently
        ///   executing method.
        /// </summary>
        public static IInvocationExpression InvocationSymbol => inContext ? invocationSymbol : throw NotInContext;

        /// <summary>
        ///   Gets the statement in which the <see cref="InvocationSymbol"/> is.
        /// </summary>
        public static IOperation StatementSymbol => inContext ? statementSymbol.Value : throw NotInContext;

        /// <summary>
        ///   Gets the <see cref="MethodInfo"/> describing the target of the call.
        /// </summary>
        public static MethodInfo TargetInfo => inContext ? methodInfo : throw NotInContext;

        /// <summary>
        ///   Gets the <see cref="IMethodSymbol"/> that represents the target of the call.
        /// </summary>
        public static IMethodSymbol TargetSymbol => inContext ? methodSymbol : throw NotInContext;

        /// <summary>
        ///   Gets the <see cref="MethodInfo"/> describing the method in which the call to the current method was made.
        /// </summary>
        public static MethodInfo CallerInfo => inContext ? callerInfo.Value : throw NotInContext;

        /// <summary>
        ///   Gets the <see cref="IMethodSymbol"/> that represents the method in which the call of the current method was made.
        /// </summary>
        public static IMethodSymbol CallerSymbol => inContext ? callerSymbol.Value : throw NotInContext;

        /// <summary>
        ///   Gets or sets the <see cref="ExpressionSyntax"/> representing the call to the current method.
        /// </summary>
        public static ExpressionSyntax InvocationSyntax
        {
            get => inContext ? expressionSyntax : throw NotInContext;
            set
            {
                if (!inContext) throw NotInContext;

                expressionSyntax = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// <para>
        ///   Gets or sets the syntax of the statement in which the <see cref="InvocationSyntax"/> is.
        /// </para>
        /// <para>
        ///   If this value is changed, then the value of the <see cref="InvocationSyntax"/> property will
        ///   be ignored, and the whole statement replaced.
        /// </para>
        /// </summary>
        public static StatementSyntax StatementSyntax
        {
            get => inContext ? statementSyntax : throw NotInContext;
            set
            {
                if (!inContext) throw NotInContext;

                statementSyntax = value ?? throw new ArgumentNullException(nameof(value));
            }
        }
    }
}
