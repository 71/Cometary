# Remotes
Cometary is made of two main components: the runtime libraries, and the remotes.  
The runtime libraries are references you add to your project, and allow you to customize your assembly and your code.  
The remotes take care of opening your project, building it, executing your assembly, and saving it after being modified. They should not be referenced.

Right now, three remotes exist, but only one works ([as stated in the README](../README.md#current-state)). All of them depend on the [Cometary.Remote](../src/Cometary.Remote) library.

## [.NET Core remote](../src/Cometary.Remote.Core)
This remote is the command line remote, and works thanks to the [dotnet CLI tools](https://github.com/dotnet/cli). Since it doesn't depend on MSBuild, it also doesn't have many issues related to it.

#### `dotnet-cometary.exe "<project path>" [--debug] [--syntax]`
- `<project name>` is the full path to the project to build.
- `--debug` enables debugging: `Debugger.Launch()` will be called as soon as Cometary starts.
- `--syntax` enables syntax output; the modified syntax trees will be saved as `.cs` files (by default in the `[project dir]/obj/cometary-syntax` directory).

## [MSBuild remote](../src/Cometary.Remote.MSBuild)
This remote uses MSBuild tasks to better integrate in the building experience. It can be directly configured from the `.csproj` file, and needs less setup than the .NET Core remote.  
However, an issue keeps anyone from using both `Microsoft.CodeAnalysis` and `Microsoft.MSBuild` correctly, which makes this remote (currently) unavailable.

## [Visual Studio remote](../src/Cometary.Remote.VisualStudio)
This remote is based on the MSBuild task, and thus does not work yet either. Instead of opening the project file on build like the other remotes, it directly gets the `Project` object from Visual Studio.

Instead of spending many previous seconds reading files, and parsing them, this remote directly gets a fully working project, and thus works **much faster**.