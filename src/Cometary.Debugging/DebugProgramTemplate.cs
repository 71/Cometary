using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Cometary;

[CompilerGenerated]
internal static class DebugProgram
{
    // This file will be modified by the DebuggingEditor and injected into target assemblies.
    // Metadata, such as the assembly name and the references, will be set to the following fields,
    // and used to reproduce a CSharpCompilation similar to the one used to generate the original compilation.
    // Except this time, a debugger will be awaiting.

    #region Logic
    public const string AssemblyName = "";
    public const string References = "";
    public const string Files = "";
    public const string ErrorFile = "";
    public const bool Written = false;
    public const bool BreakOnEnd = false;

    public static bool IsWrittenToDisk => Written;
    public static bool ShouldBreak => BreakOnEnd;

    public static int Main(string[] args)
    {
        try
        {
            DiagnosticAnalyzer analyzer = new CometaryAnalyzer();

            CompilationWithAnalyzers compilation = CSharpCompilation.Create(
                AssemblyName,
                Files.Split(';').Select(x => CSharpSyntaxTree.ParseText(File.ReadAllText(x))),
                References.Split(';').Select(x => MetadataReference.CreateFromFile(x))
            ).WithAnalyzers(ImmutableArray.Create(analyzer));

            ExecuteAsync(compilation).Wait();

            return 0;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            Console.Error.WriteLine();
            Console.Error.WriteLine(e.StackTrace);

            Console.ReadKey();

            return 1;
        }
    }

    public static async Task ExecuteAsync(CompilationWithAnalyzers compilation)
    {
        await compilation.GetAllDiagnosticsAsync();

        using (MemoryStream assemblyStream = new MemoryStream())
        using (MemoryStream pdbStream = new MemoryStream())
        {
            var result = compilation.Compilation.Emit(assemblyStream, pdbStream);

            if (!IsWrittenToDisk || !ShouldBreak)
                return;

            Diagnostic[] diagnostics = result.Diagnostics.OrderByDescending(x => x.Severity).ToArray();

            if (result.Success)
            {
                Success(diagnostics);
                return;
            }

            string[] errors = diagnostics.TakeWhile(x => x.Severity == DiagnosticSeverity.Error).Select(x => x.ToString()).ToArray();

            try
            {
                File.WriteAllLines(ErrorFile, errors);
            }
            catch
            {
                // Don't really care tbh
            }

            Failure(diagnostics, errors);
        }
    }
    #endregion

    public static void Success(Diagnostic[] diagnostics)
    {
        // If you got here, that means the compilation was a success!
        //
        Debugger.Break();
    }

    public static void Failure(Diagnostic[] diagnostics, string[] errors)
    {
        // If you got here, that means the compilation failed...
        // You can inspect the errors via the debugger, or in the error file available at:
        //    %ERRORFILE%
        //
        Debugger.Break();
    }
}