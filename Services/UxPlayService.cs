using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using UxPlayClient.Interop;

namespace UxPlayClient.Services;

public sealed class UxPlayService : IDisposable
{
    private readonly DispatcherQueue _dq;
    private IntPtr _h;
    private UxPlayEventCallback? _eCb;
    private UxPlayLogCallback?   _lCb;

    public event EventHandler<UxPlayState>? StateChanged;
    public event EventHandler? ClientConnected;
    public event EventHandler? ClientDisconnected;
    public event EventHandler<(string? name, string? model, string? id)>? ClientInfo;
    public event EventHandler<string>? PinDisplayed;
    public event EventHandler? MirrorStarted;
    public event EventHandler? MirrorStopped;
    public event EventHandler<(string? artist, string? title, string? album)>? AudioMetadata;
    public event EventHandler<string>? ErrorOccurred;
    public event EventHandler<string>? LogMessage;

    public UxPlayService(DispatcherQueue dq) => _dq = dq;

    public void Create()
    {
        if (_h != IntPtr.Zero) return;
        try
        {
            var err = UxPlayNative.uxplay_create(out _h);
            if (err != UxPlayError.Ok) throw new InvalidOperationException($"uxplay_create: {err}");
        }
        catch (DllNotFoundException)
        {
            throw new DllNotFoundException(L10n.Get("log.dll_not_found"));
        }
        _eCb = OnEvent; _lCb = OnLog;
        UxPlayNative.uxplay_set_event_callback(_h, _eCb, IntPtr.Zero);
        UxPlayNative.uxplay_set_log_callback(_h, _lCb, IntPtr.Zero);
    }

    public void Configure(ref UxPlayConfig cfg)
    {
        if (_h == IntPtr.Zero) throw new InvalidOperationException("Call Create() first");
        var err = UxPlayNative.uxplay_configure(_h, ref cfg);
        if (err != UxPlayError.Ok) throw new InvalidOperationException($"uxplay_configure: {err}");
    }

    public Task StartAsync()
    {
        if (_h == IntPtr.Zero) throw new InvalidOperationException("Call Create() first");
        return Task.Run(() =>
        {
            try
            {
                var err = UxPlayNative.uxplay_start(_h);
                if (err != UxPlayError.Ok)
                    _dq.TryEnqueue(() => ErrorOccurred?.Invoke(this, $"uxplay_start: {err}"));
            }
            catch (Exception ex)
            {
                _dq.TryEnqueue(() => ErrorOccurred?.Invoke(this, ex.Message));
            }
        });
    }

    /// <summary>
    /// 停止并销毁原生实例。下次 Start 前会自动重新 Create。
    /// </summary>
    public async Task StopAsync()
    {
        var h = _h;
        if (h == IntPtr.Zero) return;

        // ① 先摘回调，阻断清理期间的竞态回调
        _h = IntPtr.Zero;
        try { UxPlayNative.uxplay_set_event_callback(h, null, IntPtr.Zero); } catch { }
        try { UxPlayNative.uxplay_set_log_callback(h, null, IntPtr.Zero); } catch { }

        // ② 在后台线程销毁（内部会先 stop 再 destroy）
        await Task.Run(() =>
        {
            try { UxPlayNative.uxplay_destroy(h); }
            catch { /* guard against native crash during GStreamer teardown */ }
        });

        _eCb = null; _lCb = null;

        // ③ 手动通知 UI
        _dq.TryEnqueue(() => StateChanged?.Invoke(this, UxPlayState.Idle));
    }

    public UxPlayState GetState()           => _h != IntPtr.Zero ? UxPlayNative.uxplay_get_state(_h)            : UxPlayState.Idle;
    public int         GetConnectionCount() => _h != IntPtr.Zero ? UxPlayNative.uxplay_get_connection_count(_h) : 0;
    public void        DisconnectClients()  { if (_h != IntPtr.Zero) UxPlayNative.uxplay_disconnect_clients(_h); }
    public void        SetVolume(double v)  { if (_h != IntPtr.Zero) UxPlayNative.uxplay_set_volume(_h, v); }

    private void OnEvent(IntPtr ud, ref UxPlayEventData d)
    {
        if (_h == IntPtr.Zero) return; // 已销毁，忽略残留回调
        var type = d.Type;
        switch (type)
        {
            case UxPlayEventType.StateChanged:
                var st = d.State;
                _dq.TryEnqueue(() => StateChanged?.Invoke(this, st));
                break;
            case UxPlayEventType.ClientConnected:
                var cn = d.Client;
                _dq.TryEnqueue(() => { ClientConnected?.Invoke(this, EventArgs.Empty); ClientInfo?.Invoke(this, (cn.DeviceName, cn.DeviceModel, cn.DeviceId)); });
                break;
            case UxPlayEventType.ClientDisconnected:
                _dq.TryEnqueue(() => ClientDisconnected?.Invoke(this, EventArgs.Empty));
                break;
            case UxPlayEventType.DisplayPin:
                var pin = d.Pin; if (pin != null) _dq.TryEnqueue(() => PinDisplayed?.Invoke(this, pin));
                break;
            case UxPlayEventType.MirrorStarted:
                _dq.TryEnqueue(() => MirrorStarted?.Invoke(this, EventArgs.Empty));
                break;
            case UxPlayEventType.MirrorStopped:
                _dq.TryEnqueue(() => MirrorStopped?.Invoke(this, EventArgs.Empty));
                break;
            case UxPlayEventType.AudioMetadata:
                var m = d.AudioMeta;
                _dq.TryEnqueue(() => AudioMetadata?.Invoke(this, (m.Artist, m.Title, m.Album)));
                break;
            case UxPlayEventType.Error:
                var msg = d.ErrorMsg ?? "Unknown";
                _dq.TryEnqueue(() => ErrorOccurred?.Invoke(this, msg));
                break;
        }
    }

    private void OnLog(IntPtr ud, UxPlayLogLevel lv, string message)
    {
        if (_h == IntPtr.Zero) return;
        var prefix = lv switch { UxPlayLogLevel.Error => "ERROR", UxPlayLogLevel.Warning => "WARN", UxPlayLogLevel.Info => "INFO", _ => "DEBUG" };
        _dq.TryEnqueue(() => LogMessage?.Invoke(this, $"[{prefix}] {message}"));
    }

    public void Dispose()
    {
        var h = _h; _h = IntPtr.Zero;
        if (h != IntPtr.Zero)
        {
            try { UxPlayNative.uxplay_set_event_callback(h, null, IntPtr.Zero); } catch { }
            try { UxPlayNative.uxplay_set_log_callback(h, null, IntPtr.Zero); } catch { }
            try { UxPlayNative.uxplay_destroy(h); } catch { }
        }
        _eCb = null; _lCb = null;
    }
}
