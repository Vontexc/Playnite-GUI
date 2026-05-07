<#
.SYNOPSIS
    Packs the SteamLike theme into a .pthm file using Playnite's Toolbox.

.PARAMETER PlayniteRoot
    Path to the Playnite installation directory containing Toolbox.exe.
    Defaults to the standard install location.

.PARAMETER Output
    Output directory for the .pthm file. Created if it does not exist.

.EXAMPLE
    .\pack.ps1
    .\pack.ps1 -PlayniteRoot "D:\Apps\Playnite" -Output ".\dist"
#>

[CmdletBinding()]
param(
    [string]$PlayniteRoot = "$Env:LOCALAPPDATA\Playnite",
    [string]$Output       = "$PSScriptRoot\dist"
)

$ErrorActionPreference = "Stop"

$toolbox = Join-Path $PlayniteRoot "Toolbox.exe"
if (-not (Test-Path $toolbox))
{
    $alt = "C:\Program Files\Playnite\Toolbox.exe"
    if (Test-Path $alt) { $toolbox = $alt }
    else { throw "Toolbox.exe not found under '$PlayniteRoot'. Pass -PlayniteRoot." }
}

if (-not (Test-Path $Output)) { New-Item -ItemType Directory -Path $Output | Out-Null }

Write-Host "Toolbox: $toolbox"
Write-Host "Source:  $PSScriptRoot"
Write-Host "Output:  $Output"

& $toolbox pack $PSScriptRoot $Output
if ($LASTEXITCODE -ne 0) { throw "Toolbox.exe pack failed with exit code $LASTEXITCODE" }

$pthm = Get-ChildItem -Path $Output -Filter "*.pthm" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
if ($pthm) { Write-Host "`nPacked: $($pthm.FullName)" -ForegroundColor Green }
