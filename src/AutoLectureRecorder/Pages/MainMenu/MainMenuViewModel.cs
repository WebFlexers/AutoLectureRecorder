using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Common.Core;
using AutoLectureRecorder.Common.Core.Abstractions;
using AutoLectureRecorder.Common.Navigation;
using AutoLectureRecorder.Common.Navigation.Parameters;
using AutoLectureRecorder.Domain.ReactiveModels;
using AutoLectureRecorder.Pages.Login;
using AutoLectureRecorder.Pages.MainMenu.Dashboard;
using AutoLectureRecorder.Pages.RecordLecture;
using ReactiveUI;

namespace AutoLectureRecorder.Pages.MainMenu;

public class MainMenuViewModel : RoutableViewModelHost, IActivatableViewModel
{
    private readonly IStudentAccountRepository _studentAccountRepository;
    public ViewModelActivator Activator { get; } = new();

    public ReactiveCommand<Unit, Unit> NavigateToRecordWindowCommand { get; }
    public ReactiveCommand<Type, Unit> NavigateCommand { get; }
    public ReactiveCommand<Unit, Unit> NavigateBackCommand { get; }
    public ReactiveCommand<Unit, Unit> NavigateForwardCommand { get; }

    public ReactiveCommand<Unit, Unit> LogoutCommand { get; }

    public MainMenuViewModel(INavigationService navigationService, IStudentAccountRepository studentAccountRepository,
        IWindowFactory windowFactory) 
        : base(navigationService)
    {
        _studentAccountRepository = studentAccountRepository;
        NavigateToRecordWindowCommand = ReactiveCommand.Create(() =>
        {
            // Ekpaideutiko Logismiko: https://teams.microsoft.com/l/meetup-join/19%3Ameeting_OGE0OWFiNmUtYzM4YS00Y2IwLWI1NjgtMGI4NjQ4ZWVjMTM0%40thread.v2/0?context=%7B%22Tid%22%3A%225f3b4a0c-0b1e-4776-9e95-6933e4408e97%22%2C%22Oid%22%3A%22a0248d49-c7e0-48f7-94c2-d5f02c150d78%22%7D
            // Algorithmoi: https://teams.microsoft.com/l/meetup-join/19%3ameeting_NjFmMWM0ZjctNTFiNC00MTc0LWFjYTQtMzlhMGRkNTM0NjFi%40thread.v2/0?context=%7b%22Tid%22%3a%22d9c8dee3-558b-483d-b502-d31fa0cb24de%22%2c%22Oid%22%3a%2220dcfee8-a2d9-4250-aa03-bde3530991d8%22%7d
            // TODO: Delete this test method
            var nowTimeOnly = TimeOnly.FromTimeSpan(DateTime.Now.TimeOfDay);
            var lectureToRecord = new ReactiveScheduledLecture
            {
                Id = 5000,
                SubjectName = "Αλγόριθμοι",
                Semester = 8,
                MeetingLink = @"https://teams.microsoft.com/l/meetup-join/19%3ameeting_NjFmMWM0ZjctNTFiNC00MTc0LWFjYTQtMzlhMGRkNTM0NjFi%40thread.v2/0?context=%7b%22Tid%22%3a%22d9c8dee3-558b-483d-b502-d31fa0cb24de%22%2c%22Oid%22%3a%2220dcfee8-a2d9-4250-aa03-bde3530991d8%22%7d",
                Day = DayOfWeek.Wednesday,
                StartTime = nowTimeOnly,
                EndTime = nowTimeOnly.AddMinutes(30),
                IsScheduled = true,
                WillAutoUpload = false
            };
            
            var navigationParameters = new Dictionary<string, object>()
            {
                { NavigationParameters.RecordLecture.LectureToRecord, lectureToRecord }
            };
            navigationService.AddNavigationParameters(typeof(RecordWindowViewModel), navigationParameters);

            var recordingWindow = windowFactory.CreateRecordWindow();
            recordingWindow.Show();
        });

        NavigateCommand = ReactiveCommand.Create<Type>(viewModelType => 
            navigationService.Navigate(viewModelType, HostNames.MainMenuHost));
        
        NavigateBackCommand = ReactiveCommand.Create(() => 
            navigationService.NavigateBack(HostNames.MainMenuHost));
        NavigateForwardCommand = ReactiveCommand.Create(() => 
            navigationService.NavigateForward(HostNames.MainMenuHost));

        LogoutCommand = ReactiveCommand.CreateFromTask(Logout);

        // Navigate to the Dashboard at startup
        this.WhenActivated(disposables =>
        {
            NavigateCommand.Execute(typeof(DashboardViewModel))
                .Subscribe()
                .DisposeWith(disposables);
        });
    }

    private async Task Logout()
    {
        await _studentAccountRepository.DeleteStudentAccount();
        NavigationService.NavigateAndReset(typeof(LoginViewModel), HostNames.MainWindowHost);
    }
}
