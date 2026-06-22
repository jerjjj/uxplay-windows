using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UxPlayClient.Services;
using UxPlayClient.ViewModels;

namespace UxPlayClient.Pages;

public class LogPage : Page
{
    public LogPage(MainViewModel vm)
    {
        var logContent = new TextBlock
        {
            Text = vm.LogText,
            Foreground = UI.Res("TextFillColorPrimaryBrush"),
            FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Cascadia Mono, Consolas"),
            FontSize = 12.5,
            TextWrapping = TextWrapping.Wrap,
            IsTextSelectionEnabled = true,
        };

        var clearBtn = UI.Btn(L10n.Get("log.clear"));
        clearBtn.Command = vm.ClearLogCommand;
        clearBtn.HorizontalAlignment = HorizontalAlignment.Right;
        clearBtn.Margin = new Thickness(0, 12, 0, 0);

        var root = new Grid { Padding = new Thickness(28, 20, 28, 20) };
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        // 标题行
        Grid.SetRow(clearBtn, 0);
        root.Children.Add(clearBtn);

        // 日志区域 — 使用 theme resource 背景
        var logBorder = new Border
        {
            Background = UI.Res("LayerFillColorDefaultBrush"),
            BorderBrush = UI.Res("CardStrokeColorDefaultBrush"),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(16),
            Margin = new Thickness(0, 8, 0, 0),
            Child = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = logContent,
            },
        };
        Grid.SetRow(logBorder, 1);
        root.Children.Add(logBorder);

        Content = root;

        vm.PropertyChanged += (_, a) =>
        {
            if (a.PropertyName == nameof(vm.LogText))
                DispatcherQueue.TryEnqueue(() => logContent.Text = vm.LogText);
        };
    }
}
