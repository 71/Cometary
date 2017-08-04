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
}