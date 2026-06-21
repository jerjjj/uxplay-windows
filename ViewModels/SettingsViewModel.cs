using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UxPlayClient.Interop;
using UxPlayClient.Models;

namespace UxPlayClient.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    // ── 身份标识 ──
    [ObservableProperty] string _serverName = "UxPlay", _macAddress = "";
    [ObservableProperty] bool _appendHostname = true;

    // ── 显示 ──
    [ObservableProperty] ushort _width = 1920, _height = 1080, _refreshRate = 60, _maxFps = 30;
    [ObservableProperty] bool _overscanned;

    // ── 视频 ──
    [ObservableProperty] string _videosink = "autovideosink";
    [ObservableProperty] string _videoDecoder = "decodebin", _videoConverter = "videoconvert", _videoParser = "h264parse";
    [ObservableProperty] int _videoFlipIndex;
    [ObservableProperty] bool _fullscreen, _h265Support, _videoSync = true, _bt709Fix, _useVideo = true, _noFreeze;

    // ── 音频 ──
    [ObservableProperty] string _audiosink = "autoaudiosink";
    [ObservableProperty] bool _audioSync, _useAudio = true, _taperVolume;
    [ObservableProperty] double _dbLow = -30.0, _dbHigh = 0.0;

    // ── 网络 ──
    [ObservableProperty] ushort _tcpPort1, _tcpPort2, _tcpPort3;
    [ObservableProperty] ushort _udpPort1, _udpPort2, _udpPort3;

    // ── 安全 ──
    [ObservableProperty] int _accessControlIndex;
    [ObservableProperty] string _password = "", _keyfile = "";
    [ObservableProperty] bool _registrationList, _restrictClients;

    // ── 杂项 ──
    [ObservableProperty] int _logLevelIndex = 2;
    [ObservableProperty] bool _hlsSupport, _noHold = true, _coverartDisplay, _showFpsData, _newWindowClosing;
    [ObservableProperty] string _coverartFilename = "", _lang = "";

    // ── UI 状态 ──
    [ObservableProperty] string _statusMessage = "";
    [ObservableProperty] bool _isDirty;

    public string[] VideoFlipOptions { get; } = ["无", "左旋90°", "右旋90°", "旋转180°", "垂直翻转", "水平翻转"];
    public string[] AccessControlOptions { get; } = ["无认证", "PIN 码", "密码"];
    public string[] LogLevelOptions { get; } = ["错误", "警告", "信息", "调试", "详细"];

    public void LoadSettings()
    {
        var s = AppSettings.Load();
        ServerName = s.ServerName; MacAddress = s.MacAddress ?? ""; AppendHostname = s.AppendHostname;
        Width = s.Width; Height = s.Height; RefreshRate = s.RefreshRate; MaxFps = s.MaxFps; Overscanned = s.Overscanned;
        Videosink = s.Videosink; VideoDecoder = s.VideoDecoder; VideoConverter = s.VideoConverter; VideoParser = s.VideoParser;
        VideoFlipIndex = (int)s.VideoFlip; Fullscreen = s.Fullscreen; H265Support = s.H265Support;
        VideoSync = s.VideoSync; Bt709Fix = s.Bt709Fix; UseVideo = s.UseVideo; NoFreeze = s.NoFreeze;
        Audiosink = s.Audiosink; AudioSync = s.AudioSync; UseAudio = s.UseAudio; TaperVolume = s.TaperVolume;
        DbLow = s.DbLow; DbHigh = s.DbHigh;
        TcpPort1 = s.TcpPorts.Length > 0 ? s.TcpPorts[0] : (ushort)0;
        TcpPort2 = s.TcpPorts.Length > 1 ? s.TcpPorts[1] : (ushort)0;
        TcpPort3 = s.TcpPorts.Length > 2 ? s.TcpPorts[2] : (ushort)0;
        UdpPort1 = s.UdpPorts.Length > 0 ? s.UdpPorts[0] : (ushort)0;
        UdpPort2 = s.UdpPorts.Length > 1 ? s.UdpPorts[1] : (ushort)0;
        UdpPort3 = s.UdpPorts.Length > 2 ? s.UdpPorts[2] : (ushort)0;
        AccessControlIndex = (int)s.AccessControl; Password = s.Password ?? ""; Keyfile = s.Keyfile ?? "";
        RegistrationList = s.RegistrationList; RestrictClients = s.RestrictClients;
        LogLevelIndex = (int)s.LogLevel - 3;
        HlsSupport = s.HlsSupport; NoHold = s.NoHold;
        CoverartDisplay = s.CoverartDisplay; CoverartFilename = s.CoverartFilename ?? "";
        ShowFpsData = s.ShowFpsData; NewWindowClosing = s.NewWindowClosing; Lang = s.Lang ?? "";
        IsDirty = false; StatusMessage = "";
    }

    AppSettings Build() => new()
    {
        ServerName = string.IsNullOrWhiteSpace(ServerName) ? "UxPlay" : ServerName,
        MacAddress = string.IsNullOrWhiteSpace(MacAddress) ? null : MacAddress,
        AppendHostname = AppendHostname,
        Width = Width, Height = Height, RefreshRate = RefreshRate, MaxFps = MaxFps, Overscanned = Overscanned,
        Videosink = string.IsNullOrWhiteSpace(Videosink) ? "autovideosink" : Videosink,
        VideoDecoder = string.IsNullOrWhiteSpace(VideoDecoder) ? "decodebin" : VideoDecoder,
        VideoConverter = string.IsNullOrWhiteSpace(VideoConverter) ? "videoconvert" : VideoConverter,
        VideoParser = string.IsNullOrWhiteSpace(VideoParser) ? "h264parse" : VideoParser,
        VideoFlip = (UxPlayVideoFlip)VideoFlipIndex,
        Fullscreen = Fullscreen, H265Support = H265Support, VideoSync = VideoSync,
        Bt709Fix = Bt709Fix, UseVideo = UseVideo, NoFreeze = NoFreeze,
        Audiosink = string.IsNullOrWhiteSpace(Audiosink) ? "autoaudiosink" : Audiosink,
        AudioSync = AudioSync, UseAudio = UseAudio, TaperVolume = TaperVolume,
        DbLow = DbLow, DbHigh = DbHigh,
        TcpPorts = [TcpPort1, TcpPort2, TcpPort3],
        UdpPorts = [UdpPort1, UdpPort2, UdpPort3],
        AccessControl = (UxPlayAccessControl)AccessControlIndex,
        Password = string.IsNullOrWhiteSpace(Password) ? null : Password,
        Keyfile = string.IsNullOrWhiteSpace(Keyfile) ? null : Keyfile,
        RegistrationList = RegistrationList, RestrictClients = RestrictClients,
        LogLevel = (UxPlayLogLevel)(LogLevelIndex + 3),
        HlsSupport = HlsSupport, NoHold = NoHold,
        CoverartDisplay = CoverartDisplay,
        CoverartFilename = string.IsNullOrWhiteSpace(CoverartFilename) ? null : CoverartFilename,
        ShowFpsData = ShowFpsData, NewWindowClosing = NewWindowClosing,
        Lang = string.IsNullOrWhiteSpace(Lang) ? null : Lang,
    };

    [RelayCommand] void SaveSettings() { if (Build().Save()) { IsDirty = false; StatusMessage = "设置已保存，重启投屏后生效"; } else StatusMessage = "保存失败"; }

    [RelayCommand] void ResetDefaults()
    {
        var d = new AppSettings();
        ServerName = d.ServerName; MacAddress = ""; AppendHostname = d.AppendHostname;
        Width = d.Width; Height = d.Height; RefreshRate = d.RefreshRate; MaxFps = d.MaxFps; Overscanned = d.Overscanned;
        Videosink = d.Videosink; VideoDecoder = d.VideoDecoder; VideoConverter = d.VideoConverter; VideoParser = d.VideoParser;
        VideoFlipIndex = 0; Fullscreen = d.Fullscreen; H265Support = d.H265Support;
        VideoSync = d.VideoSync; Bt709Fix = d.Bt709Fix; UseVideo = d.UseVideo; NoFreeze = d.NoFreeze;
        Audiosink = d.Audiosink; AudioSync = d.AudioSync; UseAudio = d.UseAudio; TaperVolume = d.TaperVolume;
        DbLow = d.DbLow; DbHigh = d.DbHigh;
        TcpPort1 = 0; TcpPort2 = 0; TcpPort3 = 0; UdpPort1 = 0; UdpPort2 = 0; UdpPort3 = 0;
        AccessControlIndex = 0; Password = ""; Keyfile = "";
        RegistrationList = d.RegistrationList; RestrictClients = d.RestrictClients;
        LogLevelIndex = 2; HlsSupport = d.HlsSupport; NoHold = d.NoHold;
        CoverartDisplay = d.CoverartDisplay; CoverartFilename = "";
        ShowFpsData = d.ShowFpsData; NewWindowClosing = d.NewWindowClosing; Lang = "";
        IsDirty = true; StatusMessage = "已恢复默认设置";
    }
}
