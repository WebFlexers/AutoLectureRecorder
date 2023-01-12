using AutoLectureRecorder.Sections.MainMenu.Dashboard;
using AutoLectureRecorder.Sections.MainMenu.Library;
using AutoLectureRecorder.Sections.MainMenu.Schedule;
using AutoLectureRecorder.Sections.MainMenu.Settings;
using AutoLectureRecorder.Sections.MainMenu.Upload;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace AutoLectureRecorder.Sections.MainMenu;

public partial class MainMenuView : ReactiveUserControl<MainMenuViewModel>
{
    public MainMenuView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.Router, v => v.RoutedViewHost.Router)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.MenuVisibility, v => v.MainMenuGrid.Visibility)
                .DisposeWith(disposables);

            // Navigation commands
            this.BindCommand(ViewModel, vm => vm.NavigateToDashboardCommand, v => v.DashboardButton)
                .DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm.NavigateToLibraryCommand, v => v.LibraryButton)
                .DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm.NavigateToScheduleCommand, v => v.ScheduleButton)
                .DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm.NavigateToSettingsCommand, v => v.SettingsButton)
                .DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm.NavigateToUploadCommand, v => v.UploadButton)
                .DisposeWith(disposables);

            // temp navigation to Create Lectures page (TODO: Replace it in the future)
            this.BindCommand(ViewModel, vm => vm.NavigateToCreateLectureCommand, v => v.HelpButton)
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

            this.BindCommand(ViewModel, vm => vm.LogoutCommand, v => v.LogoutButton)
                .DisposeWith(disposables);

            // Update the menu buttons to highlight the selected one and show the right side line
            // that indicates it is active
            ViewModel!.Router.NavigationChanged.Subscribe(cs =>
            {
                UpdateMenuButtonsStyle();
            }).DisposeWith(disposables);
        });
    }

    public void UpdateMenuButtonsStyle()
    {
        var colors = App.GetResourceDictionary("Colors.xaml", "Resources/Colors");
        var navigatedViewModel = ViewModel!.Router.GetCurrentViewModel();

        switch (navigatedViewModel)
        {
            case DashboardViewModel _:
                DashboardButton.Background = colors["ButtonHoverBrush"] as SolidColorBrush;
                DashboardButtonContent.FillColor = colors["PrimaryBrush"] as SolidColorBrush;
                DashboardSelectedLineGrid.Visibility = Visibility.Visible;
                ResetMenuButtonsStyleExcept("dashboard", colors);
                break;
            case LibraryViewModel _:
                LibraryButton.Background = colors["ButtonHoverBrush"] as SolidColorBrush;
                LibraryButtonContent.FillColor = colors["PrimaryBrush"] as SolidColorBrush;
                LibrarySelectedLineGrid.Visibility = Visibility.Visible;
                ResetMenuButtonsStyleExcept("library", colors);
                break;
            case ScheduleViewModel _:
                ScheduleButton.Background = colors["ButtonHoverBrush"] as SolidColorBrush;
                ScheduleButtonContent.FillColor = colors["PrimaryBrush"] as SolidColorBrush;
                ScheduleSelectedLineGrid.Visibility = Visibility.Visible;
                ResetMenuButtonsStyleExcept("schedule", colors);
                break;
            case SettingsViewModel _:
                SettingsButton.Background = colors["ButtonHoverBrush"] as SolidColorBrush;
                SettingsButtonContent.FillColor = colors["PrimaryBrush"] as SolidColorBrush;
                SettingsSelectedLineGrid.Visibility = Visibility.Visible;
                ResetMenuButtonsStyleExcept("settings", colors);
                break;
            case UploadViewModel _:
                UploadButton.Background = colors["ButtonHoverBrush"] as SolidColorBrush;
                UploadButtonContent.FillColor = colors["PrimaryBrush"] as SolidColorBrush;
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
            DashboardButtonContent.FillColor = colors["SecondaryTextBrush"] as SolidColorBrush;
            DashboardSelectedLineGrid.Visibility = Visibility.Hidden;
        }
        if (exception != "library")
        {
            LibraryButton.Background = new SolidColorBrush(Colors.Transparent);
            LibraryButtonContent.FillColor = colors["SecondaryTextBrush"] as SolidColorBrush;
            LibrarySelectedLineGrid.Visibility = Visibility.Hidden;
        }
        if (exception != "schedule")
        {
            ScheduleButton.Background = new SolidColorBrush(Colors.Transparent);
            ScheduleButtonContent.FillColor = colors["SecondaryTextBrush"] as SolidColorBrush;
            ScheduleSelectedLineGrid.Visibility = Visibility.Hidden;
        }
        if (exception != "settings")
        {
            SettingsButton.Background = new SolidColorBrush(Colors.Transparent);
            SettingsButtonContent.FillColor = colors["SecondaryTextBrush"] as SolidColorBrush;
            SettingsSelectedLineGrid.Visibility = Visibility.Hidden;
        }
        if (exception != "upload")
        {
            UploadButton.Background = new SolidColorBrush(Colors.Transparent);
            UploadButtonContent.FillColor = colors["SecondaryTextBrush"] as SolidColorBrush;
            UploadSelectedLineGrid.Visibility = Visibility.Hidden;
        }
    }
}
