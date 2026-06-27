#!/usr/bin/env pwsh
# Find all MSYS2 DLL dependencies (recursive) of libuxplaylib.dll
param(
    [string]$Dll = "$PSScriptRoot\..\libuxplay\build\libuxplaylib.dll",
    [string]$Msys2Bin = "C:\msys64\ucrt64\bin"
)

$objdump = "$Msys2Bin\objdump.exe"
if (-not (Test-Path $objdump)) { Write-Error "objdump not found: $objdump"; exit 1 }

$visited = @{}
$msys2 = @{}
$system = @{}

function Find-Deps([string]$dll) {
    if ($visited.ContainsKey($dll)) { return }
    $visited[$dll] = $true

    $lines = & $objdump -p $dll 2>$null | Select-String "DLL Name"
    foreach ($line in $lines) {
        $name = ($line -split "DLL Name: ")[1].Trim().ToLower()
        $found = Join-Path $Msys2Bin $name
        if (Test-Path $found) {
            if (-not $msys2.ContainsKey($name)) {
                $msys2[$name] = $found
                Find-Deps $found
            }
        } else {
            $system[$name] = $true
        }
    }
}

Write-Host "Analyzing: $Dll" -ForegroundColor Cyan
Find-Deps (Resolve-Path $Dll)

Write-Host "`nMSYS2 DLLs to bundle ($($msys2.Count)):" -ForegroundColor Yellow
$msys2.Keys | Sort-Object | ForEach-Object { Write-Host "  $_" }

Write-Host "`nSystem DLLs skipped ($($system.Count)):" -ForegroundColor DarkGray
$system.Keys | Sort-Object | ForEach-Object { Write-Host "  $_" }

# Output just the file list for use in build scripts
$msys2.Keys | Sort-Object | Out-File "$PSScriptRoot\..\msys2-deps.txt" -Encoding ascii
Write-Host "`nWrote msys2-deps.txt" -ForegroundColor Green
