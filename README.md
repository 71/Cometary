Cometary
========
Cometary is a C# project that aims to bring compiler plugins and meta-programming to the C# world.

Thanks to Cometary, the next snippet is valid C# code.
```csharp
// Emit CIL instructions inline.
IL.Ldnull();
IL.Ret();

// Mixins.
int i = 0;
"i++".Mixin(); // i == 1

// Macros.
Requires.NotNull(parameter);
// .. will be replaced by ..
if (parameter == null)
    throw new ArgumentNullException(nameof(parameter));
    
// Much, much more.
```

[![Latest release](https://img.shields.io/github/release/6A/Cometary.svg)](../../releases/latest)
[![Issues](https://img.shields.io/github/issues-raw/6A/Cometary.svg)](../../issues)
[![License](https://img.shields.io/github/license/6A/Cometary.svg)](./LICENSE.md)

> **Note**  
> This project is still in active development, and is *extremely* unstable. Please proceed with caution.

# Get started
Cometary requires two core components to work normally:
- The [Analyzer][Analyzer], required to extend the Roslyn build process.
- The [Core][Core] library, on which the Analyzer depends.

They can be installed via the NuGet package manager.
```powershell
Install-Package Cometary.Core Cometary.Analyzer
```
Once installed, all you have to do is build your project normally (via `csc`, `dotnet build` or Visual Studio), and Cometary takes care of the rest.

You now have the required dependencies, but nothing much will change. You need to either [install an extension](#installing-existing-extensions), or [make your own](#making-your-own-extension).

## Installing existing extensions
Some existing extensions can be found in the "[src](./src)" directory, and installed through the NuGet package manager. Please consult their respective READMEs to learn how to install and use them.

By convention, extensions must be installed as a dependency, **and** configured properly using attributes. For example, an extension would be registered with the following syntax:

```csharp
[assembly: Foo(Bar = true)]
```

### Existing extensions
- [Cometary.Metaprogramming][Metaprogramming], which will run extensions directly defined in your assembly, and execute some methods marked with the `Invoke` attribute. It allows an assembly to edit its own syntax and resulting compilation before being emitted by Roslyn. To do so, [CTFE](https://www.wikiwand.com/en/Compile_time_function_execution), [Mixins](https://www.wikiwand.com/en/Mixin), and [Macros](https://www.wikiwand.com/en/Macro_(computer_science)) will be made available to the user.
- [Cometary.IL](./src/Cometary.IL), which will allow you to print your own IL code inline.

## Making your own extension
An extension is a normal .NET library that defines one or more `CompilationEditor`s (an example is available below). However, simply having a dependency on the extension is not enough to install it. You also need to create an attribute inheriting [`CometaryAttribute`][CometaryAttribute], and have the user set it on its assembly.

Don't worry, it's easy.

#### 1. Define a [`CompilationEditor`][Editor]
Create a class that inherits [`CompilationEditor`][Editor]:
```csharp
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Cometary;

/// <summary>
///   <see cref="CompilationEditor" /> that adds the 'Answers' static class to
///   assemblies it edits.
/// </summary>
internal sealed class DeepThoughtEditor : CompilationEditor
{
    public string Namespace { get; }
    
    public DeepThoughtEditor(string @namespace)
    {
        Namespace = @namespace;
    }
  
    /// <inheritdoc />
    public override void Initialize(CSharpCompilation compilation, CancellationToken cancellationToken)
    {
        // RegisterEdit takes a delegate of type (CSharpCompilation, CancellationToken) -> CSharpCompilation.
        // It also exists for SyntaxNode, CSharpSyntaxTree, ISymbol, and IOperation.
        RegisterEdit(EditCompilation);
    }

    /// <summary>
    ///   Edits the given <paramref name="compilation"/>, adding a <see cref="CSharpSyntaxTree"/>
    ///   defining the 'Answers' class.
    /// </summary>
    private CSharpCompilation EditCompilation(CSharpCompilation compilation, CancellationToken cancellationToken)
    {
        if (this.State == CompilationState.End)
            return compilation;

        var tree = SyntaxFactory.ParseSyntaxTree(@$"
            namespace {Namespace} {
                public static class Answers {
                    public static int LifeTheUniverseAndEverything => 42;
                }
            }
        ");

        return compilation.AddSyntaxTrees(tree);
    }
}
```

#### 2. Define a configuration attribute
Create an attribute that inherits [`CometaryAttribute`][CometaryAttribute]:
```csharp
using System;
using Cometary;

[AttributeUsage(AttributeTargets.Assembly)]
public sealed class DeepThoughtAttribute : CometaryAttribute
{
    public string Namespace { get; }
    
    public DeepThoughtAttribute(string @namespace)
    {
        Namespace = @namespace;
    }
    
    public override IEnumerable<CompilationEditor> Initialize()
    {
        yield return new DeepThoughtEditor(Namespace);
    }
}
```

#### 3. Build the library, and use it
In any assembly that references the previously defined assembly, add the following code at the top level of a file:
```csharp
[assembly: DeepThought("TestAssembly")]
```

Since the library modifies the build process, not only will the `Answers` class be defined in the output, it can also be used!

```csharp
Assert.Equal(Answers.LifeTheUniverseAndEverything, 42); // will not throw.
```

However, the IDE intergration isn't perfect, and an error will be shown while editing. During compilation, though...

```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

# Compatibility
- The analyzer is available for Roslyn 2.0.0 and up (VS 2017 and up). It should also work with `dotnet` and `csc`, as long as they use a recent version of Roslyn.
- [Cometary.Core][Core] and other extensions are available for .NET Standard 1.4 and up.
- [Cometary.Metaprogramming][Metaprogramming] is available for .NET Standard 1.5 and up.
- For building, Visual Studio 2017 with the [Scry](https://github.com/6A/Scry) extension is required.

# How does it work?
- When loaded, the [Analyzer][Analyzer] loads the [Cometary.Core][Core] library (optionally resolving dependencies thanks to the references of the compilation it analyzes).
- When [Cometary.Core][Core] is loaded, some hooks are created (using [Ryder](https://github.com/6A/Ryder), via the [`Hooks` class](./src/Cometary.Core/Hooks.cs)). Those hooks redirect calls made to some internal Roslyn methods, to custom methods.
- When a `CSharpCompilation` is about to be emitted (via [`CheckOptionsAndCreateModuleBuilder`](http://source.roslyn.io/#Microsoft.CodeAnalysis/Compilation/Compilation.cs,42341c66e909e676)), Cometary intercepts the call and does the following things:
   1. Create a [`CometaryManager`][Manager] and bind it to the `CSharpCompilation`.
   2. Find all attributes set on the assembly to emit that inherit [`CometaryAttribute`][CometaryAttribute], and initialize them. During initialization, those attributes have the ability to register [`CompilationEditor`s][Editor].
   3. All registered editors are initialized in their turn, allowing them to suppress `Diagnostic`s (by registering a `Predicate<Diagnostic>`), or edit the compilation, and its syntax and symbols.
   4. Let all the now-initialized editors actually edit the original `CSharpCompilation`.
   5. Return the modified `CSharpCompilation`, and dispose of all the editors and attributes. However, since the hooked method isn't static, a different object cannot be used to emit the assembly. As a workaround, all fields from the modified `CSharpCompilation` are copied to the original one.
- The emission process goes back to its normal flow with the modified `CSharpCompilation`.


[CometaryAttribute]: ./src/Cometary.Core/Attributes/CometaryAttribute.cs
[Editor]: ./src/Cometary.Core/CompilationEditor.cs
[Manager]: ./src/Cometary.Core/CometaryManager.cs
[Core]: ./src/Cometary.Core
[Analyzer]: ./src/Cometary.Analyzer
[Metaprogramming]: ./src/Cometary.Metaprogramming