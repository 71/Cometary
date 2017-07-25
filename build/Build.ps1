param([switch] $Publish, [string] $Configuration = "Release")

# Build all projects
foreach ($Project in Get-ChildItem ../src/**/*.csproj) {
    dotnet.exe pack $Project --configuration $Configuration --output $(Get-Location)
}

if ($Publish) {
    # Optionally publish the package on NuGet
    foreach ($Package in Get-ChildItem ./*.nupkg) {
        nuget.exe push $Package
    }
}