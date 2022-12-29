using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutoLectureRecorder.WPF.Resources.MainMenu;

public partial class Upload : UserControl
{
    public Upload()
    {
        InitializeComponent();
    }

    public SolidColorBrush FillColor
    {
        get { return (SolidColorBrush)GetValue(FillColorProperty); }
        set { SetValue(FillColorProperty, value); }
    }

    public static readonly DependencyProperty FillColorProperty =
        DependencyProperty.Register("FillColor", typeof(SolidColorBrush), typeof(Upload), new PropertyMetadata(
            new SolidColorBrush(), new PropertyChangedCallback(OnFillColorChanged)));

    private static void OnFillColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Upload upload = d as Upload;
        upload.OnFillColorChanged(e);
    }

    private void OnFillColorChanged(DependencyPropertyChangedEventArgs e)
    {
        var newColor = e.NewValue as SolidColorBrush;
        this.shapePath.Fill = newColor;
        this.Foreground = newColor;
    }
}
