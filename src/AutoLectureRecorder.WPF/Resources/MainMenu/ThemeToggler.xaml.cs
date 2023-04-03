using System.Windows;
using System.Windows.Controls;
using AutoLectureRecorder.Resources.Themes;

namespace AutoLectureRecorder.Resources.MainMenu;

public partial class ThemeToggler : UserControl
{
    public ThemeToggler()
    {
        InitializeComponent();
    }

    private void ToggleButton_Checked(object sender, RoutedEventArgs e)
    {
        ThemeManager.SwitchToLightTheme(Application.Current.MainWindow!);
    }

    private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
    {
        ThemeManager.SwitchToDarkTheme(Application.Current.MainWindow!);
    }
}
