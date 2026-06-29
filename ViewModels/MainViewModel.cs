using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using UxPlayClient.Interop;
using UxPlayClient.Models;
using UxPlayClient.Services;

namespace UxPlayClient.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly UxPlayService _svc;
    private readonly StringBuilder _logBuf = new(4096);
    private bool _userStopping;

    [ObservableProperty] string _statusText = L10n.Get("status.idle");
    [ObservableProperty] string _statusColor = "Gray";
    [ObservableProperty] bool _isRunning, _isStarting;
    [ObservableProperty] int _connectionCount;
    [ObservableProperty] string _pinCode = "";
    [ObservableProperty] bool _pinVisible;
    [ObservableProperty] string _audioArtist = "", _audioTitle = "";
    [ObservableProperty] bool _audioMetaVisible;
    [ObservableProperty] string _logText = "";
    [ObservableProperty] string _errorMessage = "";
    [ObservableProperty] string _libVersion = "";
    [ObservableProperty] string _videoSizeInfo = "";
    [ObservableProperty] bool _videoSizeVisible;

    public ObservableCollection<string> ConnectedDevices { get; } = new();

    private UxPlayState _currentState;

    // ── 配置缓存：避免每次 DoStart 都重新分配 UxPlayConfig 字符串 ──
    private AppSettings? _lastCfg;
    private UxPlayConfig _cachedNativeCfg;
    private bool _cfgInitialized;

    public MainViewModel(UxPlayService svc, DispatcherQueue dq)
    {
        _svc = svc;

        L10n.LanguageChanged += () => { if (_svc is not null) ApplyState(_currentState); };

        svc.StateChanged       += (_, s) => { ApplyState(s); if (!_userStopping && s == UxPlayState.Idle) { Log(L10n.Get("log.auto_restart")); _ = AutoRestart(); } };
        svc.ClientConnected    += (_, _) => { ConnectionCount = svc.GetConnectionCount(); };
        svc.ClientDisconnected += (_, _) => { ConnectionCount = svc.GetConnectionCount(); if (ConnectionCount == 0) ConnectedDevices.Clear(); };
        svc.ClientInfo         += (_, c) => { ConnectedDevices.Clear(); ConnectedDevices.Add($"{c.name} ({c.model})"); Log($"{L10n.Get("log.device_connected")}: {c.name} ({c.model}) ID={c.id}"); };
        svc.PinDisplayed       += (_, p) => { PinCode = p; PinVisible = true; Log($"{L10n.Get("log.pin")}: {p}"); };
        svc.MirrorStarted      += (_, _) => Log(L10n.Get("log.mirror_started"));
        svc.MirrorStopped      += (_, _) => Log(L10n.Get("log.mirror_stopped"));
        svc.AudioMetadata      += (_, m) => { AudioArtist = m.artist ?? ""; AudioTitle = m.title ?? ""; AudioMetaVisible = !string.IsNullOrEmpty(m.title); };
        svc.ErrorOccurred      += (_, m) => { ErrorMessage = m; Log($"[ERROR] {m}"); };
        svc.LogMessage         += (_, m) => Log(m);
        svc.VideoSizeChanged   += (_, s) => { VideoSizeInfo = $"{s.ws:F0}x{s.hs:F0} → {s.w:F0}x{s.h:F0}"; VideoSizeVisible = true; };

        // Library version
        try { LibVersion = System.Runtime.InteropServices.Marshal.PtrToStringUTF8(UxPlayNative.uxplay_version()) ?? ""; } catch { LibVersion = "?"; }
    }

    // ── 状态映射（单一来源） ──

    void ApplyState(UxPlayState s)
    {
        _currentState = s;
        (StatusText, StatusColor, IsRunning, IsStarting) = s switch
        {
            UxPlayState.Idle     => (L10n.Get("status.idle"),     "Gray",   false, false),
            UxPlayState.Starting => (L10n.Get("status.starting"), "Orange", false, true),
            UxPlayState.Running  => (L10n.Get("status.running"),  "Green",  true,  false),
            UxPlayState.Stopping => (L10n.Get("status.stopping"), "Orange", false, true),
            UxPlayState.Error    => (L10n.Get("status.error"),    "Red",    false, false),
            _ => (L10n.Get("status.unknown"), "Gray", false, false),
        };
        NotifyCommands();
        if (s is UxPlayState.Idle or UxPlayState.Error)
        {
            ConnectionCount = 0; ConnectedDevices.Clear();
            PinVisible = false; AudioMetaVisible = false;
        }
    }

    void NotifyCommands()
    {
        StartCommand.NotifyCanExecuteChanged();
        StopCommand.NotifyCanExecuteChanged();
        RestartCommand.NotifyCanExecuteChanged();
    }

    // ── 启动流程（单一来源） ──

    async Task DoStart()
    {
        _svc.Create();
        var s = AppSettings.Load();

        // 始终 Configure（UpdateFrom 仅在字符串变化时 Marshal 重分配，零变更近乎零开销）
        if (!_cfgInitialized)
            _cachedNativeCfg = new UxPlayConfig();
        _cachedNativeCfg.UpdateFrom(s);
        _svc.Configure(ref _cachedNativeCfg);
        _lastCfg = s;
        _cfgInitialized = true;

        await _svc.StartAsync();
        await Task.Delay(500);
        ApplyState(_svc.GetState());
        ConnectionCount = _svc.GetConnectionCount();
    }

    // ── 命令 ──

    [RelayCommand(CanExecute = nameof(CanStart))]
    async Task Start()
    {
        _userStopping = false;
        ErrorMessage = "";
        try { await DoStart(); }
        catch (Exception ex) { ErrorMessage = $"{L10n.Get("msg.start_failed")}: {ex.Message}"; Log($"[ERROR] {ex.Message}"); }
    }
    bool CanStart => !IsRunning && !IsStarting;

    [RelayCommand(CanExecute = nameof(CanStop))]
    async Task Stop()
    {
        _userStopping = true;
        try { await _svc.StopAsync(); ConnectionCount = 0; ConnectedDevices.Clear(); }
        catch (Exception ex) { ErrorMessage = $"{L10n.Get("msg.stop_failed")}: {ex.Message}"; }
    }
    bool CanStop => IsRunning || IsStarting;

    [RelayCommand(CanExecute = nameof(CanStop))]
    async Task Restart()
    {
        _userStopping = true;
        Log(L10n.Get("log.restart"));
        try
        {
            await _svc.StopAsync();
            ConnectionCount = 0; ConnectedDevices.Clear();
            await Task.Delay(500);
            _userStopping = false;
            await DoStart();
            Log(L10n.Get("log.restart_ok"));
        }
        catch (Exception ex) { ErrorMessage = $"{L10n.Get("msg.restart_failed")}: {ex.Message}"; Log($"[ERROR] {ex.Message}"); }
    }

    async Task AutoRestart()
    {
        try
        {
            await Task.Delay(1000);
            if (_userStopping || IsRunning) return;
            await DoStart();
            Log(L10n.Get("log.auto_restart_ok"));
        }
        catch (Exception ex) { Log($"[WARN] {L10n.Get("log.auto_restart_fail")}: {ex.Message}"); }
    }

    [RelayCommand] void DisconnectAll()
    {
        try { _svc.DisconnectClients(); ConnectedDevices.Clear(); ConnectionCount = 0; Log(L10n.Get("log.disconnected_all")); }
        catch (Exception ex) { Log($"[ERROR] {ex.Message}"); }
    }

    [RelayCommand] void ClearLog() { _logBuf.Clear(); LogText = ""; }

    // ── 日志（StringBuilder 避免 O(n) 字符串拷贝） ──

    void Log(string m)
    {
        _logBuf.Append('[').Append(DateTime.Now.ToString("HH:mm:ss")).Append("] ").AppendLine(m);
        if (_logBuf.Length > 10000)
            _logBuf.Remove(0, _logBuf.Length - 5000);
        LogText = _logBuf.ToString();
    }
}
