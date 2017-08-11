Cometary.Composition
====================

This library brings true composition to the C# world.

# Installation
[![NuGet](https://img.shields.io/nuget/vpre/Cometary.Composition.svg)](https://nuget.org/packages/Cometary.Composition)
```powershell
Install-Package Cometary.Composition
```

# Overview
The `Compose` attribute is at the core of this library, and allows a class to implement multiple components.

Components are classes marked with the `Component` attribute.

### Example
```csharp
[Component]
public class Named
{
    public string Name { get; set; }
}

[Compose(typeof(Named))]
public class Person
{
    public int Age { get; set; }

    public override string ToString() => $"{this.Name}, aged {this.Age}";
}
```

Every single member of the `Named` component will be copied to the `Person` class.

## Dynamic components
Another way to define a component is to create a class that inherits the `Component` class, and that defines an empty constructor. A component defined this way is known as a "dynamic" component.  
Dynamic components must declare an `Apply` method that allows a dynamically add members to a class.

### Example
```csharp
public class Immutable : Component
{
    public override ClassDeclarationSyntax Apply(ClassDeclarationSyntax node, INamedTypeSymbol symbol, CancellationToken cancellationToken)
    {
        return node.AddMembers(...);
    }
}

[Compose(typeof(Immutable))]
public class ImmutablePerson
{
    public string Name { get; }
    public int Age { get; }

	public ImmutablePerson(string name)
	{
		Name = name;
	}
}

// The immutable person now declares WithName(string name) and WithAge(int age).
ImmutablePerson bob = new ImmutablePerson("Bob");
ImmutablePerson dan = bob.WithName("Dan");

ReferenceEquals(bob, dan); // => false
bob.Name; // => "Bob"
dan.Name; // => "Dan"
```

> **Note**:  
> The full content of the `Immutable` class is available in the [Immutable.cs](../../test/Cometary.Composition.Tests.Library/Immutable.cs) file.