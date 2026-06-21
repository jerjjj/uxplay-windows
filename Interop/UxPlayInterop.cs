using System;
using System.Runtime.InteropServices;

namespace UxPlayClient.Interop;

// ==================== Enumerations ====================

public enum UxPlayState   { Idle = 0, Starting = 1, Running = 2, Stopping = 3, Error = 4 }
public enum UxPlayVideoFlip { None = 0, Left = 1, Right = 2, Invert = 3, VFlip = 4, HFlip = 5 }
public enum UxPlayLogLevel  { Error = 3, Warning = 4, Info = 5, Debug = 6, Verbose = 7 }
public enum UxPlayAccessControl { Free = 0, Pin = 1, Password = 2 }
public enum UxPlayEventType { StateChanged=0, ClientConnected=1, ClientDisconnected=2, DisplayPin=3, MirrorStarted=4, MirrorStopped=5, AudioStarted=6, AudioStopped=7, AudioMetadata=8, VideoSizeChanged=9, Error=10 }
public enum UxPlayError { Ok=0, InvalidArgument=-1, AlreadyRunning=-2, NotRunning=-3, GStreamerInit=-4, RaopInit=-5, DnssdInit=-6, Network=-7, OutOfMemory=-8, Internal=-9 }

// ==================== Structs ====================

[StructLayout(LayoutKind.Sequential)]
public struct UxPlayClientInfo
{
    public IntPtr DeviceIdPtr, DeviceModelPtr, DeviceNamePtr;
    public string? DeviceId    => Marshal.PtrToStringUTF8(DeviceIdPtr);
    public string? DeviceModel => Marshal.PtrToStringUTF8(DeviceModelPtr);
    public string? DeviceName  => Marshal.PtrToStringUTF8(DeviceNamePtr);
}

[StructLayout(LayoutKind.Sequential)]
public struct UxPlayAudioMeta
{
    public IntPtr ArtistPtr, TitlePtr, AlbumPtr;
    public string? Artist => Marshal.PtrToStringUTF8(ArtistPtr);
    public string? Title  => Marshal.PtrToStringUTF8(TitlePtr);
    public string? Album  => Marshal.PtrToStringUTF8(AlbumPtr);
}

[StructLayout(LayoutKind.Sequential)]
public struct UxPlayVideoSize { public float WidthSource, HeightSource, Width, Height; }

[StructLayout(LayoutKind.Explicit, Size = 40)]
public struct UxPlayEventData
{
    [FieldOffset(0)] public UxPlayEventType Type;
    [FieldOffset(8)] public UxPlayState State;
    [FieldOffset(8)] public UxPlayClientInfo Client;
    [FieldOffset(8)] public IntPtr PinPtr;
    [FieldOffset(8)] public UxPlayAudioMeta AudioMeta;
    [FieldOffset(8)] public UxPlayVideoSize VideoSize;
    [FieldOffset(8)] public IntPtr ErrorMsgPtr;
    public string? Pin      => Marshal.PtrToStringUTF8(PinPtr);
    public string? ErrorMsg => Marshal.PtrToStringUTF8(ErrorMsgPtr);
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct UxPlayConfig : IDisposable
{
    public IntPtr ServerNamePtr, MacAddressPtr;
    [MarshalAs(UnmanagedType.U1)] public bool AppendHostname;
    public ushort Width, Height, RefreshRate, MaxFps;
    public IntPtr VideosinkPtr, VideosinkOptionsPtr, VideoDecoderPtr, VideoConverterPtr, VideoParserPtr;
    public UxPlayVideoFlip VideoFlip;
    [MarshalAs(UnmanagedType.U1)] public bool Fullscreen, H265Support, VideoSync, Bt709Fix, UseVideo, NoFreeze;
    public IntPtr AudiosinkPtr;
    [MarshalAs(UnmanagedType.U1)] public bool AudioSync, UseAudio;
    public double InitialVolume, DbLow, DbHigh;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] public ushort[] TcpPorts;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] public ushort[] UdpPorts;
    public UxPlayAccessControl AccessControl;
    public IntPtr PasswordPtr, KeyfilePtr;
    [MarshalAs(UnmanagedType.U1)] public bool RegistrationList;
    public UxPlayLogLevel LogLevel;
    [MarshalAs(UnmanagedType.U1)] public bool CoverartDisplay;
    public IntPtr CoverartFilenamePtr;
    [MarshalAs(UnmanagedType.U1)] public bool HlsSupport;
    public IntPtr LangPtr;
    [MarshalAs(UnmanagedType.U1)] public bool NoHold;

    // Managed alloc tracking
    IntPtr _a0,_a1,_a2,_a3,_a4,_a5,_a6,_a7,_a8,_a9,_a10,_a11;

    public string? ServerName     { get => Marshal.PtrToStringUTF8(ServerNamePtr);     set { F(ref _a0); _a0=A(value); ServerNamePtr=_a0; } }
    public string? MacAddress     { get => Marshal.PtrToStringUTF8(MacAddressPtr);     set { F(ref _a1); _a1=A(value); MacAddressPtr=_a1; } }
    public string? Videosink      { get => Marshal.PtrToStringUTF8(VideosinkPtr);      set { F(ref _a2); _a2=A(value); VideosinkPtr=_a2; } }
    public string? VideosinkOptions{get => Marshal.PtrToStringUTF8(VideosinkOptionsPtr);set { F(ref _a3); _a3=A(value); VideosinkOptionsPtr=_a3; } }
    public string? VideoDecoder   { get => Marshal.PtrToStringUTF8(VideoDecoderPtr);   set { F(ref _a4); _a4=A(value); VideoDecoderPtr=_a4; } }
    public string? VideoConverter { get => Marshal.PtrToStringUTF8(VideoConverterPtr); set { F(ref _a5); _a5=A(value); VideoConverterPtr=_a5; } }
    public string? VideoParser    { get => Marshal.PtrToStringUTF8(VideoParserPtr);    set { F(ref _a6); _a6=A(value); VideoParserPtr=_a6; } }
    public string? Audiosink      { get => Marshal.PtrToStringUTF8(AudiosinkPtr);      set { F(ref _a7); _a7=A(value); AudiosinkPtr=_a7; } }
    public string? Password       { get => Marshal.PtrToStringUTF8(PasswordPtr);       set { F(ref _a8); _a8=A(value); PasswordPtr=_a8; } }
    public string? Keyfile        { get => Marshal.PtrToStringUTF8(KeyfilePtr);        set { F(ref _a9); _a9=A(value); KeyfilePtr=_a9; } }
    public string? CoverartFilename{get => Marshal.PtrToStringUTF8(CoverartFilenamePtr);set{ F(ref _a10);_a10=A(value);CoverartFilenamePtr=_a10; } }
    public string? Lang           { get => Marshal.PtrToStringUTF8(LangPtr);           set { F(ref _a11);_a11=A(value);LangPtr=_a11; } }

    public void Dispose() { F(ref _a0);F(ref _a1);F(ref _a2);F(ref _a3);F(ref _a4);F(ref _a5);F(ref _a6);F(ref _a7);F(ref _a8);F(ref _a9);F(ref _a10);F(ref _a11); }
    static IntPtr A(string? s) { if(s is null) return IntPtr.Zero; var b=System.Text.Encoding.UTF8.GetBytes(s+'\0'); var p=Marshal.AllocHGlobal(b.Length); Marshal.Copy(b,0,p,b.Length); return p; }
    static void F(ref IntPtr p) { if(p!=IntPtr.Zero){Marshal.FreeHGlobal(p);p=IntPtr.Zero;} }
}

