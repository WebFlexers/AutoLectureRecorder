using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Application.Common.Abstractions.Validation;
using AutoLectureRecorder.Application.ScheduledLectures.Commands.DeleteScheduledLectures;
using AutoLectureRecorder.Application.ScheduledLectures.Commands.UpdateScheduledLecture.Mapping;
using AutoLectureRecorder.Application.ScheduledLectures.Common;
using AutoLectureRecorder.Common.Core;
using AutoLectureRecorder.Common.Navigation;
using AutoLectureRecorder.Common.Navigation.Parameters;
using AutoLectureRecorder.Domain.ReactiveModels;
using AutoLectureRecorder.Pages.MainMenu.CreateLecture;
using AutoLectureRecorder.Pages.MainMenu.Schedule.CustomControls;
using MediatR;
using ReactiveUI;
using Unit = System.Reactive.Unit;

namespace AutoLectureRecorder.Pages.MainMenu.Schedule;

public class ScheduleViewModel : RoutableViewModel, IActivatableViewModel
{
    private readonly IScheduledLectureRepository _scheduledLectureRepository;
    
    private ReactiveScheduledLecture[]? _allLecturesSorted;
    public int AllLecturesCount => _allLecturesSorted?.Length ?? 0;
    
    public ViewModelActivator Activator { get; }
    
    private bool _isDeleteLecturesConfirmationDialogOpen;
    public bool IsDeleteLecturesConfirmationDialogOpen
    {
        get => _isDeleteLecturesConfirmationDialogOpen; 
        set => this.RaiseAndSetIfChanged(ref _isDeleteLecturesConfirmationDialogOpen, value);
    }

    private int _selectedLecturesCount;
    public int SelectedLecturesCount
    {
        get => _selectedLecturesCount;
        set => this.RaiseAndSetIfChanged(ref _selectedLecturesCount, value);
    }

    public ReactiveCommand<Unit, Unit> NavigateToCreateLecture { get; }
    public ReactiveCommand<ReactiveScheduledLecture, Unit> NavigateToEditLecture { get; }
    public ReactiveCommand<ReactiveScheduledLecture, Unit> ModifyLectureStateCommand { get; }
    public ReactiveCommand<bool, Unit> ChangeSelectedLecturesCountCommand { get; }
    public ReactiveCommand<Unit, Unit> AttemptToDeleteLecturesCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteLecturesCommand { get; }
    public ReactiveCommand<Unit, Unit> ChangeAllLecturesSelectionCommand { get; }
    
    public Dictionary<DayOfWeek, ObservableCollection<LectureViewModel>?> FilteredLecturesByDay { get; } = new()
    {
        { DayOfWeek.Monday, null },
        { DayOfWeek.Tuesday, null },
        { DayOfWeek.Wednesday, null },
        { DayOfWeek.Thursday, null },
        { DayOfWeek.Friday, null },
        { DayOfWeek.Saturday, null },
        { DayOfWeek.Sunday, null },
    };

    #region Filters

    private string? _subjectNameSearchFilter;
    public string? SubjectNameSearchFilter
    {
        get => _subjectNameSearchFilter; 
        set => this.RaiseAndSetIfChanged(ref _subjectNameSearchFilter, value);
    }
    
    private string? _semesterFilter;
    public string? SemesterFilter 
    { 
        get => _semesterFilter; 
        set => this.RaiseAndSetIfChanged(ref _semesterFilter, value); 
    }
    
    private string? _activeStateFilter;
    public string? ActiveStateFilter 
    { 
        get => _activeStateFilter; 
        set => this.RaiseAndSetIfChanged(ref _activeStateFilter, value); 
    }
    
    private string? _uploadStateFilterFilter;
    public string? UploadStateFilter 
    { 
        get => _uploadStateFilterFilter; 
        set => this.RaiseAndSetIfChanged(ref _uploadStateFilterFilter, value); 
    }

    #endregion
    
