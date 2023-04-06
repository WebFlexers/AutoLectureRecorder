using AutoLectureRecorder.Data.ReactiveModels;
using FluentValidation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Threading.Tasks;
using AutoLectureRecorder.Services.DataAccess.Validation;

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
    // Warning is less severe than error. A lecture with warnings
    // should still be created, but with extra verification
    [Reactive]
    public string? TimeWarning { get; set; }

    [Reactive] 
    public bool HasWarnings { get; set; } = false;

    /// <summary>
    /// Validates the Scheduled Lecture class instance and stores any error messages
    /// </summary>
    /// <param name="validator"></param>
    /// <param name="lecture"></param>
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
                    if (error.ErrorCode == ScheduledLectureErrorCodes.OverlappingLecture)
                    {
                        HasWarnings = true;
                        TimeWarning = error.ErrorMessage;
                    }
                    else
                    {
                        TimeError = error.ErrorMessage;
                    }
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
