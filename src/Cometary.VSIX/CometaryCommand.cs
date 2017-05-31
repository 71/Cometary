//------------------------------------------------------------------------------
// <copyright file="CometaryCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Cometary.VSIX
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CometaryCommand
    {
        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CometaryCommand Instance { get; private set; }

        /// <summary>
        /// <see cref="Processor"/> of the current solution.
        /// </summary>
        public Processor Processor { get; private set; }

        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("12786129-2956-4c78-9e30-2ddf2d253572");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly CometaryCommandPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="CometaryCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private CometaryCommand(CometaryCommandPackage package)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));

            if (this.ServiceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);

                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => this.package;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(CometaryCommandPackage package)
        {
            Instance = new CometaryCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private async void MenuItemCallback(object sender, EventArgs e)
        {
            MenuCommand cmd = sender as MenuCommand;

            if (cmd == null)
                return;

            if (!IsSingleProjectItemSelection(out IVsHierarchy hierarchy, out uint itemid))
                return;

            ((IVsProject)hierarchy).GetMkDocument(itemid, out string docPath);

            if (docPath == null)
                return;

            var proj = package.Workspace.CurrentSolution.Projects.FirstOrDefault(x => x.FilePath == docPath);

            if (proj == null)
                return;

            Processor = await Processor.CreateAsync(package.Workspace, proj);
            await Processor.ProcessAsync();
        }

        public static bool IsSingleProjectItemSelection(out IVsHierarchy hierarchy, out uint itemid)
        {
            hierarchy = null;
            itemid = VSConstants.VSITEMID_NIL;

            var monitorSelection = Package.GetGlobalService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;
            var solution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
            if (monitorSelection == null || solution == null)
            {
                return false;
            }

            IntPtr selectionContainerPtr = IntPtr.Zero;
            IntPtr hierarchyPtr = IntPtr.Zero;

            try
            {
                int hr = monitorSelection.GetCurrentSelection(out hierarchyPtr, out itemid,
                    out IVsMultiItemSelect multiItemSelect, out selectionContainerPtr);

                if (ErrorHandler.Failed(hr) || hierarchyPtr == IntPtr.Zero || itemid == VSConstants.VSITEMID_NIL)
                {
                    // there is no selection
                    return false;
                }

                // multiple items are selected
                if (multiItemSelect != null) return false;

                // there is a hierarchy root node selected, thus it is not a single item inside a project
                var obj = Marshal.GetObjectForIUnknown(hierarchyPtr);
                hierarchy = Marshal.GetObjectForIUnknown(hierarchyPtr) as IVsHierarchy;

                if (hierarchy == null) return false;

                if (ErrorHandler.Failed(solution.GetGuidOfProject(hierarchy, out Guid _)))
                {
                    return false; // hierarchy is not a project inside the Solution if it does not have a ProjectID Guid
                }

                // if we got this far then there is a single project item selected
                return true;
            }
            finally
            {
                if (selectionContainerPtr != IntPtr.Zero)
                {
                    Marshal.Release(selectionContainerPtr);
                }

                if (hierarchyPtr != IntPtr.Zero)
                {
                    Marshal.Release(hierarchyPtr);
                }
            }
        }
    }
}
