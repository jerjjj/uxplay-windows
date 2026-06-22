using System;
using System.Collections.Generic;
using UxPlayClient.Models;

namespace UxPlayClient.Services;

public enum AppTheme { System = 0, Light = 1, Dark = 2 }

public static class L10n
{
    public static event Action? LanguageChanged;
    public static event Action<AppTheme>? ThemeChanged;

    static readonly Dictionary<string, string> _en = new()
    {
        ["app.title"] = "UxPlay Client",
        ["app.version"] = "v1.1.0",
        ["nav.home"] = "Home",
        ["nav.settings"] = "Settings",
        ["nav.log"] = "Log",
        ["heading.home"] = "Home",
        ["heading.settings"] = "Settings",
        ["heading.log"] = "Log",

        ["status.idle"] = "Idle",
        ["status.starting"] = "Starting...",
        ["status.running"] = "Running",
        ["status.stopping"] = "Stopping...",
        ["status.error"] = "Error",
        ["status.unknown"] = "Unknown",
        ["status.connections"] = "Connections",
        ["status.title"] = "Server Status",

        ["ctrl.title"] = "Casting Control",
        ["ctrl.start"] = "Start Casting",
        ["ctrl.stop"] = "Stop Casting",
        ["ctrl.restart"] = "Restart Casting",
        ["ctrl.disconnect"] = "Disconnect",

        ["pin.title"] = "Access PIN",
        ["pin.hint"] = "Enter this PIN on your iOS / macOS device",

        ["audio.title"] = "Now Playing",

        ["devices.title"] = "Connected Devices",
        ["devices.none"] = "No devices connected",

        ["error.title"] = "Error",

        ["log.clear"] = "Clear Log",

        ["settings.title"] = "Settings",
        ["settings.saved"] = "Settings saved. Restart casting to apply.",
        ["settings.save_failed"] = "Save failed",
        ["settings.defaults"] = "Reset to Defaults",
        ["settings.save"] = "Save Settings",
        ["settings.section.identity"] = "Identity",
        ["settings.section.display"] = "Display",
        ["settings.section.video"] = "Video Rendering",
        ["settings.section.audio"] = "Audio Rendering",
        ["settings.section.network"] = "Network Ports",
        ["settings.section.security"] = "Security",
        ["settings.section.misc"] = "Other Options",

        ["label.server_name"] = "Server Name",
        ["label.mac_address"] = "MAC Address",
        ["label.append_hostname"] = "Append Hostname",
        ["label.width"] = "Width",
        ["label.height"] = "Height",
        ["label.refresh_rate"] = "Refresh Hz",
        ["label.max_fps"] = "Max FPS",
        ["label.overscan"] = "Overscanned",
        ["label.use_video"] = "Enable Video",
        ["label.video_sink"] = "Video Sink",
        ["label.video_decoder"] = "Video Decoder",
        ["label.video_converter"] = "Video Converter",
        ["label.video_parser"] = "Video Parser",
        ["label.video_flip"] = "Video Flip",
        ["label.fullscreen"] = "Fullscreen",
        ["label.h265"] = "H.265 Support",
        ["label.video_sync"] = "Video Sync",
        ["label.bt709"] = "BT.709 Fix",
        ["label.nofreeze"] = "No Freeze",
        ["label.use_audio"] = "Enable Audio",
        ["label.audio_sink"] = "Audio Sink",
        ["label.audio_sync"] = "Audio Sync",
        ["label.taper"] = "Taper Volume",
        ["label.db_low"] = "dB Low",
        ["label.db_high"] = "dB High",
        ["label.tcp_port"] = "TCP Port",
        ["label.access_control"] = "Access Control",
        ["label.password"] = "Password",
        ["label.keyfile"] = "Key File",
        ["label.reg_list"] = "Registration List",
        ["label.restrict"] = "Restrict Clients",
        ["label.log_level"] = "Log Level",
        ["label.nohold"] = "Allow Kick",
        ["label.hls"] = "HLS Support",
        ["label.coverart"] = "Show Cover Art",
        ["label.coverart_file"] = "Cover Art File",
        ["label.lang_pref"] = "Language",
        ["label.show_fps"] = "Show FPS",
        ["label.new_window"] = "New Window Close",
        ["label.language"] = "Language",
        ["label.theme"] = "Theme",

        ["desc.server_name"] = "AirPlay service name visible on the network (-n)",
        ["desc.mac_address"] = "MAC / DeviceID in AA:BB:CC:DD:EE:FF format (-m). Leave blank to auto-detect.",
        ["desc.append_hostname"] = "Append @hostname to the server name (-nh to disable)",
        ["desc.width"] = "Requested video width from client (-s WxH W part)",
        ["desc.height"] = "Requested video height from client (-s WxH H part)",
        ["desc.refresh_rate"] = "Display refresh rate, default 60 (-s WxH@r r part)",
        ["desc.max_fps"] = "Max streaming FPS, default 30 (-fps)",
        ["desc.overscan"] = "Set display to overscanned mode (usually not needed) (-o)",
        ["desc.use_video"] = "Disable to stream audio only, no video window (-vs 0)",
        ["desc.video_sink"] = "GStreamer video output plugin (-vs)",
        ["desc.video_decoder"] = "GStreamer H.264 decoder. decodebin = auto-select (-vd)",
        ["desc.video_converter"] = "GStreamer video format converter (-vc)",
        ["desc.video_parser"] = "GStreamer H.264 parser (-vp)",
        ["desc.video_flip"] = "Flip or rotate the video output (-r L/R, -f H/V/I)",
        ["desc.fullscreen"] = "Fullscreen display. On Windows, auto-selects d3d11videosink (-fs)",
        ["desc.h265"] = "Enable 4K H.265 video. Default resolution becomes 3840x2160 (-h265)",
        ["desc.video_sync"] = "Sync A/V using timestamps in mirror mode (-vsync no to disable)",
        ["desc.bt709"] = "May be required for Raspberry Pi with Video4Linux2 (-bt709)",
        ["desc.nofreeze"] = "Do not keep frozen frame after disconnect (-nofreeze)",
        ["desc.use_audio"] = "Disable to stream video only, no audio (-as 0)",
        ["desc.audio_sink"] = "GStreamer audio output plugin (-as)",
        ["desc.audio_sync"] = "Sync audio to client video in audio-only mode (~2s latency) (-async)",
        ["desc.taper"] = "Use a tapered AirPlay volume control profile (-taper)",
        ["desc.db_low"] = "Minimum volume attenuation in dB, default -30.0 (-db low)",
        ["desc.db_high"] = "Maximum volume gain in dB, default 0.0 (-db high)",
        ["desc.tcp_port"] = "Fixed TCP port (0 = auto). Required for firewall setups (-p)",
        ["desc.access_control"] = "None / AppleTV-style PIN (-pin) / Fixed password (-pw)",
        ["desc.password"] = "All clients must enter this password (-pw). Blank = no password",
        ["desc.keyfile"] = "Persistent public key storage path (-key). Blank = default",
        ["desc.reg_list"] = "Maintain a register to verify returning pin-registered clients (-reg)",
        ["desc.restrict"] = "Only allow connections from devices specified with -allow (-restrict)",
        ["desc.log_level"] = "Debug level shows full protocol communication details (-d)",
        ["desc.nohold"] = "Drop the current connection when a new client connects (-nohold)",
        ["desc.hls"] = "Enable HTTP Live Streaming (YouTube app video only) (-hls)",
        ["desc.coverart"] = "Render Apple Music cover art inside UxPlay in audio mode (-ca)",
        ["desc.coverart_file"] = "Export cover art to a file for external viewer (-ca filename)",
        ["desc.lang_pref"] = "HLS video language priority. Overrides $LANGUAGE (-lang)",
        ["desc.show_fps"] = "Show video streaming performance reports in logs (-FPSdata)",
        ["desc.new_window"] = "Close video window when client stops mirroring (-nc to cancel)",

        ["lang.zh-CN"] = "Chinese (Simplified)",
        ["lang.en"] = "English",
        ["theme.system"] = "Follow System",
        ["theme.light"] = "Light",
        ["theme.dark"] = "Dark",

        // Log messages
        ["log.server_started"] = "UxPlay server started",
        ["log.server_stopped"] = "UxPlay server stopped",
        ["log.auto_restart"] = "Video window closed. Auto-restarting...",
        ["log.auto_restart_ok"] = "Auto-restart complete. Waiting for new connections.",
        ["log.auto_restart_fail"] = "Auto-restart failed",
        ["log.restart"] = "Restarting casting...",
        ["log.restart_ok"] = "Casting restarted. New settings applied.",
        ["log.device_connected"] = "Device connected",
        ["log.device_disconnected"] = "Device disconnected",
        ["log.mirror_started"] = "Screen mirroring started",
        ["log.mirror_stopped"] = "Screen mirroring stopped",
        ["log.pin"] = "PIN code",
        ["log.disconnected_all"] = "All clients disconnected",
        ["log.start_failed"] = "Start failed",
        ["log.stop_failed"] = "Stop failed",
        ["log.restart_failed"] = "Restart failed",
        ["log.dll_not_found"] = "libuxplaylib.dll not found! Please build libuxplay first.",

        // Placeholders
        ["placeholder.mac"] = "Leave blank to auto-detect",
        ["placeholder.password"] = "Leave blank for no password",
        ["placeholder.keyfile"] = "Leave blank for default",
        ["placeholder.lang"] = "e.g. fr:es:en",
        ["placeholder.coverart"] = "e.g. cover.jpg",

        // Flip / Access / Log options
        ["flip.none"] = "None",
        ["flip.left"] = "Rotate Left 90°",
        ["flip.right"] = "Rotate Right 90°",
        ["flip.invert"] = "Rotate 180°",
        ["flip.vflip"] = "Flip Vertical",
        ["flip.hflip"] = "Flip Horizontal",

        ["access.free"] = "No Auth",
        ["access.pin"] = "PIN Code",
        ["access.password"] = "Password",

        ["log.level.error"] = "Error",
        ["log.level.warning"] = "Warning",
        ["log.level.info"] = "Info",
        ["log.level.debug"] = "Debug",
        ["log.level.verbose"] = "Verbose",

        ["theme.restart_title"] = "Restart Required",
        ["theme.restart_msg"] = "Theme changes require an app restart to take full effect. Restart now?",
        ["theme.restart_now"] = "Restart Now",
        ["theme.restart_later"] = "Later",

        ["msg.start_failed"] = "Start failed",
        ["msg.stop_failed"] = "Stop failed",
        ["msg.restart_failed"] = "Restart failed",
        ["msg.settings_reset"] = "Restored default settings",
    };

