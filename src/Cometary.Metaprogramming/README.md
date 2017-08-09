Cometary.Metaprogramming
========================

This assembly provides the ability of any assembly to define `CompilationEditor`'s that will edit
the assembly in which they are defined.

# Installation
```powershell
Install-Package Cometary.Metaprogramming
```

# Getting started
### Declare a `CompilationEditor` anywhere in your code.
For example, the following `CompilationEditor` adds a simple class to the final compilation.

```csharp
using Cometary;

internal class CustomEditor : CompilationEditor
{
	/// <inheritdoc />
	public override void Initialize(CSharpCompilation _compilation, CancellationToken _cancellationToken)
	{
		RegisterEdit((compilation, cancellationToken) => {
			var tree = SyntaxFactory.ParseSyntaxTree(@"
                namespace Tests {
                    public static class Answers {
                        public static int LifeTheUniverseAndEverything => 42;
                    }
                }
            ");

            return compilation.AddSyntaxTrees(tree);
		});
	}
}
```

### Tell Cometary you'll be metaprogramming your assembly
In any C# file, add the following lines.
```csharp
using Cometary;

[assembly: EditSelf(typeof(CustomEditor))] // Or whatever your editor is named.
```

### Let the magic be
That's it.

# Additional features

### The `META` preprocessor symbol name
You may want some code to only be compiled during the Metaprogramming phase. This can easily be done with the `META` preprocessor symbol name.

#### Example
```csharp
#if META
// Only apply an attribute when editing the declaring assembly.
[assembly: SomeAttribute]
#endif

#if !META
// Only declare a type in the end result.
public class MyClass
{
}
#endif
```