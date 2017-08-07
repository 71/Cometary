Cometary.Debugging
==================

This library eases debugging in three situations:
 1. Debugging a `CompilationEditor`'s lifetime.
 2. Debugging an assembly that is editing itself.
 3. Debugging an assembly that has been modified by Cometary.

# Installation
```powershell
Install-Package Cometary.Debugging
```

# Features

### Debugging an assembly's modification
```csharp
[assembly: DebugCometary]
```

When the `DebugCometary` attribute is placed on your assembly, a new entry point will be defined on Debug (by default, but it can be configured). That entry point is given every necessary information to execute Cometary on a copy of the compilation that led to it, and will do so.

Since it runs Cometary in its own process, debugging is as easy as setting a breakpoint in your `CompilationEditor`, or `CometaryAttribute`.

### Debugging an assembly modified by Cometary
```csharp
[assembly: OutputAllTrees]
```

Most of the time, Cometary will change the Syntax of your files during its execution. The problem with this is that if a file changes, sequence points do not, and the debugging experience gets very bad.

Setting the `OutputAllTrees` attribute on your assembly does two things:
- Write all new syntax trees to a temporary file, and point the debugger there.
- Write all syntax trees that have been modified to a new temporary file, and point the debugger there.