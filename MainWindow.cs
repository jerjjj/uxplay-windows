using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
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
    private bool _ready;

    public MainWindow()
    {
        Title = "UxPlay Client";

        // ★ Mica 云母背景
        SystemBackdrop = new MicaBackdrop();

        // ★ 默认窗口尺寸
        AppWindow.Resize(new SizeInt32(1100, 720));

        // 页标题
        _headerText = new TextBlock
        {
            FontSize = 18,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(4, 0, 0, 0),
        };

        _nav = new NavigationView
        {
            PaneDisplayMode = NavigationViewPaneDisplayMode.Left,
            IsSettingsVisible = false,
            IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed,
            Header = _headerText,
        };

        _nav.MenuItems.Add(new NavigationViewItem { Content = "主页", Tag = "Home", Icon = new SymbolIcon(Symbol.Home) });
        _nav.MenuItems.Add(new NavigationViewItem { Content = "设置", Tag = "Settings", Icon = new SymbolIcon(Symbol.Setting) });
        _nav.MenuItems.Add(new NavigationViewItem { Content = "日志", Tag = "Log", Icon = new SymbolIcon(Symbol.List) });

        _nav.PaneFooter = new StackPanel
        {
            Padding = new Thickness(12, 8, 12, 8),
            Children = {
                new TextBlock { Text = "UxPlay Client", FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                    FontSize = 13, Opacity = 0.7 },
                new TextBlock { Text = "v1.0.0", FontSize = 11, Opacity = 0.4,
                    Margin = new Thickness(0, 2, 0, 0) },
            }
        };

        _nav.SelectionChanged += OnNavSelectionChanged;
        _nav.Loaded += OnNavLoaded;
        this.Closed += (_, _) =>
        {
            // 异步清理，不阻塞窗口关闭
            _ = Task.Run(() =>
            {
                try { _service?.Dispose(); } catch { }
                Environment.Exit(0);
            });
        };
        Content = _nav;
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
    }

    private void OnNavSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (!_ready) return;
        if (args.SelectedItem is NavigationViewItem item)
            ShowPage(item.Tag?.ToString() ?? "");
    }

    private void ShowPage(string tag)
    {
        (_nav.Content, _headerText.Text) = tag switch
        {
            "Home"     => ((object)new MainPage(_mainVm!), "主页"),
            "Settings" => (new SettingsPage(_settingsVm!), "设置"),
            "Log"      => (new LogPage(_mainVm!), "日志"),
            _          => (new MainPage(_mainVm!), "主页"),
        };
    }
}
