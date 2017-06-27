using System;
using System.Linq;

AutoWriteIndentation = true;

var filters = new Dictionary<string, string>
{

};

string Name(Type type)
{
    if (type == null)
        return null;

    if (type.IsNotPublic)
        type = FindFirstPublicType(type);

    if (type == null)
        return null;

    string str = type.Name;

    if (type.IsGenericType)
    {
        int i = str.IndexOf('`');

        if (i != -1)
            str = str.Substring(0, i);

        str += '<';
        str += string.Join(", ", type.GetGenericArguments().Select(Name));
        str += '>';
    }
    else if (type.IsArray)
    {
        int i = str.IndexOf('`');

        if (i != -1)
            str = str.Substring(0, i);

        str += "[]";
    }

    return str;
}

Type FindFirstPublicType(Type type)
{
    var result = type.GetTypeInfo().ImplementedInterfaces
        .Where(x => x.IsPublic && (x.GetInterface("ISymbol") != null || x.GetInterface("IOperation") != null))
        .OrderBy(x => x.GetInterfaces().Count())
        .FirstOrDefault();

    if (result != null)
        return result;

    result = type;

    while (result.BaseType != null)
    {
        result = result.BaseType;

        if (result.IsPublic)
            return result;
    }

    return null;
}

Type symbolType = typeof(ISymbol);
Type operationType = typeof(IOperation);

var symbols = typeof(CSharpCompilation).Assembly.DefinedTypes.Where(x => !x.IsAbstract && x.ImplementedInterfaces.Contains(symbolType));
var operations = typeof(CSharpCompilation).Assembly.DefinedTypes.Where(x => !x.IsAbstract && x.ImplementedInterfaces.Contains(operationType));

Context
    .WriteUsings("System",
                 "System.Collections.Generic",
                 "System.Collections.Immutable",
                 "System.Reflection.Metadata",
                 "Microsoft.CodeAnalysis",
                 "Microsoft.CodeAnalysis.CSharp",
                 "Microsoft.CodeAnalysis.CSharp.Syntax",
                 "Microsoft.CodeAnalysis.Semantics",
                 "Microsoft.CodeAnalysis.Text")
    .WriteLine()
    .WriteNamespace("Cometary.Symbols")
    .WriteLine("partial class SymbolsFactory")
    .WriteLine('{')
    .IncreaseIndentation();

foreach (var ctor in symbols.SelectMany(x => x.DeclaredConstructors).Concat(operations.SelectMany(x => x.DeclaredConstructors)))
{
    if (ctor.IsStatic || ctor.IsPrivate)
        continue;

    var inheriting = FindFirstPublicType(ctor.DeclaringType);

    if (inheriting == null)
        continue;

    Context
        .WriteIndentation()
        .Write($"public static extern {inheriting.Name} {ctor.DeclaringType.Name}");

    if (inheriting.ContainsGenericParameters)
    {
        Write(string.Join(", ", inheriting.GetGenericArguments().Select(Name)));
    }

    Write('(');

    var parameters = ctor.GetParameters();

    for (int i = 0; i < parameters.Length; i++)
    {
        var param = parameters[i];

        Write($"{Name(param.ParameterType)} @{param.Name}");

        if (param.HasDefaultValue)
        {
            string def;

            switch (param.DefaultValue)
            {
                case bool b: def = b ? " = true" : " = false"; break;
                case string s: def = " = \"" + s + '"'; break;
                case Enum e: def = $" = {e}.{Enum.GetName(e.GetType(), e)}"; break;
                case object obj: def = " = null"; break;
                default: def = ""; break;
            }

            Write(def);
        }

        if (i < parameters.Length - 1)
            Write(", ");
    }

    Output.WriteLine(");");
}

Context
    .DecreaseIndentation()

    .WriteLine('}')
    .WriteEnd();