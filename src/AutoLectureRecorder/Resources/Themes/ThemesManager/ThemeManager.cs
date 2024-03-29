﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace AutoLectureRecorder.Resources.Themes.ThemesManager;

public class ThemeManager : IThemeManager
{
    [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
    public static extern bool ShouldSystemUseDarkMode();

    private static Window[] AllActiveWindows => System.Windows.Application.Current.Windows.Cast<Window>().ToArray();
    public ColorTheme CurrentColorTheme { get; set; } = ColorTheme.Light;

    public void InitializeTheme()
    {
        if (ShouldSystemUseDarkMode())
        {
            SwitchToDarkTheme(AllActiveWindows);
        }
        else
        {
            SwitchToLightTheme(AllActiveWindows);
        }
    }

    /// <summary>
    /// Reapplies the current theme in case it was not applied to specific elements
    /// </summary>
    public void RefreshTheme()
    {
        switch (CurrentColorTheme)
        {
            case ColorTheme.Light:
                SwitchToLightTheme(AllActiveWindows);
                break;
            case ColorTheme.Dark:
                SwitchToDarkTheme(AllActiveWindows);
                break;
        }
    }

    public void SwitchToLightTheme(ICollection<DependencyObject> windows)
    {
        CurrentColorTheme = ColorTheme.Light;
        
        SetMaterialDesignTheme();

        AppThemeSelector.SetCurrentThemeDictionary(windows, new Uri(
            $"/{Assembly.GetEntryAssembly()!.GetName().Name};component/Resources/Themes/LightTheme.xaml",
            UriKind.RelativeOrAbsolute));

        // TODO: See what we are going to do with this
        // MessageBus.Current.SendMessage(Unit.Default, PubSubMessages.UpdateTheme);
    }

    public void SwitchToDarkTheme(ICollection<DependencyObject> windows)
    {
        CurrentColorTheme = ColorTheme.Dark;
        
        SetMaterialDesignTheme();

        AppThemeSelector.SetCurrentThemeDictionary(windows, new Uri(
            $"/{Assembly.GetEntryAssembly()!.GetName().Name};component/Resources/Themes/DarkTheme.xaml",
            UriKind.RelativeOrAbsolute));

        // TODO: See what we are going to do with this
        // MessageBus.Current.SendMessage(Unit.Default, PubSubMessages.UpdateTheme);
    }

    private void SetMaterialDesignTheme()
    {
        var palette = new PaletteHelper();
        var colors = GetCurrentThemeDictionary();
        var primaryColor = (Color)colors["PrimaryColor"];
        var secondaryColor = (Color)colors["SecondaryColor"];

        var theme = Theme.Create(CurrentColorTheme == ColorTheme.Light 
            ? Theme.Light 
            : Theme.Dark, primaryColor, secondaryColor);

        palette.SetTheme(theme);
    }

    private static ResourceDictionary? _resourceDictionary;
    private static ColorTheme? _currentDictionaryTheme;

    public ResourceDictionary GetCurrentThemeDictionary()
    {
        if (_currentDictionaryTheme == CurrentColorTheme)
        {
            return _resourceDictionary!;
        }

        _currentDictionaryTheme ??= CurrentColorTheme;

        if (CurrentColorTheme == ColorTheme.Light)
        {
            _resourceDictionary = new ResourceDictionary
            {
                Source = new Uri(
                    $"/{Assembly.GetEntryAssembly()!.GetName().Name};component/Resources/Themes/LightTheme.xaml",
                    UriKind.RelativeOrAbsolute)
            };
        }
        else
        {
            _resourceDictionary = new ResourceDictionary
            {
                Source = new Uri(
                    $"/{Assembly.GetEntryAssembly()!.GetName().Name};component/Resources/Themes/DarkTheme.xaml",
                    UriKind.RelativeOrAbsolute)
            };
        }

        return _resourceDictionary;
    }
}
