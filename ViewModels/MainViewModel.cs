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

    [ObservableProperty] string _statusText = "空闲";
    [ObservableProperty] string _statusColor = "Gray";
    [ObservableProperty] bool _isRunning, _isStarting;
    [ObservableProperty] int _connectionCount;
    [ObservableProperty] string _pinCode = "";
    [ObservableProperty] bool _pinVisible;
    [ObservableProperty] string _audioArtist = "", _audioTitle = "";
    [ObservableProperty] bool _audioMetaVisible;
    [ObservableProperty] double _volume = 1.0;
    [ObservableProperty] string _logText = "";
    [ObservableProperty] string _errorMessage = "";

    public ObservableCollection<string> ConnectedDevices { get; } = new();

    public MainViewModel(UxPlayService svc, DispatcherQueue dq)
    {
        _svc = svc;

        svc.StateChanged       += (_, s) => { ApplyState(s); if (!_userStopping && s == UxPlayState.Idle) { Log("投屏窗口已关闭，自动重启中…"); _ = AutoRestart(); } };
        svc.ClientConnected    += (_, _) => { ConnectionCount = svc.GetConnectionCount(); };
        svc.ClientDisconnected += (_, _) => { ConnectionCount = svc.GetConnectionCount(); if (ConnectionCount == 0) ConnectedDevices.Clear(); };
        svc.ClientInfo         += (_, c) => { ConnectedDevices.Clear(); ConnectedDevices.Add($"{c.name} ({c.model})"); Log($"{L10n.Get("log.device_connected")}: {c.name} ({c.model}) ID={c.id}"); };
        svc.PinDisplayed       += (_, p) => { PinCode = p; PinVisible = true; Log($"{L10n.Get("log.pin")}: {p}"); };
        svc.MirrorStarted      += (_, _) => Log(L10n.Get("log.mirror_started"));
        svc.MirrorStopped      += (_, _) => Log(L10n.Get("log.mirror_stopped"));
        svc.AudioMetadata      += (_, m) => { AudioArtist = m.artist ?? ""; AudioTitle = m.title ?? ""; AudioMetaVisible = !string.IsNullOrEmpty(m.title); };
        svc.ErrorOccurred      += (_, m) => { ErrorMessage = m; Log($"[ERROR] {m}"); };
        svc.LogMessage         += (_, m) => Log(m);
    }

    // ── 状态映射（单一来源） ──

    void ApplyState(UxPlayState s)
    {
        (StatusText, StatusColor, IsRunning, IsStarting) = s switch
        {
            UxPlayState.Idle     => ("空闲",     "Gray",   false, false),
            UxPlayState.Starting => ("启动中...", "Orange", false, true),
            UxPlayState.Running  => ("运行中",   "Green",  true,  false),
            UxPlayState.Stopping => ("停止中...", "Orange", false, true),
            UxPlayState.Error    => ("错误",     "Red",    false, false),
            _ => ("未知", "Gray", false, false),
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
        var cfg = s.ToNativeConfig();
        _svc.Configure(ref cfg);
        cfg.Dispose();
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
        catch (Exception ex) { ErrorMessage = $"启动失败: {ex.Message}"; Log($"[ERROR] {ex.Message}"); }
    }
    bool CanStart => !IsRunning && !IsStarting;

    [RelayCommand(CanExecute = nameof(CanStop))]
    async Task Stop()
    {
        _userStopping = true;
        try { await _svc.StopAsync(); ConnectionCount = 0; ConnectedDevices.Clear(); }
        catch (Exception ex) { ErrorMessage = $"停止失败: {ex.Message}"; }
    }
    bool CanStop => IsRunning || IsStarting;

    [RelayCommand(CanExecute = nameof(CanStop))]
    async Task Restart()
    {
        _userStopping = true;
        Log("正在重启投屏…");
        try
        {
            await _svc.StopAsync();
            ConnectionCount = 0; ConnectedDevices.Clear();
            await Task.Delay(500);
            _userStopping = false;
            await DoStart();
            Log("投屏已重启，新设置已生效");
        }
        catch (Exception ex) { ErrorMessage = $"重启失败: {ex.Message}"; Log($"[ERROR] {ex.Message}"); }
    }

    async Task AutoRestart()
    {
        try
        {
            await Task.Delay(1000);
            if (_userStopping || IsRunning) return;
            await DoStart();
            Log("自动重启完成，等待新连接");
        }
        catch (Exception ex) { Log($"[WARN] 自动重启失败: {ex.Message}"); }
    }

    [RelayCommand] void DisconnectAll()
    {
        try { _svc.DisconnectClients(); ConnectedDevices.Clear(); ConnectionCount = 0; Log("已断开所有客户端"); }
        catch (Exception ex) { Log($"[ERROR] {ex.Message}"); }
    }

    [RelayCommand] void VolumeChanged() { try { _svc.SetVolume(Volume); } catch { } }
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
