using System;
using System.Reflection;

namespace Cometary
{
    /// <summary>
    ///   Shim for APIs available only on CoreCLR.
    /// </summary>
    /// <seealso href="http://source.roslyn.io/#Microsoft.CodeAnalysis.Scripting/CoreClrShim.cs" />
    /// <seealso href="https://github.com/dotnet/core-setup/blob/release/1.0.0/src/corehost/cli/hostpolicy.cpp#L91-L123" />
    internal static class Shim
    {
        internal static readonly Type AssemblyLoadContext
            = Type.GetType("System.Runtime.Loader.AssemblyLoadContext, System.Runtime.Loader, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false);

        internal static readonly Type AppContext
            = Type.GetType("System.AppContext, System.AppContext, Version=4.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false);

        internal static readonly Func<string, object> GetData
            = AppContext?.GetTypeInfo().GetDeclaredMethod(nameof(GetData))?.CreateDelegate(typeof(Func<string, object>)) as Func<string, object>;


        internal static readonly bool IsRunningOnCoreCLR
            = AssemblyLoadContext != null;


        internal static readonly string[] TrustedPlatformAssemblies
            = (GetData?.Invoke("TRUSTED_PLATFORM_ASSEMBLIES") as string)?.Split(';');

        internal static readonly string[] NativeDLLSearchDirectories
            = (GetData?.Invoke("NATIVE_DLL_SEARCH_DIRECTORIES") as string)?.Split(';');

        internal static readonly string[] AppPaths
            = (GetData?.Invoke("APP_PATHS") as string)?.Split(';');
    }
}
