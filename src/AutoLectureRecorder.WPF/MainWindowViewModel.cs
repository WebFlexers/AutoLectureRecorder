using AutoLectureRecorder.WPF.DependencyInjection.Factories;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;

namespace AutoLectureRecorder.WPF;

public class MainWindowViewModel : ReactiveObject, IScreen
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly ILogger<MainWindowViewModel> _logger;

    public RoutingState Router { get; } = new RoutingState();

    public ReactiveCommand<Unit, Unit> ExitAppCommand { get; set; }
    public ReactiveCommand<Unit, Unit> ToggleWindowStateCommand { get; set; }
    public ReactiveCommand<string, Unit> UpdateMaximizedButtonStyle { get; set; }
    public ReactiveCommand<Unit, WindowState> MinimizeWindowCommand { get; set; }
    public ReactiveCommand<Type, Unit> Navigate { get; private set; }

    public MainWindowViewModel(IViewModelFactory viewModelFactory, ILogger<MainWindowViewModel> logger)
    {
        _viewModelFactory = viewModelFactory;
        _logger = logger;

        Navigate = ReactiveCommand.Create<Type>(SetRoutedViewHostContent);

        MaximizeButtonStyle = ((App)Application.Current)
                .GetStyleFromResourceDictionary("TitlebarMaximizeButton", "TitleBar.xaml")!;

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

    public void SetRoutedViewHostContent(Type type)
    {
        if (Router.NavigationStack.LastOrDefault()?.GetType() == type)
        {
            return;
        }

        Router.Navigate.Execute(_viewModelFactory.CreateRoutableViewModel(type));

        _logger.LogDebug("Navigated to {viewModel}", type.Name);
    }
}
