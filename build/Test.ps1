<#  
.SYNOPSIS  
    Cometary test utility

.DESCRIPTION  
    This script runs every available test.

.EXAMPLES
    .\Test.ps1
#>

param([switch] $ContinueOnError, [switch] $BuildDependencies, [string[]] $SkipTests = ("Cometary", "Cometary.IL", "Cometary.Expressions", "Cometary.CleanUp"), [string[]] $Tests)

foreach ($ProjectFile in Get-ChildItem ../test/**/*.Tests.csproj) {
    $FriendlyName = $ProjectFile.BaseName -replace '.Tests', ''

    if ($Tests -ne $null -and $Tests -notcontains $FriendlyName) {
        continue
    }
    if ($SkipTests -contains $FriendlyName) {
        continue
    }

    Write-Host -ForegroundColor Green "[i] Building $($ProjectFile.BaseName)..."

    if ($BuildDependencies) {
        $Output = dotnet.exe build "$($ProjectFile.FullName)" --configuration Test --verbosity quiet
    } else {
        $Output = dotnet.exe build "$($ProjectFile.FullName)" --configuration Test --verbosity quiet --no-dependencies
    }

    if (-not $?) {
        Write-Host -ForegroundColor Red "[-] Error building the $($FriendlyName) project."

        if ($ContinueOnError) {
            continue
        }

        return
    }

    Write-Host -ForegroundColor Green "[i] Successfully built $($ProjectFile.BaseName). Testing..."

    $Output = dotnet.exe test "$($ProjectFile.FullName)" --configuration Test --no-build

    if (-not $?) {
        Write-Host -ForegroundColor Red "[-] Error testing the $($FriendlyName) project. Output:"
        
        $OutputStr = ($Output | Out-String)
        
        Write-Host -ForegroundColor DarkYellow $OutputStr.Substring($OutputStr.IndexOf("Starting test execution"))

        if ($ContinueOnError) {
            continue
        }

        return
    }

    Write-Output ($Output | Select-String '\[.+\]' -AllMatches)
    Write-Host
    Write-Host -ForegroundColor Blue "[+] Tests for the $($FriendlyName) library passed successfully."
    Write-Host
}