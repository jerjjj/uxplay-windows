using System;
using System.IO;
using Microsoft.UI.Xaml;
using UxPlayClient.Services;

namespace UxPlayClient;

public partial class App : Application
{
    private static Windows.UI.ViewManagement.UISettings? s_uiSettings;

    public App()
    {
        // Set GStreamer paths BEFORE WinUI/XAML init (needed for MSIX bundled plugins)
        SetGStreamerPaths();

        InitializeComponent();
        L10n.LoadSavedLanguage();

        var t = L10n.Theme;
        if (t == AppTheme.Light)      { RequestedTheme = ApplicationTheme.Light; UI.IsDark = false; }
        else if (t == AppTheme.Dark)  { RequestedTheme = ApplicationTheme.Dark;  UI.IsDark = true; }
        else                          { UI.IsDark = IsSystemDark(); } // System: follow actual OS theme

        // Listen for system theme changes (only relevant when theme == System)
        s_uiSettings = new Windows.UI.ViewManagement.UISettings();
        s_uiSettings.ColorValuesChanged += (_, _) =>
        {
            if (L10n.Theme == AppTheme.System)
            {
                var dark = IsSystemDark();
                if (UI.IsDark != dark)
                {
                    UI.IsDark = dark;
                    UI.ThemeChanged?.Invoke();
                }
            }
        };
    }

    /// <summary>Detect whether Windows is currently using dark mode.</summary>
    static bool IsSystemDark()
    {
        var settings = new Windows.UI.ViewManagement.UISettings();
        var bg = settings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Background);
        // Dark backgrounds have low RGB values
        return bg.R < 128 && bg.G < 128 && bg.B < 128;
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
