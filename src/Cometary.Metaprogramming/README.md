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

[CompilationEditor]
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