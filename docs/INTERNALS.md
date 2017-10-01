# Internals
In this document, some advanced concepts are explained, especially
about the inner functionement of Cometary.

## Quotes
Quotes are automatically injected during compile time HOWEVER they have an undocumented limitation. In order to allow quotes to call other methods that accept quotes (for example, [`SyntaxExtensions.MemberSwitch`](../src/Cometary/Extensions/SyntaxExtensions.Lib.cs#L137)), the [`MacroVisitor`](../src/Cometary/Macros/MacroVisitor.cs) only expands macros in methods that **do not declare a `Quote` parameter**.