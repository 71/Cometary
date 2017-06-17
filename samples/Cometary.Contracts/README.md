# Cometary.Contracts
Adds a few contracts to methods, as attributes. The contracts inject code in the body of the method directly.

#### Example
```csharp
[return: NotNull]
public void GetTypeName(Type type)
{
	return type?.Name;
}

public void DontAcceptNull([NotNull] string str)
{
	DoSomething(str);
}
```

gets compiled to

```csharp
public void GetTypeName(Type type)
{
	return type?.Name ?? throw new ArgumentNullException("return value");
}

public void DontAcceptNull(string str)
{
	if (str == null)
		throw new ArgumentNullException(nameof(str));

	DoSomething(str);
}
```

Additionally, some static methods are available in the `Requires` class, and help debugging bugs by specifying the expression that caused the error, its source file, line and character numbers.

```csharp
// Before:
Requires.NotNull(user.FirstName);

// After:
if (user.FirtName == null)
    throw new AssertionException("Expected expression to not be null",
                                 "user.FirstName", "file.cs", 1, 0);
                                 
// Other available methods:
Requires.Null(null);
Requires.True(1 == 1);
Requires.False(1 == 2);
```