using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows;
using AutoLectureRecorder.Common.Core;
using AutoLectureRecorder.Common.Navigation;
using ReactiveUI;

namespace AutoLectureRecorder.Pages.MainMenu;

public class MainMenuViewModel : NavigationHostViewModel, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();

    public ReactiveCommand<Type, Unit> NavigateCommand { get; }
    public ReactiveCommand<Unit, Unit> NavigateBackCommand { get; }
    public ReactiveCommand<Unit, Unit> NavigateForwardCommand { get; }

    public ReactiveCommand<Unit, Unit> LogoutCommand { get; }

    public MainMenuViewModel(INavigationService navigationService) : base(navigationService)
    {
        /*NavigateToRecordWindowCommand = ReactiveCommand.Create(() =>
        {
            // Algorithmoi: https://teams.microsoft.com/l/meetup-join/19%3ameeting_NjFmMWM0ZjctNTFiNC00MTc0LWFjYTQtMzlhMGRkNTM0NjFi%40thread.v2/0?context=%7b%22Tid%22%3a%22d9c8dee3-558b-483d-b502-d31fa0cb24de%22%2c%22Oid%22%3a%2220dcfee8-a2d9-4250-aa03-bde3530991d8%22%7d
            // TODO: Modify this test method
            var recordWindow = windowFactory.CreateRecordWindow(new ReactiveScheduledLecture
            {
                Id = 5000,
                SubjectName = "Εκπαιδευτικό Λογισμικό",
                Semester = 8,
                MeetingLink = @"https://teams.microsoft.com/l/meetup-join/19%3Ameeting_OGE0OWFiNmUtYzM4YS00Y2IwLWI1NjgtMGI4NjQ4ZWVjMTM0%40thread.v2/0?context=%7B%22Tid%22%3A%225f3b4a0c-0b1e-4776-9e95-6933e4408e97%22%2C%22Oid%22%3A%22a0248d49-c7e0-48f7-94c2-d5f02c150d78%22%7D",
                Day = DayOfWeek.Friday,
                StartTime = DateTime.MinValue.AddHours(8),
                EndTime = DateTime.MinValue.AddHours(10).AddMinutes(15),
                IsScheduled = true,
                WillAutoUpload = false
            });
            recordWindow.Show();
        });*/

        NavigateCommand = ReactiveCommand.Create<Type>(viewModelType => 
            navigationService.Navigate(viewModelType, HostNames.MainMenuHost));
        
        NavigateBackCommand = ReactiveCommand.Create(() => 
            navigationService.NavigateBack(HostNames.MainMenuHost));
        NavigateForwardCommand = ReactiveCommand.Create(() => 
            navigationService.NavigateForward(HostNames.MainMenuHost));

        LogoutCommand = ReactiveCommand.CreateFromTask(Logout);

        // Navigate to the Dashboard at startup
        /*this.WhenActivated(disposables =>
        {
            NavigateCommand.Execute(typeof())
                .Subscribe()
                .DisposeWith(disposables);
        });*/
    }

    private async Task Logout()
    {
        /*await _studentAccountData.DeleteStudentAccount();
        HostScreen.Router.NavigateAndReset.Execute(_viewModelFactory.CreateRoutableViewModel(typeof(LoginViewModel)));*/
    }
}
