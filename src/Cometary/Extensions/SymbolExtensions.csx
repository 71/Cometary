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

IEnumerable<Type> FindValidInterfaces(Type type)
{
    return type.GetInterfaces().Where(x =>
    {
        string name = x.Name;

        return x.IsPublic &&
            (name.EndsWith("Symbol") ||
             name.EndsWith("Expression") ||
             name.EndsWith("Statement") ||
             name.EndsWith("Operation"));
    });
}

Type FindFirstPublicAncestor(Type type)
{
    if (type.GenericTypeArguments.Length > 0)
    {
        return type.GetGenericTypeDefinition()
            .MakeGenericType(type.GenericTypeArguments.Select(FindFirstPublicAncestor).ToArray());
    }

    foreach (var interf in FindValidInterfaces(type).OrderByDescending(x => x.GetInterfaces().Length))
    {
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

    .WriteLine("public static partial class UpdateExtensions")
    .WriteLine('{')
    .IncreaseIndentation(4);

// Find all "SourceSymbol" types
List<MethodInfo> doneMethods = new List<MethodInfo>();

foreach (TypeInfo type in from type in typeof(CSharpCompilation).Assembly.DefinedTypes
                          where (type.Name.StartsWith("Bound") || type.Name.EndsWith("Symbol")) && !type.IsInterface
                          select type)
{
    Type publicType = FindFirstPublicAncestor(type.AsType());

    if (publicType == typeof(object) || publicType == typeof(IOperation))
        continue;

    string friendlyName = GetFriendlyName(publicType);

    // Find all "Update" methods
    foreach (var method in from method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                           where method.Name == "Update" && !method.IsAbstract
                           select method)
    {
        StringBuilder pStr = new StringBuilder();

        foreach (var param in method.GetParameters())
        {
            Type paramPublicType = FindFirstPublicAncestor(param.ParameterType);

            pStr.AppendFormat(", {0} @{1}", GetFriendlyName(paramPublicType), param.Name);

            if (param.HasDefaultValue)
            {
                pStr.AppendFormat(" = {0}", param.DefaultValue);
            }
        }

        string paramsStr = pStr.ToString();

        if (Output.ToString().Contains(paramsStr))
            continue;

        string fullString = $"public static {friendlyName} Update(this {friendlyName} self{pStr}) => self;";

        if (doneMethods.Any(x => x.DeclaringType.BaseType == method.DeclaringType || x.DeclaringType == method.DeclaringType.BaseType))
            continue;

        doneMethods.Add(method);
        WriteLine(fullString);
    }
}

// Write footer
Context
    .DecreaseIndentation(4)
    .WriteLine('}')

    .WriteEnd();