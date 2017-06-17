# Cometary.IL
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

```
.method public hidebysig static string Greet() cil managed
{
	ldstr "Hello, world!"
	ret

	ldnull
	ret
}
```