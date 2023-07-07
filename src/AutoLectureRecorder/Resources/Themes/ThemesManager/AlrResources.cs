using System;
using System.Reflection;
using System.Windows;

namespace AutoLectureRecorder.Resources.Themes.ThemesManager;

public static class AlrResources
{
    public static class ResourceDictionaries
    {
        public const string TitleBar = $"Titlebar/{nameof(TitleBar)}.xaml";
    }
    
    public static class Styles
    {
        public static Style TitlebarRestoreDownButton => 
            GetStyleFromResourceDictionary(nameof(TitlebarRestoreDownButton),
                ResourceDictionaries.TitleBar)!;
        public static Style TitlebarMaximizeButton =>
            GetStyleFromResourceDictionary(nameof(TitlebarMaximizeButton),
                ResourceDictionaries.TitleBar)!;
    }
    
    public static Style? GetStyleFromResourceDictionary(string styleName, string resourceDictionaryPathFromResources)
    {
        var resourceDictionary = new ResourceDictionary
        {
            Source = new Uri($"/{Assembly.GetEntryAssembly()!.GetName().Name};component/Resources/{resourceDictionaryPathFromResources}",
                UriKind.RelativeOrAbsolute)
        };
        return resourceDictionary[styleName] as Style;
    }
}