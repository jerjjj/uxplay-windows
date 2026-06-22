using System.IO;
using Microsoft.UI.Xaml;
using UxPlayClient.Services;

namespace UxPlayClient;

public partial class App : Application
{
    private Window? _window;

    public App()
    {
        InitializeComponent();
        L10n.LoadSavedLanguage();
        ApplyTheme();
        L10n.ThemeChanged += t => ApplyTheme();
    }

    void ApplyTheme()
    {
        RequestedTheme = L10n.Theme switch
        {
            AppTheme.Light  => ApplicationTheme.Light,
            AppTheme.Dark   => ApplicationTheme.Dark,
            _               => ApplicationTheme.Dark // still respect system; Framework handles switch
        };
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libuxplaylib.dll");
        if (!File.Exists(dllPath))
            System.Diagnostics.Debug.WriteLine($"[WARN] libuxplaylib.dll not found at {dllPath}");

        _window = new MainWindow();
        _window.Activate();
    }
}
