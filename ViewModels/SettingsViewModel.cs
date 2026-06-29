using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UxPlayClient.Interop;
using UxPlayClient.Models;
using UxPlayClient.Services;

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
    [ObservableProperty] bool _srgbFix = true, _keepWindow, _forceSoftwareDecoder;

    // ── 音频 ──
    [ObservableProperty] string _audiosink = "autoaudiosink";
    [ObservableProperty] bool _audioSync, _useAudio = true, _taperVolume;
    [ObservableProperty] double _dbLow = -30.0, _dbHigh = 0.0, _initialVolume, _audioLatency;

    // ── 网络 ──
    [ObservableProperty] ushort _tcpPort1, _tcpPort2, _tcpPort3;

    // ── 安全 ──
    [ObservableProperty] int _accessControlIndex;
    [ObservableProperty] string _password = "", _keyfile = "";
    [ObservableProperty] bool _registrationList, _restrictClients;

    // ── 杂项 ──
    [ObservableProperty] int _logLevelIndex = 2;
    [ObservableProperty] bool _hlsSupport, _noHold = true, _coverartDisplay, _showFpsData, _newWindowClosing;
    [ObservableProperty] string _coverartFilename = "", _lang = "", _metadataFilename = "", _recordFilename = "";
    [ObservableProperty] int _resetTimeout;
    [ObservableProperty] int _languageIndex = 1; // default zh-CN
    [ObservableProperty] int _themeIndex = 0;    // default System

    // ── UI 状态 ──
    [ObservableProperty] string _statusMessage = "";
    [ObservableProperty] bool _isDirty;

    public string[] VideoFlipOptions => [L10n.Get("flip.none"), L10n.Get("flip.left"), L10n.Get("flip.right"), L10n.Get("flip.invert"), L10n.Get("flip.vflip"), L10n.Get("flip.hflip")];
    public string[] AccessControlOptions => [L10n.Get("access.free"), L10n.Get("access.pin"), L10n.Get("access.password")];
    public string[] LogLevelOptions => [L10n.Get("log.level.error"), L10n.Get("log.level.warning"), L10n.Get("log.level.info"), L10n.Get("log.level.debug"), L10n.Get("log.level.verbose")];
    public string[] LanguageOptions => [L10n.Get("lang.en"), L10n.Get("lang.zh-CN")];
    public string[] ThemeOptions => [L10n.Get("theme.system"), L10n.Get("theme.light"), L10n.Get("theme.dark")];

    public void LoadSettings()
    {
        var s = AppSettings.Load();
        ServerName = s.ServerName; MacAddress = s.MacAddress ?? ""; AppendHostname = s.AppendHostname;
        Width = s.Width; Height = s.Height; RefreshRate = s.RefreshRate; MaxFps = s.MaxFps; Overscanned = s.Overscanned;
        Videosink = s.Videosink; VideoDecoder = s.VideoDecoder; VideoConverter = s.VideoConverter; VideoParser = s.VideoParser;
        VideoFlipIndex = (int)s.VideoFlip; Fullscreen = s.Fullscreen; H265Support = s.H265Support;
        VideoSync = s.VideoSync; Bt709Fix = s.Bt709Fix; UseVideo = s.UseVideo; NoFreeze = s.NoFreeze;
        SrgbFix = s.SrgbFix; KeepWindow = s.KeepWindow; ForceSoftwareDecoder = s.ForceSoftwareDecoder;
        Audiosink = s.Audiosink; AudioSync = s.AudioSync; UseAudio = s.UseAudio; TaperVolume = s.TaperVolume;
        DbLow = s.DbLow; DbHigh = s.DbHigh; InitialVolume = s.InitialVolume; AudioLatency = s.AudioLatency;
        TcpPort1 = s.TcpPorts.Length > 0 ? s.TcpPorts[0] : (ushort)0;
        TcpPort2 = s.TcpPorts.Length > 1 ? s.TcpPorts[1] : (ushort)0;
        TcpPort3 = s.TcpPorts.Length > 2 ? s.TcpPorts[2] : (ushort)0;
        AccessControlIndex = (int)s.AccessControl; Password = s.Password ?? ""; Keyfile = s.Keyfile ?? "";
        RegistrationList = s.RegistrationList; RestrictClients = s.RestrictClients;
        LogLevelIndex = (int)s.LogLevel - 3;
        HlsSupport = s.HlsSupport; NoHold = s.NoHold;
        CoverartDisplay = s.CoverartDisplay; CoverartFilename = s.CoverartFilename ?? "";
        ShowFpsData = s.ShowFpsData; NewWindowClosing = s.NewWindowClosing; Lang = s.Lang ?? "";
        MetadataFilename = s.MetadataFilename ?? ""; RecordFilename = s.RecordFilename ?? "";
        ResetTimeout = s.ResetTimeout;
        LanguageIndex = (s.Language == "en") ? 0 : 1;
        ThemeIndex = (int)s.Theme;
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
        SrgbFix = SrgbFix, KeepWindow = KeepWindow, ForceSoftwareDecoder = ForceSoftwareDecoder,
        Audiosink = string.IsNullOrWhiteSpace(Audiosink) ? "autoaudiosink" : Audiosink,
        AudioSync = AudioSync, UseAudio = UseAudio, TaperVolume = TaperVolume,
        DbLow = DbLow, DbHigh = DbHigh, InitialVolume = InitialVolume, AudioLatency = AudioLatency,
        TcpPorts = [TcpPort1, TcpPort2, TcpPort3],
        UdpPorts = [0, 0, 0],
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
        MetadataFilename = string.IsNullOrWhiteSpace(MetadataFilename) ? null : MetadataFilename,
        RecordFilename = string.IsNullOrWhiteSpace(RecordFilename) ? null : RecordFilename,
        ResetTimeout = ResetTimeout,
        Language = (LanguageIndex == 0) ? "en" : "zh-CN",
        Theme = (Services.AppTheme)ThemeIndex,
    };

    [RelayCommand] void SaveSettings() { if (Build().Save()) { IsDirty = false; StatusMessage = L10n.Get("settings.saved"); } else StatusMessage = L10n.Get("settings.save_failed"); }

    [RelayCommand] void ResetDefaults()
    {
        var d = new AppSettings();
        ServerName = d.ServerName; MacAddress = ""; AppendHostname = d.AppendHostname;
        Width = d.Width; Height = d.Height; RefreshRate = d.RefreshRate; MaxFps = d.MaxFps; Overscanned = d.Overscanned;
        Videosink = d.Videosink; VideoDecoder = d.VideoDecoder; VideoConverter = d.VideoConverter; VideoParser = d.VideoParser;
        VideoFlipIndex = 0; Fullscreen = d.Fullscreen; H265Support = d.H265Support;
        VideoSync = d.VideoSync; Bt709Fix = d.Bt709Fix; UseVideo = d.UseVideo; NoFreeze = d.NoFreeze;
        SrgbFix = d.SrgbFix; KeepWindow = d.KeepWindow; ForceSoftwareDecoder = d.ForceSoftwareDecoder;
        Audiosink = d.Audiosink; AudioSync = d.AudioSync; UseAudio = d.UseAudio; TaperVolume = d.TaperVolume;
        DbLow = d.DbLow; DbHigh = d.DbHigh; InitialVolume = d.InitialVolume; AudioLatency = d.AudioLatency;
        TcpPort1 = 0; TcpPort2 = 0; TcpPort3 = 0;
        AccessControlIndex = 0; Password = ""; Keyfile = "";
        RegistrationList = d.RegistrationList; RestrictClients = d.RestrictClients;
        LogLevelIndex = 2; HlsSupport = d.HlsSupport; NoHold = d.NoHold;
        CoverartDisplay = d.CoverartDisplay; CoverartFilename = "";
        ShowFpsData = d.ShowFpsData; NewWindowClosing = d.NewWindowClosing; Lang = "";
        MetadataFilename = ""; RecordFilename = ""; ResetTimeout = 0;
        LanguageIndex = 1; ThemeIndex = 0;
        IsDirty = true; StatusMessage = L10n.Get("msg.settings_reset");
    }
}
