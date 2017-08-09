// Note: Scry <https://github.com/6A/Scry> is required to run this script.
#load "../ScriptUtils.csx"

using System.Text;

using Type = System.Type;
using TypeInfo = System.Reflection.TypeInfo;

AutoWriteIndentation = true;

// Write header
Context
    .WriteUsings("System", "System.Reflection", "System.Collections.Generic", "System.Collections.Immutable")
    .WriteUsings("Microsoft.CodeAnalysis", "Microsoft.CodeAnalysis.CSharp", "Microsoft.CodeAnalysis.Semantics", "Microsoft.CodeAnalysis.Text")
    .WriteLine()
    .WriteNamespace("Cometary")

    .WriteLine("partial class SyntheticSymbolFactory")
    .WriteLine('{')
    .IncreaseIndentation(4);

// Find all "SourceSymbol" types
string factoryTypeName = "Microsoft.CodeAnalysis.CSharp.SyntheticBoundNodeFactory";
Type factoryType = typeof(CSharpCompilation).Assembly.GetType(factoryTypeName);

string[] blacklist = { "ToString", "Equals", "GetHashCode", "GetType" };

// Find all public methods
foreach (var method in factoryType.GetMethods(BindingFlags.Instance | BindingFlags.Public))
{
    if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))
        continue;
    if (blacklist.Contains(method.Name))
        continue;

    WriteLine(MakeProxyMethod(method));
}

// Write footer
Context
    .DecreaseIndentation(4)
    .WriteLine('}')

    .WriteEnd();