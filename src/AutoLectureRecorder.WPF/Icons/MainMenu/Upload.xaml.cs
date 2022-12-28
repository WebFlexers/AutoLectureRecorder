﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutoLectureRecorder.WPF.Icons.MainMenu;

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
        DependencyProperty.Register("FillColor", typeof(SolidColorBrush), typeof(Dashboard), new PropertyMetadata(
            new SolidColorBrush(), new PropertyChangedCallback(OnFillColorChanged)));

    private static void OnFillColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Upload dashboard = d as Upload;
        dashboard.OnFillColorChanged(e);
    }

    private void OnFillColorChanged(DependencyPropertyChangedEventArgs e)
    {
        this.shapePath.Fill = e.NewValue as SolidColorBrush;
    }
}