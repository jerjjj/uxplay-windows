using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI;

namespace UxPlayClient;

static class UI
{
    public static bool IsDark { get; set; }
    public static Action? ThemeChanged;

    // ── Cached brushes (light + dark pairs) ──
    static readonly SolidColorBrush _cBgL = Brush("#F9F9F9"), _cBgD = Brush("#2D2D2D");
    static readonly SolidColorBrush _cBdL = Brush("#E0E0E0"), _cBdD = Brush("#3D3D3D");
    static readonly SolidColorBrush _tPriL= Brush("#000000"), _tPriD= Brush("#FFFFFF");
    static readonly SolidColorBrush _tSecL= Brush("#666666"), _tSecD= Brush("#AAAAAA");
    static readonly SolidColorBrush _aTxtL= Brush("#005FB8"), _aTxtD= Brush("#60CDFF");
    static readonly SolidColorBrush _logL = Brush("#F5F5F5"), _logD = Brush("#1E1E1E");

    public static SolidColorBrush CardBg       => IsDark ? _cBgD : _cBgL;
    public static SolidColorBrush CardBorder   => IsDark ? _cBdD : _cBdL;
    public static SolidColorBrush TextPrimary  => IsDark ? _tPriD: _tPriL;
    public static SolidColorBrush TextSecondary=> IsDark ? _tSecD: _tSecL;
    public static SolidColorBrush AccentText   => IsDark ? _aTxtD: _aTxtL;
    public static SolidColorBrush LogBg        => IsDark ? _logD : _logL;

    static SolidColorBrush Brush(string hex)
    {
        hex = hex.TrimStart('#');
        byte a = 255, r, g, b;
        if (hex.Length == 8) { a = Convert.ToByte(hex[..2], 16); hex = hex[2..]; }
        r = Convert.ToByte(hex[..2], 16); g = Convert.ToByte(hex[2..4], 16); b = Convert.ToByte(hex[4..6], 16);
        return new SolidColorBrush(ColorHelper.FromArgb(a, r, g, b));
    }

    // ── Pre-computed status dot colors ──
    static readonly SolidColorBrush _dotGreen  = new(ColorHelper.FromArgb(255, 16, 185, 129));
    static readonly SolidColorBrush _dotOrange = new(ColorHelper.FromArgb(255, 245, 158, 11));
    static readonly SolidColorBrush _dotRed    = new(ColorHelper.FromArgb(255, 239, 68, 68));
    static readonly SolidColorBrush _dotGray   = new(ColorHelper.FromArgb(255, 156, 163, 175));

    // ── UI factory methods ──

    public static Border Card(params UIElement[] children)
    {
        var stack = new StackPanel { Spacing = 12 };
        foreach (var c in children) stack.Children.Add(c);
        return new Border { Background = CardBg, BorderBrush = CardBorder, BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(8), Padding = new Thickness(20), Child = stack };
    }

    public static TextBlock Title(string text) =>
        new() { Text = text, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold, FontSize = 16, Foreground = TextPrimary };

    public static TextBlock Label(string text) =>
        new() { Text = text, VerticalAlignment = VerticalAlignment.Center, Foreground = TextPrimary };

    public static TextBlock Caption(string text) =>
        new() { Text = text, FontSize = 12, Foreground = TextSecondary };

    public static Button AccentBtn(string text)
    {
        var btn = new Button { Content = text, Padding = new Thickness(20, 8, 20, 8), CornerRadius = new CornerRadius(6) };
        if (Application.Current.Resources.TryGetValue("AccentButtonStyle", out var s) && s is Style style)
            btn.Style = style;
        return btn;
    }

    public static Button Btn(string text) =>
        new() { Content = text, Padding = new Thickness(20, 8, 20, 8), CornerRadius = new CornerRadius(6) };

    public static Ellipse StatusDot(string state) =>
        new() { Width = 12, Height = 12, VerticalAlignment = VerticalAlignment.Center,
            Fill = state switch { "Green" => _dotGreen, "Orange" => _dotOrange, "Red" => _dotRed, _ => _dotGray } };

    public static InfoBar MakeInfoBar(InfoBarSeverity severity = InfoBarSeverity.Error) =>
        new() { Severity = severity, IsOpen = false, IsClosable = true, Margin = new Thickness(0, 4, 0, 4) };

    public static Grid SettingsGrid(int rows)
    {
        var g = new Grid { ColumnSpacing = 12, RowSpacing = 10 };
        g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140) });
        g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        for (int i = 0; i < rows; i++) g.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
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
