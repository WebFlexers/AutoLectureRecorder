using AutoLectureRecorder.WPF.Sections.MainMenu.Dashboard;
using AutoLectureRecorder.WPF.Sections.MainMenu.Library;
using AutoLectureRecorder.WPF.Sections.MainMenu.Schedule;
using AutoLectureRecorder.WPF.Sections.MainMenu.Settings;
using AutoLectureRecorder.WPF.Sections.MainMenu.Upload;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Windows;
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

            this.BindCommand(ViewModel, vm => vm.LogoutCommand, v => v.logoutButton)
                .DisposeWith(disposables);

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
                ResetMenuButtonsStyleExcept("dashboard", colors);
                break;
            case LibraryViewModel _:
                libraryButton.Background = colors["ButtonHoverBrush"] as SolidColorBrush;
                libraryButtonContent.FillColor = colors["PrimaryBrush"] as SolidColorBrush;
                ResetMenuButtonsStyleExcept("library", colors);
                break;
            case ScheduleViewModel _:
                scheduleButton.Background = colors["ButtonHoverBrush"] as SolidColorBrush;
                scheduleButtonContent.FillColor = colors["PrimaryBrush"] as SolidColorBrush;
                ResetMenuButtonsStyleExcept("schedule", colors);
                break;
            case SettingsViewModel _:
                settingsButton.Background = colors["ButtonHoverBrush"] as SolidColorBrush;
                settingsButtonContent.FillColor = colors["PrimaryBrush"] as SolidColorBrush;
                ResetMenuButtonsStyleExcept("settings", colors);
                break;
            case UploadViewModel _:
                uploadButton.Background = colors["ButtonHoverBrush"] as SolidColorBrush;
                uploadButtonContent.FillColor = colors["PrimaryBrush"] as SolidColorBrush;
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
        }
        if (exception != "library")
        {
            libraryButton.Background = new SolidColorBrush(Colors.Transparent);
            libraryButtonContent.FillColor = colors["SecondaryTextBrush"] as SolidColorBrush;
        }
        if (exception != "schedule")
        {
            scheduleButton.Background = new SolidColorBrush(Colors.Transparent);
            scheduleButtonContent.FillColor = colors["SecondaryTextBrush"] as SolidColorBrush;
        }
        if (exception != "settings")
        {
            settingsButton.Background = new SolidColorBrush(Colors.Transparent);
            settingsButtonContent.FillColor = colors["SecondaryTextBrush"] as SolidColorBrush;
        }
        if (exception != "upload")
        {
            uploadButton.Background = new SolidColorBrush(Colors.Transparent);
            uploadButtonContent.FillColor = colors["SecondaryTextBrush"] as SolidColorBrush;
        }
    }
}
