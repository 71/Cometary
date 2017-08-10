using Cometary;
using Cometary.Debugging;
using Xunit;

[assembly: Macros]

#if DEBUG
[assembly: DebugCometary]
#endif

[assembly: BreakOn(typeof(FactAttribute)), OutputAllTrees]