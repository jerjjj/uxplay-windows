# UxPlay Windows

基于 [libuxplay](https://github.com/jerjjj/libuxplay) 的 Windows AirPlay 投屏接收端，使用 WinUI 3 图形界面。

支持从 iPhone / iPad / Mac 通过 AirPlay 将屏幕镜像或音频串流到 Windows 电脑。

## 功能

- AirPlay 屏幕镜像（H.264 / H.265）
- AirPlay 音频串流（ALAC / AAC）
- 可视化配置所有 UxPlay 参数（40+ 项，含中文说明）
- 实时显示连接状态、设备信息、PIN 码
- 运行日志查看
- 设置保存与热重载（修改后重启投屏即生效）
- 视频窗口关闭后自动重启服务

## 系统要求

- Windows 10 (1809+) 或 Windows 11
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [MSYS2](https://www.msys2.org/)（提供 MinGW-w64 编译工具链）
- [Bonjour SDK](https://developer.apple.com/bonjour/)（或 iTunes 中自带的 Bonjour 服务）

## 编译

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

如果已克隆但缺少 submodule：

```bash
git submodule update --init
```

### 3. 编译 libuxplay（原生库）

在 MSYS2 UCRT64 终端中执行：

```bash
mkdir -p libuxplay/build && cd libuxplay/build
cmake .. -G Ninja -DCMAKE_BUILD_TYPE=Release -DBUILD_SHARED_LIBS=ON -DBUILD_EXAMPLES=OFF
ninja -j4
cd ../..
```

编译产物：`libuxplay/build/libuxplaylib.dll`

### 4. 编译 UxPlayClient（GUI 客户端）

在任意终端中执行：

```bash
dotnet build -c Release
```

编译产物位于 `bin/x64/Release/net10.0-windows10.0.22621.0/`。

### 5. 一键构建（可选）

```powershell
pwsh build.ps1
```

该脚本会自动完成原生库编译和 .NET 构建。加 `-Publish` 参数可生成自包含发布包，加 `-Installer` 参数可生成安装程序（需 [Inno Setup 6](https://jrsoftware.org/issetup.exe)）。

## 运行

1. 确保 Bonjour 服务正在运行（安装 iTunes 或 Bonjour Print Services 即可）
2. 运行 `UxPlayClient.exe`
3. 点击「启用投屏」
4. 在 iPhone / iPad / Mac 的控制中心选择「屏幕镜像」，找到服务器名称并连接

## 设置说明

所有设置项均附有来自 UxPlay 原始文档的描述和对应的命令行参数名。修改设置后点击「保存设置」，然后点击「重启投屏」即可生效。

设置文件保存在 `%LOCALAPPDATA%\UxPlayClient\appsettings.json`。

## 致谢

- [UxPlay](https://github.com/FDH2/UxPlay) - 开源 AirPlay 镜像服务器
- [libuxplay](https://github.com/jerjjj/libuxplay) - UxPlay 库封装
- [GStreamer](https://gstreamer.freedesktop.org/) - 多媒体框架
- [Windows App SDK (WinUI 3)](https://github.com/microsoft/WindowsAppSDK) - 现代 Windows UI 框架

## 许可

GPL-3.0，与上游 UxPlay 保持一致。
