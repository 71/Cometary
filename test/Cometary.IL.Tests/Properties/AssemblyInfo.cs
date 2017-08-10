using Cometary;
using Cometary.Debugging;
using Xunit;

[assembly: SupportIL]

#if DEBUG
[assembly: DebugCometary]
#endif

[assembly: BreakOn(typeof(FactAttribute)), OutputAllTrees]