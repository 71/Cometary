using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Cometary.Debugging
{
    /// <summary>
    ///   <see cref="CompilationEditor"/> that ensures all trees are written to disk,
    ///   and have a correct <see cref="SyntaxTree.FilePath"/>.
    /// </summary>
    internal sealed class SyntaxTreeFixerEditor : CompilationEditor
    {
        private readonly bool runInRelease;
        private readonly bool runInDebug;

        internal SyntaxTreeFixerEditor(bool inRelease, bool inDebug)
        {
            runInRelease = inRelease;
            runInDebug = inDebug;
        }

        /// <inheritdoc />
        protected override void Initialize(CSharpCompilation oldCompilation, CancellationToken _)
        {
            OptimizationLevel compilationConfiguration = oldCompilation.Options.OptimizationLevel;

            if (compilationConfiguration == OptimizationLevel.Debug && !runInDebug)
                return;
            if (compilationConfiguration == OptimizationLevel.Release && !runInRelease)
                return;

            CSharpCompilation EditCompilation(CSharpCompilation compilation, CancellationToken cancellationToken)
            {
                int syntaxTreesLength  = compilation.SyntaxTrees.Length;
                string randomDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

                for (int i = 0; i < syntaxTreesLength; i++)
                {
                    SyntaxTree tree = compilation.SyntaxTrees[i];
                    SyntaxTree nextTree = tree;

                    string treePath = tree.FilePath;

                    SourceText source = null;

                    if (nextTree.Encoding == null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (StreamWriter writer = new StreamWriter(ms, Encoding.UTF8, 4096, leaveOpen: true))
                            {
                                nextTree.GetText(cancellationToken).Write(writer, cancellationToken);
                            }

                            ms.Position = 0;

                            using (StreamReader reader = new StreamReader(ms, Encoding.UTF8, false, 4096, leaveOpen: true))
                            {
                                source = SourceText.From(reader, (int)ms.Length, Encoding.UTF8);

                                nextTree = nextTree.WithChangedText(source);
                            }
                        }
                    }

                    if (treePath != null && oldCompilation.SyntaxTrees.FirstOrDefault(x => x.FilePath == treePath) == tree)
                    {
                        // The tree already exists as a file, and is the one the user has;
                        // we don't need to change anything.

                        if (source == null)
                            // No changes applied
                            continue;

                        goto Replace;
                    }

                    if (!Directory.Exists(randomDirectory))
                         Directory.CreateDirectory(randomDirectory);

                    // The tree does not exist or has been changed by an editor,
                    // we need to change its filepath, and write a new file.
                    string newPath = string.IsNullOrEmpty(treePath)
                        ? Path.Combine(randomDirectory, Path.GetRandomFileName() + ".cs")
                        : Path.Combine(randomDirectory, Path.GetFileName(treePath));

                    nextTree = nextTree.WithFilePath(newPath);

                    using (FileStream fs = File.Open(newPath, FileMode.Create, FileAccess.Write))
                    using (StreamWriter writer = new StreamWriter(fs, nextTree.Encoding, 4096))
                    {
                        if (source == null)
                            source = nextTree.GetText(cancellationToken);

                        source.Write(writer, cancellationToken);
                    }

                    Replace:
                    compilation = compilation.ReplaceSyntaxTree(tree, nextTree);
                }

                return compilation;
            }

            CompilationPipeline += EditCompilation;
        }
    }
}
