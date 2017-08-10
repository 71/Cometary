Cometary.Macros
===============

This library provides macros the ability to allow methods to redefine themselves, based on the context of the caller.

> **Note**:  
> Macros must be defined in another library in order for them to be properly expanded.
> Alternatively, the [`Cometary.Metaprogramming`](../Cometary.Metaprogramming) package must be installed.

# Installation
[![NuGet](https://img.shields.io/nuget/vpre/Cometary.Macros.svg)](https://nuget.org/packages/Cometary.Macros)
```powershell
Install-Package Cometary.Macros
```

### Activation
```
[assembly: Macros]
```

# Features
> **Note**:  
> Some examples can be found in the [tests library](../../test/Cometary.Macros.Tests.Library).

## Macro expansion
All static methods marked with the `Expand` attribute are now considered "macros." They will be called during compilation with access to the static [`CallBinder`](./CallBinder.cs) class. From there, they'll be able to change the syntax of the statement or expression that led to this call.

#### Example
```csharp
[Expand]
public static void Mixin(string expression)
{
    CallBinder.ExpressionSyntax = SyntaxFactory.ParseExpression(expression);
}

// Before:
Mixin("int i = 0;")

// After:
int i = 0;
```

## Compile-Time Dynamic Binding
Any class can now define members that are dynamically resolved during compilation by declaring a static `Bind` method.

#### Example
```csharp
public static class Numbers
{
    private static ExpressionSyntax Bind(MemberAccessExpressionSyntax node)
    {
        int number = ExampleClass.ParseNumber(node.Name.Identifier.Text);

        return SyntaxFactory.LiteralExpression(
            SyntaxKind.NumericLiteralExpression,
            SyntaxFactory.Literal(number)
        );
    }
}

// Before:
int i = Numbers.FiftyFive;

// After:
int i = 25;
```