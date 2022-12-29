using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutoLectureRecorder.WPF.Resources.MainMenu;

public partial class Schedule : UserControl
{
    public Schedule()
    {
        InitializeComponent();
    }

    public SolidColorBrush FillColor
    {
        get { return (SolidColorBrush)GetValue(FillColorProperty); }
        set { SetValue(FillColorProperty, value); }
    }

    public static readonly DependencyProperty FillColorProperty =
        DependencyProperty.Register("FillColor", typeof(SolidColorBrush), typeof(Schedule), new PropertyMetadata(
            new SolidColorBrush(), new PropertyChangedCallback(OnFillColorChanged)));

    private static void OnFillColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Schedule schedule = d as Schedule;
        schedule.OnFillColorChanged(e);
    }

    private void OnFillColorChanged(DependencyPropertyChangedEventArgs e)
    {
        var newColor = e.NewValue as SolidColorBrush;
        this.shapePath.Fill = newColor;
        this.Foreground = newColor;
    }
}
