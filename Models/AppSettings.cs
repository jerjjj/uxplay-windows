using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using UxPlayClient.Interop;

namespace UxPlayClient.Models;

public class AppSettings
{
    public string ServerName { get; set; } = "UxPlay";
    public string? MacAddress { get; set; }
    public bool AppendHostname { get; set; } = true;
    public ushort Width { get; set; } = 1920;
    public ushort Height { get; set; } = 1080;
    public ushort RefreshRate { get; set; } = 60;
    public ushort MaxFps { get; set; } = 30;
    public string Videosink { get; set; } = "autovideosink";
    public string? VideosinkOptions { get; set; }
    public string VideoDecoder { get; set; } = "decodebin";
    public string VideoConverter { get; set; } = "videoconvert";
    public string VideoParser { get; set; } = "h264parse";
    public UxPlayVideoFlip VideoFlip { get; set; }
    public bool Fullscreen { get; set; }
    public bool H265Support { get; set; }
    public bool VideoSync { get; set; } = true;
    public bool Bt709Fix { get; set; }
    public bool UseVideo { get; set; } = true;
    public bool NoFreeze { get; set; }
    public string Audiosink { get; set; } = "autoaudiosink";
    public bool AudioSync { get; set; }
    public bool UseAudio { get; set; } = true;
    public double InitialVolume { get; set; }
    public double DbLow { get; set; } = -30.0;
    public double DbHigh { get; set; }
    public ushort[] TcpPorts { get; set; } = [0, 0, 0];
    public ushort[] UdpPorts { get; set; } = [0, 0, 0];
    public UxPlayAccessControl AccessControl { get; set; }
    public string? Password { get; set; }
    public string? Keyfile { get; set; }
    public bool RegistrationList { get; set; }
    public UxPlayLogLevel LogLevel { get; set; } = UxPlayLogLevel.Info;
    public bool CoverartDisplay { get; set; }
    public string? CoverartFilename { get; set; }
    public bool HlsSupport { get; set; }
    public string? Lang { get; set; }
    public bool NoHold { get; set; } = true;
    public bool Overscanned { get; set; }
    public bool TaperVolume { get; set; }
    public bool RestrictClients { get; set; }
    public bool NewWindowClosing { get; set; }
    public bool ShowFpsData { get; set; }
    public string Language { get; set; } = "zh-CN";
    public Services.AppTheme Theme { get; set; } = Services.AppTheme.System;

    public UxPlayConfig ToNativeConfig()
    {
        // Windows 下全屏必须用 d3d11videosink，autovideosink 不支持
        var sink = Videosink;
        if (Fullscreen && System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                System.Runtime.InteropServices.OSPlatform.Windows)
            && (string.IsNullOrWhiteSpace(sink) || sink == "autovideosink"))
        {
            sink = "d3d11videosink";
        }

        return new UxPlayConfig
        {
            ServerName = ServerName, MacAddress = MacAddress, AppendHostname = AppendHostname,
            Width = Width, Height = Height, RefreshRate = RefreshRate, MaxFps = MaxFps,
            Videosink = sink, VideosinkOptions = VideosinkOptions,
            VideoDecoder = VideoDecoder, VideoConverter = VideoConverter, VideoParser = VideoParser,
            VideoFlip = VideoFlip, Fullscreen = Fullscreen, H265Support = H265Support,
            VideoSync = VideoSync, Bt709Fix = Bt709Fix, UseVideo = UseVideo, NoFreeze = NoFreeze,
            Audiosink = Audiosink, AudioSync = AudioSync, UseAudio = UseAudio,
            InitialVolume = InitialVolume, DbLow = DbLow, DbHigh = DbHigh,
            TcpPorts = (ushort[])TcpPorts.Clone(), UdpPorts = (ushort[])UdpPorts.Clone(),
            AccessControl = AccessControl, Password = Password, Keyfile = Keyfile,
            RegistrationList = RegistrationList, LogLevel = LogLevel,
            CoverartDisplay = CoverartDisplay, CoverartFilename = CoverartFilename,
            HlsSupport = HlsSupport, Lang = Lang, NoHold = NoHold,
        };
    }

    static readonly JsonSerializerOptions s_opt = new() { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
    static string Path_ => System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UxPlayClient", "appsettings.json");

    // ── 内存缓存：避免每次属性访问都读盘+反序列化 ──
    static AppSettings? s_cached;
    static readonly object s_lock = new();

    public bool Save()
    {
        try
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path_)!);
            File.WriteAllText(Path_, JsonSerializer.Serialize(this, s_opt));
            lock (s_lock) { s_cached = this; } // 写盘后同步缓存
            return true;
        }
        catch { return false; }
    }

    public static AppSettings Load()
    {
        // 快速路径：缓存命中则零 I/O
        lock (s_lock) { if (s_cached is not null) return s_cached; }

        try
        {
            var p = Path_;
            var result = File.Exists(p) ? JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(p), s_opt) ?? new() : new();
            lock (s_lock) { s_cached = result; }
            return result;
        }
        catch { return new(); }
    }

    /// <summary>强制下次 Load() 重新读盘（外部修改了配置文件时调用）</summary>
    public static void InvalidateCache() { lock (s_lock) { s_cached = null; } }
}
