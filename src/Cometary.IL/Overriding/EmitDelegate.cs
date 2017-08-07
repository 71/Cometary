using System;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace Cometary
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="expression"></param>
    public delegate void EmitDelegate(CodeGeneratorContext context, IOperation expression, bool used);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="operation"></param>
    /// <param name="used"></param>
    /// <param name="next"></param>
    public delegate void AlternateEmitDelegate(CodeGeneratorContext context, IOperation operation, bool used, Action next);
}