using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using TextSpan = Microsoft.CodeAnalysis.Text.TextSpan;

namespace Cometary
{
    internal static class Program
    {
        private static bool CanGetBuffer;

        /// <summary>
        ///   Entry point of the program, that serves as a synchronous
        ///   wrapper around <see cref="ExecuteAsync"/>.
        /// </summary>
        public static int Main(string[] args) => ExecuteAsync(args).Result;

        /// <summary>
        ///   Asynchronously processes the specified project.
        /// </summary>
        public static async Task<int> ExecuteAsync(string[] args)
        {
            Console.WriteLine();

            if (args.Length == 0)
            {
#if DEBUG
                args = new[] { @"..\Cometary\Cometary.csproj", "--syntax" };
#else
                DisplayHelp();
                return 1;
#endif
            }

            bool willBeDebugging  = false,
                 willOutputSyntax = false;
            string projectFile = null,
                   outputFile  = null;

            // Args parsing
            for (int i = 0; i < args.Length; ++i)
            {
                string arg = args[i];

                if (arg == "-d" || arg == "--debug")
                {
                    willBeDebugging = true;
                }
                else if (arg == "-s" || arg == "--syntax")
                {
                    willOutputSyntax = true;
                }
                else if (arg == "-o" || arg == "--output")
                {
                    if (args.Length - 1 == i)
                    {
                        Console.Error.WriteLine("[-] Missing argument value: output.");
                        return 1;
                    }

                    outputFile = args[++i];
                }
                else if (arg[0] == '-')
                {
                    DisplayHelp(arg);
                    return 1;
                }
                else
                {
                    projectFile = arg;
                }
            }

            if (projectFile == null)
            {
                Console.Error.WriteLine("[-] No target given.");
                return 1;
            }

            try
            {
                CanGetBuffer = Console.BufferWidth > 0;
            }
            catch
            {
                CanGetBuffer = false;
            }

            if (willBeDebugging)
            {
                if (Debugger.IsAttached)
                    Debugger.Break();
                else
                    Debugger.Launch();
            }

            try
            {
                using (MSBuildWorkspace workspace = MSBuildWorkspace.Create())
                using (ProcessorHost host = ProcessorHost.GetHost(workspace))
                {
                    host.DebugMessageLogged += DebugMessageLogged;
                    host.MessageLogged += MessageLogged;
                    host.WarningLogged += WarningLogged;

                    Project project = await workspace.OpenProjectAsync(projectFile);

                    using (Processor processor = await host.GetProcessorAsync(project))
                    {
                        processor.TrackChanges = true;

                        await processor.WriteAssemblyAsync(outputFile);

                        if (willOutputSyntax)
                            await processor.OutputChangedSyntaxTreesAsync(project);
                    }
                }

                return 0;
            }
            catch (Exception e)
            {
                while (e is TargetInvocationException)
                    e = e.InnerException;

                do
                {
                    if (e is AggregateException ae)
                    {
                        foreach (Exception _e in ae.InnerExceptions)
                            Log(new ProcessingMessage(_e.Message, _e is ProcessingException _pe ? _pe.Span : default(TextSpan), _e.Source), '-', ConsoleColor.Red);

                        break;
                    }
                    if (e is ReflectionTypeLoadException re)
                    {
                        Log(new ProcessingMessage(e.Message), '-', ConsoleColor.Red);

                        foreach (Exception _e in re.LoaderExceptions)
                            Log(new ProcessingMessage(_e.Message), '-', ConsoleColor.Red);

                        break;
                    }

                    if (!string.IsNullOrWhiteSpace(e.Message))
                        Log(new ProcessingMessage(e.Message, e is ProcessingException pe ? pe.Span : default(TextSpan), e.Source), '-', ConsoleColor.Red);

                    if (!string.IsNullOrWhiteSpace(e.StackTrace))
                        Log(new ProcessingMessage(e.StackTrace), '-', ConsoleColor.DarkRed);

                    e = e.InnerException;
                } while (e != null);

                return 1;
            }
        }

        private static void DisplayHelp(string arg = null)
        {
            if (arg != null)
                Console.Error.WriteLine("[-] Invalid argument: {0}", arg);

            Console.WriteLine("USAGE:");
            Console.WriteLine("    dotnet-cometary [project] [--debug]");
            Console.WriteLine();
            Console.WriteLine("WITH:");
            Console.WriteLine("    project   Path to the assembly to process.");
            Console.WriteLine("    debug     Debug the processing operation.");
            Console.WriteLine();
            Console.WriteLine("EXAMPLE:");
            Console.WriteLine("    dotnet cometary Project.csproj");
        }

        #region Logging
        private static void WarningLogged(object sender, ProcessingMessage e)
            => Log(e, '!', ConsoleColor.Yellow);

        private static void MessageLogged(object sender, ProcessingMessage e)
            => Log(e, '+', ConsoleColor.White);

        private static void DebugMessageLogged(object sender, ProcessingMessage e)
            => Log(e, '#', ConsoleColor.Gray);

        private static void Log(ProcessingMessage message, char prefix, ConsoleColor color)
        {
            Console.ForegroundColor = color;

            int prefixWidth = 4;

            Console.Write("[{0}] ", prefix);

            if (message.Node != null)
            {
                string msg = $"({message.Span}) ";
                Console.Write(msg);

                prefixWidth += msg.Length;
            }

            string str = message.Message;

            if (!CanGetBuffer)
            {
                Console.WriteLine(str);
                Console.ResetColor();

                return;
            }

            int maxLength = Console.BufferWidth - prefixWidth - 1;
            int msgLength = str.Length;

            for (int i = 0; i < msgLength;)
            {
                if (i != 0)
                    Console.Write(new string(' ', prefixWidth));

                int length = Math.Min(maxLength, msgLength - i);
                int nextLR = str.IndexOf('\n', i + 1);

                if (nextLR != -1)
                    length = Math.Min(length, nextLR - i);

                Console.WriteLine(str.Substring(i, length).Trim('\r', '\n'));

                i += length;
            }

            Console.ResetColor();
        }
        #endregion
    }
}