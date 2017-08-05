using System.Linq.Expressions;

namespace Cometary.Tests
{
    internal static class Utils
    {
        public static ConstantExpression AsExpression<T>(this T obj)
        {
            return Expression.Constant(obj, typeof(T));
        }

        public static TDelegate Compile<TDelegate>(this Expression expr, params ParameterExpression[] parameters)
        {
            expr = expr.ReduceExtensions();

            return Expression.Lambda<TDelegate>(
                Expression.Block(
                    Expression.DebugInfo(Expression.SymbolDocument("debug"), 1, 1, 1, 1),
                    expr
                ), parameters
            ).Compile();
        }
    }
}
