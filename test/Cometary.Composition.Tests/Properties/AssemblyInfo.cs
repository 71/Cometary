using Cometary.Composition;
using Cometary.Debugging;
using Xunit;

[assembly: EnableComposition]

#if DEBUG
[assembly: DebugCometary]
#endif

[assembly: BreakOn(typeof(FactAttribute)), OutputAllTrees]