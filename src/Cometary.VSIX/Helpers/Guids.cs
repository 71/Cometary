using System;

namespace Cometary.VSIX
{
    /// <summary>
    /// Enables global access to the package's defined <see cref="Guid"/>'s.
    /// </summary>
    internal static class Guids
    {
        public const string CometaryCommandPackage = "8fad0f9b-a3b2-433e-81f2-525d2d9de9d2";
        public const string CometaryCommandPackageCmdSet = "12786129-2956-4c78-9e30-2ddf2d253572";
        public const string ExecuteImages = "87cb0680-8b6c-4671-b205-4b34716e2720";
        public const string AddTaskImages = "87cb0680-8b6c-4671-b205-4b34716e2720";
    }

    /// <summary>
    /// Enables global access to the package's defined ID's.
    /// </summary>
    internal static class Ids
    {
        public const int ExecuteCommandID = 0x100;
        public const int AddTaskCommandID = 0x200;
    }
}
