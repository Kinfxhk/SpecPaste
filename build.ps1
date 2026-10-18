$ErrorActionPreference = "Stop"

$Name    = "SpecPaste"
$Version = "1.0.0"
$Root    = $PSScriptRoot

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    $dotnetDir = "C:\Program Files\dotnet"
    if (Test-Path $dotnetDir) { $env:Path = "$dotnetDir;$env:Path" }
}

Write-Host "Build..."
dotnet build "$Root\src\SpecPaste\SpecPaste.csproj" -c Release
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Test..."
dotnet test "$Root\tests\SpecPaste.Tests\SpecPaste.Tests.csproj" -c Release
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Publish..."
$publishDir = "$Root\dist\publish"
if (Test-Path $publishDir) { Remove-Item -Recurse -Force $publishDir }
dotnet publish "$Root\src\SpecPaste\SpecPaste.csproj" -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o $publishDir
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Zip..."
New-Item -ItemType Directory -Force -Path "$Root\dist" | Out-Null
$zip = "$Root\dist\$Name-v$Version-win-x64.zip"
if (Test-Path $zip) { Remove-Item -Force $zip }
Compress-Archive -Path "$publishDir\*" -DestinationPath $zip -Force

Write-Host "Done: $zip"