    public ScheduleViewModel(INavigationService navigationService, IScheduledLectureRepository scheduledLectureRepository, 
        ISender mediatorSender, IPersistentValidationContext persistentValidationContext) : base(navigationService)
    {
        Activator = new();
        _scheduledLectureRepository = scheduledLectureRepository;

        NavigateToCreateLecture = ReactiveCommand.Create(() => 
            NavigationService.Navigate(typeof(CreateLectureViewModel), HostNames.MainMenuHost));

        NavigateToEditLecture = ReactiveCommand.Create<ReactiveScheduledLecture>(lecture =>
        {
            var parameters = new Dictionary<string, object>
            {
                { NavigationParameters.CreateLecture.IsUpdateMode, true },
                { NavigationParameters.CreateLecture.ScheduledLectureToUpdate, lecture }
            };
            NavigationService.Navigate(typeof(CreateLectureViewModel), HostNames.MainMenuHost, parameters);
        });

        ModifyLectureStateCommand = ReactiveCommand.CreateFromTask<ReactiveScheduledLecture>(async lecture =>
        {
            persistentValidationContext.AddValidationParameter(
                ValidationParameters.ScheduledLectures.IgnoreOverlappingLectures, true);
            persistentValidationContext.AddValidationParameter(
                ValidationParameters.ScheduledLectures.IsOnUpdateMode, true);
                
            var result = await mediatorSender.Send(lecture.MapToUpdateCommand());
            if (result.IsError) return;

            List<ReactiveScheduledLecture>? deactivatedLectures = result.Value;

            if (deactivatedLectures is null) return;

            UpdateLecturesByDay(lecture.Day!.Value, deactivatedLectures);
        });

        ChangeSelectedLecturesCountCommand = ReactiveCommand.Create<bool, Unit>(isSelected =>
        {
            if (isSelected)
            {
                SelectedLecturesCount += 1;
            }
            else
            {
                SelectedLecturesCount -= 1;
            }

            return Unit.Default;
        });
        
        var deleteLecturesCanExecute = this.WhenAnyValue(
            vm => vm.SelectedLecturesCount, count => count > 0);
        
        AttemptToDeleteLecturesCommand = ReactiveCommand.Create(() =>
        {
            IsDeleteLecturesConfirmationDialogOpen = true;
        }, deleteLecturesCanExecute);
        
        DeleteLecturesCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var lecturesToDelete = new List<ReactiveScheduledLecture>();
            
            foreach (var keyValuePair in FilteredLecturesByDay)
            {
                if (keyValuePair.Value is null) continue;
                
                lecturesToDelete.AddRange(keyValuePair.Value
                    .Where(lectureVm => lectureVm.IsSelected)
                    .Select(lectureVm => lectureVm.ScheduledLecture));
            }

            var deletedLecturesIds = await mediatorSender.Send(
                new DeleteScheduledLecturesCommand(lecturesToDelete));

            // Remove the deleted lectures from the observable collections
            foreach (var keyValuePair in FilteredLecturesByDay)
            {
                if (keyValuePair.Value is null) continue;
                
                for (int i = keyValuePair.Value.Count - 1; i >= 0; i--)
                {
                    var lectureVm = keyValuePair.Value[i];

                    if (deletedLecturesIds.Contains(lectureVm.ScheduledLecture.Id))
                    {
                        FilteredLecturesByDay[keyValuePair.Key]!.RemoveAt(i);
                    }
                }
            }

            SelectedLecturesCount = 0;
            IsDeleteLecturesConfirmationDialogOpen = false;
        }, deleteLecturesCanExecute);

        ChangeAllLecturesSelectionCommand = ReactiveCommand.Create(() =>
        {
            var shouldGetSelected = SelectedLecturesCount != AllLecturesCount;
            
            foreach (var keyValuePair in FilteredLecturesByDay)
            {
                if (keyValuePair.Value is null) continue;
                
                for (int lectureIndex = 0; lectureIndex < keyValuePair.Value.Count; lectureIndex++)
                {
                    var scheduledLecture = keyValuePair.Value[lectureIndex].ScheduledLecture;
                    keyValuePair.Value[lectureIndex] = new LectureViewModel(scheduledLecture, shouldGetSelected);
                }
            }
            
            SelectedLecturesCount = shouldGetSelected ? AllLecturesCount : 0;
        });
        
        var loadLecturesObservable = Observable.FromAsync(LoadLectures)
            .Subscribe();
        
        this.WhenActivated(disposables =>
        {
            loadLecturesObservable.DisposeWith(disposables);
        });
       
    }

    /// <summary>
    /// Modifies the IsScheduled and WillAutoUpload properties of the lectures of the given day that correspond to the
    /// given list of modified lectures
    /// </summary>
    private void UpdateLecturesByDay(DayOfWeek day, List<ReactiveScheduledLecture> modifiedLectures)
    {
        foreach (var modifiedLecture in CollectionsMarshal.AsSpan(modifiedLectures))
        {
            int? foundIndex = null;

            for (int i = 0; i < FilteredLecturesByDay[day]?.Count; i++)
            {
                if (FilteredLecturesByDay[day]?[i].ScheduledLecture.Id != modifiedLecture.Id) continue;
                
                foundIndex = i;
                break;
            }

            if (foundIndex.HasValue == false) return;

            var existingLectureVm = FilteredLecturesByDay[day]?[foundIndex.Value];
            if (existingLectureVm is null) return;    
            
            existingLectureVm.ScheduledLecture.IsScheduled = modifiedLecture.IsScheduled;
            existingLectureVm.ScheduledLecture.WillAutoUpload = modifiedLecture.WillAutoUpload;
        
            // Create a new instance of LectureViewModel and update it in the list in order for the observable collection
            // to propagate the change.
            var updatedLectureVm = new LectureViewModel(existingLectureVm.ScheduledLecture, existingLectureVm.IsSelected);
            FilteredLecturesByDay[day]![foundIndex.Value] = updatedLectureVm;
        }
    }
    
    /// <summary>
    /// Adds each lecture in the given array of ReactiveScheduledLecture to the corresponding ObservableCollection
    /// based on the day of the lecture
    /// </summary>
    /// <param name="lectures"></param>
    private void AssignLecturesByDay(ReactiveScheduledLecture[] lectures)
    {
        foreach (var keyValuePair in FilteredLecturesByDay)
        {
            FilteredLecturesByDay[keyValuePair.Key] = new ObservableCollection<LectureViewModel>();
        }
        
        foreach (var lecture in lectures)
        {
            FilteredLecturesByDay[lecture.Day!.Value]?.Add(new LectureViewModel(lecture, false));
        }
    }
    
    private IEnumerable<ReactiveScheduledLecture>? FilterLectures(IEnumerable<ReactiveScheduledLecture>? allLectures)
    {
        if (allLectures is null) return null;
        
        if (string.IsNullOrWhiteSpace(SubjectNameSearchFilter) == false)
        {
            allLectures = allLectures
                .Where(lecture => lecture.SubjectName!.ToLower().Contains(SubjectNameSearchFilter.ToLower()));
        }

        if (string.IsNullOrEmpty(SemesterFilter) == false && SemesterFilter.ToLower() != "all")
        {
            allLectures = allLectures
                .Where(lecture => lecture.Semester == Convert.ToInt32(SemesterFilter));
        }

        if (string.IsNullOrEmpty(ActiveStateFilter) == false && ActiveStateFilter.ToLower() != "all")
        {
            var activeStateFilterNormalized = ActiveStateFilter.ToLower();
            bool activeStateFilterBool = activeStateFilterNormalized == "active";

            allLectures = allLectures
                .Where(lecture => lecture.IsScheduled == activeStateFilterBool);
        }

        if (string.IsNullOrEmpty(UploadStateFilter) == false && UploadStateFilter.ToLower() != "all")
        {
            var uploadStateFilterNormalized = UploadStateFilter.ToLower();
            bool uploadStateFilterBool = uploadStateFilterNormalized == "upload";

            allLectures = allLectures
                .Where(lecture => lecture.WillAutoUpload = uploadStateFilterBool);
        }

        return allLectures
            .OrderBy(lecture => lecture.Day)
            .ThenBy(lecture => lecture.StartTime)
            .ThenBy(lecture => lecture.SubjectName);
    }
    
    private async Task LoadLectures()
    {
        _allLecturesSorted = (await _scheduledLectureRepository
            .GetScheduledLecturesOrdered())?.ToArray();

        var filteredLectures = FilterLectures(_allLecturesSorted);

        if (filteredLectures is null) return;
        
        AssignLecturesByDay(filteredLectures.ToArray());
    }
}