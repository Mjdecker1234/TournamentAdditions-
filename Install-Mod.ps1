<#
.SYNOPSIS
    Builds TournamentMastery and installs it into your Bannerlord Modules folder.

.DESCRIPTION
    Run this script (double-click in Explorer or execute in a PowerShell window)
    to compile the mod in Release mode and copy the output files directly into:

        <BannerlordDir>\Modules\TournamentMastery\

    The script will:
      1. Resolve your Bannerlord installation directory.
      2. Build TournamentMastery.csproj (Release, with auto-deploy).
      3. Verify the files arrived in the correct Modules sub-folder.

.NOTES
    Prerequisites
    -------------
    - .NET SDK 6, 7, or 8 installed (https://dotnet.microsoft.com/download)
    - BANNERLORD_GAME_DIR environment variable set, OR the script will prompt
      you to enter the path on first run and remember it for the session.

    To set BANNERLORD_GAME_DIR permanently (recommended):
      1. Open Windows Settings → System → About → Advanced system settings
      2. Click "Environment Variables"
      3. Under "User variables" add:
           Variable: BANNERLORD_GAME_DIR
           Value:    C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord

    Alternatively, create Directory.Build.props from the provided example:
      1. Copy Directory.Build.props.example → Directory.Build.props (repo root)
      2. Uncomment the <BannerlordDir> line and set your path
      (This file is gitignored so your local path won't be committed.)
#>

#Requires -Version 5.1
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# ── Locate script / project root ──────────────────────────────────────────────
$ScriptDir  = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectFile = Join-Path $ScriptDir 'TournamentMastery.csproj'

if (-not (Test-Path $ProjectFile)) {
    Write-Error "Cannot find TournamentMastery.csproj in '$ScriptDir'. Run this script from the repository root."
    exit 1
}

# ── Resolve Bannerlord directory ───────────────────────────────────────────────
$BannerlordDir = $env:BANNERLORD_GAME_DIR

if ([string]::IsNullOrWhiteSpace($BannerlordDir)) {
    Write-Host ""
    Write-Host "BANNERLORD_GAME_DIR is not set." -ForegroundColor Yellow
    Write-Host "Enter the full path to your Bannerlord installation."
    Write-Host "  Example: C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord"
    Write-Host ""
    $BannerlordDir = Read-Host "Bannerlord path"
}

$BannerlordDir = $BannerlordDir.Trim().TrimEnd('\')

if (-not (Test-Path $BannerlordDir)) {
    Write-Error "Path not found: '$BannerlordDir'. Check your BANNERLORD_GAME_DIR value."
    exit 1
}

$ModuleDir = Join-Path $BannerlordDir 'Modules\TournamentMastery'
Write-Host ""
Write-Host "=== TournamentMastery Installer ===" -ForegroundColor Cyan
Write-Host "Project   : $ProjectFile"
Write-Host "Target    : $ModuleDir"
Write-Host ""

# ── Build ──────────────────────────────────────────────────────────────────────
Write-Host "Building Release..." -ForegroundColor Cyan

$buildArgs = @(
    'build', $ProjectFile,
    '--configuration', 'Release',
    '--nologo',
    "-p:DeployAfterBuild=true",
    "-p:BannerlordDir=$BannerlordDir"
)

& dotnet @buildArgs

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Error "Build failed (exit code $LASTEXITCODE). Fix the errors above and re-run the script."
    exit $LASTEXITCODE
}

# ── Verify ─────────────────────────────────────────────────────────────────────
$DllPath = Join-Path $ModuleDir 'bin\Win64_Shipping_Client\TournamentMastery.dll'
$XmlPath = Join-Path $ModuleDir 'SubModule.xml'

Write-Host ""
if ((Test-Path $DllPath) -and (Test-Path $XmlPath)) {
    Write-Host "Installation successful!" -ForegroundColor Green
    Write-Host "  DLL      : $DllPath"
    Write-Host "  Manifest : $XmlPath"
    Write-Host ""
    Write-Host "Launch Bannerlord, enable TournamentMastery in the mod list, and enjoy!" -ForegroundColor Green
} else {
    Write-Host "Build completed but files were not found at the expected location." -ForegroundColor Yellow
    Write-Host "Expected DLL : $DllPath"
    Write-Host "Expected XML : $XmlPath"
    Write-Host ""
    Write-Host "Check that BANNERLORD_GAME_DIR points to your real Bannerlord folder." -ForegroundColor Yellow
}

Write-Host ""
# Keep the window open when double-clicked from Explorer
if ($Host.Name -eq 'ConsoleHost') {
    Read-Host "Press Enter to close"
}
