<#  
.SYNOPSIS  
    Cometary build utility

.DESCRIPTION  
    This script provides the following abilities:
        - Incremental build (with automatic patch number increase)
        - Automatic publishing of packages on NuGet

.EXAMPLES
    .\Build.ps1 -Publish -Stable
#>

param([switch] $Reset, [switch] $Publish, [switch] $Stable, [switch] $Verbose, [string] $Configuration = "Release", [string[]] $Blacklist = ("Cometary", "Cometary.Expressions", "Cometary.IL", "Cometary.CleanUp"))

# Read versions
$Data = Get-Content -Raw ./data.json | ConvertFrom-Json
$Versions = $Data.versions
$Hashes = $Data.hashes

function Persist-Data {    
    $Data.versions = $Versions
    $Data.hashes = $Hashes

    Set-Content ./data.json ($Data | ConvertTo-Json)
}

if ($Reset) {
    # Simply reset versions and hashes
    $Versions = @{}
    $Hashes = @{}

    Persist-Data
    return
}

$Verbosity = "minimal"

if ($Verbose) {
    $Verbosity = "detailed"
}

# Clean previous builds.
foreach ($Package in Get-ChildItem ./*.nupkg) {
    Remove-Item $Package
}

# Build all projects.
foreach ($ProjectFile in Get-ChildItem ../src/**/*.csproj) {
    $ProjectName = $ProjectFile.BaseName
    
	if ($Blacklist -contains $ProjectName) {
        # Not ready for a release build yet
        continue
    }

    # Find patch number for project
    $Patch = $Versions.$ProjectName

    if ($Patch -eq $null) {
        $Versions | Add-Member -MemberType NoteProperty -Name $ProjectName -Value 0
    }
    
    if ($Stable) {
         # Stable release, reset patch number
         $Versions.$ProjectName = 0
    }

    $Patch = $Versions.$ProjectName

    # Build project
    ($BuildOutput = dotnet.exe build "$($ProjectFile.FullName)" /nologo /p:PatchNumber=$Patch --configuration $Configuration --verbosity $Verbosity)

    if (-not $?) {
        Write-Host -ForegroundColor Red "Error while building $ProjectName, skipping..."
        continue
    }

    # Find built dll
    $BuiltLibrary = ($BuildOutput | Select-String "-> (.+\.dll)$" -AllMatches).Matches[-1].Groups[1].Value

    # Compare hashes
    $ProjectHash = (Get-FileHash $BuiltLibrary -Algorithm SHA256).Hash

    $PreviousHash = $Hashes.$ProjectName

    if ($PreviousHash -eq $null) {
        # Hash has never computed before
        $Hashes | Add-Member -MemberType NoteProperty -Name $ProjectName -Value $ProjectHash
    } elseif ($PreviousHash -eq $ProjectHash) {
        # Same hash
        Write-Host -ForegroundColor Blue "Project $ProjectName hasn't been updated, skipping..."
        continue
    } else {
        # Different hash
        $Hashes.$ProjectName = $ProjectHash
    }

    # Different hash: pack the project
    dotnet.exe pack $($ProjectFile.FullName) /p:PatchNumber=$Patch --configuration $Configuration --output $(Get-Location) --no-build

    if (-not $?) {
        Write-Host -ForegroundColor Red "Error while packing $ProjectName, skipping..."
        continue
    }

    # Increase the patch number
    $Versions.$ProjectName = $Patch + 1
    Write-Host
}

if ($Publish) {
    # Optionally publish the package on NuGet
    foreach ($Package in Get-ChildItem ./*.nupkg) {
        nuget.exe push $Package
    }
}

# Write versions / hashes to disk
Persist-Data