$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

# Sylpha.props から Version を取得
$doc = [XML](Get-Content "..\Sylpha.props")
$Version = $doc.Project.PropertyGroup.Version

$DistPath = ".\dist\$Version"

# nuget.config から APIKey を取得
$nugetConfig = [XML](Get-Content ".\nuget.config")
$apiKey = $nugetConfig.configuration.apikeys.add | 
    Where-Object { $_.key -eq "https://api.nuget.org/v3/index.json" } | 
    Select-Object -ExpandProperty value

if (-not $apiKey) {
    Write-Host "Error: API Key not found in nuget.config" -ForegroundColor Red
    exit 1
}

Get-ChildItem -Path $DistPath\* -Include "*.nupkg" | ForEach-Object {
    Write-Host "Pushing $($_.Name)..." -ForegroundColor Cyan
    dotnet nuget push $_.FullName -s nuget --api-key $apiKey --skip-duplicate
}


