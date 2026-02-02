$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

# Sylpha.props から Version を取得
$doc = [XML](Get-Content "..\Sylpha.props")
$Version = $doc.Project.PropertyGroup.Version

$DistPath = ".\dist\$Version"

# Please get an API Key from nuget.org, and set the key using: dotnet nuget setApiKey xxxx
Get-ChildItem -Path $DistPath -Filter "*.nupkg" | ForEach-Object {
    Write-Host "Pushing $($_.Name)..." -ForegroundColor Cyan
    dotnet nuget push $_.FullName -s https://www.nuget.org/api/v2/package
}

