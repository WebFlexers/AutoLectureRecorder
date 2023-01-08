using AutoLectureRecorder.Data.ReactiveModels;
using FluentValidation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Threading.Tasks;
using System.Windows;

namespace AutoLectureRecorder.WPF.Sections.MainMenu.CreateLecture;

public class ValidatableScheduledLecture : ReactiveObject
{
    [Reactive]
    public string? SubjectNameError { get; set; }
    [Reactive]
    public string? SemesterError { get; set; }
    [Reactive]
    public string? MeetingLinkError { get; set; }
    [Reactive]
    public string? DayError { get; set; }
    [Reactive]
    public string? TimeError { get; set; }

    public async Task<bool> ValidateAndPopulateErrors(IValidator<ReactiveScheduledLecture> validator, ReactiveScheduledLecture lecture)
    {
        var result = await validator.ValidateAsync(lecture);

        ClearErrors();

        foreach (var error in result.Errors)
        {

            switch (error.PropertyName)
            {
                case nameof(lecture.SubjectName):
                    SubjectNameError = error.ErrorMessage;
                    break;
                case nameof(lecture.Semester):
                    SemesterError = error.ErrorMessage;
                    break;
                case nameof(lecture.MeetingLink):
                    MeetingLinkError = error.ErrorMessage;
                    break;
                case nameof(lecture.Day):
                    DayError = error.ErrorMessage;
                    break;
                case nameof(lecture.StartTime):
                    TimeError = error.ErrorMessage;
                    break;
                case nameof(lecture.EndTime):
                    TimeError = error.ErrorMessage;
                    break;
            }
        }
        
        return result.IsValid;
    }

    private void ClearErrors()
    {
        SubjectNameError = string.Empty;
        SemesterError = string.Empty;
        MeetingLinkError = string.Empty;
        DayError = string.Empty;
        TimeError = string.Empty;
    }
}
