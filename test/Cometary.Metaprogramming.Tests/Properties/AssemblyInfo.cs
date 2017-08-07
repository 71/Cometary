using System.Diagnostics.CodeAnalysis;
using Cometary;
using Cometary.Tests;

[assembly: NoExterns]
[assembly: EditSelf(typeof(TestEditor), typeof(InvokeEditor))]
[assembly: Invoke]

[assembly: SuppressMessage("Compiler", "CS0116")]
[cometary: NoExterns]