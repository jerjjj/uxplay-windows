using System;
using System.IO;
using Microsoft.UI.Xaml;
using UxPlayClient.Services;

namespace UxPlayClient;

public partial class App : Application
{
    public App()
    {
        // Set GStreamer paths BEFORE WinUI/XAML init (needed for MSIX bundled plugins)
        SetGStreamerPaths();

        InitializeComponent();
        L10n.LoadSavedLanguage();
        UI.IsDark = L10n.Theme == AppTheme.Dark;

        var t = L10n.Theme;
        if (t == AppTheme.Light)      RequestedTheme = ApplicationTheme.Light;
        else if (t == AppTheme.Dark)  RequestedTheme = ApplicationTheme.Dark;
    }

    static void SetGStreamerPaths()
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var pluginDir = Path.Combine(baseDir, "gstreamer-1.0");
        if (Directory.Exists(pluginDir))
        {
            Environment.SetEnvironmentVariable("GST_PLUGIN_PATH", pluginDir);
            Environment.SetEnvironmentVariable("GST_PLUGIN_SYSTEM_PATH", pluginDir);
        }
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libuxplaylib.dll");
        if (!File.Exists(dllPath))
            System.Diagnostics.Debug.WriteLine($"[WARN] libuxplaylib.dll not found at {dllPath}");

        var window = new MainWindow();
        window.Activate();
    }
}
