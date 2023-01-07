using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.WPF.DependencyInjection.Factories;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;

namespace AutoLectureRecorder.WPF.Sections.MainMenu.CreateLecture;

public class CreateLectureViewModel : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment => nameof(CreateLectureViewModel);
    public IScreen HostScreen { get; }

    public CreateLectureViewModel(IScreenFactory hostScreen)
    {
        HostScreen = hostScreen.GetMainMenuViewModel();

        ScheduledLecture = new ReactiveScheduledLecture
        {
            SubjectName = "Αντικειμενοστρεφής ανάπτυξη εφαρμογών",
            Semester = 5,
            MeetingLink = "https://www.test.com",
            Day = DayOfWeek.Monday,
            StartTime = default(DateTime).AddHours(8).AddMinutes(15),
            EndTime = default(DateTime).AddHours(10).AddMinutes(15),
            IsScheduled = true,
            WillAutoUpload = false
        };
    }

    [Reactive]
    public ReactiveScheduledLecture ScheduledLecture { get; set; }
}
