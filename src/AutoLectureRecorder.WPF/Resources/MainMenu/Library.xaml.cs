using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutoLectureRecorder.WPF.Resources.MainMenu;

public partial class Library : UserControl
{
    public Library()
    {
        InitializeComponent();
    }

    public SolidColorBrush FillColor
    {
        get { return (SolidColorBrush)GetValue(FillColorProperty); }
        set { SetValue(FillColorProperty, value); }
    }

    public static readonly DependencyProperty FillColorProperty =
        DependencyProperty.Register("FillColor", typeof(SolidColorBrush), typeof(Library), new PropertyMetadata(
            new SolidColorBrush(), new PropertyChangedCallback(OnFillColorChanged)));

    private static void OnFillColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Library library = d as Library;
        library.OnFillColorChanged(e);
    }

    private void OnFillColorChanged(DependencyPropertyChangedEventArgs e)
    {
        var newColor = e.NewValue as SolidColorBrush;
        this.shapePath.Fill = newColor;
        this.Foreground = newColor;
    }
}
