using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
using UxPlayClient.Services;
using UxPlayClient.ViewModels;

namespace UxPlayClient.Pages;

public class MainPage : Page
{
    private readonly MainViewModel _vm;

    private readonly TextBlock _statusText = new() { FontSize = 18, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold };
    private readonly TextBlock _connText;
    private readonly Ellipse _statusDot;
    private readonly ProgressRing _statusRing;
    private readonly Button _startBtn, _stopBtn, _disconnBtn;
    private readonly Border _pinCard;
    private readonly TextBlock _pinCodeText;
    private readonly Border _audioCard;
    private readonly TextBlock _audioTitleText, _audioArtistText;
    private readonly Border _videoSizeCard;
    private readonly TextBlock _videoSizeText;
    private readonly ListView _deviceList;
    private readonly TextBlock _noDeviceText;
    private readonly InfoBar _errorBar;

    public MainPage(MainViewModel vm)
    {
        _vm = vm;

        // ── 状态指示 ──
        _statusDot = UI.StatusDot("Gray");
        _statusRing = new ProgressRing { IsActive = false, Width = 16, Height = 16, Visibility = Visibility.Collapsed };
        _connText = UI.Caption("");

        var statusRow = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10 };
        statusRow.Children.Add(_statusDot);
        statusRow.Children.Add(_statusRing);
        statusRow.Children.Add(_statusText);

        // ── 按钮 ──
        _startBtn = UI.AccentBtn(L10n.Get("ctrl.start"));
        _startBtn.Command = vm.StartCommand;
        _stopBtn = UI.Btn(L10n.Get("ctrl.stop"));
        _stopBtn.Command = vm.StopCommand;
        var restartBtn = UI.Btn(L10n.Get("ctrl.restart"));
        restartBtn.Command = vm.RestartCommand;
        _disconnBtn = UI.Btn(L10n.Get("ctrl.disconnect"));
        _disconnBtn.Command = vm.DisconnectAllCommand;

        var btns = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        btns.Children.Add(_startBtn);
        btns.Children.Add(_stopBtn);
        btns.Children.Add(restartBtn);
        btns.Children.Add(_disconnBtn);

        // ── PIN ──
        _pinCodeText = new TextBlock
        {
            FontSize = 42,
            FontWeight = Microsoft.UI.Text.FontWeights.Bold,
            Foreground = UI.AccentText,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 8, 0, 8),
            CharacterSpacing = 200,
        };
        _pinCard = UI.Card(UI.Title(L10n.Get("pin.title")), _pinCodeText,
            new TextBlock { Text = L10n.Get("pin.hint"),
                Foreground = UI.TextSecondary,
                HorizontalAlignment = HorizontalAlignment.Center });
        _pinCard.Visibility = Visibility.Collapsed;

        // ── 音频 ──
        _audioTitleText = new TextBlock { FontSize = 16, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold };
        _audioArtistText = new TextBlock { Foreground = UI.TextSecondary };
        _audioCard = UI.Card(UI.Title(L10n.Get("audio.title")), _audioTitleText, _audioArtistText);
        _audioCard.Visibility = Visibility.Collapsed;

        // ── 视频尺寸 ──
        _videoSizeText = new TextBlock { FontSize = 14, Foreground = UI.TextSecondary };
        _videoSizeCard = UI.Card(UI.Title(L10n.Get("video.size")), _videoSizeText);
        _videoSizeCard.Visibility = Visibility.Collapsed;

        // ── 设备列表 ──
        _deviceList = new ListView
        {
            MinHeight = 48, MaxHeight = 200,
            BorderThickness = new Thickness(0),
            ItemsSource = vm.ConnectedDevices,
            SelectionMode = ListViewSelectionMode.None,
        };
        _noDeviceText = new TextBlock
        {
            Text = L10n.Get("devices.none"),
            Foreground = UI.TextSecondary,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 12, 0, 12),
        };

        // ── 错误 InfoBar ──
        _errorBar = UI.MakeInfoBar(InfoBarSeverity.Error);
        _errorBar.Title = L10n.Get("error.title");

        // ── 组装 ──
        Content = new ScrollViewer
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Padding = new Thickness(28, 20, 28, 20),
            Content = new StackPanel
            {
                Spacing = 16, MaxWidth = 720, HorizontalAlignment = HorizontalAlignment.Center,
                Children =
                {
                    UI.Card(UI.Title(L10n.Get("status.title")), statusRow, _connText),
                    UI.Card(UI.Title(L10n.Get("ctrl.title")), btns),
                    _pinCard,
                    _audioCard,
                    UI.Card(UI.Title(L10n.Get("devices.title")), _deviceList, _noDeviceText),
                    _videoSizeCard,
                    _errorBar,
                }
            }
        };

        Refresh(null);
        vm.PropertyChanged += (_, e) => DispatcherQueue.TryEnqueue(() => Refresh(e.PropertyName));
    }

    private void Refresh(string? prop)
    {
        // 按属性选择性更新，避免每次全量刷新
        if (prop is null or nameof(MainViewModel.StatusText) or nameof(MainViewModel.StatusColor))
        {
            _statusText.Text = _vm.StatusText;
            var isStarting = _vm.StatusColor is "Orange";
            _statusRing.IsActive = isStarting;
            _statusRing.Visibility = isStarting ? Visibility.Visible : Visibility.Collapsed;
            _statusDot.Visibility = isStarting ? Visibility.Collapsed : Visibility.Visible;
            _statusDot.Fill = UI.StatusDot(_vm.StatusColor).Fill;
        }
        if (prop is null or nameof(MainViewModel.ConnectionCount))
            _connText.Text = $"{L10n.Get("status.connections")}  {_vm.ConnectionCount}";
        if (prop is null or nameof(MainViewModel.PinVisible) or nameof(MainViewModel.PinCode))
        {
            _pinCard.Visibility = _vm.PinVisible ? Visibility.Visible : Visibility.Collapsed;
            _pinCodeText.Text = _vm.PinCode;
        }
        if (prop is null or nameof(MainViewModel.AudioMetaVisible) or nameof(MainViewModel.AudioTitle) or nameof(MainViewModel.AudioArtist))
        {
            _audioCard.Visibility = _vm.AudioMetaVisible ? Visibility.Visible : Visibility.Collapsed;
            _audioTitleText.Text = _vm.AudioTitle;
            _audioArtistText.Text = _vm.AudioArtist;
        }
        if (prop is null or nameof(MainViewModel.ConnectionCount))
            _noDeviceText.Visibility = _vm.ConnectionCount == 0 ? Visibility.Visible : Visibility.Collapsed;
        if (prop is null or nameof(MainViewModel.VideoSizeVisible) or nameof(MainViewModel.VideoSizeInfo))
        {
            _videoSizeCard.Visibility = _vm.VideoSizeVisible ? Visibility.Visible : Visibility.Collapsed;
            _videoSizeText.Text = _vm.VideoSizeInfo;
        }
        if (prop is null or nameof(MainViewModel.ErrorMessage))
        {
            var hasErr = !string.IsNullOrEmpty(_vm.ErrorMessage);
            _errorBar.IsOpen = hasErr;
            if (hasErr) _errorBar.Message = _vm.ErrorMessage;
        }
    }
}
