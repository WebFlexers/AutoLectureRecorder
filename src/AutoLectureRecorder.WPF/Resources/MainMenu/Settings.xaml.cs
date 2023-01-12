using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutoLectureRecorder.Resources.MainMenu;

public partial class Settings : UserControl
{
    public Settings()
    {
        InitializeComponent();
    }

    public SolidColorBrush FillColor
    {
        get => (SolidColorBrush)GetValue(FillColorProperty); 
        set => SetValue(FillColorProperty, value);
    }

    public static readonly DependencyProperty FillColorProperty =
        DependencyProperty.Register(nameof(FillColor), typeof(SolidColorBrush), typeof(Settings), 
            new PropertyMetadata(new SolidColorBrush(), OnFillColorChanged));

    private static void OnFillColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var settings = d as Settings;
        settings!.OnFillColorChanged(e);
    }

    private void OnFillColorChanged(DependencyPropertyChangedEventArgs e)
    {
        var newColor = e.NewValue as SolidColorBrush;
        this.ShapePath.Fill = newColor;
        this.Foreground = newColor;
    }
}
