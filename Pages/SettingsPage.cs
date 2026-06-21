using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UxPlayClient.ViewModels;

namespace UxPlayClient.Pages;

public class SettingsPage : Page
{
    private readonly SettingsViewModel _vm;
    private readonly InfoBar _statusBar;

    // ── 预设选项 ──
    static readonly string[] VideoSinks     = ["autovideosink", "d3d11videosink", "d3d12videosink", "glimagesink", "waylandsink", "ximagesink", "xvimagesink", "vaapisink", "kmssink", "fakesink"];
    static readonly string[] VideoDecoders  = ["decodebin", "avdec_h264", "d3d11h264dec", "v4l2h264dec", "vaapih264dec", "nvh264dec"];
    static readonly string[] VideoConverters= ["videoconvert", "v4l2convert", "d3d11convert"];
    static readonly string[] VideoParsers   = ["h264parse"];
    static readonly string[] AudioSinks     = ["autoaudiosink", "wasapisink", "directsoundsink", "pulsesink", "pipewiresink", "alsasink", "jackaudiosink"];

    public SettingsPage(SettingsViewModel vm)
    {
        _vm = vm;
        _statusBar = UI.MakeInfoBar(InfoBarSeverity.Success);
        _statusBar.Title = "设置";

        // ── 身份标识 ──
        var g1 = UI.SettingsGrid(3);
        Txt(g1, 0, "服务器名称", vm.ServerName, v => vm.ServerName = v,
            desc: "AirPlay 服务在网络上显示的名称（-n）");
        Txt(g1, 1, "MAC 地址", vm.MacAddress, v => vm.MacAddress = v,
            placeholder: "留空自动检测",
            desc: "指定网卡 MAC 地址 / DeviceID，格式 AA:BB:CC:DD:EE:FF（-m）");
        Tog(g1, 2, "追加主机名", vm.AppendHostname, v => vm.AppendHostname = v,
            desc: "在服务器名称后追加 @hostname（关闭则为 -nh）");

        // ── 显示 ──
        var g2 = UI.SettingsGrid(5);
        Num(g2, 0, "分辨率宽度", vm.Width, 320, 7680, v => vm.Width = (ushort)v,
            desc: "请求客户端的视频宽度（-s WxH 的 W 部分）");
        Num(g2, 1, "分辨率高度", vm.Height, 240, 4320, v => vm.Height = (ushort)v,
            desc: "请求客户端的视频高度（-s WxH 的 H 部分）");
        Num(g2, 2, "刷新率 Hz", vm.RefreshRate, 24, 240, v => vm.RefreshRate = (ushort)v,
            desc: "显示刷新率，默认 60（-s WxH@r 的 r 部分）");
        Num(g2, 3, "最大 FPS", vm.MaxFps, 1, 120, v => vm.MaxFps = (ushort)v,
            desc: "限制客户端最大流媒体帧率，默认 30（-fps）");
        Tog(g2, 4, "过扫描", vm.Overscanned, v => vm.Overscanned = v,
            desc: "设置显示过扫描模式（通常不需要）（-o）");

        // ── 视频 ──
        var g3 = UI.SettingsGrid(11);
        Tog(g3, 0, "启用视频", vm.UseVideo, v => vm.UseVideo = v,
            desc: "关闭则只有音频，无视频窗口（-vs 0）");
        EditCmb(g3, 1, "视频 Sink", VideoSinks, vm.Videosink, v => vm.Videosink = v,
            desc: "GStreamer 视频输出插件（-vs）");
        EditCmb(g3, 2, "视频解码器", VideoDecoders, vm.VideoDecoder, v => vm.VideoDecoder = v,
            desc: "GStreamer H.264 解码器：decodebin 自动选择（-vd）");
        EditCmb(g3, 3, "视频转换器", VideoConverters, vm.VideoConverter, v => vm.VideoConverter = v,
            desc: "GStreamer 视频格式转换器（-vc）");
        EditCmb(g3, 4, "视频解析器", VideoParsers, vm.VideoParser, v => vm.VideoParser = v,
            desc: "GStreamer H.264 解析器（-vp）");
        Cmb(g3, 5, "视频翻转", vm.VideoFlipOptions, vm.VideoFlipIndex, v => vm.VideoFlipIndex = v,
            desc: "画面翻转或旋转（-r L/R，-f H/V/I）");
        Tog(g3, 6, "全屏", vm.Fullscreen, v => vm.Fullscreen = v,
            desc: "全屏显示（Windows 下自动使用 d3d11videosink）（-fs）");
        Tog(g3, 7, "H.265 支持", vm.H265Support, v => vm.H265Support = v,
            desc: "启用 4K H.265 视频，分辨率默认变为 3840x2160（-h265）");
        Tog(g3, 8, "视频同步", vm.VideoSync, v => vm.VideoSync = v,
            desc: "镜像模式下用时间戳同步音视频（默认开启，关闭 = -vsync no）");
        Tog(g3, 9, "BT.709 修复", vm.Bt709Fix, v => vm.Bt709Fix = v,
            desc: "树莓派使用 Video4Linux2 时可能需要此修复（-bt709）");
        Tog(g3, 10, "断开不冻结", vm.NoFreeze, v => vm.NoFreeze = v,
            desc: "网络断开后不保留冻结画面（-nofreeze）");

        // ── 音频 ──
        var g4 = UI.SettingsGrid(6);
        Tog(g4, 0, "启用音频", vm.UseAudio, v => vm.UseAudio = v,
            desc: "关闭则只有视频，无音频（-as 0）");
        EditCmb(g4, 1, "音频 Sink", AudioSinks, vm.Audiosink, v => vm.Audiosink = v,
            desc: "GStreamer 音频输出插件（-as）");
        Tog(g4, 2, "音频同步", vm.AudioSync, v => vm.AudioSync = v,
            desc: "纯音频模式下同步音频与客户端视频（有约 2 秒延迟）（-async）");
        Tog(g4, 3, "锥形音量", vm.TaperVolume, v => vm.TaperVolume = v,
            desc: "使用渐变式 AirPlay 音量控制曲线（-taper）");
        Num(g4, 4, "低 dB 限", vm.DbLow, -60, 0, v => vm.DbLow = v,
            desc: "最小音量衰减（分贝），默认 -30.0（-db low:high 的 low）");
        Num(g4, 5, "高 dB 限", vm.DbHigh, -30, 10, v => vm.DbHigh = v,
            desc: "最大音量增益（分贝），默认 0.0（-db low:high 的 high）");

        // ── 网络 ──
        var g5 = UI.SettingsGrid(3);
        Num(g5, 0, "TCP 端口 1", vm.TcpPort1, 0, 65535, v => vm.TcpPort1 = (ushort)v,
            desc: "固定 TCP 端口（0 = 自动分配），防火墙场景需要（-p）");
        Num(g5, 1, "TCP 端口 2", vm.TcpPort2, 0, 65535, v => vm.TcpPort2 = (ushort)v);
        Num(g5, 2, "TCP 端口 3", vm.TcpPort3, 0, 65535, v => vm.TcpPort3 = (ushort)v);

        // ── 安全 ──
        var g6 = UI.SettingsGrid(5);
        Cmb(g6, 0, "访问控制", vm.AccessControlOptions, vm.AccessControlIndex, v => vm.AccessControlIndex = v,
            desc: "无认证 / AppleTV 风格 PIN 码（-pin）/ 固定密码（-pw）");
        Txt(g6, 1, "密码", vm.Password, v => vm.Password = v,
            placeholder: "留空则无密码",
            desc: "设置固定密码后所有客户端必须输入相同密码（-pw password）");
        Txt(g6, 2, "密钥文件", vm.Keyfile, v => vm.Keyfile = v,
            placeholder: "留空使用默认",
            desc: "持久公钥存储路径（-key）");
        Tog(g6, 3, "注册列表", vm.RegistrationList, v => vm.RegistrationList = v,
            desc: "维护 PIN 已认证客户端列表，已注册客户端下次连接免 PIN（-reg）");
        Tog(g6, 4, "限制客户端", vm.RestrictClients, v => vm.RestrictClients = v,
            desc: "仅允许通过 -allow 指定的设备连接（-restrict）");

        // ── 杂项 ──
        var g7 = UI.SettingsGrid(8);
        Cmb(g7, 0, "日志级别", vm.LogLevelOptions, vm.LogLevelIndex, v => vm.LogLevelIndex = v,
            desc: "调试级别显示完整协议通信细节（-d）");
        Tog(g7, 1, "允许踢人", vm.NoHold, v => vm.NoHold = v,
            desc: "新客户端连接时自动断开当前客户端（-nohold）");
        Tog(g7, 2, "HLS 支持", vm.HlsSupport, v => vm.HlsSupport = v,
            desc: "启用 HTTP Live Streaming，目前仅支持 YouTube 视频（-hls）");
        Tog(g7, 3, "显示封面", vm.CoverartDisplay, v => vm.CoverartDisplay = v,
            desc: "音频模式下在 UxPlay 内渲染 Apple Music 封面（-ca）");
        Txt(g7, 4, "封面文件名", vm.CoverartFilename, v => vm.CoverartFilename = v,
            desc: "将封面导出到指定文件以供外部查看器显示（-ca filename）");
        Txt(g7, 5, "语言偏好", vm.Lang, v => vm.Lang = v,
            placeholder: "如 fr:es:en",
            desc: "HLS 视频语言优先级（覆盖 $LANGUAGE 环境变量）（-lang）");
        Tog(g7, 6, "显示 FPS", vm.ShowFpsData, v => vm.ShowFpsData = v,
            desc: "在日志中显示客户端发送的视频流性能报告（-FPSdata）");
        Tog(g7, 7, "新窗口关闭", vm.NewWindowClosing, v => vm.NewWindowClosing = v,
            desc: "客户端停止镜像时关闭视频窗口（-nc 取消此行为）");

        // ── 按钮 ──
        var btnReset = UI.Btn("恢复默认"); btnReset.Command = vm.ResetDefaultsCommand;
        var btnSave = UI.AccentBtn("保存设置"); btnSave.Command = vm.SaveSettingsCommand;
        var btnRow = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8,
            HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 8, 0, 24) };
        btnRow.Children.Add(btnReset); btnRow.Children.Add(btnSave);

        Content = new ScrollViewer
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Padding = new Thickness(28, 20, 28, 20),
            Content = new StackPanel
            {
                Spacing = 16, MaxWidth = 720, HorizontalAlignment = HorizontalAlignment.Center,
                Children =
                {
                    _statusBar,
                    UI.Card(UI.Title("身份标识"), g1),
                    UI.Card(UI.Title("显示设置"), g2),
                    UI.Card(UI.Title("视频渲染"), g3),
                    UI.Card(UI.Title("音频渲染"), g4),
                    UI.Card(UI.Title("网络端口"), g5),
                    UI.Card(UI.Title("安全认证"), g6),
                    UI.Card(UI.Title("其他选项"), g7),
                    btnRow,
                }
            }
        };

        vm.PropertyChanged += (_, a) => DispatcherQueue.TryEnqueue(() =>
        {
            if (a.PropertyName == nameof(vm.StatusMessage))
            {
                _statusBar.IsOpen = !string.IsNullOrEmpty(vm.StatusMessage);
                _statusBar.Message = vm.StatusMessage;
            }
        });
    }

    // ── 辅助方法 ──

    void Txt(Grid g, int row, string label, string val, Action<string> set,
             string? placeholder = null, string? desc = null)
    {
        var tb = new TextBox { Text = val, PlaceholderText = placeholder ?? "" };
        tb.Loaded += (_, _) => tb.TextChanged += (_, _) => set(tb.Text);
        UI.Row(g, row, label, tb, desc);
    }

    void Tog(Grid g, int row, string label, bool val, Action<bool> set, string? desc = null)
    {
        var ts = new ToggleSwitch { IsOn = val };
        ts.Loaded += (_, _) => ts.Toggled += (_, _) => set(ts.IsOn);
        UI.Row(g, row, label, ts, desc);
    }

    void Num(Grid g, int row, string label, double val, double min, double max,
             Action<double> set, string? desc = null)
    {
        var nb = new NumberBox { Value = val, Minimum = min, Maximum = max,
            SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Compact };
        nb.Loaded += (_, _) => nb.ValueChanged += (_, _) => set(nb.Value);
        UI.Row(g, row, label, nb, desc);
    }

    void Cmb(Grid g, int row, string label, string[] items, int idx,
             Action<int> set, string? desc = null)
    {
        var cb = new ComboBox();
        foreach (var o in items) cb.Items.Add(o);
        cb.SelectedIndex = idx;
        cb.Loaded += (_, _) => cb.SelectionChanged += (_, _) => set(cb.SelectedIndex);
        UI.Row(g, row, label, cb, desc);
    }

    /// <summary>可编辑下拉框：既能从预设列表选，也能手动输入自定义值</summary>
    void EditCmb(Grid g, int row, string label, string[] presets, string val,
                 Action<string> set, string? desc = null)
    {
        var cb = new ComboBox { IsEditable = true, Text = val };
        foreach (var p in presets) cb.Items.Add(p);
        // 选中匹配项
        for (int i = 0; i < presets.Length; i++)
            if (presets[i] == val) { cb.SelectedIndex = i; break; }
        cb.Loaded += (_, _) =>
        {
            cb.TextSubmitted += (s, _) => set(s.Text);
            cb.SelectionChanged += (_, _) =>
            {
                if (cb.SelectedItem is string s) set(s);
            };
        };
        UI.Row(g, row, label, cb, desc);
    }
}
