using Cometary.Debugging;
using Xunit;

#if DEBUG
[assembly: DebugCometary]
#endif

[assembly: BreakOn(typeof(FactAttribute)), OutputAllTrees]