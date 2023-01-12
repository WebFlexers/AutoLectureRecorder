using AutoLectureRecorder.Sections.MainMenu;
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

namespace AutoLectureRecorder.WPF.Sections.MainMenu;

public partial class MainMenuView : ReactiveUserControl<MainMenuViewModel>
{
    public MainMenuView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.Router, v => v.routedViewHost.Router)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.MenuVisibility, v => v.mainMenuGrid.Visibility)
                .DisposeWith(disposables);

            // Navigation commands
            this.BindCommand(ViewModel, vm => vm.NavigateToDashboardCommand, v => v.dashboardButton)
                .DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm.NavigateToLibraryCommand, v => v.libraryButton)
                .DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm.NavigateToScheduleCommand, v => v.scheduleButton)
                .DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm.NavigateToSettingsCommand, v => v.settingsButton)
                .DisposeWith(disposables);
            this.BindCommand(ViewModel, vm => vm.NavigateToUploadCommand, v => v.uploadButton)
                .DisposeWith(disposables);

            // temp navigation to Create Lectures page (TODO: Replace it in the future)
            this.BindCommand(ViewModel, vm => vm.NavigateToCreateLectureCommand, v => v.helpButton)
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

            this.BindCommand(ViewModel, vm => vm.LogoutCommand, v => v.logoutButton)
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
        var colors = ((App)Application.Current).GetResourceDictionary("Colors.xaml", "Resources/Colors");
        var navigatedViewModel = ViewModel!.Router.GetCurrentViewModel();

        switch (navigatedViewModel)
        {
            case DashboardViewModel _:
                dashboardButton.Background = colors["ButtonHoverBrush"] as SolidColorBrush;
                dashboardButtonContent.FillColor = colors["PrimaryBrush"] as SolidColorBrush;
                dashboardSelectedLineGrid.Visibility = Visibility.Visible;
                ResetMenuButtonsStyleExcept("dashboard", colors);
                break;
            case LibraryViewModel _:
                libraryButton.Background = colors["ButtonHoverBrush"] as SolidColorBrush;
                libraryButtonContent.FillColor = colors["PrimaryBrush"] as SolidColorBrush;
                librarySelectedLineGrid.Visibility = Visibility.Visible;
                ResetMenuButtonsStyleExcept("library", colors);
                break;
            case ScheduleViewModel _:
                scheduleButton.Background = colors["ButtonHoverBrush"] as SolidColorBrush;
                scheduleButtonContent.FillColor = colors["PrimaryBrush"] as SolidColorBrush;
                scheduleSelectedLineGrid.Visibility = Visibility.Visible;
                ResetMenuButtonsStyleExcept("schedule", colors);
                break;
            case SettingsViewModel _:
                settingsButton.Background = colors["ButtonHoverBrush"] as SolidColorBrush;
                settingsButtonContent.FillColor = colors["PrimaryBrush"] as SolidColorBrush;
                settingsSelectedLineGrid.Visibility = Visibility.Visible;
                ResetMenuButtonsStyleExcept("settings", colors);
                break;
            case UploadViewModel _:
                uploadButton.Background = colors["ButtonHoverBrush"] as SolidColorBrush;
                uploadButtonContent.FillColor = colors["PrimaryBrush"] as SolidColorBrush;
                uploadSelectedLineGrid.Visibility = Visibility.Visible;
                ResetMenuButtonsStyleExcept("upload", colors);
                break;
        }
    }

    private void ResetMenuButtonsStyleExcept(string exception, ResourceDictionary colors)
    {
        if (exception != "dashboard")
        {
            dashboardButton.Background = new SolidColorBrush(Colors.Transparent);
            dashboardButtonContent.FillColor = colors["SecondaryTextBrush"] as SolidColorBrush;
            dashboardSelectedLineGrid.Visibility = Visibility.Hidden;
        }
        if (exception != "library")
        {
            libraryButton.Background = new SolidColorBrush(Colors.Transparent);
            libraryButtonContent.FillColor = colors["SecondaryTextBrush"] as SolidColorBrush;
            librarySelectedLineGrid.Visibility = Visibility.Hidden;
        }
        if (exception != "schedule")
        {
            scheduleButton.Background = new SolidColorBrush(Colors.Transparent);
            scheduleButtonContent.FillColor = colors["SecondaryTextBrush"] as SolidColorBrush;
            scheduleSelectedLineGrid.Visibility = Visibility.Hidden;
        }
        if (exception != "settings")
        {
            settingsButton.Background = new SolidColorBrush(Colors.Transparent);
            settingsButtonContent.FillColor = colors["SecondaryTextBrush"] as SolidColorBrush;
            settingsSelectedLineGrid.Visibility = Visibility.Hidden;
        }
        if (exception != "upload")
        {
            uploadButton.Background = new SolidColorBrush(Colors.Transparent);
            uploadButtonContent.FillColor = colors["SecondaryTextBrush"] as SolidColorBrush;
            uploadSelectedLineGrid.Visibility = Visibility.Hidden;
        }
    }
}