    static readonly Dictionary<string, string> _zh = new()
    {
        ["app.title"] = "UxPlay Client",
        ["app.version"] = "v1.1.0",
        ["nav.home"] = "主页",
        ["nav.settings"] = "设置",
        ["nav.log"] = "日志",
        ["heading.home"] = "主页",
        ["heading.settings"] = "设置",
        ["heading.log"] = "日志",

        ["status.idle"] = "空闲",
        ["status.starting"] = "启动中...",
        ["status.running"] = "运行中",
        ["status.stopping"] = "停止中...",
        ["status.error"] = "错误",
        ["status.unknown"] = "未知",
        ["status.connections"] = "连接数",
        ["status.title"] = "服务器状态",

        ["ctrl.title"] = "投屏控制",
        ["ctrl.start"] = "启用投屏",
        ["ctrl.stop"] = "关闭投屏",
        ["ctrl.restart"] = "重启投屏",
        ["ctrl.disconnect"] = "断开连接",

        ["pin.title"] = "访问 PIN 码",
        ["pin.hint"] = "请在 iOS / macOS 设备上输入此 PIN 码",

        ["audio.title"] = "正在播放",

        ["devices.title"] = "已连接设备",
        ["devices.none"] = "暂无设备连接",

        ["error.title"] = "错误",

        ["log.clear"] = "清空日志",

        ["settings.title"] = "设置",
        ["settings.saved"] = "设置已保存，重启投屏后生效",
        ["settings.save_failed"] = "保存失败",
        ["settings.defaults"] = "恢复默认",
        ["settings.save"] = "保存设置",
        ["settings.section.identity"] = "身份标识",
        ["settings.section.display"] = "显示设置",
        ["settings.section.video"] = "视频渲染",
        ["settings.section.audio"] = "音频渲染",
        ["settings.section.network"] = "网络端口",
        ["settings.section.security"] = "安全认证",
        ["settings.section.misc"] = "其他选项",

        ["label.server_name"] = "服务器名称",
        ["label.mac_address"] = "MAC 地址",
        ["label.append_hostname"] = "追加主机名",
        ["label.width"] = "分辨率宽度",
        ["label.height"] = "分辨率高度",
        ["label.refresh_rate"] = "刷新率 Hz",
        ["label.max_fps"] = "最大 FPS",
        ["label.overscan"] = "过扫描",
        ["label.use_video"] = "启用视频",
        ["label.video_sink"] = "视频 Sink",
        ["label.video_decoder"] = "视频解码器",
        ["label.video_converter"] = "视频转换器",
        ["label.video_parser"] = "视频解析器",
        ["label.video_flip"] = "视频翻转",
        ["label.fullscreen"] = "全屏",
        ["label.h265"] = "H.265 支持",
        ["label.video_sync"] = "视频同步",
        ["label.bt709"] = "BT.709 修复",
        ["label.nofreeze"] = "断开不冻结",
        ["label.use_audio"] = "启用音频",
        ["label.audio_sink"] = "音频 Sink",
        ["label.audio_sync"] = "音频同步",
        ["label.taper"] = "锥形音量",
        ["label.db_low"] = "低 dB 限",
        ["label.db_high"] = "高 dB 限",
        ["label.tcp_port"] = "TCP 端口",
        ["label.access_control"] = "访问控制",
        ["label.password"] = "密码",
        ["label.keyfile"] = "密钥文件",
        ["label.reg_list"] = "注册列表",
        ["label.restrict"] = "限制客户端",
        ["label.log_level"] = "日志级别",
        ["label.nohold"] = "允许踢人",
        ["label.hls"] = "HLS 支持",
        ["label.coverart"] = "显示封面",
        ["label.coverart_file"] = "封面文件名",
        ["label.lang_pref"] = "语言偏好",
        ["label.show_fps"] = "显示 FPS",
        ["label.new_window"] = "新窗口关闭",
        ["label.language"] = "界面语言",
        ["label.theme"] = "主题",

        ["desc.server_name"] = "AirPlay 服务在网络上显示的名称（-n）",
        ["desc.mac_address"] = "指定网卡 MAC 地址 / DeviceID，格式 AA:BB:CC:DD:EE:FF（-m）。留空自动检测",
        ["desc.append_hostname"] = "在服务器名称后追加 @hostname（关闭则为 -nh）",
        ["desc.width"] = "请求客户端的视频宽度（-s WxH 的 W 部分）",
        ["desc.height"] = "请求客户端的视频高度（-s WxH 的 H 部分）",
        ["desc.refresh_rate"] = "显示刷新率，默认 60（-s WxH@r 的 r 部分）",
        ["desc.max_fps"] = "限制客户端最大流媒体帧率，默认 30（-fps）",
        ["desc.overscan"] = "设置显示过扫描模式（通常不需要）（-o）",
        ["desc.use_video"] = "关闭则只有音频，无视频窗口（-vs 0）",
        ["desc.video_sink"] = "GStreamer 视频输出插件（-vs）",
        ["desc.video_decoder"] = "GStreamer H.264 解码器。decodebin 自动选择（-vd）",
        ["desc.video_converter"] = "GStreamer 视频格式转换器（-vc）",
        ["desc.video_parser"] = "GStreamer H.264 解析器（-vp）",
        ["desc.video_flip"] = "画面翻转或旋转（-r L/R，-f H/V/I）",
        ["desc.fullscreen"] = "全屏显示。Windows 下自动使用 d3d11videosink（-fs）",
        ["desc.h265"] = "启用 4K H.265 视频，分辨率默认变为 3840x2160（-h265）",
        ["desc.video_sync"] = "镜像模式下用时间戳同步音视频（关闭 = -vsync no）",
        ["desc.bt709"] = "树莓派使用 Video4Linux2 时可能需要此修复（-bt709）",
        ["desc.nofreeze"] = "网络断开后不保留冻结画面（-nofreeze）",
        ["desc.use_audio"] = "关闭则只有视频，无音频（-as 0）",
        ["desc.audio_sink"] = "GStreamer 音频输出插件（-as）",
        ["desc.audio_sync"] = "纯音频模式下同步音频与客户端视频（约 2 秒延迟）（-async）",
        ["desc.taper"] = "使用渐变式 AirPlay 音量控制曲线（-taper）",
        ["desc.db_low"] = "最小音量衰减（分贝），默认 -30.0（-db low:high 的 low）",
        ["desc.db_high"] = "最大音量增益（分贝），默认 0.0（-db low:high 的 high）",
        ["desc.tcp_port"] = "固定 TCP 端口（0 = 自动分配），防火墙场景需要（-p）",
        ["desc.access_control"] = "无认证 / AppleTV 风格 PIN 码（-pin）/ 固定密码（-pw）",
        ["desc.password"] = "设置固定密码后所有客户端必须输入相同密码（-pw password）。留空则无密码",
        ["desc.keyfile"] = "持久公钥存储路径（-key）。留空使用默认",
        ["desc.reg_list"] = "维护 PIN 已认证客户端列表，已注册客户端下次连接免 PIN（-reg）",
        ["desc.restrict"] = "仅允许通过 -allow 指定的设备连接（-restrict）",
        ["desc.log_level"] = "调试级别显示完整协议通信细节（-d）",
        ["desc.nohold"] = "新客户端连接时自动断开当前客户端（-nohold）",
        ["desc.hls"] = "启用 HTTP Live Streaming，目前仅支持 YouTube 视频（-hls）",
        ["desc.coverart"] = "音频模式下在 UxPlay 内渲染 Apple Music 封面（-ca）",
        ["desc.coverart_file"] = "将封面导出到指定文件以供外部查看器显示（-ca filename）",
        ["desc.lang_pref"] = "HLS 视频语言优先级。覆盖 $LANGUAGE 环境变量（-lang）",
        ["desc.show_fps"] = "在日志中显示客户端发送的视频流性能报告（-FPSdata）",
        ["desc.new_window"] = "客户端停止镜像时关闭视频窗口（-nc 取消此行为）",

        ["lang.zh-CN"] = "简体中文",
        ["lang.en"] = "English",
        ["theme.system"] = "跟随系统",
        ["theme.light"] = "浅色",
        ["theme.dark"] = "深色",

        ["log.server_started"] = "UxPlay 服务器已启动",
        ["log.server_stopped"] = "UxPlay 服务器已停止",
        ["log.auto_restart"] = "投屏窗口已关闭，自动重启中…",
        ["log.auto_restart_ok"] = "自动重启完成，等待新连接",
        ["log.auto_restart_fail"] = "自动重启失败",
        ["log.restart"] = "正在重启投屏…",
        ["log.restart_ok"] = "投屏已重启，新设置已生效",
        ["log.device_connected"] = "设备已连接",
        ["log.device_disconnected"] = "设备已断开",
        ["log.mirror_started"] = "屏幕镜像已开始",
        ["log.mirror_stopped"] = "屏幕镜像已停止",
        ["log.pin"] = "PIN 码",
        ["log.disconnected_all"] = "已断开所有客户端",
        ["log.start_failed"] = "启动失败",
        ["log.stop_failed"] = "停止失败",
        ["log.restart_failed"] = "重启失败",
        ["log.dll_not_found"] = "libuxplaylib.dll 未找到！请先构建 libuxplay。",

        // Placeholders
        ["placeholder.mac"] = "留空自动检测",
        ["placeholder.password"] = "留空则无密码",
        ["placeholder.keyfile"] = "留空使用默认",
        ["placeholder.lang"] = "如 fr:es:en",
        ["placeholder.coverart"] = "如 cover.jpg",

        // Flip / Access / Log options
        ["flip.none"] = "无",
        ["flip.left"] = "左旋90°",
        ["flip.right"] = "右旋90°",
        ["flip.invert"] = "旋转180°",
        ["flip.vflip"] = "垂直翻转",
        ["flip.hflip"] = "水平翻转",

        ["access.free"] = "无认证",
        ["access.pin"] = "PIN 码",
        ["access.password"] = "密码",

        ["log.level.error"] = "错误",
        ["log.level.warning"] = "警告",
        ["log.level.info"] = "信息",
        ["log.level.debug"] = "调试",
        ["log.level.verbose"] = "详细",

        ["theme.restart_title"] = "需要重启",
        ["theme.restart_msg"] = "主题变更需要重启应用才能完全生效。是否立即重启？",
        ["theme.restart_now"] = "立即重启",
        ["theme.restart_later"] = "稍后",

        ["msg.start_failed"] = "启动失败",
        ["msg.stop_failed"] = "停止失败",
        ["msg.restart_failed"] = "重启失败",
        ["msg.settings_reset"] = "已恢复默认设置",
    };

    static Dictionary<string, string> _cur = _zh;

    public static string Get(string key, params object[] args)
    {
        if (_cur.TryGetValue(key, out var v))
            return args.Length > 0 ? string.Format(v, args) : v;
        return key;
    }

    public static string Lang => (L is "zh-CN") ? "zh-CN" : "en";
    static string L { get; set; } = "zh-CN";

    public static void SetLanguage(string lang)
    {
        L = lang;
        _cur = (lang == "zh-CN") ? _zh : _en;
        var s = AppSettings.Load();
        s.Language = lang;
        s.Save();
        LanguageChanged?.Invoke();
    }

    public static void LoadSavedLanguage()
    {
        var s = AppSettings.Load();
        if (!string.IsNullOrEmpty(s.Language))
        {
            _cur = (s.Language == "zh-CN") ? _zh : _en;
            L = s.Language;
        }
    }

    public static AppTheme Theme
    {
        get
        {
            var s = AppSettings.Load();
            return s.Theme;
        }
    }

    public static void SetTheme(AppTheme theme)
    {
        var s = AppSettings.Load();
        s.Theme = theme;
        s.Save();
        ThemeChanged?.Invoke(theme);
    }
}
