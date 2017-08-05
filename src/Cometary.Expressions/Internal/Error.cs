using System;

namespace Cometary.Expressions
{
    internal static class Error
    {
        internal static Exception ArgumentMustBeBoolean(string paramName) => new ArgumentException("The given argument must be a boolean.", paramName);
        internal static Exception LabelTypeMustBeVoid(string paramName) => new ArgumentException("The given label must return void.", paramName);
        internal static Exception ArgumentMustImplement<T>(string paramName) => new ArgumentException($"The given argument must implement {typeof(T).Name}.", paramName);
        internal static Exception ArgumentMustBeAssignableTo<T>(string paramName) => new ArgumentException($"The given argument must be assignable to {typeof(T).Name}.", paramName);
        internal static Exception ReturnTypeMustBeAssignableTo<T>(string paramName) => new ArgumentException($"The expression's return type be assignable to {typeof(T).Name}.", paramName);
    }
}
