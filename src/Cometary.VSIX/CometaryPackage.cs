using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using Project = Microsoft.CodeAnalysis.Project;

namespace Cometary.VSIX
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuidString)]
    public sealed class CometaryPackage : Package
    {
        /// <summary>
        /// CometaryCommandPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "8fad0f9b-a3b2-433e-81f2-525d2d9de9d2";

        /// <summary>
        /// Gets the static instance of the Cometary package.
        /// </summary>
        public static CometaryPackage Instance { get; private set; }

        /// <summary>
        /// Gets the <see cref="VisualStudioWorkspace"/> of the current solution.
        /// </summary>
        public VisualStudioWorkspace Workspace { get; private set; }

        /// <summary>
        /// Gets the processors of the current solution.
        /// </summary>
        public Dictionary<ProjectId, Processor> Processors { get; } = new Dictionary<ProjectId, Processor>();

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            if (GetGlobalService(typeof(SComponentModel)) is IComponentModel componentModel)
                Workspace = componentModel.GetService<VisualStudioWorkspace>();

            Workspace.WorkspaceChanged += WorkspaceChanged;

            CometaryCommand.Initialize(this);

            base.Initialize();
        }

        /// <summary>
        /// Disposes of all processors.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (Processor processor in Processors.Values)
                    processor.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Keeps track of changed workspaces.
        /// </summary>
        private async void WorkspaceChanged(object sender, WorkspaceChangeEventArgs e)
        {
            switch (e.Kind)
            {
                case WorkspaceChangeKind.SolutionChanged:
                case WorkspaceChangeKind.SolutionRemoved:
                case WorkspaceChangeKind.SolutionCleared:
                case WorkspaceChangeKind.SolutionReloaded:
                    Processors.Clear();
                    break;
                case WorkspaceChangeKind.ProjectRemoved:
                    Processors.Remove(e.ProjectId);
                    break;
            }

            if (e.ProjectId == null)
                return;

            Project proj = e.NewSolution.GetProject(e.ProjectId);

            if (e.Kind == WorkspaceChangeKind.ProjectAdded)
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                if (GetGlobalService(typeof(SVsSolution)) is IVsSolution solution &&
                    solution.GetProjectOfGuid(proj.Id.Id, out IVsHierarchy hierarchy) == VSConstants.S_OK &&
                    hierarchy is IVsBuildPropertyStorage storage)
                    storage.SetPropertyValue("HasCometaryExtension", null, (uint)_PersistStorageType.PST_USER_FILE, "True");
            }

            Processors[e.ProjectId] = await Processor.CreateAsync(sender as Workspace, proj);
        }
    }
}
