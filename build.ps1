#!/usr/bin/env pwsh
# ========================================================
# UxPlay Client — 统一构建脚本
#   1. 构建 libuxplay (C/C++ → libuxplaylib.dll)
#   2. 构建 UxPlayClient (.NET 10 → UxPlayClient.exe)
#   3. 可选：发布 + 生成安装程序
# ========================================================
param(
    [string]$Config    = "Release",
    [string]$Runtime   = "win-x64",
    [switch]$SkipNative,
    [switch]$Publish,
    [switch]$Installer
)
$ErrorActionPreference = "Stop"
$Root = $PSScriptRoot

# ── 查找 dotnet ──
$dotnet = $null
foreach ($p in @(
    "$env:LOCALAPPDATA\Microsoft\dotnet\dotnet.exe",
    "$env:ProgramFiles\dotnet\dotnet.exe",
    "dotnet"
)) {
    try { $v = & $p --version 2>$null; if ($v -match '^\d') { $dotnet = $p; break } } catch {}
}
if (-not $dotnet) { Write-Error "未找到 .NET SDK。请安装 .NET 10: https://dot.net"; exit 1 }
Write-Host "=== UxPlay Client Build ===" -ForegroundColor Cyan
Write-Host "  dotnet : $dotnet ($(& $dotnet --version))"
Write-Host "  Config : $Config"

# ── 1. 构建 libuxplay ──
if (-not $SkipNative) {
    $nativeDll = "$Root\libuxplay\build\libuxplaylib.dll"
    if (Test-Path $nativeDll) {
        Write-Host "`n> libuxplaylib.dll 已存在，跳过原生构建 (-SkipNative 可强制跳过)" -ForegroundColor DarkGray
    } else {
        Write-Host "`n> 构建 libuxplay (cmake + ninja)..." -ForegroundColor Yellow
        $buildDir = "$Root\libuxplay\build"
        New-Item -ItemType Directory -Force -Path $buildDir | Out-Null
        Push-Location $buildDir
        cmake .. -G Ninja -DCMAKE_BUILD_TYPE=Release -DBUILD_SHARED_LIBS=ON -DBUILD_EXAMPLES=OFF
        if ($LASTEXITCODE -ne 0) { Pop-Location; Write-Error "cmake 配置失败"; exit 1 }
        ninja -j4
        if ($LASTEXITCODE -ne 0) { Pop-Location; Write-Error "ninja 构建失败"; exit 1 }
        Pop-Location
        Write-Host "  libuxplaylib.dll → $nativeDll" -ForegroundColor Green
    }
}

# ── 2. 构建 .NET 客户端 ──
Write-Host "`n> dotnet build -c $Config" -ForegroundColor Yellow
& $dotnet build -c $Config
if ($LASTEXITCODE -ne 0) { Write-Error "dotnet build 失败"; exit 1 }

# ── 3. 发布 ──
if ($Publish -or $Installer) {
    Write-Host "`n> dotnet publish -r $Runtime -c $Config --self-contained" -ForegroundColor Yellow
    & $dotnet publish -r $Runtime -c $Config --self-contained
    if ($LASTEXITCODE -ne 0) { Write-Error "dotnet publish 失败"; exit 1 }
    $pubDir = "$Root\bin\$Config\net10.0-windows10.0.22621.0\$Runtime\publish"
    $files = (Get-ChildItem $pubDir -Recurse -File).Count
    Write-Host "  Published $files files → $pubDir" -ForegroundColor Green
}

# ── 4. 安装程序 ──
if ($Installer) {
    $iscc = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
    if (-not (Test-Path $iscc)) {
        Write-Warning "Inno Setup 6 未找到（$iscc）。请从 https://jrsoftware.org/issetup.exe 安装。"
    } else {
        Write-Host "`n> 生成安装程序..." -ForegroundColor Yellow
        & $iscc "/DPublishDir=$pubDir" "/DAppVersion=1.0.0" "$Root\installer.iss"
        if ($LASTEXITCODE -ne 0) { Write-Error "安装程序生成失败"; exit 1 }
        Write-Host "  安装程序 → $Root\Output\" -ForegroundColor Green
    }
}

Write-Host "`n=== 完成 ===" -ForegroundColor Cyan
