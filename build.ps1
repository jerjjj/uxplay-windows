#!/usr/bin/env pwsh
<#
.SYNOPSIS
  UxPlay Client 统一构建脚本

.PARAMETER Config
  构建配置 (Debug/Release)，默认 Release

.PARAMETER Runtime
  目标运行时 (win-x64/win-x86/win-arm64)，默认 win-x64

.PARAMETER SkipNative
  跳过 libuxplay 原生库编译

.PARAMETER Publish
  生成自包含发布包（仅 Unpackaged 模式）

.PARAMETER Installer
  生成 Inno Setup 安装程序（需安装 Inno Setup 6）

.PARAMETER Packaged
  生成 MSIX 侧载包（自签名，用于本地测试）

.PARAMETER Store
  生成微软商店上传包（无签名）

.EXAMPLE
  pwsh build.ps1                           # 构建 Unpackaged EXE
  pwsh build.ps1 -Packaged                 # 构建 MSIX 侧载包
  pwsh build.ps1 -Store                    # 构建微软商店包
  pwsh build.ps1 -Publish -Installer       # 发布 + 安装程序
#>
param(
    [ValidateSet("Debug","Release")][string]$Config    = "Release",
    [ValidateSet("win-x64","win-x86","win-arm64")][string]$Runtime = "win-x64",
    [switch]$SkipNative,
    [switch]$Publish,
    [switch]$Installer,
    [switch]$Packaged,
    [switch]$Store
)
$ErrorActionPreference = "Stop"
$Root = $PSScriptRoot
$Platform = ($Runtime -replace '^win-','')

# ── 模式设置 ──
$ModeStr = if ($Store) { "MSIX Store" } elseif ($Packaged) { "MSIX" } else { "Unpackaged" }

# ── 查找 dotnet ──
$dotnet = $null
foreach ($p in @("$env:LOCALAPPDATA\Microsoft\dotnet\dotnet.exe", "$env:ProgramFiles\dotnet\dotnet.exe", "dotnet")) {
    try { $v = & $p --version 2>$null; if ($v -match '^\d') { $dotnet = $p; break } } catch {}
}
if (-not $dotnet) { Write-Error "未找到 .NET SDK。请安装 .NET 10: https://dot.net"; exit 1 }

$ModeStr = if ($Store) { "MSIX Store" } elseif ($Packaged) { "MSIX Sideload" } else { "Unpackaged" }
Write-Host "=== UxPlay Client Build ($ModeStr) ===" -ForegroundColor Cyan
Write-Host "  dotnet  : $dotnet ($(& $dotnet --version))"
Write-Host "  Config  : $Config"
Write-Host "  Runtime : $Runtime"
Write-Host "  Mode    : $ModeStr"

# ════════════════════════════════════════════
# 0. 准备配置文件
# ════════════════════════════════════════════
Write-Host "`n--- 准备配置 ---" -ForegroundColor DarkGray
$samples = @(
    @{Sample="local.props.sample";            Target="local.props"},
    @{Sample="Package.appxmanifest.sample";   Target="Package.appxmanifest"}
)
foreach ($s in $samples) {
    $target = "$Root\$($s.Target)"
    if (-not (Test-Path $target)) {
        $sample = "$Root\$($s.Sample)"
        if (Test-Path $sample) {
            Copy-Item $sample $target -ErrorAction SilentlyContinue
            Write-Host "  [+] $($s.Target) (from .sample — edit with your values)" -ForegroundColor Yellow
        }
    } else {
        Write-Host "  [✓] $($s.Target)" -ForegroundColor DarkGray
    }
}

# ════════════════════════════════════════════
# 1. 构建 libuxplay（原生库）
# ════════════════════════════════════════════
if (-not $SkipNative) {
    Write-Host "`n--- 原生库 ---" -ForegroundColor DarkGray
    $buildDir = "$Root\libuxplay\build"
    New-Item -ItemType Directory -Force -Path $buildDir | Out-Null
    Push-Location $buildDir
    
    # Always re-run cmake (picks up new/removed sources in submodule updates)
    cmake .. -G Ninja -DCMAKE_BUILD_TYPE=Release -DBUILD_SHARED_LIBS=ON -DBUILD_EXAMPLES=OFF
    if ($LASTEXITCODE -ne 0) { Pop-Location; Write-Error "cmake 配置失败"; exit 1 }
    
    # ninja 是增量构建，无变更时近乎零开销
    ninja -j4
    if ($LASTEXITCODE -ne 0) { Pop-Location; Write-Error "ninja 构建失败"; exit 1 }
    Pop-Location
    
    # Copy to Output/ for F5 debugging
    $outDll = "$Root\Output\libuxplaylib.dll"
    Copy-Item -Force "$buildDir\libuxplaylib.dll" $outDll
    Write-Host "  [+] libuxplaylib.dll -> Output/" -ForegroundColor Green
}

