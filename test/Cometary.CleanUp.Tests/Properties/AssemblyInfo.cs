using Cometary;
using Cometary.Debugging;
using Xunit;

[assembly: CleanUp]

#if DEBUG
[assembly: DebugCometary]
#endif

[assembly: BreakOn(typeof(FactAttribute)), OutputAllTrees]