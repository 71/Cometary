Cometary.IL
===========

This assembly provides an easy way to change what IL is emitted
by the compiler. It can modify any expression that is about to be emitted,
and emit raw IL.

# Installation
```powershell
Install-Package Cometary.IL
```

In any file:
```csharp
[assembly: SupportIL]
```

# Features

## Raw IL Emission
The static `IL` class can be used to emit any IL instruction,
as long as its operands are constants.

```csharp
public void ReturnFalse()
{
	IL.Ldc_I4_0();
	IL.Ret();

	return true;
}

ReturnFalse(); // => false.
```

Specifying non-constants will lead to an exception.

```csharp
public void Illegal()
{
	Type type = typeof(int);

	IL.Emit(ILOpCode.Ldtoken, type); // Throws.
	IL.Emit(ILOpCode.Ldtoken, typeof(int)); // Does not throw.
}
```

## Emission hooks
TODO, although you can have a look at the [IL](./IL.cs) class to understand how it works.