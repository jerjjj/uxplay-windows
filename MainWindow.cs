using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UxPlayClient.Pages;
using UxPlayClient.Services;
using UxPlayClient.ViewModels;
using Windows.Graphics;

namespace UxPlayClient;

public class MainWindow : Window
{
    private UxPlayService? _service;
    private MainViewModel? _mainVm;
    private SettingsViewModel? _settingsVm;
    private readonly NavigationView _nav;
    private readonly TextBlock _headerText;
    private readonly TextBlock _footerTitle, _footerVersion;
    private bool _ready;
    private string _currentTag = "Home";

    public MainWindow()
    {
        Title = "UxPlay Client";
        SystemBackdrop = null;
        AppWindow.Resize(new SizeInt32(1100, 720));
        SetIcon();

        _headerText = new TextBlock { FontSize = 18, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(4, 0, 0, 0) };

        _nav = new NavigationView { PaneDisplayMode = NavigationViewPaneDisplayMode.Left, IsSettingsVisible = false, IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed, Header = _headerText };
        _nav.MenuItems.Add(MenuItem("nav.home", "Home", Symbol.Home));
        _nav.MenuItems.Add(MenuItem("nav.settings", "Settings", Symbol.Setting));
        _nav.MenuItems.Add(MenuItem("nav.log", "Log", Symbol.List));

        _footerTitle = new TextBlock { Text = L10n.Get("app.title"), FontWeight = Microsoft.UI.Text.FontWeights.Bold, FontSize = 13, Opacity = 0.7 };
        _footerVersion = new TextBlock { Text = L10n.Get("app.version"), FontSize = 11, Opacity = 0.4, Margin = new Thickness(0, 2, 0, 0) };
        _nav.PaneFooter = new StackPanel { Padding = new Thickness(12, 8, 12, 8), Children = { _footerTitle, _footerVersion } };

        _nav.SelectionChanged += OnNavSelectionChanged;
        _nav.Loaded += OnNavLoaded;
        this.Closed += (_, _) => { _ = Task.Run(() => { try { _service?.Dispose(); } catch { } Environment.Exit(0); }); };
        Content = _nav;

        // Apply theme
        L10n.ThemeChanged += async t =>
        {
            var dlg = new ContentDialog
            {
                Title = L10n.Get("theme.restart_title"),
                Content = L10n.Get("theme.restart_msg"),
                PrimaryButtonText = L10n.Get("theme.restart_now"),
                CloseButtonText = L10n.Get("theme.restart_later"),
                XamlRoot = _nav.XamlRoot,
            };
            var result = await dlg.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
            }
        };
    }

    static NavigationViewItem MenuItem(string key, string tag, Symbol icon) =>
        new() { Content = L10n.Get(key), Tag = tag, Icon = new SymbolIcon(icon) };

    void SetIcon()
    {
        try
        {
            var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appicon.ico");
            if (System.IO.File.Exists(path))
                AppWindow.SetIcon(path);
        }
        catch { /* icon is cosmetic */ }
    }

    private void OnNavLoaded(object sender, RoutedEventArgs e)
    {
        _service = new UxPlayService(DispatcherQueue);
        _mainVm = new MainViewModel(_service, DispatcherQueue);
        _settingsVm = new SettingsViewModel();
        _settingsVm.LoadSettings();

        _ready = true;

        ShowPage("Home");
        _nav.SelectedItem = _nav.MenuItems[0];

        L10n.LanguageChanged += RefreshAll;
    }

    private void OnNavSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (!_ready) return;
        if (args.SelectedItem is NavigationViewItem item && item.Tag is string tag) { _currentTag = tag; ShowPage(tag); }
    }

    private void ShowPage(string tag)
    {
        _nav.Content = tag switch { "Home" => new MainPage(_mainVm!), "Settings" => new SettingsPage(_settingsVm!), "Log" => new LogPage(_mainVm!), _ => new MainPage(_mainVm!) };
        _headerText.Text = L10n.Get($"heading.{tag.ToLower()}");
    }

    void RefreshAll()
    {
        _footerTitle.Text = L10n.Get("app.title");
        _footerVersion.Text = L10n.Get("app.version");
        foreach (NavigationViewItem item in _nav.MenuItems)
        {
            var key = ((string)item.Tag) switch { "Home" => "nav.home", "Settings" => "nav.settings", "Log" => "nav.log", _ => null };
            if (key != null) item.Content = L10n.Get(key);
        }
        ShowPage(_currentTag);
    }
}
