using AutoLectureRecorder.WPF.DependencyInjection.Factories;
using AutoLectureRecorder.WPF.Sections.Home;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;

namespace AutoLectureRecorder.WPF;
public class MainWindowViewModel : ReactiveObject, IScreen
{
    private readonly IViewModelFactory _viewModelFactory;

    public RoutingState Router { get; } = new RoutingState();

    public ReactiveCommand<Unit, Unit> ExitAppCommand { get; set; }
    public ReactiveCommand<Unit, Unit> ToggleWindowStateCommand { get; set; }
    public ReactiveCommand<string, Unit> UpdateMaximizedButtonStyle { get; set; }
    public ReactiveCommand<Unit, WindowState> MinimizeWindowCommand { get; set; }
    public ReactiveCommand<Type, Unit> Navigate { get; private set; }

    public MainWindowViewModel(IViewModelFactory viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;

        Navigate = ReactiveCommand.Create<Type>(SetRoutedViewHostContent);
        MaximizeButtonStyle = GetStyleFromResourceDictionary("TitlebarMaximizeButton", "TitleBar.xaml")!;

        ExitAppCommand = ReactiveCommand.Create(Application.Current.Shutdown);
        ToggleWindowStateCommand = ReactiveCommand.Create(() =>
        {
            if (MainWindowState == WindowState.Maximized)
            {
                MainWindowState = WindowState.Normal;
            }
            else
            {
                MainWindowState = WindowState.Maximized;
            }
        });
        MinimizeWindowCommand = ReactiveCommand.Create(() => MainWindowState = WindowState.Minimized);
    }

    [Reactive]
    public WindowState MainWindowState { get; set; }
    [Reactive]
    public Style MaximizeButtonStyle { get; set; }

    public Style? GetStyleFromResourceDictionary(string styleName, string resourceDictionaryName)
    {
        var titleBarResources = new ResourceDictionary();
        titleBarResources.Source = new Uri($"/{Assembly.GetEntryAssembly()!.GetName().Name};component/Resources/{resourceDictionaryName}",
                        UriKind.RelativeOrAbsolute);
        return titleBarResources[styleName] as Style;
    }

    public void SetRoutedViewHostContent(Type type)
    {
        if (Router.NavigationStack.LastOrDefault()?.GetType() == type)
        {
            return;
        }

        Router.Navigate.Execute(_viewModelFactory.CreateRoutableViewModel(type));
    }
}
