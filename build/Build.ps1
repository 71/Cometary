param([switch] $Publish, [string] $Configuration = "Release")

# Some projects aren't ready yet, don't build 'em.
$Blacklist = "Cometary", "Cometary.Expressions", "Cometary.IL"

# Clean previous builds.
foreach ($Package in Get-ChildItem ./*.nupkg) {
    Remove-Item $Package
}

# Build all projects.
foreach ($Project in Get-ChildItem ../src/**/*.csproj) {
	if ($Blacklist -contains $Project.BaseName) {
        continue;
    }

    dotnet.exe pack $Project --configuration $Configuration --output $(Get-Location)
}

if ($Publish) {
    # Optionally publish the package on NuGet
    foreach ($Package in Get-ChildItem ./*.nupkg) {
        nuget.exe push $Package
    }
}