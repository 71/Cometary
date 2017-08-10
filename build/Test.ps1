<#  
.SYNOPSIS  
    Cometary test utility

.DESCRIPTION  
    This script runs every available test.

.EXAMPLES
    .\Test.ps1
#>

foreach ($ProjectFile in Get-ChildItem ../test/**/*.Tests.csproj) {
    dotnet.exe test "$($ProjectFile.FullName)" /nologo --configuration Test
}