# ════════════════════════════════════════════
# 2. 构建 .NET 客户端
# ════════════════════════════════════════════
Write-Host "`n--- .NET 构建 ---" -ForegroundColor DarkGray
$buildArgs = @("-c", $Config, "-p:Platform=$Platform")
if ($Packaged) { $buildArgs += "-p:Packaged=true" }
if ($Store)    { $buildArgs += "-p:Store=true"; $buildArgs += "-p:GenerateAppxPackageOnBuild=true" }

Write-Host "  dotnet build $($buildArgs -join ' ')" -ForegroundColor DarkGray
& $dotnet build @buildArgs
if ($LASTEXITCODE -ne 0) { Write-Error "dotnet build 失败"; exit 1 }

# ════════════════════════════════════════════
# 3. 输出结果
# ════════════════════════════════════════════
$outBase = "$Root\bin\$Platform\$Config\net10.0-windows10.0.22621.0"

if ($Packaged -or $Store) {
    $msixDir = Get-ChildItem "$outBase\AppPackages" -Directory -Filter "*_Test" | Select-Object -First 1
    if ($msixDir) {
        $msix = Get-ChildItem $msixDir.FullName -Filter "*.msix" | Select-Object -First 1
        Write-Host "`n=== MSIX 包 ===" -ForegroundColor Green
        Write-Host "  $($msix.FullName)" -ForegroundColor White
    }
} else {
    $exe = "$outBase\UxPlayClient.exe"
    if (Test-Path $exe) {
        $dllCount = (Get-ChildItem "$outBase\*.dll" -ErrorAction SilentlyContinue).Count
        $gstCount = (Get-ChildItem "$outBase\gstreamer-1.0\*.dll" -ErrorAction SilentlyContinue).Count
        Write-Host "`n=== 输出 ===" -ForegroundColor Green
        Write-Host "  $exe (+ $dllCount runtime DLLs, $gstCount GStreamer plugins)" -ForegroundColor White
    }
}

# ════════════════════════════════════════════
# 4. 发布 (仅 Unpackaged)
# ════════════════════════════════════════════
if ($Publish) {
    Write-Host "`n--- 发布 ---" -ForegroundColor DarkGray
    $pubArgs = @("-r", $Runtime, "-c", $Config, "-p:Platform=$Platform", "--self-contained")
    Write-Host "  dotnet publish $($pubArgs -join ' ')" -ForegroundColor DarkGray
    & $dotnet publish @pubArgs
    if ($LASTEXITCODE -ne 0) { Write-Error "dotnet publish 失败"; exit 1 }
    $pubDir = "$outBase\$Runtime\publish"
    $files = (Get-ChildItem $pubDir -Recurse -File).Count
    Write-Host "  [+] $files files -> $pubDir" -ForegroundColor Green
}

# ════════════════════════════════════════════
# 5. 安装程序 (仅 Unpackaged)
# ════════════════════════════════════════════
if ($Installer) {
    Write-Host "`n--- 安装程序 ---" -ForegroundColor DarkGray
    $iscc = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
    if (-not (Test-Path $iscc)) {
        Write-Warning "Inno Setup 6 未找到。请从 https://jrsoftware.org/issetup.exe 安装"
    } else {
        $pubDir = "$outBase\$Runtime\publish"
        if (-not (Test-Path $pubDir)) {
            Write-Warning "请先运行 -Publish 生成发布包"
        } else {
            & $iscc "/DPublishDir=$pubDir" "/DAppVersion=1.2.0" "$Root\installer.iss"
            if ($LASTEXITCODE -ne 0) { Write-Error "安装程序生成失败"; exit 1 }
            Write-Host "  [+] $Root\Output\UxPlayClient-1.2.0-setup.exe" -ForegroundColor Green
        }
    }
}

Write-Host "`n=== 完成 ===" -ForegroundColor Cyan
