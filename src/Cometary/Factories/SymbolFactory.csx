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

    .WriteLine("partial class SymbolFactory")
    .WriteLine('{')
    .IncreaseIndentation(4);

string binderTypeName = "Microsoft.CodeAnalysis.CSharp.Binder";
Type binderType = typeof(CSharpCompilation).Assembly.GetType(binderTypeName);

string[] blacklist = { "ToString", "Equals", "GetHashCode", "GetType", "Finalize", "MemberwiseClone", "WrapWithVariablesIfAny", "BuildArgumentsForErrorRecovery" };

// Find all public methods
foreach (var method in binderType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
{
    if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))
        continue;
    if (blacklist.Contains(method.Name))
        continue;
    if (method.Name.Contains("<"))
        continue;

    WriteLine(MakeProxyMethod(method));
}

// Write footer
Context
    .DecreaseIndentation(4)
    .WriteLine('}')

    .WriteEnd();