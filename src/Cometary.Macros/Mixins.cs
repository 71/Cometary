using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cometary.Macros
{
    /// <summary>
    ///   Provides mechanisms for using mixins.
    /// </summary>
    public static class Mixins
    {
        /// <summary>
        ///   Mixins the given <paramref name="expression"/> in the context of the call.
        /// </summary>
        [Expand]
        public static void Mixin(this string expression)
        {
            InvocationExpressionSyntax invocation = (InvocationExpressionSyntax)CallBinder.InvocationSyntax;

            LiteralExpressionSyntax argument = (invocation.Expression as MemberAccessExpressionSyntax)?.Expression as LiteralExpressionSyntax
                                            ?? invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression as LiteralExpressionSyntax;

            if (argument == null)
                throw new DiagnosticException($"The given parameter must be a literal string: {invocation.Expression}.", invocation.GetLocation());

            CallBinder.StatementSyntax = SyntaxFactory.ParseStatement(argument.Token.ValueText);
        }

        /// <summary>
        ///   Mixin the given <paramref name="expression"/> in the context of the call,
        ///   as a <typeparamref name="T"/> value.
        /// </summary>
        [Expand]
        public static T Mixin<T>(this string expression)
        {
            InvocationExpressionSyntax invocation = (InvocationExpressionSyntax)CallBinder.InvocationSyntax;

            LiteralExpressionSyntax argument = (invocation.Expression as MemberAccessExpressionSyntax)?.Expression as LiteralExpressionSyntax
                                            ?? invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression as LiteralExpressionSyntax;

            if (argument == null)
                throw new DiagnosticException("The given parameter must be a literal string.", invocation.GetLocation());

            CallBinder.InvocationSyntax = SyntaxFactory.ParseExpression(argument.Token.ValueText);

            return default(T);
        }
    }
}
