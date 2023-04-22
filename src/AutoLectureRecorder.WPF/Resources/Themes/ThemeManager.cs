using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using AutoLectureRecorder.ReactiveUiUtilities;
using MaterialDesignThemes.Wpf;
using ReactiveUI;

namespace AutoLectureRecorder.Resources.Themes;

public static class ThemeManager
{
    [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
    public static extern bool ShouldSystemUseDarkMode();

    public static ColorTheme CurrentColorTheme { get; set; } = ColorTheme.Light;

    //public static void InitializeTheme()
    //{
    //    if (ShouldSystemUseDarkMode())
    //    {
    //        SwitchToDarkTheme();
    //    }
    //    else
    //    {
    //        SwitchToLightTheme();
    //    }
    //}

    public static void SwitchToLightTheme(ICollection<DependencyObject> windows)
    {
        CurrentColorTheme = ColorTheme.Light;
        
        SetMaterialDesignTheme();

        AppThemeSelector.SetCurrentThemeDictionary(windows, new Uri(
            $"/{Assembly.GetEntryAssembly()!.GetName().Name};component/Resources/Themes/LightTheme.xaml",
            UriKind.RelativeOrAbsolute));

        MessageBus.Current.SendMessage(Unit.Default, PubSubMessages.UpdateTheme);
    }

    public static void SwitchToDarkTheme(ICollection<DependencyObject> windows)
    {
        CurrentColorTheme = ColorTheme.Dark;
        
        SetMaterialDesignTheme();

        AppThemeSelector.SetCurrentThemeDictionary(windows, new Uri(
            $"/{Assembly.GetEntryAssembly()!.GetName().Name};component/Resources/Themes/DarkTheme.xaml",
            UriKind.RelativeOrAbsolute));

        MessageBus.Current.SendMessage(Unit.Default, PubSubMessages.UpdateTheme);
    }

    private static void SetMaterialDesignTheme()
    {
        var palette = new PaletteHelper();
        var colors = App.GetCurrentThemeDictionary();
        var primaryColor = (Color)colors["PrimaryColor"];
        var secondaryColor = (Color)colors["SecondaryTextColor"];

        Theme theme;

        if (CurrentColorTheme == ColorTheme.Light)
        {
            theme = Theme.Create(Theme.Light, primaryColor, secondaryColor);
        }
        else
        {
            theme = Theme.Create(Theme.Dark, primaryColor, secondaryColor);
        }

        palette.SetTheme(theme);
    }
}
