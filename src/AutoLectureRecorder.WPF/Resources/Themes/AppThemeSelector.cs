using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace AutoLectureRecorder.Resources.Themes;

public class AppThemeSelector : DependencyObject
{

    public static readonly DependencyProperty CurrentThemeDictionaryProperty =
        DependencyProperty.RegisterAttached("CurrentThemeDictionary", typeof(Uri),
            typeof(AppThemeSelector),
            new UIPropertyMetadata(null, CurrentThemeDictionaryChanged));

    public static Uri GetCurrentThemeDictionary(DependencyObject obj)
    {
        return (Uri)obj.GetValue(CurrentThemeDictionaryProperty);
    }

    public static void SetCurrentThemeDictionary(ICollection<DependencyObject> windows, Uri value)
    {
        foreach (var obj in windows)
        {
            obj.SetValue(CurrentThemeDictionaryProperty, value);
        }
    }

    private static void CurrentThemeDictionaryChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        if (obj is FrameworkElement element) // Works only on FrameworkElement objects
        {
            ApplyTheme(element, GetCurrentThemeDictionary(element));
        }
    }

    private static void ApplyTheme(FrameworkElement? targetElement, Uri? dictionaryUri)
    {
        if (targetElement == null) return;

        try
        {
            ThemeResourceDictionary? newThemeDictionary = null;
            if (dictionaryUri != null)
            {
                newThemeDictionary = new ThemeResourceDictionary
                {
                    Source = dictionaryUri
                };

                // Add the new dictionary to the collection of merged dictionaries of the target object
                targetElement.Resources.MergedDictionaries.Insert(0, newThemeDictionary);
            }

            // Find if the target element already has a theme applied
            var existingDictionaries =
                (from dictionary in targetElement.Resources.MergedDictionaries.OfType<ThemeResourceDictionary>()
                 select dictionary).ToList();

            // Remove the existing dictionaries
            foreach (var existingThemeDictionary in existingDictionaries)
            {
                if (newThemeDictionary == existingThemeDictionary) continue; // Don't remove the newly added dictionary
                targetElement.Resources.MergedDictionaries.Remove(existingThemeDictionary);
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.ToString());
        }
    }
}