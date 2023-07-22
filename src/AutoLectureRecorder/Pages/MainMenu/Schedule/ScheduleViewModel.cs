using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Application.Common.Abstractions.Validation;
using AutoLectureRecorder.Application.ScheduledLectures.Commands.UpdateScheduledLecture.Mapping;
using AutoLectureRecorder.Application.ScheduledLectures.Common;
using AutoLectureRecorder.Common.Core;
using AutoLectureRecorder.Common.Navigation;
using AutoLectureRecorder.Common.Navigation.Parameters;
using AutoLectureRecorder.Domain.ReactiveModels;
using AutoLectureRecorder.Pages.MainMenu.CreateLecture;
using MediatR;
using ReactiveUI;
using Unit = System.Reactive.Unit;

namespace AutoLectureRecorder.Pages.MainMenu.Schedule;

public class ScheduleViewModel : RoutableViewModel, IActivatableViewModel
{
    private readonly CompositeDisposable _disposables;
    
    public ViewModelActivator Activator { get; }

    public ObservableCollection<ReactiveScheduledLecture> AllScheduledLectures { get; set; } = new();
    public ReactiveCommand<ReactiveScheduledLecture, Unit> UpdateScheduledLectureCommand { get; private set; }
    public ReactiveCommand<ReactiveScheduledLecture, Unit> NavigateToUpdateScheduledLecture { get; private set; }

    public ScheduleViewModel(INavigationService navigationService, ISender mediatorSender,
        IScheduledLectureRepository scheduledLectureRepository, IPersistentValidationContext persistentValidationContext)
        : base(navigationService)
    {
        _disposables = new CompositeDisposable();
        Activator = new ViewModelActivator();
        
        UpdateScheduledLectureCommand = ReactiveCommand.CreateFromTask<ReactiveScheduledLecture, Unit>(
        async lecture =>
        {
            persistentValidationContext.AddValidationParameter(
                ValidationParameters.ScheduledLectures.IgnoreOverlappingLectures,
                true);
            var errorOrConflictingLectureIds = await mediatorSender.Send(lecture.MapToUpdateCommand());
            if (errorOrConflictingLectureIds.IsError == false)
            {
                var conflictingLectures = errorOrConflictingLectureIds.Value;
                if (conflictingLectures is null) return Unit.Default;
                
                foreach (var conflictingLecture in conflictingLectures)
                {
                    AllScheduledLectures.First(currentLecture => currentLecture.Id == conflictingLecture.Id)
                        .IsScheduled = conflictingLecture.IsScheduled;
                }
            }
            
            return Unit.Default;
        });

        NavigateToUpdateScheduledLecture = ReactiveCommand.Create<ReactiveScheduledLecture, Unit>(
             lecture =>
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
        }).Subscribe()
        .DisposeWith(_disposables);

        this.WhenActivated(disposables =>
        {
            _disposables.DisposeWith(disposables);
        });
    }
}