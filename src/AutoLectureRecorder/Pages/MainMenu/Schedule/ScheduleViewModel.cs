using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Common.Core;
using AutoLectureRecorder.Common.Navigation;
using AutoLectureRecorder.Domain.ReactiveModels;
using ReactiveUI;

namespace AutoLectureRecorder.Pages.MainMenu.Schedule;

public class ScheduleViewModel : RoutableViewModel, IActivatableViewModel
{
    private readonly IScheduledLectureRepository _scheduledLectureRepository;
    public ViewModelActivator Activator { get; }

    private IEnumerable<ReactiveScheduledLecture>? _allLectures;
    
    #region Filtered Lectures Observable Collections

    private ObservableCollection<ReactiveScheduledLecture> _mondayFilteredLectures;
    public ObservableCollection<ReactiveScheduledLecture> MondayFilteredLectures
    {
        get => _mondayFilteredLectures;
        set => this.RaiseAndSetIfChanged(ref _mondayFilteredLectures, value);
    }
    
    private ObservableCollection<ReactiveScheduledLecture> _tuesdayFilteredLectures;
    public ObservableCollection<ReactiveScheduledLecture> TuesdayFilteredLectures
    {
        get => _tuesdayFilteredLectures;
        set => this.RaiseAndSetIfChanged(ref _tuesdayFilteredLectures, value);
    }
    
    private ObservableCollection<ReactiveScheduledLecture> _wednesdayFilteredLectures;
    public ObservableCollection<ReactiveScheduledLecture> WednesdayFilteredLectures
    {
        get => _wednesdayFilteredLectures;
        set => this.RaiseAndSetIfChanged(ref _wednesdayFilteredLectures, value);
    }
    
    private ObservableCollection<ReactiveScheduledLecture> _thursdayFilteredLectures;
    public ObservableCollection<ReactiveScheduledLecture> ThursdayFilteredLectures
    {
        get => _thursdayFilteredLectures;
        set => this.RaiseAndSetIfChanged(ref _thursdayFilteredLectures, value);
    }
    
    private ObservableCollection<ReactiveScheduledLecture> _fridayFilteredLectures;
    public ObservableCollection<ReactiveScheduledLecture> FridayFilteredLectures
    {
        get => _fridayFilteredLectures;
        set => this.RaiseAndSetIfChanged(ref _fridayFilteredLectures, value);
    }
    
    private ObservableCollection<ReactiveScheduledLecture> _saturdayFilteredLectures;
    public ObservableCollection<ReactiveScheduledLecture> SaturdayFilteredLectures
    {
        get => _saturdayFilteredLectures;
        set => this.RaiseAndSetIfChanged(ref _saturdayFilteredLectures, value);
    }
    
    private ObservableCollection<ReactiveScheduledLecture> _sundayFilteredLectures;
    public ObservableCollection<ReactiveScheduledLecture> SundayFilteredLectures
    {
        get => _sundayFilteredLectures;
        set => this.RaiseAndSetIfChanged(ref _sundayFilteredLectures, value);
    }

    #endregion

    #region Filters

    private string _subjectNameSearchFilter;
    public string SubjectNameSearchFilter
    {
        get => _subjectNameSearchFilter; 
        set => this.RaiseAndSetIfChanged(ref _subjectNameSearchFilter, value);
    }
    
    private string _semesterFilter;
    public string SemesterFilter 
    { 
        get => _semesterFilter; 
        set => this.RaiseAndSetIfChanged(ref _semesterFilter, value); 
    }
    
    private string _activeStateFilter = "Active";
    public string ActiveStateFilter 
    { 
        get => _activeStateFilter; 
        set => this.RaiseAndSetIfChanged(ref _activeStateFilter, value); 
    }
    
    private string _uploadStateFilterFilter;
    public string UploadStateFilter 
    { 
        get => _uploadStateFilterFilter; 
        set => this.RaiseAndSetIfChanged(ref _uploadStateFilterFilter, value); 
    }

    #endregion
    
    public ScheduleViewModel(INavigationService navigationService, IScheduledLectureRepository scheduledLectureRepository) 
        : base(navigationService)
    {
        Activator = new();
        
        _scheduledLectureRepository = scheduledLectureRepository;

        var loadLecturesObservable = Observable.FromAsync(LoadLectures)
            .Subscribe();
        
        this.WhenActivated(disposables =>
        {
            loadLecturesObservable.DisposeWith(disposables);
        });
       
    }

    private void AssignLecturesByDay(ReactiveScheduledLecture[] lectures)
    {
        MondayFilteredLectures = new ObservableCollection<ReactiveScheduledLecture>(
            lectures.Where(lecture => lecture.Day == DayOfWeek.Monday));
        TuesdayFilteredLectures = new ObservableCollection<ReactiveScheduledLecture>(
            lectures.Where(lecture => lecture.Day == DayOfWeek.Tuesday));
        WednesdayFilteredLectures = new ObservableCollection<ReactiveScheduledLecture>(
            lectures.Where(lecture => lecture.Day == DayOfWeek.Wednesday));
        ThursdayFilteredLectures = new ObservableCollection<ReactiveScheduledLecture>(
            lectures.Where(lecture => lecture.Day == DayOfWeek.Thursday));
        FridayFilteredLectures = new ObservableCollection<ReactiveScheduledLecture>(
            lectures.Where(lecture => lecture.Day == DayOfWeek.Friday));
        SaturdayFilteredLectures = new ObservableCollection<ReactiveScheduledLecture>(
            lectures.Where(lecture => lecture.Day == DayOfWeek.Saturday));
        SundayFilteredLectures = new ObservableCollection<ReactiveScheduledLecture>(
            lectures.Where(lecture => lecture.Day == DayOfWeek.Sunday));
    }
    
    private List<ReactiveScheduledLecture> _allFilteredLectures = new();
    private void FilterLectures(IEnumerable<ReactiveScheduledLecture>? allLectures)
    {
        if (allLectures is null) return;
        
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

        AssignLecturesByDay(allLectures.ToArray());
    }
    
    private async Task LoadLectures()
    {
        _allLectures = await _scheduledLectureRepository
            .GetAllScheduledLectures();

        FilterLectures(_allLectures?.ToList());
    }
}