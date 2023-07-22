using System.Collections.Generic;
using System.Windows;

namespace AutoLectureRecorder.Resources.Themes.ThemesManager;

public interface IThemeManager
{
    ColorTheme CurrentColorTheme { get; set; }

    /// <summary>
    /// Reapplies the current theme in case it was not applied to specific elements
    /// </summary>
    void RefreshTheme();

    void SwitchToLightTheme(ICollection<DependencyObject> windows);
    void SwitchToDarkTheme(ICollection<DependencyObject> windows);
    ResourceDictionary GetCurrentThemeDictionary();
}