// ==================== Callbacks ====================

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void UxPlayEventCallback(IntPtr userData, ref UxPlayEventData eventData);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void UxPlayLogCallback(IntPtr userData, UxPlayLogLevel level, [MarshalAs(UnmanagedType.LPUTF8Str)] string message);

// ==================== Native API ====================

public static class UxPlayNative
{
    const string D = "libuxplaylib";

    [DllImport(D, CallingConvention = CallingConvention.Cdecl)] public static extern UxPlayConfig  uxplay_default_config();
    [DllImport(D, CallingConvention = CallingConvention.Cdecl)] public static extern IntPtr         uxplay_version();
    [DllImport(D, CallingConvention = CallingConvention.Cdecl)] public static extern IntPtr         uxplay_error_string(UxPlayError error);

    [DllImport(D, CallingConvention = CallingConvention.Cdecl)] public static extern UxPlayError   uxplay_create(out IntPtr handle);
    [DllImport(D, CallingConvention = CallingConvention.Cdecl)] public static extern UxPlayError   uxplay_configure(IntPtr handle, ref UxPlayConfig config);
    [DllImport(D, CallingConvention = CallingConvention.Cdecl)] public static extern UxPlayError   uxplay_set_event_callback(IntPtr handle, UxPlayEventCallback? cb, IntPtr ud);
    [DllImport(D, CallingConvention = CallingConvention.Cdecl)] public static extern UxPlayError   uxplay_set_log_callback(IntPtr handle, UxPlayLogCallback? cb, IntPtr ud);
    [DllImport(D, CallingConvention = CallingConvention.Cdecl)] public static extern UxPlayError   uxplay_start(IntPtr handle);
    [DllImport(D, CallingConvention = CallingConvention.Cdecl)] public static extern UxPlayError   uxplay_stop(IntPtr handle);
    [DllImport(D, CallingConvention = CallingConvention.Cdecl)] public static extern void          uxplay_destroy(IntPtr handle);

    [DllImport(D, CallingConvention = CallingConvention.Cdecl)] public static extern UxPlayState   uxplay_get_state(IntPtr handle);
    [DllImport(D, CallingConvention = CallingConvention.Cdecl)] public static extern UxPlayError   uxplay_set_volume(IntPtr handle, double volume);
    [DllImport(D, CallingConvention = CallingConvention.Cdecl)] public static extern int            uxplay_get_connection_count(IntPtr handle);
    [DllImport(D, CallingConvention = CallingConvention.Cdecl)] public static extern UxPlayError   uxplay_disconnect_clients(IntPtr handle);
}
