using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI;
using Windows.UI;

namespace UxPlayClient;

/// <summary>
/// UI 工厂 — 所有颜色取自 WinUI ThemeResource，自动适配亮色/暗色主题。
/// </summary>
static class UI
{
    // ─── Theme Resource 取色 ────────────────────────────────

    public static Brush Res(string key) =>
        Application.Current.Resources.TryGetValue(key, out var v) && v is Brush b
            ? b : new SolidColorBrush(Colors.Transparent);

    // ─── Card ───────────────────────────────────────────────

    public static Border Card(params UIElement[] children)
    {
        var stack = new StackPanel { Spacing = 12 };
        foreach (var c in children) stack.Children.Add(c);
        return new Border
        {
            Background = Res("CardBackgroundFillColorDefaultBrush"),
            BorderBrush = Res("CardStrokeColorDefaultBrush"),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(20),
            Child = stack,
        };
    }

    // ─── Text ───────────────────────────────────────────────

    public static TextBlock Title(string text) =>
        new()
        {
            Text = text,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            FontSize = 16,
            Foreground = Res("TextFillColorPrimaryBrush"),
        };

    public static TextBlock Label(string text) =>
        new()
        {
            Text = text,
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = Res("TextFillColorPrimaryBrush"),
        };

    public static TextBlock Caption(string text) =>
        new()
        {
            Text = text,
            FontSize = 12,
            Foreground = Res("TextFillColorSecondaryBrush"),
        };

    // ─── Button ─────────────────────────────────────────────

    /// <summary>主操作按钮（Accent 样式）</summary>
    public static Button AccentBtn(string text)
    {
        var btn = new Button
        {
            Content = text,
            Padding = new Thickness(20, 8, 20, 8),
            CornerRadius = new CornerRadius(6),
        };
        if (Application.Current.Resources.TryGetValue("AccentButtonStyle", out var s) && s is Style style)
            btn.Style = style;
        return btn;
    }

    /// <summary>普通按钮（跟随系统主题）</summary>
    public static Button Btn(string text)
    {
        return new Button
        {
            Content = text,
            Padding = new Thickness(20, 8, 20, 8),
            CornerRadius = new CornerRadius(6),
        };
    }

    // ─── StatusDot ──────────────────────────────────────────

    public static Ellipse StatusDot(string state)
    {
        var color = state switch
        {
            "Green"  => ColorHelper.FromArgb(255, 16, 185, 129),   // Emerald-500
            "Orange" => ColorHelper.FromArgb(255, 245, 158, 11),   // Amber-500
            "Red"    => ColorHelper.FromArgb(255, 239, 68, 68),    // Red-500
            _        => ColorHelper.FromArgb(255, 156, 163, 175),  // Gray-400
        };
        return new Ellipse
        {
            Width = 12, Height = 12,
            Fill = new SolidColorBrush(color),
            VerticalAlignment = VerticalAlignment.Center,
        };
    }

    // ─── InfoBar ────────────────────────────────────────────

    public static InfoBar MakeInfoBar(InfoBarSeverity severity = InfoBarSeverity.Error)
    {
        return new InfoBar
        {
            Severity = severity,
            IsOpen = false,
            IsClosable = true,
            Margin = new Thickness(0, 4, 0, 4),
        };
    }

    // ─── Settings Grid ──────────────────────────────────────

    public static Grid SettingsGrid(int rows)
    {
        var g = new Grid { ColumnSpacing = 12, RowSpacing = 10 };
        g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140) });
        g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        for (int i = 0; i < rows; i++)
            g.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        return g;
    }

    public static (TextBlock label, T ctrl) Row<T>(Grid grid, int row, string label, T ctrl, string? desc = null) where T : FrameworkElement
    {
        var lbl = Label(label);
        Grid.SetRow(lbl, row); Grid.SetColumn(lbl, 0);
        grid.Children.Add(lbl);

        if (string.IsNullOrEmpty(desc))
        {
            Grid.SetRow(ctrl, row); Grid.SetColumn(ctrl, 1);
            grid.Children.Add(ctrl);
        }
        else
        {
            var stack = new StackPanel { Spacing = 2 };
            stack.Children.Add(ctrl);
            stack.Children.Add(Caption(desc));
            Grid.SetRow(stack, row); Grid.SetColumn(stack, 1);
            grid.Children.Add(stack);
        }
        return (lbl, ctrl);
    }
}
