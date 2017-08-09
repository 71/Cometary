// Note: Scry <https://github.com/6A/Scry> is required to run this script.
#load "../ScriptUtils.csx"

using System.Text;

using Type = System.Type;
using TypeInfo = System.Reflection.TypeInfo;

AutoWriteIndentation = true;

// Write header
Context
    .WriteUsings("System", "System.Reflection", "System.Collections.Generic", "System.Collections.Immutable")
    .WriteUsings("Microsoft.CodeAnalysis", "Microsoft.CodeAnalysis.CSharp", "Microsoft.CodeAnalysis.Semantics", "Microsoft.CodeAnalysis.CSharp.Syntax")
    .WriteLine()
    .WriteNamespace("Cometary")

    .WriteLine("partial class SourceSymbolFactory")
    .WriteLine('{')
    .IncreaseIndentation(4);

var assembly = typeof(CSharpCompilation).Assembly;
int processedMethods = 0;

string[] blacklist = { "ToString", "Equals", "GetHashCode", "GetType", "Finalize", "MemberwiseClone", "WrapWithVariablesIfAny", "BuildArgumentsForErrorRecovery" };

// Find all public methods
foreach (var method in from type in assembly.DefinedTypes
                       where type.Namespace == "Microsoft.CodeAnalysis.CSharp.Symbols" && type.IsSealed && !type.Name.Contains("<")
                       from method in type.DeclaredMembers.OfType<MethodBase>()
                       where (method.IsStatic && method.IsPublic && method.Name.StartsWith("Create")) || method.IsConstructor
                       where !method.Name.Contains("<") && !method.ContainsGenericParameters
                       select method)
{
    Type returnType = method.IsConstructor ? method.DeclaringType : ((MethodInfo)method).ReturnType;
    string methodName = method.IsConstructor ? $"Create{returnType.Name}" : method.Name;

    Type publicReturnType = FindFirstPublicAncestor(returnType);
    ParameterInfo[] parameters = method.GetParameters();
    var (paramsStr, argsStr) = MakeParametersString(parameters);

    End:
    StringBuilder callBuilder = new StringBuilder();
    string returnName = GetFriendlyName(publicReturnType);

    if (publicReturnType != typeof(void))
        callBuilder.AppendFormat("({0})", returnName);

    string fullTypeStr = $"CSharpAssembly.GetType(\"{method.DeclaringType.FullName}\")";
    string nameStr = method.IsConstructor ? $", nameof({methodName})" : string.Empty; 

    if (parameters.Length > 0)
        callBuilder.AppendFormat("Invoke({0}, {1}{2}, {3})", processedMethods++, fullTypeStr, nameStr, argsStr);
    else
        callBuilder.AppendFormat("Invoke({0}, {1}{2})", processedMethods++, fullTypeStr, nameStr);

    WriteLine($"public static {returnName} {methodName}({paramsStr}) => {callBuilder};");
}

// Write footer
Context
    .DecreaseIndentation(4)
    .WriteLine('}')

    .WriteEnd();