using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cometary
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class ScriptGlobals : IDisposable
    {
        private int _indent;

        public ScriptGlobals Context => this;

        public Workspace Workspace { get; }
        public Project Project { get; }

        public TextWriter Output { get; }
        public string Extension { get; set; }

        public string ScriptFile { get; }

        public int Indentation
        {
            get => _indent;
            set => _indent = value < 0 ? 0 : value;
        }

        public int Indent
        {
            get => _indent;
            set => _indent = value < 0 ? 0 : value;
        }

        public bool AutoWriteIndentation { get; set; }

        internal MemoryStream OutputStream { get; }

        public ScriptGlobals(string script, string projfile, Workspace w)
        {
            Workspace = w;
            Project = w.CurrentSolution.Projects.FirstOrDefault(x => x.FilePath == projfile);

            if (Project == null)
                throw new Exception("Could not open project. Are you sure it has fully loaded?");

            ScriptFile = script;
            Output = new StreamWriter(OutputStream = new MemoryStream(), Encoding.UTF8);
        }

        public ScriptGlobals WriteLine(string format, params object[] args)
        {
            if (AutoWriteIndentation)
                this.WriteIndentation();

            Output.WriteLine(format, args);
            return this;
        }

        public ScriptGlobals Write(string format, params object[] args)
        {
            Output.Write(format, args);
            return this;
        }

        public ScriptGlobals WriteLine(params object[] args)
        {
            if (AutoWriteIndentation)
                this.WriteIndentation();

            Output.WriteLine(string.Join(" ", args));
            return this;
        }

        public ScriptGlobals Write(params object[] args)
        {
            Output.Write(string.Join(" ", args));
            return this;
        }

        public ScriptGlobals WriteUsings(params string[] usings)
        {
            foreach (string u in usings)
                WriteIndentation().Output.WriteLine("using {0};", u);

            return this;
        }

        public ScriptGlobals WriteNamespace(string @namespace)
        {
            this.WriteLine("namespace {0}", @namespace)
                .WriteLine('{')
                .Indentation += 4;

            return this;
        }

        public ScriptGlobals WriteEnd()
        {
            this.DecreaseIndentation()
                .WriteLine('}');

            return this;
        }

        public ScriptGlobals WriteIndentation()
        {
            Output.Write(new string(' ', Indentation));
            return this;
        }

        public ScriptGlobals IncreaseIndentation(int indent = 4)
        {
            Indentation += indent;
            return this;
        }

        public ScriptGlobals DecreaseIndentation(int indent = 4)
        {
            Indentation -= indent;
            return this;
        }

        internal async Task WriteToFileAsync()
        {
            await Output.FlushAsync();

            OutputStream.Seek(0, SeekOrigin.Begin);

            using (FileStream fs = File.OpenWrite(Path.ChangeExtension(ScriptFile, Extension)))
            {
                fs.SetLength(0);

                await OutputStream.CopyToAsync(fs);
            }
        }

        public void Dispose()
        {
            OutputStream.Dispose();
        }

        public SyntaxNode Syntax(string filename)
        {
            return Project.Documents.FirstOrDefault(x => x.Name.EndsWith(filename))?.GetSyntaxRootAsync().Result;
        }

        public CSharpSyntaxTree Tree(string filename)
        {
            return Project.Documents.FirstOrDefault(x => x.Name.EndsWith(filename))?.GetSyntaxTreeAsync().Result as CSharpSyntaxTree;
        }

        public SemanticModel Model(string filename)
        {
            return Project.Documents.FirstOrDefault(x => x.Name.EndsWith(filename))?.GetSemanticModelAsync().Result;
        }

        public IEnumerable<SyntaxNode> Syntaxes => Project.Documents.Select(x => x.GetSyntaxRootAsync().Result);
        public IEnumerable<CSharpSyntaxTree> Trees => Project.Documents.Select(x => x.GetSyntaxTreeAsync().Result).OfType<CSharpSyntaxTree>();
        public IEnumerable<SemanticModel> Models => Project.Documents.Select(x => x.GetSemanticModelAsync().Result);
    }
}