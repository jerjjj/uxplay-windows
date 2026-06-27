# UxPlay Windows

基于 [libuxplay](https://github.com/jerjjj/libuxplay) 的 Windows AirPlay 投屏接收端，使用 WinUI 3 图形界面。

支持从 iPhone / iPad / Mac 通过 AirPlay 将屏幕镜像或音频串流到 Windows 电脑。

<p align="center">
  <img src="Assets/AppIcon.svg" width="128" alt="UxPlay Client Icon">
</p>

## 功能

- AirPlay 屏幕镜像（H.264 / H.265）
- AirPlay 音频串流（ALAC / AAC）
- 可视化配置所有 UxPlay 参数（40+ 项，含中文说明）
- 实时显示连接状态、设备信息、PIN 码
- 运行日志查看
- 设置保存（重启投屏或重启应用后生效）
- 视频窗口关闭后自动重启服务
- 中英文界面切换
- 浅色/深色主题（需重启应用生效）

## 系统要求

- Windows 10 (19041+) 或 Windows 11
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [MSYS2](https://www.msys2.org/) + [Bonjour SDK](https://developer.apple.com/bonjour/)（仅构建时需要，运行时已全部打包在内）
- 运行投屏功能需要 Bonjour 服务（安装 iTunes 或 [Bonjour Print Services](https://support.apple.com/bonjour)）

## 快速开始

### 1. 安装依赖

在 MSYS2 UCRT64 终端中执行：

```bash
pacman -S mingw-w64-ucrt-x86_64-cmake \
          mingw-w64-ucrt-x86_64-ninja \
          mingw-w64-ucrt-x86_64-gcc \
          mingw-w64-ucrt-x86_64-gstreamer \
          mingw-w64-ucrt-x86_64-gst-plugins-base \
          mingw-w64-ucrt-x86_64-gst-plugins-good \
          mingw-w64-ucrt-x86_64-gst-plugins-bad \
          mingw-w64-ucrt-x86_64-gst-libav \
          mingw-w64-ucrt-x86_64-openssl \
          mingw-w64-ucrt-x86_64-libplist \
          mingw-w64-ucrt-x86_64-glib2 \
          mingw-w64-ucrt-x86_64-pkg-config
```

确保已安装 [Bonjour SDK](https://developer.apple.com/bonjour/)（默认路径 `C:\Program Files\Bonjour SDK`）。

### 2. 克隆仓库

```bash
git clone --recursive https://github.com/jerjjj/uxplay-windows.git
cd uxplay-windows
```

> 如已克隆但缺少 submodule：`git submodule update --init`

### 3. 配置敏感信息

首次构建时，`build.ps1` 会自动从 `.sample` 模板复制配置文件：

```powershell
# 编辑 local.props，填入你的证书指纹（本地开发用）
notepad local.props

# 编辑 Package.appxmanifest，填入你的应用标识
notepad Package.appxmanifest
```

> 这两个文件已加入 `.gitignore`，不会被提交到 GitHub。

### 4. 一键构建

构建脚本会自动打包运行所需的全部 MSYS2 依赖（15 个运行时 DLL + 239 个 GStreamer 插件），用户无需手动安装。

```powershell
# Unpackaged EXE（本地开发/调试）
pwsh build.ps1

# MSIX 侧载包（本地测试安装）
pwsh build.ps1 -Packaged

# 自包含发布 + 安装程序
pwsh build.ps1 -Publish -Installer

# 完整选项
pwsh build.ps1 -Config Release -Runtime win-x64 -SkipNative
```

## 手动编译

### 编译 libuxplay（原生库）

在 MSYS2 UCRT64 终端中：

```bash
mkdir -p libuxplay/build && cd libuxplay/build
cmake .. -G Ninja -DCMAKE_BUILD_TYPE=Release -DBUILD_SHARED_LIBS=ON -DBUILD_EXAMPLES=OFF
ninja -j4
```

产物：`libuxplay/build/libuxplaylib.dll`

### 编译 .NET 客户端

```bash
# Unpackaged
dotnet build -c Release

# MSIX 侧载
dotnet build -c Release -p:Packaged=true
```

## 运行

1. 确保 **Bonjour 服务** 正在运行（安装 iTunes 或 [Bonjour Print Services](https://support.apple.com/bonjour)）
2. 运行 `UxPlayClient.exe`
3. 点击「启用投屏」
4. 在 iPhone / iPad / Mac 控制中心选择「屏幕镜像」，找到服务器名称并连接

## 项目结构

```
UxPlayClient/
├── Assets/              ← 图标资产（SVG, ICO, PNG，静态文件）
├── Interop/             ← C# ↔ C++ P/Invoke 互操作
├── Models/              ← 数据模型（AppSettings）
├── Pages/               ← UI 页面（MainPage, SettingsPage, LogPage）
├── Services/            ← 服务层（UxPlayService, L10n 本地化）
├── ViewModels/          ← MVVM ViewModels
├── libuxplay/           ← 原生 AirPlay 协议栈（git submodule）
├── App.xaml             ← WinUI 应用入口
├── MainWindow.cs        ← 主窗口
├── UI.cs                ← UI 工厂（画刷、控件创建）
├── UxPlayClient.csproj  ← 项目文件
├── Package.appxmanifest ← MSIX 清单（gitignored，.sample 为模板）
├── local.props          ← 本地敏感配置（gitignored，.sample 为模板）
├── build.ps1            ← 一键构建脚本
└── README.md
```

## 构建选项

| 参数 | 说明 |
|------|------|
| `-Config` | Debug / Release（默认 Release） |
| `-Runtime` | win-x64 / win-x86 / win-arm64 |
| `-SkipNative` | 跳过 libuxplay 编译 |
| `-Packaged` | 生成 MSIX 侧载包（自签名） |
| `-Publish` | 生成自包含发布目录 |
| `-Installer` | 生成 Inno Setup 安装程序 |

## 设置说明

所有设置项附有 UxPlay 原始文档描述和命令行参数名。设置保存在 `%LOCALAPPDATA%\UxPlayClient\appsettings.json`。

**主题切换**：WinUI 3 限制 `Application.RequestedTheme` 只能在启动时设置。选择新主题后应用会弹窗提示重启，重启后完整生效。

**语言切换**：中英文即时切换，无需重启。

## 致谢

- [UxPlay](https://github.com/FDH2/UxPlay) - 开源 AirPlay 镜像服务器
- [libuxplay](https://github.com/jerjjj/libuxplay) - UxPlay 库封装
- [GStreamer](https://gstreamer.freedesktop.org/) - 多媒体框架
- [Windows App SDK (WinUI 3)](https://github.com/microsoft/WindowsAppSDK) - 现代 Windows UI 框架
- [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) - MVVM 工具包

## 许可

GPL-3.0，与上游 UxPlay 保持一致。
