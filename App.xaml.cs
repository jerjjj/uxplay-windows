using System.IO;
using Microsoft.UI.Xaml;
using UxPlayClient.Services;

namespace UxPlayClient;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        L10n.LoadSavedLanguage();

        // Apply theme at STARTUP only (API doesn't support runtime changes)
        var t = L10n.Theme;
        UI.IsDark = t == AppTheme.Dark;
        if (t == AppTheme.Light)
            RequestedTheme = ApplicationTheme.Light;
        else if (t == AppTheme.Dark)
            RequestedTheme = ApplicationTheme.Dark;
        // System: don't touch — WinUI follows OS
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libuxplaylib.dll");
        if (!File.Exists(dllPath))
            System.Diagnostics.Debug.WriteLine($"[WARN] libuxplaylib.dll not found at {dllPath}");

        var window = new MainWindow();
        _window = window;
        window.Activate();
    }

    private Window? _window;
}
