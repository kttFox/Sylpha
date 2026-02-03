# Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope Process

$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

# Sylpha.props から Version を取得
$doc = [XML](Get-Content "..\Sylpha.props")
$Version = $doc.Project.PropertyGroup.Version

Write-Host "Building version: $Version"

dotnet build ..\Sylpha.Code.slnx -c Release
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$distPath = "dist\$Version"
New-Item -ItemType Directory -Path $distPath -Force | Out-Null

$packages = @(
    "Sylpha",
    "Sylpha.EventListeners",
    "Sylpha.Messaging",
    "Sylpha.Messaging.Extensions"
)

foreach ($pkg in $packages) {
    Copy-Item "..\$pkg\bin\Release\$pkg.$Version.nupkg" $distPath
    Copy-Item "..\$pkg\bin\Release\$pkg.$Version.snupkg" $distPath
}

Write-Host "Packages copied to $distPath"