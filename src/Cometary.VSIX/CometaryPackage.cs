using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using Project = Microsoft.CodeAnalysis.Project;

namespace Cometary.VSIX
{
    /* See:
     * - https://github.com/Microsoft/VSSDK-Extensibility-Samples/tree/master/Code_Sweep
     *   uses VS to modify the build process, but does not build things itself. That's what I'd like
     *   to do, in order to allow the user to use any version of Cometary, no matter the version of the VSIX.
     *   
     * - https://github.com/6A/Scry
     *   is my first VS package, and has some code that can be reused.
     */

    /// <summary>
    ///   Visual Studio <see cref="Package"/> class
    ///   that allows Cometary to be integrated in the IDE.
    /// </summary>
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExistsAndFullyLoaded_string)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(Guids.CometaryCommandPackage)]
    public sealed class CometaryPackage : Package
    {
        #region Static members
        /// <summary>
        ///   Gets the global instance of the Cometary package.
        /// </summary>
        public static CometaryPackage Instance { get; private set; }

        /// <summary>
        ///   Gets the global instance of the <see cref="VisualStudioWorkspace"/>
        ///   used by Visual Studio.
        /// </summary>
        public static VisualStudioWorkspace GlobalWorkspace
            => GetGlobalService(typeof(SComponentModel)) is IComponentModel componentModel
             ? componentModel.GetService<VisualStudioWorkspace>()
             : null; 
        #endregion

        private BuildEvents buildEvents;
        private Projects projects;

        private readonly Dictionary<ProjectElement, string> modifiedElements = new Dictionary<ProjectElement, string>();

        /// <summary>
        ///   Gets the <see cref="VisualStudioWorkspace"/> of the current solution.
        /// </summary>
        public VisualStudioWorkspace Workspace { get; private set; }

        /// <summary>
        ///   Gets the processor host of the current VS instance.
        /// </summary>
        public ProcessorHost Host { get; private set; }

        /// <summary>
        ///   Initialization of the package; this method is called right after the package is sited, so this is the place
        ///   where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            if (GetGlobalService(typeof(SComponentModel)) is IComponentModel componentModel)
            {
                Workspace = componentModel.GetService<VisualStudioWorkspace>();
                Host = ProcessorHost.GetHost(Workspace);
            }

            if (GetGlobalService(typeof(SDTE)) is DTE dte)
            {
                buildEvents = dte.Events.BuildEvents;
                buildEvents.OnBuildBegin += OnBuildBegin;
                buildEvents.OnBuildDone  += OnBuildDone;

                projects = dte.Solution.Projects;
            }

            Workspace.WorkspaceChanged += WorkspaceChanged;

            ExecuteCometaryCommand.Initialize(this);
            AddCometaryTaskCommand.Initialize(this);

            base.Initialize();
        }

        private void OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
        {
            foreach (EnvDTE.Project envProject in projects)
            {
                var project = ProjectCollection.GlobalProjectCollection
                    .GetLoadedProjects(envProject.FileName)
                    .FirstOrDefault();

                var task = project?.Xml.Targets
                    .FirstOrDefault(x => x.Name == nameof(Cometary));

                if (task == null)
                    continue;

                modifiedElements.Add(task, task.Condition);

                task.Condition = "False";
            }
        }

        private void OnBuildDone(vsBuildScope Scope, vsBuildAction Action)
        {
            foreach (var pair in modifiedElements)
            {
                pair.Key.Condition = pair.Value;
            }

            modifiedElements.Clear();
        }

        /// <summary>
        ///   Disposes of all processors.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Host.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        ///   Keeps track of changed workspaces.
        /// </summary>
        private async void WorkspaceChanged(object sender, WorkspaceChangeEventArgs e)
        {
            switch (e.Kind)
            {
                case WorkspaceChangeKind.SolutionChanged:
                case WorkspaceChangeKind.SolutionRemoved:
                case WorkspaceChangeKind.SolutionCleared:
                case WorkspaceChangeKind.SolutionReloaded:
                    Host.Dispose();
                    Host = ProcessorHost.GetHost(Workspace);
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

            await Host.GetProcessorAsync(proj);
        }
    }
}
