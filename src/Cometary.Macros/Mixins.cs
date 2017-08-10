using Microsoft.CodeAnalysis.CSharp;

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
            CallBinder.InvocationSyntax = SyntaxFactory.ParseExpression(expression);
        }

        /// <summary>
        ///   Mixin the given <paramref name="expression"/> in the context of the call,
        ///   as a <typeparamref name="T"/> value.
        /// </summary>
        [Expand]
        public static T Mixin<T>(this string expression)
        {
            CallBinder.InvocationSyntax = SyntaxFactory.ParseExpression(expression);

            return default(T);
        }
    }
}
