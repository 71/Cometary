using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis;

using Task = Microsoft.Build.Utilities.Task;

namespace Cometary
{
    /// <summary>
    /// MSBuild <see cref="Task"/> that executes the <see cref="Processor"/>.
    /// </summary>
    public class CometaryTask : Task, ICancelableTask
    {
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        /// <inheritdoc />
        public void Cancel() => cts.Cancel();

        /// <summary>
        /// Gets or sets whether or not the process should be debugged.
        /// </summary>
        public bool IsDebugging { get; set; }

        /// <summary>
        /// Gets or sets whether or not the produced syntax should be output.
        /// </summary>
        public bool OutputSyntax { get; set; }

        /// <summary>
        /// Gets or sets the host used by Visual Studio.
        /// </summary>
        public ProcessorHost Host { get; set; }

        /// <summary>
        /// Synchronously execute this task.
        /// </summary>
        public override bool Execute() => ExecuteAsync(Processor.CreateAsync(BuildEngine.ProjectFileOfTaskNode, cts.Token)).Result;

        /// <summary>
        /// Asynchronously execute the voltaire task.
        /// </summary>
        public async Task<bool> ExecuteAsync(Task<Processor> processorTask)
        {
            if (IsDebugging)
            {
                if (Debugger.IsAttached)
                    Debugger.Break();
                else
                    Debugger.Launch();
            }

            try
            {
                using (Processor processor = await processorTask)
                {
                    processor.DebugMessageLogged += this.DebugMessageLogged;
                    processor.MessageLogged += this.MessageLogged;
                    processor.WarningLogged += this.WarningLogged;

                    await processor.ProcessAsync(cts.Token);

                    if (OutputSyntax)
                        await processor.OutputChangedSyntaxTreesAsync(null, cts.Token);
                }

                return true;
            }
            catch (Exception e)
            {
                while (e is TargetInvocationException)
                    e = e.InnerException;

                do
                {
                    Debug.Assert(e != null);

                    if (e is ProcessingException pe)
                    {
                        FileLinePositionSpan position = pe.Node.GetLocation().GetLineSpan();

                        BuildEngine.LogErrorEvent(new BuildErrorEventArgs(
                            "Processing error", null, position.Path,
                            position.StartLinePosition.Line, position.StartLinePosition.Character,
                            position.EndLinePosition.Line, position.EndLinePosition.Character,
                            e.Message, null, e.Source
                         ));
                    }
                    else if (e is ReflectionTypeLoadException tle)
                    {
                        BuildEngine.LogErrorEvent(new BuildErrorEventArgs(
                            "Type load error", null, null, 0, 0, 0, 0, tle.Message + " : " + string.Join<Exception>(Environment.NewLine, tle.LoaderExceptions), null, e.TargetSite?.ToString()
                        ));
                    }
                    else
                    {
                        BuildEngine.LogErrorEvent(new BuildErrorEventArgs(
                            "Unknown error", null, null, 0, 0, 0, 0, e.Message, null, e.Source
                        ));
                    }

                    e = e.InnerException;
                } while (e != null);

                return false;
            }
        }

        private void WarningLogged(object sender, ProcessingMessage e)
        {
            const string Subcategory = "Warning";

            if (e.Node == null)
            {
                BuildEngine.LogWarningEvent(new BuildWarningEventArgs(Subcategory, null, null, 0, 0, 0, 0, e.Message, null, e.Sender));
                return;
            }

            FileLinePositionSpan position = e.Node.GetLocation().GetLineSpan();

            BuildEngine.LogWarningEvent(new BuildWarningEventArgs(Subcategory, null, position.Path,
                position.StartLinePosition.Line, position.StartLinePosition.Character,
                position.EndLinePosition.Line, position.EndLinePosition.Character,
                e.Message, null, e.Sender
            ));
        }

        private void MessageLogged(object sender, ProcessingMessage e)
        {
            const string Subcategory = "Info";

            if (e.Node == null)
            {
                BuildEngine.LogMessageEvent(new BuildMessageEventArgs(Subcategory, null, null, 0, 0, 0, 0, e.Message, null, e.Sender, MessageImportance.Normal));
                return;
            }

            FileLinePositionSpan position = e.Node.GetLocation().GetLineSpan();

            BuildEngine.LogMessageEvent(new BuildMessageEventArgs(Subcategory, null, position.Path,
                position.StartLinePosition.Line, position.StartLinePosition.Character,
                position.EndLinePosition.Line, position.EndLinePosition.Character,
                e.Message, null, e.Sender, MessageImportance.Normal
            ));
        }

        private void DebugMessageLogged(object sender, ProcessingMessage e)
        {
            const string Subcategory = "Debug";

            if (e.Node == null)
            {
                BuildEngine.LogMessageEvent(new BuildMessageEventArgs(Subcategory, null, null, 0, 0, 0, 0, e.Message, null, e.Sender, MessageImportance.Low));
                return;
            }

            FileLinePositionSpan position = e.Node.GetLocation().GetLineSpan();

            BuildEngine.LogMessageEvent(new BuildMessageEventArgs(Subcategory, null, position.Path,
                position.StartLinePosition.Line, position.StartLinePosition.Character,
                position.EndLinePosition.Line, position.EndLinePosition.Character,
                e.Message, null, e.Sender, MessageImportance.Low
            ));
        }
    }
}
