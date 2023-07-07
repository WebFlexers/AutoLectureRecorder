using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AutoLectureRecorder.Resources.Themes.ThemesManager;
using Splat;

namespace AutoLectureRecorder.Common.CustomControls;

public partial class ThemeToggler : UserControl
{
    private readonly IThemeManager? _themeManager;

    public ThemeToggler()
    {
        _themeManager = Locator.Current.GetService<IThemeManager>();
        InitializeComponent();
    }

    private void ToggleButton_Checked(object sender, RoutedEventArgs e)
    {
        _themeManager?.SwitchToLightTheme(System.Windows.Application.Current.Windows.Cast<Window>().ToArray());
    }

    private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
    {
        _themeManager?.SwitchToDarkTheme(System.Windows.Application.Current.Windows.Cast<Window>().ToArray());
    }
}
