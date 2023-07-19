using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Common.Core;
using AutoLectureRecorder.Common.Navigation;
using AutoLectureRecorder.Common.Navigation.Parameters;
using AutoLectureRecorder.Domain.ReactiveModels;
using AutoLectureRecorder.Pages.MainMenu.CreateLecture;
using DynamicData;
using DynamicData.Binding;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace AutoLectureRecorder.Pages.MainMenu.Schedule;

public class ScheduleViewModel : RoutableViewModel, IActivatableViewModel
{
    private readonly CompositeDisposable _disposables;
    
    public ViewModelActivator Activator { get; }
    
    public ObservableCollection<ReactiveScheduledLecture> AllScheduledLectures { get; set; }
    public ReactiveCommand<ReactiveScheduledLecture, Unit> UpdateScheduledLectureCommand { get; private set; }
    public ReactiveCommand<ReactiveScheduledLecture, Unit> NavigateToUpdateScheduledLecture { get; private set; }

    public ScheduleViewModel(INavigationService navigationService, IScheduledLectureRepository scheduledLectureRepository,
        ILogger<ScheduleViewModel> logger) 
        : base(navigationService)
    {
        _disposables = new CompositeDisposable();
        Activator = new ViewModelActivator();
        
        UpdateScheduledLectureCommand = ReactiveCommand.CreateFromTask<ReactiveScheduledLecture, Unit>(
        async lecture =>
        {
            await scheduledLectureRepository.UpdateScheduledLecture(lecture);
            return Unit.Default;
        });

        NavigateToUpdateScheduledLecture = ReactiveCommand.CreateFromTask<ReactiveScheduledLecture, Unit>(
            async lecture =>
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add(NavigationParameters.CreateLecture.IsUpdateMode, true);
            parameters.Add(NavigationParameters.CreateLecture.ScheduledLectureToUpdate, lecture);
            
            NavigationService.Navigate(typeof(CreateLectureViewModel), HostNames.MainMenuHost, parameters);
            return Unit.Default;
        });
        
        Observable.FromAsync(async () =>
        {
            var fetchedLectures =
                await scheduledLectureRepository.GetScheduledLecturesOrderedByDayAndStartTime();
            
            if (fetchedLectures is null) return;
            
            AllScheduledLectures = new ObservableCollection<ReactiveScheduledLecture>(fetchedLectures);
            
            AllScheduledLectures.ToObservableChangeSet()
                .WhenPropertyChanged(scheduledLecture => scheduledLecture.IsScheduled)
                .Skip(AllScheduledLectures.Count)
                .Select(x => x.Sender)
                .InvokeCommand(UpdateScheduledLectureCommand)
                .DisposeWith(_disposables);
            
            AllScheduledLectures.ToObservableChangeSet()
                .WhenPropertyChanged(scheduledLecture => scheduledLecture.WillAutoUpload)
                .Skip(AllScheduledLectures.Count)
                .Select(x => x.Sender)
                .InvokeCommand(UpdateScheduledLectureCommand)
                .DisposeWith(_disposables);
            
        }).Subscribe()
        .DisposeWith(_disposables);

        this.WhenActivated(disposables =>
        {
            _disposables.DisposeWith(disposables);
        });
    }
}