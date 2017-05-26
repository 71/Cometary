# Extensions
A few libraries have been built using Cometary, mostly as samples.  
However, they can be useful in real-world scenarios, which is why they are
published on NuGet, and called *Extensions*.

## Contracts
Adds a few contracts to methods, as attributes. The contracts inject code in the body
of the method directly.

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
public void DontReturnNull(Type type)
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

## IL
Uses [Mono.Cecil](https://github.com/jbevain/cecil) to further edit the produced assembly.  
This sort of low-level access provides the ability to add IL code directly into methods,
thanks to the `IL` class.

#### Example
```csharp
public string Greet()
{
	IL.Emit(OpCodes.Ldstr, "Hello, world!");
	IL.Emit(OpCodes.Ret);

	return null;
}
```

gets compiled to

```cil
.method public hidebysig static string Greet() cil managed
{
	ldstr "Hello, world!"
	ret

	ldnull
	ret
}
```

## Rest
[Refit](https://github.com/paulcbetts/refit)-inspired HTTP client.

#### Example
```csharp
[Get("https://github.com")]
public extern Task<HttpResponseMessage> GetGithub();
```

gets compiled to

```csharp
public Task<HttpResponseMessage> GetGithub()
	=> new HttpClient().SendAsync(new HttpRequestMessage(new HttpMethod("Get"), "https://github.com"));
```