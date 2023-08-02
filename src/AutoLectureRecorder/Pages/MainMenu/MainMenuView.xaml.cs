using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using AutoLectureRecorder.Common.Messages;
using AutoLectureRecorder.Pages.MainMenu.Dashboard;
using AutoLectureRecorder.Pages.MainMenu.Library;
using AutoLectureRecorder.Pages.MainMenu.Settings;
using AutoLectureRecorder.Resources.Themes.ThemesManager;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using Splat;
using AutoLectureRecorder.Pages.MainMenu.Schedule;
using AutoLectureRecorder.Pages.MainMenu.Upload;

namespace AutoLectureRecorder.Pages.MainMenu;

public partial class MainMenuView : ReactiveUserControl<MainMenuViewModel>
{
    private readonly IThemeManager? _themeManager;
    
    public MainMenuView()
    {
        InitializeComponent();

        _themeManager = Locator.Current.GetService<IThemeManager>();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, 
                    vm => vm.Router,
                    v => v.RoutedViewHost.Router)
                .DisposeWith(disposables);

            // Navigation commands
            this.BindCommand(ViewModel,
                    vm => vm.NavigateCommand,
                    v => v.DashboardButton,
                    Observable.Return(typeof(DashboardViewModel)))
                .DisposeWith(disposables);
            
            this.BindCommand(ViewModel,
                    vm => vm.NavigateCommand,
                    v => v.ScheduleButton,
                    Observable.Return(typeof(ScheduleViewModel)))
                .DisposeWith(disposables);
            
            this.BindCommand(ViewModel,
                    vm => vm.NavigateCommand,
                    v => v.UploadButton,
                    Observable.Return(typeof(UploadViewModel)))
                .DisposeWith(disposables);
            
            this.BindCommand(ViewModel,
                    vm => vm.NavigateCommand,
                    v => v.LibraryButton,
                    Observable.Return(typeof(LibraryViewModel)))
                .DisposeWith(disposables);
            
            this.BindCommand(ViewModel,
                    vm => vm.NavigateCommand,
                    v => v.SettingsButton,
                    Observable.Return(typeof(SettingsViewModel)))
                .DisposeWith(disposables);
            
            this.BindCommand(ViewModel,
                vm => vm.NavigateToRecordWindowCommand,
                v => v.HelpButton)
                .DisposeWith(disposables);

            // Navigate back and forward using the 2 arrow-like mouse buttons (if the user's mouse has them)
            this.Events().MouseDown
                .Subscribe(e =>
                {
                    switch (e.ChangedButton)
                    {
                        case MouseButton.XButton1:
                            ViewModel!.NavigateBackCommand.Execute().Subscribe().DisposeWith(disposables);
                            break;
                        case MouseButton.XButton2:
                            ViewModel!.NavigateForwardCommand.Execute().Subscribe().DisposeWith(disposables);
                            break;
                    }
                }).DisposeWith(disposables);

            this.BindCommand(ViewModel, 
                    vm => vm.LogoutCommand, 
                    v => v.LogoutButton)
                .DisposeWith(disposables);

            // Update the menu buttons to highlight the selected one and show the right side line
            // that indicates it is active
            ViewModel!.Router.NavigationChanged.Subscribe(_ => 
                UpdateMenuButtonsStyle()).DisposeWith(disposables);

            MessageBus.Current.Listen<Unit>(PubSubMessages.UpdateTheme)
                .Subscribe(_ => UpdateMenuButtonsStyle())
                .DisposeWith(disposables);
        });
    }

    public void UpdateMenuButtonsStyle()
    {
        var colors = _themeManager?.GetCurrentThemeDictionary();
        Debug.Assert(colors is not null);
        
        var navigatedViewModel = ViewModel!.Router.GetCurrentViewModel();

        switch (navigatedViewModel)
        {
            case DashboardViewModel _:
                DashboardButton.Background = colors["ButtonHoverBrush"] as SolidColorBrush;
                DashboardButtonContent.FillColor = (colors["PrimaryBrush"] as SolidColorBrush)!;
                DashboardSelectedLineGrid.Visibility = Visibility.Visible;
                ResetMenuButtonsStyleExcept("dashboard", colors);
                break;
            case LibraryViewModel _:
                LibraryButton.Background = colors["ButtonHoverBrush"] as SolidColorBrush;
                LibraryButtonContent.FillColor = (colors["PrimaryBrush"] as SolidColorBrush)!;
                LibrarySelectedLineGrid.Visibility = Visibility.Visible;
                ResetMenuButtonsStyleExcept("library", colors);
                break;
            case ScheduleViewModel _:
                ScheduleButton.Background = colors["ButtonHoverBrush"] as SolidColorBrush;
                ScheduleButtonContent.FillColor = (colors["PrimaryBrush"] as SolidColorBrush)!;
                ScheduleSelectedLineGrid.Visibility = Visibility.Visible;
                ResetMenuButtonsStyleExcept("schedule", colors);
                break;
            case SettingsViewModel _:
                SettingsButton.Background = colors["ButtonHoverBrush"] as SolidColorBrush;
                SettingsButtonContent.FillColor = (colors["PrimaryBrush"] as SolidColorBrush)!;
                SettingsSelectedLineGrid.Visibility = Visibility.Visible;
                ResetMenuButtonsStyleExcept("settings", colors);
                break;
            case UploadViewModel _:
                UploadButton.Background = colors["ButtonHoverBrush"] as SolidColorBrush;
                UploadButtonContent.FillColor = (colors["PrimaryBrush"] as SolidColorBrush)!;
                UploadSelectedLineGrid.Visibility = Visibility.Visible;
                ResetMenuButtonsStyleExcept("upload", colors);
                break;
        }
    }

    private void ResetMenuButtonsStyleExcept(string exception, ResourceDictionary colors)
    {
        if (exception != "dashboard")
        {
            DashboardButton.Background = new SolidColorBrush(Colors.Transparent);
            DashboardButtonContent.FillColor = (colors["SecondaryTextBrush"] as SolidColorBrush)!;
            DashboardSelectedLineGrid.Visibility = Visibility.Hidden;
        }
        if (exception != "library")
        {
            LibraryButton.Background = new SolidColorBrush(Colors.Transparent);
            LibraryButtonContent.FillColor = (colors["SecondaryTextBrush"] as SolidColorBrush)!;
            LibrarySelectedLineGrid.Visibility = Visibility.Hidden;
        }
        if (exception != "schedule")
        {
            ScheduleButton.Background = new SolidColorBrush(Colors.Transparent);
            ScheduleButtonContent.FillColor = (colors["SecondaryTextBrush"] as SolidColorBrush)!;
            ScheduleSelectedLineGrid.Visibility = Visibility.Hidden;
        }
        if (exception != "settings")
        {
            SettingsButton.Background = new SolidColorBrush(Colors.Transparent);
            SettingsButtonContent.FillColor = (colors["SecondaryTextBrush"] as SolidColorBrush)!;
            SettingsSelectedLineGrid.Visibility = Visibility.Hidden;
        }
        if (exception != "upload")
        {
            UploadButton.Background = new SolidColorBrush(Colors.Transparent);
            UploadButtonContent.FillColor = (colors["SecondaryTextBrush"] as SolidColorBrush)!;
            UploadSelectedLineGrid.Visibility = Visibility.Hidden;
        }
    }
}
