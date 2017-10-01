Cometary
========

Development has shifted to the [master](../../tree/master) branch. This branch only exists to store the old code, which may contain useful code snippets.

-------

Old documentation:
==================

Cometary is a C# project that aims at bringing true meta-programming to the C# world. It features [CTFE](https://www.wikiwand.com/en/Compile_time_function_execution), [Mixins](https://www.wikiwand.com/en/Mixin), and other goodies.

# Installation
Cometary can be installed in different ways, depending on your setup.  
Since Cometary loads your project, parses it and its references, and compiles it twice, things can get pretty slow. If possible, users should install the `Cometary.VisualStudio` MSBuild task for the best performances.

Otherwise, depending on what you need, there is:
- `Cometary.MSBuild` for non-VS users that use MSBuild anyway.
- `Cometary.Core` for .NET Core users (via the `dotnet cometary`) command.

(Yep, those packages don't exist yet)

### Compatibility
Cometary is available for `.NET Standard` 1.3 and up. The hosts are available starting at .NET Framework 4.7.  
Support for Linux and macOS is planned, but not yet ready.

# Features

## CTFE
Mark any static method with the `[CTFE]` attribute to execute it during compilation. Additionally, the following parameters are supported in any order: `MethodInfo`, `MethodDeclarationSyntax`, `TypeInfo`, `ClassDeclarationSyntax`.

#### Example
```csharp
/// <summary>
/// Execute all test methods in this type.
/// </summary>
[CTFE]
private static void Unittests(TypeInfo info)
{
    foreach (MethodInfo method in info.GetMethods().Where(IsTestMethod))
    {
        method.Invoke(null, new object[0]);
    }
}
```

## Code injection
Any method body can use the `void Mixin(string)` and `T Mixin<T>(string)` extension methods to print the content of the string as code.

#### Example
```csharp
int i = 0;
"i++".Mixin();

i.ShouldBe(1); // Does not throw.
"(i + 1)".Mixin<int>().ShouldBe(2); // Ditto.
```

## Templates
New methods called templates can now be defined. A method is a template if:
- Is is static.
- It contains an optional `Quote` parameter.

Everytime your method is called in user code, it will have the occasion to replace the syntax of the call itself.

#### Example
Actually, that's exactly how the mixin works!
```csharp
T Mixin<T>(this string str, Quote quote = null)
    => quote.Unquote<T>(str.Syntax<ExpressionSyntax>());

// Before
bool b = Mixin<bool>("true");

// After
bool b = true;
```
Check out the [Tests](./test/Cometary.Tests/TemplateTests.cs) for a better example.

## Visitor attributes
Any attribute can now implement a few [interfaces](./src/Cometary.Rewriting/Attributes/Interfaces.cs) to interact with the marked members during compilation.

#### Example
```csharp
/// <summary>
/// Ensures the marked parameter is never <see langword="null"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class NotNullAttribute : Attribute, IParameterVisitor
{
    private static StatementSyntax GetCheckStatement(string parameterName)
        => F.IfStatement($"{parameterName} == null".Syntax<BinaryExpressionSyntax>(),
                         $"throw new ArgumentNullException(nameof({parameterName}));".Syntax<StatementSyntax>());

    /// <inheritdoc />
    public ParameterSyntax Visit(ParameterInfo parameter, ParameterSyntax node)
    {
        node.Method()
            .Replace(method => method.With(
               x => x.Body.Statements,
               stmts => stmts.Insert(0, GetCheckStatement(parameter.Name))));

        return node;
    }
}
```

## Visitor
And for the ones of you who *really* need to modify everything...  
Create your own `AssemblyVisitor`, and override whatever method you want. We take care of the rest.

#### Example
```csharp
/// <summary>
/// <see cref="AssemblyVisitor"/> that sets all declared
/// enum flags to the same value.
/// </summary>
public sealed class EvilVisitor : AssemblyVisitor
{
    public override float Priority => 100.0f;

    public override EnumDeclarationSyntax Visit(TypeInfo @enum, EnumDeclarationSyntax node)
    {
        EqualsValueClauseSyntax clause = F.EqualsValueClause(
            F.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                F.Literal(0)));

        return node.WithMembers(
            F.SeparatedList(node.Members.Select(x => x.WithEqualsValue(clause)))
        );
    }
}
```

## Other features
- **Support for `extern` methods**: If you want a mixin or attribute to replace an `extern` method during compilation, worry not! Cometary will automatically edit your assembly to run it, even if it contains extern methods.
- **Fluent utilities**: Let's say you have a `TypeDeclarationSyntax` parameter named `t`. Let's say you want to get its first static field's value. `t.GetField(x => x.Symbol().IsStatic).Info().GetValue(null);` That's *fifty-nine* characters.
- **`Compilation` editing**: Yup, any `AssemblyVisitor` can also modify the `Compilation` object of the assembly by overriding the `CSharpCompilation Visit(Assembly assembly, CSharpCompilation compilation)` method.
- **Short `With` syntax**: Cometary provides an utility method named `With` that gives you the ability to edit a deeper part of your tree easily. Here's an example: `return method.With(x => (x.Body.Statements.First(IsValid) as ExpressionStatementSyntax).Expression, oldNode => newNode)`.

# Documentation
- [Getting started](./docs/README.md)
- [Debugging](./docs/DEBUGGING.md)
- [Hosts](./docs/HOSTS.md)
- [Internals](./docs/INTERNALS.md)

# Components
All components have individual READMEs, which can be found in the following directories.

- [Cometary](./src/Cometary) (NuGet metapackage)
- [Cometary.Common](./src/Cometary.Common) (Core package)
- [Cometary.VSIX](./src/Cometary.VSIX) (Visual Studio extension)

### Libraries
- [Cometary.Generation](./src/Cometary.Generation)
- [Cometary.Rewriting](./src/Cometary.Rewriting)
- [Cometary.Scripting](./src/Cometary.Scripting)
- [Cometary.Symbols](./src/Cometary.Symbols)

### Hosts
- [Cometary.Hosting](./src/Cometary.Hosting)
- [Cometary.Hosting.Core](./src/Cometary.Hosting.Core)
- [Cometary.Hosting.MSBuild](./src/Cometary.Hosting.MSBuild)
- [Cometary.Hosting.VisualStudio](./src/Cometary.Hosting.VisualStudio)

### Samples
See [`samples`](./samples) directory.

# Current state
It works, as long as you stick to the [.NET Core version](src/Cometary.Hosting.Core).

Whatever uses MSBuild will fail because of these issues:
 - https://github.com/dotnet/corefx/issues/19548
 - https://github.com/Microsoft/msbuild/issues/1309

# Building
The following components are currently required to build Cometary:
- [Visual Studio 2017](https://www.visualstudio.com/)
- [Scry](https://github.com/6A/Scry)
