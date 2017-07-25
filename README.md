Cometary
========
Cometary is a C# project that aims at bringing true meta-programming to the C# world. It features [CTFE](https://www.wikiwand.com/en/Compile_time_function_execution), [Mixins](https://www.wikiwand.com/en/Mixin), and other goodies.

[![Latest release](https://img.shields.io/github/release/6A/Cometary.svg)](../../releases/latest)
[![Issues](https://img.shields.io/github/issues-raw/6A/Cometary.svg)](../../issues)
[![License](https://img.shields.io/github/license/6A/Cometary.svg)](./LICENSE.md)

> **Note**  
> Even though all the tests are passing, this project is still in early development, and *extremely* unstable. Proceed with caution.

# Overview
Cometary both provides a library to create your own analyzers, and some already existing analyzers.

## Libraries
### [`Cometary`](./src/Cometary)
Base library that provides the `CompilationEditor` class, the class used to edit all compilations.

## Analyzers
### [`Cometary.Analyzer`](./src/Cometary.Analyzer)
Analyzer that provides the `HookingAnalyzer`, which enables editing the internally used `CSharpCompilation`.

### [`Cometary.SelfAnalyzer`](./src/Cometary.SelfAnalyzer)
Analyzer that emits the compilation it analyzes, loads it in memory, and allows it to analyze itself. It will load every `CompilationEditor` in the assembly, and unleash them on the compilation.

# Installation
Cometary is currently being rewritten as a [Roslyn Analyzer](https://github.com/dotnet/roslyn/tree/master/docs/analyzers), and is thus currently unavailable on NuGet. However, I will soon publish it on NuGet. Then, installing it on your project will be very simple:

```powershell
Install-Package Cometary
Install-Package Cometary.Analyzer
```

### Compatibility
- All analyzers are available for Roslyn 2.0.0 and up (VS 2017 and up). It should also work with `dotnet` and `csc`, as long as they use a recent version of Roslyn.
- `Cometary` itself is available for .NET Standard 1.4 and up.

# How does it work?
Using [Ryder](https://github.com/6A/Ryder), Cometary redirects calls to internal Roslyn methods to allow the [`HookingAnalyzer`](./src/Cometary.Analyzer/HookingAnalyzer.cs) to edit the compilation it analyzes, and replace Roslyn's compilation with its own.  
At this point, all there is to do is to actually edit the compilation: Enter the [`CompilationEditor`](./src/Cometary/CompilationEditor.cs), which provides a way to edit the assembly.