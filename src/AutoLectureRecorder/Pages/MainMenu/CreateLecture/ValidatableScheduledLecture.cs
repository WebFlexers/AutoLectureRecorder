using System.Reactive.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using AutoLectureRecorder.Application.ScheduledLectures.Common;
using AutoLectureRecorder.Domain.ReactiveModels;
using FluentValidation;
using ReactiveUI;

namespace AutoLectureRecorder.Pages.MainMenu.CreateLecture;

public class ValidatableScheduledLecture : ReactiveScheduledLecture, IValidatableScheduledLecture, 
    INotifyDataErrorInfo, IDisposable
{
    private readonly CompositeDisposable _disposables;
    
    private readonly IValidator<ValidatableScheduledLecture> _validator;

    private DateTime? _startTimeForView;
    public DateTime? StartTimeForView
    {
        get => _startTimeForView;
        set
        {
            this.RaiseAndSetIfChanged(ref _startTimeForView, value);
            StartTime = value.HasValue ? TimeOnly.FromDateTime(value.Value) : default;
        }
    }

    private DateTime? _endTimeForView;
    public DateTime? EndTimeForView
    {
        get => _endTimeForView;
        set
        {
            this.RaiseAndSetIfChanged(ref _endTimeForView, value);
            EndTime = value.HasValue ? TimeOnly.FromDateTime(value.Value) : default;
        }
    }

    private string _timeError = string.Empty;
    public string TimeError
    {
        get => _timeError;
        set => this.RaiseAndSetIfChanged(ref _timeError, value);
    }

    private bool _isTimeErrorLecturesOverlap;
    public bool IsTimeErrorLecturesOverlap
    {
        get => _isTimeErrorLecturesOverlap; 
        set => this.RaiseAndSetIfChanged(ref _isTimeErrorLecturesOverlap, value);
    }

    public ValidatableScheduledLecture(IValidator<ValidatableScheduledLecture> validator)
    {
        _disposables = new CompositeDisposable();
        _validator = validator;
        var validate = 
            ReactiveCommand.CreateFromTask<Expression<Func<ValidatableScheduledLecture, object>>[], Unit>(
            async parameters =>
            {
                await Validate(parameters);
                return Unit.Default;
            });
        var validateTime = ReactiveCommand.CreateFromTask(
        async () =>
        {
            await ValidateTime();
            return Unit.Default;
        });

        this.WhenAnyValue(x => x.SubjectName)
            .Do(_ => ClearPropertyErrors(nameof(this.SubjectName)))
            .Where(subjectName => subjectName?.Length >= 3)
            .Select(_ => CreateValidatableExpression(x => x.SubjectName!))
            .InvokeCommand(validate)
            .DisposeWith(_disposables);

        this.WhenAnyValue(x => x.Semester)
            .Do(_ => ClearPropertyErrors(nameof(this.Semester)))
            .Where(semester => semester != default)
            .Select(_ => CreateValidatableExpression(x => x.Semester))
            .InvokeCommand(validate)
            .DisposeWith(_disposables);

        this.WhenAnyValue(x => x.MeetingLink)
            .Do(_ => ClearPropertyErrors(nameof(this.MeetingLink)))
            .Where(meetingLink => meetingLink?.Length >= 3)
            .Select(_ => CreateValidatableExpression(x => x.MeetingLink!))
            .InvokeCommand(validate)
            .DisposeWith(_disposables);
        
        this.WhenAnyValue(x => x.Day)
            .Subscribe(_ => ClearPropertyErrors(nameof(this.Day)))
            .DisposeWith(_disposables);

        this.WhenAnyValue(x => x.StartTime)
            .Do(_ =>
            {
                ClearPropertyErrors(nameof(StartTimeForView));
                ClearPropertyErrors(nameof(EndTimeForView));
                TimeError = string.Empty;
            })
            .Where(_ => StartTimeForView is not null && EndTimeForView is not null)
            .Select(_ => Unit.Default) 
            .InvokeCommand(validateTime)
            .DisposeWith(_disposables);
        
        this.WhenAnyValue(x => x.EndTime)
            .Do(_ =>
            {
                ClearPropertyErrors(nameof(StartTimeForView));
                ClearPropertyErrors(nameof(EndTimeForView));
                TimeError = string.Empty;
            })
            .Where(_ => StartTimeForView is not null && EndTimeForView is not null)
            .Select(_ => Unit.Default)
            .InvokeCommand(validateTime)
            .DisposeWith(_disposables);
    }

    private async Task Validate(params Expression<Func<ValidatableScheduledLecture, object>>[] 
        propertyExpressions)
    {
        var result = await _validator.ValidateAsync(this, options =>
            options.IncludeProperties(propertyExpressions));

        if (result.IsValid) return;
        
        foreach (var error in result.Errors)
        {
            AddError(error.PropertyName, error.ErrorMessage);
        }
    }

    /// <summary>
    /// For time we always validate both fields at once 
    /// </summary>
    private async Task ValidateTime()
    {
        
        var result = await _validator.ValidateAsync(this, options =>
        {
            options.IncludeProperties(lecture => lecture.StartTime, lecture => lecture.EndTime);
        });
        
        if (result.IsValid) return;

        var error = result.Errors.First();

        if (error.ErrorCode == "OverlappingLecture")
        {
            IsTimeErrorLecturesOverlap = true;
            TimeError = error.ErrorMessage;
            return;
        }
        else
        {
            IsTimeErrorLecturesOverlap = false;
        }
        
        TimeError = error.ErrorMessage;
        
        // In the live validation we don't want the field to become errored when it is empty,
        // since the user might not have filled it in yet. We will check for emptiness
        // at the command submission.
        if (StartTimeForView is not null)
        {
            AddError(nameof(StartTimeForView), string.Empty);
        }

        if (EndTimeForView is not null)
        {
            AddError(nameof(EndTimeForView), string.Empty);
        }
    }

    private Expression<Func<ValidatableScheduledLecture, object>>[] CreateValidatableExpression(
        params Expression<Func<ValidatableScheduledLecture, object>>[] propertyExpressions)
    {
        return propertyExpressions;
    }

    private readonly Dictionary<string, List<string>> _propertyErrors = new();

    public IEnumerable GetErrors(string? propertyName)
    {
        if (propertyName is null) return Enumerable.Empty<string>();

        return _propertyErrors.TryGetValue(propertyName, out var errors) 
            ? errors 
            : Enumerable.Empty<string>();
    }

    public bool HasErrors => _propertyErrors.Any();
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public void AddError(string propertyName, string errorMessage)
    {
        if (_propertyErrors.ContainsKey(propertyName) == false)
        {
            _propertyErrors.Add(propertyName, new List<string>());
        }
        
        _propertyErrors[propertyName].Add(errorMessage);
        OnErrorsChanged(propertyName);
    }

    public void ClearError(string propertyName, string errorToClear)
    {
        if (_propertyErrors.TryGetValue(propertyName, out var error))
        {
            error.Remove(errorToClear);
            OnErrorsChanged(propertyName);
        }
    }
    
    public void ClearPropertyErrors(string? propertyName)
    {
        if (propertyName is null) return;
        
        if (_propertyErrors.Remove(propertyName))
        {
            OnErrorsChanged(propertyName);   
        }
    }
    
    private void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    public void Dispose()
    {
        var events = ErrorsChanged?.GetInvocationList();
        if (events is null) return;
        
        foreach(var d in events)
        {
            ErrorsChanged -= (EventHandler<DataErrorsChangedEventArgs>)d;
        }
        
        _disposables.Dispose();
    }
}