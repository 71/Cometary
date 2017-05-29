# Cometary Docs

## Getting started
What follows doesn't work yet, sorry. But it will, soon.

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