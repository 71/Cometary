// Note: Scry <https://github.com/6A/Scry> is required to run this script.

using System.Text;

using Type = System.Type;
using TypeInfo = System.Reflection.TypeInfo;

// Utils
string GetFriendlyName(Type type)
{
    string name = type.Name;

    var genArgs = type.GetGenericArguments();

    if (genArgs.Length != 0)
    {
        name = name.Substring(0, name.Length - 2);
        name += $"<{string.Join(", ", genArgs.Select(x => GetFriendlyName(FindFirstPublicAncestor(x))))}>";
    }

    if (type.IsArray)
        name += "[]";

    return name;
}

Type FindFirstPublicAncestor(Type type)
{
    foreach (var interf in type.GetInterfaces())
    {
        if (interf.IsNotPublic)
            continue;

        if (interf.Name.EndsWith("Symbol") ||
            interf.Name.EndsWith("Expression") ||
            interf.Name.EndsWith("Statement"))
            return interf;
    }

    while (!type.IsPublic)
    {
        type = type.BaseType;
    }

    return type;
}

AutoWriteIndentation = true;

// Write header
Context
    .WriteUsings("System", "System.Reflection", "System.Collections.Immutable")
    .WriteUsings("Microsoft.CodeAnalysis", "Microsoft.CodeAnalysis.CSharp", "Microsoft.CodeAnalysis.Semantics", "Microsoft.CodeAnalysis.Text")
    .WriteLine()
    .WriteNamespace("Cometary")

    .WriteLine("partial class SymbolExtensions")
    .WriteLine('{')
    .IncreaseIndentation(4);

// Find all "SourceSymbol" types
foreach (TypeInfo type in from type in typeof(CSharpCompilation).Assembly.DefinedTypes
                          where type.Name.StartsWith("Bound")
                          select type)
{
    Type publicType = FindFirstPublicAncestor(type.AsType());

    if (publicType == typeof(object))
        continue;

    string friendlyName = GetFriendlyName(publicType);

    // Find all "Update" methods
    foreach (var method in from method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                           where method.Name == "Update"
                           select method)
    {
        StringBuilder pStr = new StringBuilder();

        foreach (var param in method.GetParameters())
        {
            Type paramPublicType = FindFirstPublicAncestor(param.ParameterType);

            pStr.AppendFormat(", {0} @{1}", GetFriendlyName(paramPublicType), param.Name);
        }

        string paramsStr = pStr.ToString();

        if (Output.ToString().Contains(paramsStr))
            continue;

        WriteLine($"public static {friendlyName} Update(this {friendlyName} self{pStr}) => self;");
    }
}

// Write footer
Context
    .DecreaseIndentation(4)
    .WriteLine('}')

    .WriteEnd();