using Cometary;
using Cometary.Debugging;
using Cometary.Tests;
using Xunit;

[assembly: NoExterns]

#if DEBUG && !META
[assembly: DebugCometary]
#endif

[assembly: EditSelf(typeof(TestEditor))]

#if !META
[assembly: BreakOn(typeof(FactAttribute)), OutputAllTrees]
#endif