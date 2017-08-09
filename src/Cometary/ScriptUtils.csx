using System.Text;

string GetFriendlyName(Type type)
{
    if (type == typeof(void))
        return "void";
    if (type.IsArray)
        return GetFriendlyName(type.GetElementType()) + "[]";
    if (Nullable.GetUnderlyingType(type) != null)
        return GetFriendlyName(Nullable.GetUnderlyingType(type)) + "?";

    string name = type.Name;

    var genArgs = type.GetGenericArguments();

    if (genArgs.Length != 0)
    {
        name = name.Substring(0, name.Length - 2);
        name += $"<{string.Join(", ", genArgs.Select(x => GetFriendlyName(FindFirstPublicAncestor(x))))}>";
    }

    return name;
}

bool IsKeyword(string name)
{
    switch (name)
    {
        case "operator":
        case "checked":
        case "delegate":
        case "enum":
        case "object":
        case "event":
            return true;
        default:
            return false;
    }
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
    if (type.IsArray)
        return FindFirstPublicAncestor(type.GetElementType()).MakeArrayType();
    if (type.BaseType == typeof(Enum))
        return Enum.GetUnderlyingType(type);
    if (type.GenericTypeArguments.Length > 0 && type.IsPublic && !type.GetGenericTypeDefinition().GenericTypeArguments.Any(x => x.GetGenericParameterConstraints().Length > 0))
        return type.GetGenericTypeDefinition().MakeGenericType(type.GenericTypeArguments.Select(FindFirstPublicAncestor).ToArray());

    foreach (var interf in FindValidInterfaces(type).OrderByDescending(x => x.GetInterfaces().Length))
        return interf;

    while (!type.IsPublic)
    {
        type = type.BaseType;

        if (type == null)
            return typeof(object);
    }

    return type;
}

(string p, string a) MakeParametersString(ParameterInfo[] parameters)
{
    StringBuilder paramsBuilder = new StringBuilder();

    for (int i = 0; i < parameters.Length; i++)
    {
        ParameterInfo param = parameters[i];
        Type paramPublicType = FindFirstPublicAncestor(param.ParameterType);
        string paramName = IsKeyword(param.Name) ? $"@{param.Name}" : param.Name;

        paramsBuilder.AppendFormat("{0} {1}", GetFriendlyName(paramPublicType), paramName);

        if (param.HasDefaultValue)
        {
            paramsBuilder.Append(" = ");

            object defaultValue = param.DefaultValue;

            if (defaultValue == null)
            {
                if (paramPublicType.IsValueType)
                    paramsBuilder.AppendFormat("default({0})", GetFriendlyName(paramPublicType));
                else
                    paramsBuilder.Append("null");
            }
            else if (paramPublicType.IsEnum)
            {
                paramsBuilder.AppendFormat("{0}.{1}", paramPublicType.Name, defaultValue);
            }
            else if (param.ParameterType.IsEnum) // enum, but not a public one
            {
                paramsBuilder.Append(Convert.ChangeType(defaultValue, typeof(int)).ToString());
            }
            else if (paramPublicType == typeof(bool))
            {
                paramsBuilder.Append((bool)defaultValue ? "true" : "false");
            }
            else if (paramPublicType == typeof(string))
            {
                paramsBuilder.AppendFormat("\"{0}\"", defaultValue);
            }
            else
            {
                paramsBuilder.Append(defaultValue.ToString());
            }
        }

        if (i < parameters.Length - 1)
            paramsBuilder.Append(", ");
    }

    return (paramsBuilder.ToString(), string.Join(", ", parameters.Select(x => IsKeyword(x.Name) ? $"@{x.Name}" : x.Name)));
}

string MakeProxyMethod(MethodBase method)
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

    if (parameters.Length > 0)
        callBuilder.AppendFormat("proxy.Invoke(nameof({0}), {1})", methodName, argsStr);
    else
        callBuilder.AppendFormat("proxy.Invoke(nameof({0}))", methodName);

    return $"public {returnName} {methodName}({paramsStr}) => {callBuilder};";
}