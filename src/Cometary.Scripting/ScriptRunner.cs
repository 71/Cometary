using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Cometary
{
    internal class ScriptRunner
    {
        // In theory, ScriptRunner isn't used till the ScryPackage is initialized
        public static ErrorListProvider ErrorListProvider = new ErrorListProvider(ScryPackage.Instance);

        public static async Task<(ScriptGlobals globals, ScriptState<object> state, ImmutableArray<Diagnostic> diags, Exception exception)> RunAsync(string scriptpath, string proj, string content)
        {
            var globals = new ScriptGlobals(scriptpath, proj, ScryPackage.GlobalWorkspace)
            {
                Extension = ".g.cs"
            };

            var opts = ScriptOptions.Default
                .WithFilePath(scriptpath)
                .AddImports("System",
                            "System.Collections.Generic",
                            "System.Linq",
                            "System.Reflection",
                            "System.Threading.Tasks",
                            "Microsoft.CodeAnalysis",
                            "Microsoft.CodeAnalysis.CSharp",
                            "Microsoft.CodeAnalysis.CSharp.Syntax")
                .AddReferences(typeof(string).Assembly,
                               typeof(MethodInfo).Assembly,
                               typeof(Task).Assembly,
                               typeof(SyntaxNode).Assembly,
                               typeof(CSharpCompilation).Assembly,
                               typeof(Workspace).Assembly);

            var script = CSharpScript.Create(content, opts, globals.GetType());
            var diagnostics = script.Compile();

            bool hadError = diagnostics.Any(x => x.IsWarningAsError || x.Severity == DiagnosticSeverity.Error);

            if (hadError)
                return (globals, null, diagnostics, null);

            try
            {
                ScriptState<object> state = await script.RunAsync(globals);

                await globals.WriteToFileAsync();
                globals.Dispose();

                return (globals, state, diagnostics, null);
            }
            catch (Exception x)
            {
                return (globals, null, diagnostics, x);
            }
        }

        public static async Task RunScripts(string[] scripts)
        {
            ErrorListProvider.Tasks.Clear();

            foreach (string script in scripts)
            {
                string content = File.ReadAllText(script);

                var (_,_,diags,ex) = await RunAsync(script, GetMatchingProject(script), content);

                foreach (var diag in diags)
                    Log(script, diag);
                if (ex != null)
                    ErrorListProvider.Tasks.Add(new ErrorTask(ex) { Document = script });
            }
        }

        public static void Log(string script, Diagnostic diag)
        {
            var loc = diag.Location.GetMappedLineSpan();

            ErrorListProvider.Tasks.Add(new ErrorTask
            {
                Category = TaskCategory.BuildCompile,
                ErrorCategory = diag.IsWarningAsError || diag.Severity == DiagnosticSeverity.Error
                    ? TaskErrorCategory.Error : TaskErrorCategory.Warning,
                Line = loc.StartLinePosition.Line,
                Column = loc.StartLinePosition.Character,
                Document = script,
                Text = diag.GetMessage(),
                HelpKeyword = diag.Descriptor.HelpLinkUri
            });
        }

        public static string GetMatchingProject(string filename)
        {
            DTE2 dte = Package.GetGlobalService(typeof(SDTE)) as DTE2;

            Debug.Assert(dte != null);

            var projItem = dte.Solution.FindProjectItem(filename);

            return projItem.ContainingProject.FileName;
        }
    }
}
