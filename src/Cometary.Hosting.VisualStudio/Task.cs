using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using DTE = EnvDTE.DTE;

namespace Cometary
{
    /// <summary>
    /// MSBuild task that uses Visual Studio to immediately
    /// get a parsed project.
    /// </summary>
    /// <remarks>
    /// Implements a way to hook into Visual Studio to directly get
    /// the compilation object.
    /// Pretty useful since things get reaaal slow when using <see cref="MSBuildWorkspace"/>.
    /// </remarks>
    /// <seealso href="http://www.helixoft.com/blog/the-msbuild-task-for-executing-any-visual-studio-command.html"/>
    public sealed class VSCometaryTask : CometaryTask
    {
        /// <inheritdoc />
        public override bool Execute()
        {
            DTE dte = GetDTE();

            if (dte == null)
                return base.Execute();

            // ReSharper disable once SuspiciousTypeConversion.Global
            IServiceProvider sp = new ServiceProvider(dte as Microsoft.VisualStudio.OLE.Interop.IServiceProvider);
            IComponentModel  cm = sp.GetService(typeof(SComponentModel)) as IComponentModel;

            if (cm == null)
                return base.Execute();

            VisualStudioWorkspace workspace = cm.GetService<VisualStudioWorkspace>();

            if (workspace == null)
                return base.Execute();

            Project project = workspace.CurrentSolution.Projects.FirstOrDefault(x => x.FilePath == BuildEngine.ProjectFileOfTaskNode);

            if (project == null)
                return base.Execute();

            return ExecuteAsync(Processor.CreateAsync(workspace, project)).Result;
        }

        #region DTE utils
        /// <seealso href="https://msdn.microsoft.com/en-us/library/ms228772"/>
        public sealed class MessageFilter : IOleMessageFilter
        {
            // Start the filter.
            public static void Register()
            {
                IOleMessageFilter newFilter = new MessageFilter();

                CoRegisterMessageFilter(newFilter, out IOleMessageFilter _);
            }

            // Done with the filter, close it.
            public static void Revoke()
            {
                CoRegisterMessageFilter(null, out IOleMessageFilter _);
            }

            //
            // IOleMessageFilter functions.
            // Handle incoming thread requests.
            int IOleMessageFilter.HandleInComingCall(int dwCallType, IntPtr hTaskCaller, int dwTickCount, IntPtr lpInterfaceInfo)
            {
                //Return the flag SERVERCALL_ISHANDLED.
                return 0;
            }

            // Thread call was rejected, so try again.
            int IOleMessageFilter.RetryRejectedCall(IntPtr hTaskCallee, int dwTickCount, int dwRejectType)
            {
                if (dwRejectType == 2)
                // flag = SERVERCALL_RETRYLATER.
                {
                    // Retry the thread call immediately if return >=0 &
                    // <100.
                    return 99;
                }
                // Too busy; cancel call.
                return -1;
            }

            int IOleMessageFilter.MessagePending(IntPtr hTaskCallee, int dwTickCount, int dwPendingType)
            {
                //Return the flag PENDINGMSG_WAITDEFPROCESS.
                return 2;
            }

            // Implement the IOleMessageFilter interface.
            [DllImport("Ole32.dll")]
            private static extern int CoRegisterMessageFilter(IOleMessageFilter newFilter, out IOleMessageFilter oldFilter);
        }

        [ComImport, Guid("00000016-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IOleMessageFilter
        {
            [PreserveSig]
            int HandleInComingCall(int dwCallType, IntPtr hTaskCaller, int dwTickCount, IntPtr lpInterfaceInfo);

            [PreserveSig]
            int RetryRejectedCall(IntPtr hTaskCallee, int dwTickCount, int dwRejectType);

            [PreserveSig]
            int MessagePending(IntPtr hTaskCallee, int dwTickCount, int dwPendingType);
        }

        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(uint reserved, out IBindCtx ppbc);

        /// <summary>
        /// A utility class to determine a process parent.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ParentProcessUtilities
        {
            // These members must match PROCESS_BASIC_INFORMATION
            internal IntPtr Reserved1;
            internal IntPtr PebBaseAddress;
            internal IntPtr Reserved2_0;
            internal IntPtr Reserved2_1;
            internal IntPtr UniqueProcessId;
            internal IntPtr InheritedFromUniqueProcessId;

            [DllImport("ntdll.dll")]
            private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref ParentProcessUtilities processInformation, int processInformationLength, out int returnLength);

            /// <summary>
            /// Gets the parent process of specified process.
            /// </summary>
            /// <returns>An instance of the Process class.</returns>
            public static Process GetParentProcess(Process process)
            {
                return GetParentProcess(process.Handle);
            }

            /// <summary>
            /// Gets the parent process of a specified process.
            /// </summary>
            /// <returns>An instance of the Process class or null if an error occurred.</returns>
            public static Process GetParentProcess(IntPtr handle)
            {
                ParentProcessUtilities pbi = new ParentProcessUtilities();
                int status = NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), out int _);

                if (status != 0)
                    return null;

                try
                {
                    return Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
                }
                catch (ArgumentException)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the VS <see cref="DTE"/>.
        /// </summary>
        internal static DTE GetDTE()
        {
            try
            {
                Process vsProcess = Process.GetCurrentProcess();

                while (!vsProcess.ProcessName.Equals("devenv"))
                {
                    vsProcess = ParentProcessUtilities.GetParentProcess(vsProcess);

                    if (vsProcess == null)
                        return null;
                }

                // Register the IOleMessageFilter to handle any threading errors.
                // See https://msdn.microsoft.com/en-us/library/ms228772
                MessageFilter.Register();

                return GetDTE(vsProcess.Id);
            }
            catch
            {
                return null;
            }
            finally
            {
                // turn off the IOleMessageFilter.
                MessageFilter.Revoke();
            }
        }

        /// <summary>
        /// Gets the VS <see cref="DTE"/>, given the ID of the VS process.
        /// </summary>
        private static DTE GetDTE(int processId)
        {
            Regex monikerRegex = new Regex(@"!VisualStudio.DTE\.\d+\.\d+\:" + processId, RegexOptions.IgnoreCase);
            object runningObject = null;

            IBindCtx bindCtx = null;
            IRunningObjectTable rot = null;
            IEnumMoniker enumMonikers = null;

            try
            {
                Marshal.ThrowExceptionForHR(CreateBindCtx(0, out bindCtx));
                bindCtx.GetRunningObjectTable(out rot);
                rot.EnumRunning(out enumMonikers);

                IMoniker[] moniker = new IMoniker[1];
                IntPtr numberFetched = IntPtr.Zero;

                while (enumMonikers.Next(1, moniker, numberFetched) == 0)
                {
                    IMoniker runningObjectMoniker = moniker[0];

                    string name = null;

                    try
                    {
                        runningObjectMoniker?.GetDisplayName(bindCtx, null, out name);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Do nothing, there is something in the ROT that we do not have access to.
                    }

                    if (!string.IsNullOrEmpty(name) && monikerRegex.IsMatch(name))
                    {
                        Marshal.ThrowExceptionForHR(rot.GetObject(runningObjectMoniker, out runningObject));
                        break;
                    }
                }
            }
            finally
            {
                if (enumMonikers != null)
                    Marshal.ReleaseComObject(enumMonikers);

                if (rot != null)
                    Marshal.ReleaseComObject(rot);

                if (bindCtx != null)
                    Marshal.ReleaseComObject(bindCtx);
            }

            return runningObject as DTE;
        }
        #endregion
    }
}
