using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;

namespace Cometary.VSIX
{
    internal static class Utils
    {
        public static readonly IComponentModel ComponentModel
            = Package.GetGlobalService(typeof(SComponentModel)) as IComponentModel;

        public static VisualStudioWorkspace Workspace
            => ComponentModel.GetService<VisualStudioWorkspace>();
    }
}
