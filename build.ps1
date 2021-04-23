[CmdletBinding(PositionalBinding=$false)]
param(
    [bool] $CreatePackages = $false,
    [bool] $RunTests = $true
)

Write-Host "Run Parameters:" -ForegroundColor Cyan
Write-Host "  CreatePackages: $CreatePackages"
Write-Host "  RunTests: $RunTests"

$packageOutputFolder = "$PSScriptRoot\.nupkgs"
$projectFile = ".\Dapper.Apex\Dapper.Apex.csproj";
$testProjectFile = ".\Dapper.Apex.Test\Dapper.Apex.Test.csproj";

Write-Host "Building Dapper.Apex..." -ForegroundColor Yellow
dotnet build $projectFile -c Release /p:CI=true
Write-Host "Done building." -ForegroundColor Green

if ($RunTests) {
    Write-Host "Running tests: Dapper.Apex.Test" -ForegroundColor Yellow
    dotnet build $testProjectFile -c Release /p:CI=true
    dotnet test $testProjectFile -c Release --no-build
    if ($LastExitCode -ne 0) {
        Write-Host "Error with tests, aborting..." -Foreground Red
        Exit 1
    }
    Write-Host "Tests passed!" -ForegroundColor Green
}

if ($CreatePackages) {
    New-Item -ItemType Directory -Path $packageOutputFolder -Force | Out-Null
    Write-Host "Clearing existing $packageOutputFolder..." -NoNewline
    Get-ChildItem $packageOutputFolder | Remove-Item
    Write-Host "done." -ForegroundColor Green

    Write-Host "Building Dappe.Apex package..." -ForegroundColor Yellow
    dotnet pack $projectFile --no-build -c Release /p:PackageOutputPath=$packageOutputFolder /p:CI=true
}
Write-Host "Build Complete." -ForegroundColor Green