using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    
    private List<ReactiveScheduledLecture> _filteredLectures;
    
    public ViewModelActivator Activator { get; }
    private bool _isLoaded = false;
    
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

    public ReactiveCommand<Unit, Unit> LoadLecturesCommand { get; }
    public ReactiveCommand<Unit, Unit> NavigateToCreateLecture { get; }
    public ReactiveCommand<ReactiveScheduledLecture, Unit> NavigateToEditLecture { get; }
    public ReactiveCommand<ReactiveScheduledLecture, Unit> ModifyLectureStateCommand { get; }
    public ReactiveCommand<bool, Unit> ChangeSelectedLecturesCountCommand { get; }
    public ReactiveCommand<Unit, Unit> AttemptToDeleteLecturesCommand { get; }
    public ReactiveCommand<Unit, Unit> DeleteLecturesCommand { get; }
    public ReactiveCommand<Unit, Unit> ChangeAllLecturesSelectionCommand { get; }

    public Dictionary<DayOfWeek, ObservableCollection<LectureViewModel>?> FilteredLecturesByDay { get; } = new()
    {
        { DayOfWeek.Monday, new() },
        { DayOfWeek.Tuesday, new() },
        { DayOfWeek.Wednesday, new() },
        { DayOfWeek.Thursday, new() },
        { DayOfWeek.Friday, new() },
        { DayOfWeek.Saturday, new() },
        { DayOfWeek.Sunday, new() },
    };

    #region Filters

    private string? _subjectNameSearchFilter;
    public string? SubjectNameSearchFilter
    {
        get => _subjectNameSearchFilter; 
        set => this.RaiseAndSetIfChanged(ref _subjectNameSearchFilter, value);
    }
    
    private string? _semesterFilter = "All";
    public string? SemesterFilter 
    { 
        get => _semesterFilter; 
        set => this.RaiseAndSetIfChanged(ref _semesterFilter, value); 
    }
    
    private string? _activeStateFilter = "All";
    public string? ActiveStateFilter 
    { 
        get => _activeStateFilter; 
        set => this.RaiseAndSetIfChanged(ref _activeStateFilter, value); 
    }
    
    private string? _uploadStateFilterFilter = "All";
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

        LoadLecturesCommand = ReactiveCommand.CreateFromTask(LoadLectures);
        
        NavigateToCreateLecture = ReactiveCommand.Create(() =>
        {
            persistentValidationContext.RemoveAllValidationParameters();
            var parameters = new Dictionary<string, object>
                { { NavigationParameters.CreateLecture.IsUpdateMode, false } };
            NavigationService.Navigate(typeof(CreateLectureViewModel), HostNames.MainMenuHost, parameters);
        });

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
            // When updating a lecture we want to ignore overlapping lectures during validation,
            // since they will be disabled later
            persistentValidationContext.AddValidationParameter(
                ValidationParameters.ScheduledLectures.IgnoreOverlappingLectures, true);
            persistentValidationContext.AddValidationParameter(
                ValidationParameters.ScheduledLectures.IsOnUpdateMode, true);
                
            var result = await mediatorSender.Send(lecture.MapToUpdateCommand());
            if (result.IsError) return;

            List<ReactiveScheduledLecture>? deactivatedLectures = result.Value;

            if (deactivatedLectures is null) return;

            UpdateLectures(lecture.Day!.Value, deactivatedLectures);
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
        
        ChangeAllLecturesSelectionCommand = ReactiveCommand.Create(() =>
        {
            var shouldGetSelected = SelectedLecturesCount != AllLecturesCount;
            
            foreach (var keyValuePair in FilteredLecturesByDay)
            {
                for (int lectureIndex = 0; lectureIndex < keyValuePair.Value!.Count; lectureIndex++)
                {
                    var scheduledLecture = keyValuePair.Value[lectureIndex].ScheduledLecture;
                    keyValuePair.Value[lectureIndex] = new LectureViewModel(scheduledLecture, shouldGetSelected);
                }
            }
            
            SelectedLecturesCount = shouldGetSelected ? AllLecturesCount : 0;
        });

        var filterLecturesCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (_allLecturesSorted is null) return;
                    
            var filteredLectures = FilterLectures(_allLecturesSorted);
            await AssignLecturesToDaysCollections(filteredLectures);
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
        
        var loadLecturesObservable = Observable.FromAsync(LoadLectures)
            .Subscribe();
        
        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(vm => vm.SubjectNameSearchFilter)
                .Where(_ => _isLoaded)
                .Throttle(TimeSpan.FromMilliseconds(300))
                .DistinctUntilChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Select(_ => Unit.Default)
                .InvokeCommand(filterLecturesCommand)
                .DisposeWith(disposables);
            
            this.WhenAnyValue(vm => vm.SemesterFilter,
                    vm => vm.ActiveStateFilter, vm => vm.UploadStateFilter)
                .Where(_ => _isLoaded)
                .DistinctUntilChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Select(_ => Unit.Default)
                .InvokeCommand(filterLecturesCommand)
                .DisposeWith(disposables);
            
            loadLecturesObservable.DisposeWith(disposables);
        });
    }

    /// <summary>
    /// Modifies the IsScheduled and WillAutoUpload properties of the lectures of the given day that correspond to the
    /// given list of modified lectures
    /// </summary>
    private void UpdateLectures(DayOfWeek day, List<ReactiveScheduledLecture> modifiedLectures)
    {
        if (_allLecturesSorted is null) return;
        
        foreach (var modifiedLecture in CollectionsMarshal.AsSpan(modifiedLectures))
        {
            // Update all lectures list
            for (int i = 0; i < _allLecturesSorted.Length; i++)
            {
                if (_allLecturesSorted[i].Id == modifiedLecture.Id)
                {
                    _allLecturesSorted[i].IsScheduled = modifiedLecture.IsScheduled;
                    _allLecturesSorted[i].WillAutoUpload = modifiedLecture.WillAutoUpload;
                    break;
                }
            }
            
            // Update observable collections
            int? modifiedLectureIndex = null;

            for (int i = 0; i < FilteredLecturesByDay[day]?.Count; i++)
            {
                if (FilteredLecturesByDay[day]?[i].ScheduledLecture.Id != modifiedLecture.Id) continue;
                
                modifiedLectureIndex = i;
                break;
            }

            if (modifiedLectureIndex.HasValue == false) continue;

            var existingLectureVm = FilteredLecturesByDay[day]?[modifiedLectureIndex.Value];
            if (existingLectureVm is null) continue;    
            
            existingLectureVm.ScheduledLecture.IsScheduled = modifiedLecture.IsScheduled;
            existingLectureVm.ScheduledLecture.WillAutoUpload = modifiedLecture.WillAutoUpload;
        
            // Create a new instance of LectureViewModel and update it in the list in order for the observable collection
            // to propagate the change.
            var updatedLectureVm = new LectureViewModel(existingLectureVm.ScheduledLecture, existingLectureVm.IsSelected);
            FilteredLecturesByDay[day]![modifiedLectureIndex.Value] = updatedLectureVm;
        }
    }
    
    /// <summary>
    /// Assigns each lecture in the given array of ReactiveScheduledLecture to the corresponding ObservableCollection
    /// based on the day of the lecture
    /// </summary>
    private async Task AssignLecturesToDaysCollections(IEnumerable<ReactiveScheduledLecture> lectures)
    {
        // Clear previous lectures to load the new ones
        foreach (var keyValuePair in FilteredLecturesByDay)
        {
            FilteredLecturesByDay[keyValuePair.Key]!.Clear();
        }

        // To prevent a freezing screen when loading a big amount of lectures we will lazy load the lectures and
        // wait 15 milliseconds between loading each lecture. For the best user experience we will load the lectures
        // horizontally by day. That means that the first lecture of each day will be loaded and shown to the screen, 
        // then the second lecture of each day and so on, until all the lectures are finished loading
        
        // First lets group the lectures by day
        var lecturesByDay = lectures
            .GroupBy(lecture => lecture.Day)
            .ToDictionary(g => g.Key!.Value, 
                g => g.ToArray());

        // This is needed to know when to stop the loop. If the loop goes through all the days and no lecture is loaded
        // it means that there are no lectures left to load
        bool lectureIsLoadedAtCurrentIndex = false;
        
        // This is the index we will use to load the lectures
        int lectureIndex = 0;
        
        do
        {
            // Because the DayOfWeek starts with Sunday, but we want to start from Monday we do the following:
            // If a Sunday lecture is found, instead of adding it to the ObservableCollection and therefore loading it
            // on the screen immediately, we store it in this variable. After the lecture of the given index in each day
            // is finished loading then we add the Sunday lecture as well
            LectureViewModel? sundayLectureVm = null;
            
            foreach (var dayLectures in lecturesByDay)
            {
                bool lectureExistsAtIndex = lectureIndex < dayLectures.Value.Length;

                if (lectureExistsAtIndex)
                {
                    lectureIsLoadedAtCurrentIndex = true;

                    if (dayLectures.Key == DayOfWeek.Sunday)
                    {
                        sundayLectureVm = new LectureViewModel(dayLectures.Value[lectureIndex], false);
                    }
                    else
                    {
                        FilteredLecturesByDay[dayLectures.Key]!
                            .Add(new LectureViewModel(dayLectures.Value[lectureIndex], false));
                        await Task.Delay(15);
                    }
                }
                else
                {
                    lectureIsLoadedAtCurrentIndex = false;
                }
            }

            if (sundayLectureVm is not null)
            {
                FilteredLecturesByDay[DayOfWeek.Sunday]!
                    .Add(sundayLectureVm);   
                await Task.Delay(15);
            }

            lectureIndex++;
        } while (lectureIsLoadedAtCurrentIndex);
    }
    
    private IEnumerable<ReactiveScheduledLecture> FilterLectures(IEnumerable<ReactiveScheduledLecture> lectures)
    {
        if (string.IsNullOrWhiteSpace(SubjectNameSearchFilter) == false)
        {
            lectures = lectures
                .Where(lecture => lecture.SubjectName!.ToLower().Contains(SubjectNameSearchFilter.ToLower()));
        }

        if (string.IsNullOrEmpty(SemesterFilter) == false && SemesterFilter.ToLower() != "all")
        {
            lectures = lectures
                .Where(lecture => lecture.Semester == Convert.ToInt32(SemesterFilter));
        }

        if (string.IsNullOrEmpty(ActiveStateFilter) == false && ActiveStateFilter.ToLower() != "all")
        {
            var activeStateFilterNormalized = ActiveStateFilter.ToLower();
            bool activeStateFilterBool = activeStateFilterNormalized == "active";

            lectures = lectures
                .Where(lecture => lecture.IsScheduled == activeStateFilterBool);
        }

        if (string.IsNullOrEmpty(UploadStateFilter) == false && UploadStateFilter.ToLower() != "all")
        {
            var uploadStateFilterNormalized = UploadStateFilter.ToLower();
            bool uploadStateFilterBool = uploadStateFilterNormalized == "upload";

            lectures = lectures
                .Where(lecture => lecture.WillAutoUpload == uploadStateFilterBool);
        }

        return lectures
            .OrderBy(lecture => lecture.Day)
            .ThenBy(lecture => lecture.StartTime)
            .ThenBy(lecture => lecture.SubjectName);
    }
    
    private async Task LoadLectures()
    {
        _allLecturesSorted = (await _scheduledLectureRepository
            .GetScheduledLecturesOrdered())?.ToArray();

        if (_allLecturesSorted is null) return;
        
        _filteredLectures = FilterLectures(_allLecturesSorted).ToList();

        await AssignLecturesToDaysCollections(_filteredLectures);

        _isLoaded = true;
    }
}