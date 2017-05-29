using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

using TextSpan = Microsoft.CodeAnalysis.Text.TextSpan;

namespace Cometary
{
    internal static class Program
    {
        private static bool CanGetBuffer;

        /// <summary>
        /// Entry point of the program, that serves as a synchronous
        /// wrapper around <see cref="Execute"/>.
        /// </summary>
        public static int Main(string[] args) => Execute(args).Result;

        /// <summary>
        /// Asynchronously processes the specified project.
        /// </summary>
        public static async Task<int> Execute(string[] args)
        {
            Console.WriteLine();

            if (args.Length == 0)
            {
#if DEBUG
                args = new[] { @"..\..\test\Cometary.Tests\Cometary.Tests.csproj", "--syntax" };
#else
                DisplayHelp();
                return 1;
#endif
            }

            bool willBeDebugging = false,
                 willOutputSyntax = false;
            string projectFile = null;

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
                using (Processor processor = await Processor.CreateAsync(projectFile))
                {
                    processor.DebugMessageLogged += DebugMessageLogged;
                    processor.MessageLogged += MessageLogged;
                    processor.WarningLogged += WarningLogged;

                    await processor.ProcessAsync();

                    if (willOutputSyntax)
                        await processor.OutputChangedSyntaxTreesAsync();
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