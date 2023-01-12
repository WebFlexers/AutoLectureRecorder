using AutoLectureRecorder.Data.ReactiveModels;
using FluentValidation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Threading.Tasks;
using System.Windows;

namespace AutoLectureRecorder.Sections.MainMenu.CreateLecture;

/// <summary>
/// Responsible for validating the scheduled lecture. Contains all the error messages as string fields
/// </summary>
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

    /// <summary>
    /// Validates the Scheduled Lecture class instance and stores any error messages
    /// </summary>
    /// <param name="validator"></param>
    /// <param name="lecture"></param>
    /// <returns></returns>
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
