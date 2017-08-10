using Cometary;
using Cometary.Debugging;
using Xunit;

[assembly: SupportIL]

#if DEBUG && false
[assembly: DebugCometary]
#endif

[assembly: /*BreakOn(typeof(FactAttribute)),*/ OutputAllTrees]