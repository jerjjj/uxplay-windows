using System.IO;
using Microsoft.UI.Xaml;

namespace UxPlayClient;

public partial class App : Application
{
    private Window? _window;

    public App()
    {
        this.InitializeComponent();
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
