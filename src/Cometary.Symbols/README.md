# Cometary.Symbols
`Cometary.Symbols` adds the ability to manipulate `ISymbol` and `IOperation` objets, instead of syntax. The produced code is safer, and easier to manipulate.  
Unfortunately, since Roslyn does not allow direct editing or creation of symbols, this library does a lot of black magic behind the scenes, heavily using LINQ Expressions to access internal members, and scripts to generate code.

## Build process
This library can be thought as two different parts:
- The "black magic" part, which uses [Scripty](https://github.com/daveaglick/Scripty) to generate code that manipulates internal members of Roslyn using LINQ Expressions. Indeed, there is no need to worry about the slowness brought by reflection, since code is directly generated to IL, bypassing visibility checks.
- The "wrapper" part, which gives a few abilities to the user to edit a symbol tree: the `SymbolVisitor`, and various attributes.