﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutoLectureRecorder.Common.CustomControls.MainMenu;

public partial class Library : UserControl
{
    public Library()
    {
        InitializeComponent();
    }

    public SolidColorBrush FillColor
    {
        get => (SolidColorBrush)GetValue(FillColorProperty); 
        set => SetValue(FillColorProperty, value);
    }

    public static readonly DependencyProperty FillColorProperty =
        DependencyProperty.Register(nameof(FillColor), typeof(SolidColorBrush), typeof(Library), new PropertyMetadata(
            new SolidColorBrush(), OnFillColorChanged));

    private static void OnFillColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var library = d as Library;
        library!.OnFillColorChanged(e);
    }

    private void OnFillColorChanged(DependencyPropertyChangedEventArgs e)
    {
        var newColor = e.NewValue as SolidColorBrush;
        this.ShapePath.Fill = newColor;
        this.Foreground = newColor;
    }
}
