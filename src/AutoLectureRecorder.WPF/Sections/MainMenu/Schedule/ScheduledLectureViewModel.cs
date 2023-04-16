using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.ReactiveUiUtilities;
using AutoLectureRecorder.Sections.MainMenu.CreateLecture;
using AutoLectureRecorder.Services.DataAccess.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace AutoLectureRecorder.Sections.MainMenu.Schedule;

public class ScheduledLectureViewModel : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();

    private readonly IScheduledLectureRepository _lectureRepository;
    private readonly IViewModelFactory _viewModelFactory;

    [Reactive]
    public ReactiveScheduledLecture? ScheduledLecture { get; set; }

    public ReactiveCommand<Unit, Unit> UpdateScheduledLectureCommand { get; }
    public ReactiveCommand<Unit, Unit>? NavigateToCreateLectureCommand { get; }

    public string DisplayTime => _displayTime.Value;
    private ObservableAsPropertyHelper<string> _displayTime;

    public ScheduledLectureViewModel(IScheduledLectureRepository lectureRepository, IViewModelFactory viewModelFactory)
    {
        _lectureRepository = lectureRepository;
        _viewModelFactory = viewModelFactory;

        NavigateToCreateLectureCommand = ReactiveCommand.Create(() =>
        {
            var mainMenuVm = _viewModelFactory.CreateRoutableViewModel(typeof(MainMenuViewModel));
            ((MainMenuViewModel)mainMenuVm).SetRoutedViewHostContent(typeof(CreateLectureViewModel));
            MessageBus.Current.SendMessage(ScheduledLecture, PubSubMessages.SetUpdateModeToScheduledLecture);
        });

        var isFirstTime = true;
        UpdateScheduledLectureCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (isFirstTime)
            {
                // This is to avoid updating the Scheduled Lecture in the database
                // when the ViewModel is initialized, because of the WhenAnyValue
                // on the scheduled lecture
                isFirstTime = false;
                return;
            }
            if (ScheduledLecture == null) return;

            var result = await _lectureRepository.UpdateScheduledLectureAsync(ScheduledLecture);

            if (result == false) return;

            MessageBus.Current.SendMessage(ScheduledLecture, PubSubMessages.DisableConflictingLectures);
        });

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(vm => vm.ScheduledLecture.IsScheduled,
                    vm => vm.ScheduledLecture.WillAutoUpload)
                .Select(args => Unit.Default)
                .InvokeCommand(UpdateScheduledLectureCommand)
                .DisposeWith(disposables);

            _displayTime = 
                this.WhenAnyValue(vm => vm.ScheduledLecture.StartTime, 
                                        vm => vm.ScheduledLecture.EndTime)
                    .Select(times => 
                        $"{times.Item1!.Value.ToString("hh:mm tt")} - {times.Item2!.Value.ToString("hh:mm tt")}")
                    .ToProperty(this, vm => vm.DisplayTime);
        });
    }

}
