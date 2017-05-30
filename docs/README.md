# Cometary Docs

## Getting started
What follows doesn't work yet, sorry. But it will, soon. See [the end of the page](#manual-setup) to use Cometary anyway.

#### 1. Installation
Choose one of the [existing remotes](REMOTES.md), and install it through Nuget. For example, to use the .NET Core remote (currently the only one that works), execute `Install-Package Cometary.Remote.Core`.

#### 2. Write your first Cometary-powered class
If you're working with XAML, you probably use many [dependency properties](). You'd probably like to reduce this declaration:
```csharp
public static readonly DependencyProperty UsernameProperty = 
    DependencyProperty.Register(nameof(Username), typeof(string), typeof(MyClass), null);

public string Username
{
    get => (bool)GetValue(UsernameProperty);
    set => SetValue(UsernameProperty, value);
}
```
to this one:
```csharp
[DependencyProperty]
public extern string Username { get; set; }
```

Well you can, just copy-paste the following snippet into a file named `DependencyProperty.cs` (or anything, really), and you're ready to go!

```csharp
[AttributeUsage(AttributeTargets.Property)]
public sealed class DependencyPropertyAttribute : Attribute, IPropertyVisitor
{
    PropertyDeclarationSyntax IPropertyVisitor.Visit(PropertyInfo property, PropertyDeclarationSyntax node)
    {
        // TODO: Finish this snippet
    }
}
```

#### 3. See the magic
You can directly execute your code to see the magic happen, but if you want to go further, you may want to inspect the produced assembly with [dnSpy](https://github.com/0xd4d/dnSpy) or [dotPeek (closed-source)](https://www.jetbrains.com/decompiler).

#### Dig deeper
- [Debugging guide](DEBUGGING.md), during compilation and runtime.
- [The different remotes](REMOTES.md), and how they work.
- [Built-in extensions](EXTENSIONS.md)

## Manual setup
Cometary isn't ready for production yet, which is why it hasn't been published on Nuget. It can still be used, however.  
The MSBuild and Visual Studio tasks do not work yet (as stated in the [README](../README.md)), but the .NET Core task *does* work.

### 1. Building
Clone this repository somewhere (`git clone https://github.com/6A/Cometary`), fire up any tool you like, and build Cometary. You're gonna need a remote (only the [.NET Core](../src/Cometary.Remote.Core) remote works for now), the [Core library](../src/Cometary.Core), and (optionally) the [main library](../src/Cometary). The latter is optional, but greatly recommended.

### 2. Setup dependencies
Add the produced binary of the Core library as a dependency in your project, and optionally the main library as well. The remote should **not** be referenced. If it does not happen automatically, please reference [`Microsoft.CodeAnalysis.CSharp`](https://github.com/dotnet/roslyn) as well.

### 3. Setup build process
You can either execute Cometary manually everytime you need rebuild your project, or set up an MSBuild task that'll do it automatically.

#### Manual
Use the command line tool.  
The synopsis is:
##### `dotnet dotnet-cometary.dll "<project path>" [--debug] [--syntax]`
- `<project name>` is the full path to the project to build.
- `--debug` enables debugging: `Debugger.Launch()` will be called as soon as Cometary starts.
- `--syntax` enables syntax output; the modified syntax trees will be saved as `.cs` files (by default in the `[project dir]/obj/cometary-syntax` directory).

#### MSBuild task
Setup an MSBuild task that executes the previous command automatically.

```csproj
<PropertyGroup>
    <RemotePath>D:\full\path\to\dotnet-cometary.exe</RemotePath>
</PropertyGroup>

<Target Name="Build" DependsOnTargets="$(BuildDependsOn)">
    <Exec Command="$(RemotePath) &quot;$(ProjectPath)&quot;" ConsoleToMSBuild="True" />
</Target>
```