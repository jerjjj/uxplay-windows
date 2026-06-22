using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UxPlayClient.Services;
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
        _statusBar.Title = L10n.Get("settings.title");

        // ── 身份标识 ──
        var g1 = UI.SettingsGrid(3);
        Txt(g1, 0, L10n.Get("label.server_name"), vm.ServerName, v => vm.ServerName = v,
            desc: L10n.Get("desc.server_name"));
        Txt(g1, 1, L10n.Get("label.mac_address"), vm.MacAddress, v => vm.MacAddress = v,
            placeholder: L10n.Get("placeholder.mac"),
            desc: L10n.Get("desc.mac_address"));
        Tog(g1, 2, L10n.Get("label.append_hostname"), vm.AppendHostname, v => vm.AppendHostname = v,
            desc: L10n.Get("desc.append_hostname"));

        // ── 显示 ──
        var g2 = UI.SettingsGrid(5);
        Num(g2, 0, L10n.Get("label.width"), vm.Width, 320, 7680, v => vm.Width = (ushort)v,
            desc: L10n.Get("desc.width"));
        Num(g2, 1, L10n.Get("label.height"), vm.Height, 240, 4320, v => vm.Height = (ushort)v,
            desc: L10n.Get("desc.height"));
        Num(g2, 2, L10n.Get("label.refresh_rate"), vm.RefreshRate, 24, 240, v => vm.RefreshRate = (ushort)v,
            desc: L10n.Get("desc.refresh_rate"));
        Num(g2, 3, L10n.Get("label.max_fps"), vm.MaxFps, 1, 120, v => vm.MaxFps = (ushort)v,
            desc: L10n.Get("desc.max_fps"));
        Tog(g2, 4, L10n.Get("label.overscan"), vm.Overscanned, v => vm.Overscanned = v,
            desc: L10n.Get("desc.overscan"));

        // ── 视频 ──
        var g3 = UI.SettingsGrid(11);
        Tog(g3, 0, L10n.Get("label.use_video"), vm.UseVideo, v => vm.UseVideo = v,
            desc: L10n.Get("desc.use_video"));
        EditCmb(g3, 1, L10n.Get("label.video_sink"), VideoSinks, vm.Videosink, v => vm.Videosink = v,
            desc: L10n.Get("desc.video_sink"));
        EditCmb(g3, 2, L10n.Get("label.video_decoder"), VideoDecoders, vm.VideoDecoder, v => vm.VideoDecoder = v,
            desc: L10n.Get("desc.video_decoder"));
        EditCmb(g3, 3, L10n.Get("label.video_converter"), VideoConverters, vm.VideoConverter, v => vm.VideoConverter = v,
            desc: L10n.Get("desc.video_converter"));
        EditCmb(g3, 4, L10n.Get("label.video_parser"), VideoParsers, vm.VideoParser, v => vm.VideoParser = v,
            desc: L10n.Get("desc.video_parser"));
        Cmb(g3, 5, L10n.Get("label.video_flip"), vm.VideoFlipOptions, vm.VideoFlipIndex, v => vm.VideoFlipIndex = v,
            desc: L10n.Get("desc.video_flip"));
        Tog(g3, 6, L10n.Get("label.fullscreen"), vm.Fullscreen, v => vm.Fullscreen = v,
            desc: L10n.Get("desc.fullscreen"));
        Tog(g3, 7, L10n.Get("label.h265"), vm.H265Support, v => vm.H265Support = v,
            desc: L10n.Get("desc.h265"));
        Tog(g3, 8, L10n.Get("label.video_sync"), vm.VideoSync, v => vm.VideoSync = v,
            desc: L10n.Get("desc.video_sync"));
        Tog(g3, 9, L10n.Get("label.bt709"), vm.Bt709Fix, v => vm.Bt709Fix = v,
            desc: L10n.Get("desc.bt709"));
        Tog(g3, 10, L10n.Get("label.nofreeze"), vm.NoFreeze, v => vm.NoFreeze = v,
            desc: L10n.Get("desc.nofreeze"));

        // ── 音频 ──
        var g4 = UI.SettingsGrid(6);
        Tog(g4, 0, L10n.Get("label.use_audio"), vm.UseAudio, v => vm.UseAudio = v,
            desc: L10n.Get("desc.use_audio"));
        EditCmb(g4, 1, L10n.Get("label.audio_sink"), AudioSinks, vm.Audiosink, v => vm.Audiosink = v,
            desc: L10n.Get("desc.audio_sink"));
        Tog(g4, 2, L10n.Get("label.audio_sync"), vm.AudioSync, v => vm.AudioSync = v,
            desc: L10n.Get("desc.audio_sync"));
        Tog(g4, 3, L10n.Get("label.taper"), vm.TaperVolume, v => vm.TaperVolume = v,
            desc: L10n.Get("desc.taper"));
        Num(g4, 4, L10n.Get("label.db_low"), vm.DbLow, -60, 0, v => vm.DbLow = v,
            desc: L10n.Get("desc.db_low"));
        Num(g4, 5, L10n.Get("label.db_high"), vm.DbHigh, -30, 10, v => vm.DbHigh = v,
            desc: L10n.Get("desc.db_high"));

        // ── 网络 ──
        var g5 = UI.SettingsGrid(3);
        Num(g5, 0, $"{L10n.Get("label.tcp_port")} 1", vm.TcpPort1, 0, 65535, v => vm.TcpPort1 = (ushort)v,
            desc: L10n.Get("desc.tcp_port"));
        Num(g5, 1, $"{L10n.Get("label.tcp_port")} 2", vm.TcpPort2, 0, 65535, v => vm.TcpPort2 = (ushort)v);
        Num(g5, 2, $"{L10n.Get("label.tcp_port")} 3", vm.TcpPort3, 0, 65535, v => vm.TcpPort3 = (ushort)v);

        // ── 安全 ──
        var g6 = UI.SettingsGrid(5);
        Cmb(g6, 0, L10n.Get("label.access_control"), vm.AccessControlOptions, vm.AccessControlIndex, v => vm.AccessControlIndex = v,
            desc: L10n.Get("desc.access_control"));
        Txt(g6, 1, L10n.Get("label.password"), vm.Password, v => vm.Password = v,
            placeholder: L10n.Get("placeholder.password"),
            desc: L10n.Get("desc.password"));
        Txt(g6, 2, L10n.Get("label.keyfile"), vm.Keyfile, v => vm.Keyfile = v,
            placeholder: L10n.Get("placeholder.keyfile"),
            desc: L10n.Get("desc.keyfile"));
        Tog(g6, 3, L10n.Get("label.reg_list"), vm.RegistrationList, v => vm.RegistrationList = v,
            desc: L10n.Get("desc.reg_list"));
        Tog(g6, 4, L10n.Get("label.restrict"), vm.RestrictClients, v => vm.RestrictClients = v,
            desc: L10n.Get("desc.restrict"));

        // ── 杂项 ──
        var g7 = UI.SettingsGrid(10);
        Cmb(g7, 0, L10n.Get("label.log_level"), vm.LogLevelOptions, vm.LogLevelIndex, v => vm.LogLevelIndex = v,
            desc: L10n.Get("desc.log_level"));
        Tog(g7, 1, L10n.Get("label.nohold"), vm.NoHold, v => vm.NoHold = v,
            desc: L10n.Get("desc.nohold"));
        Tog(g7, 2, L10n.Get("label.hls"), vm.HlsSupport, v => vm.HlsSupport = v,
            desc: L10n.Get("desc.hls"));
        Tog(g7, 3, L10n.Get("label.coverart"), vm.CoverartDisplay, v => vm.CoverartDisplay = v,
            desc: L10n.Get("desc.coverart"));
        Txt(g7, 4, L10n.Get("label.coverart_file"), vm.CoverartFilename, v => vm.CoverartFilename = v,
            desc: L10n.Get("desc.coverart_file"));
        Txt(g7, 5, L10n.Get("label.lang_pref"), vm.Lang, v => vm.Lang = v,
            placeholder: L10n.Get("placeholder.lang"),
            desc: L10n.Get("desc.lang_pref"));
        Tog(g7, 6, L10n.Get("label.show_fps"), vm.ShowFpsData, v => vm.ShowFpsData = v,
            desc: L10n.Get("desc.show_fps"));
        Tog(g7, 7, L10n.Get("label.new_window"), vm.NewWindowClosing, v => vm.NewWindowClosing = v,
            desc: L10n.Get("desc.new_window"));
        Cmb(g7, 8, L10n.Get("label.language"), vm.LanguageOptions, vm.LanguageIndex, v =>
            { vm.LanguageIndex = v; L10n.SetLanguage(v == 0 ? "en" : "zh-CN"); });
        Cmb(g7, 9, L10n.Get("label.theme"), vm.ThemeOptions, vm.ThemeIndex, v =>
            { vm.ThemeIndex = v; L10n.SetTheme((AppTheme)v); });

        // ── 按钮 ──
        var btnReset = UI.Btn(L10n.Get("settings.defaults")); btnReset.Command = vm.ResetDefaultsCommand;
        var btnSave = UI.AccentBtn(L10n.Get("settings.save")); btnSave.Command = vm.SaveSettingsCommand;
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
                    UI.Card(UI.Title(L10n.Get("settings.section.identity")), g1),
                    UI.Card(UI.Title(L10n.Get("settings.section.display")), g2),
                    UI.Card(UI.Title(L10n.Get("settings.section.video")), g3),
                    UI.Card(UI.Title(L10n.Get("settings.section.audio")), g4),
                    UI.Card(UI.Title(L10n.Get("settings.section.network")), g5),
                    UI.Card(UI.Title(L10n.Get("settings.section.security")), g6),
                    UI.Card(UI.Title(L10n.Get("settings.section.misc")), g7),
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
