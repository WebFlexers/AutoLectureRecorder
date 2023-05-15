using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.Services.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AutoLectureRecorder.ReactiveUiUtilities;
using System.Windows;

namespace AutoLectureRecorder.Sections.MainMenu.Library;

public class LibraryViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    private CompositeDisposable _disposables = new();
    public ViewModelActivator Activator { get; } = new();

    private readonly ILogger<LibraryViewModel> _logger;
    private readonly IScheduledLectureRepository _scheduledLectureRepository;

    public string UrlPathSegment => nameof(LibraryViewModel);
    public IScreen HostScreen { get; }

    [Reactive]
    public ObservableCollection<ReactiveScheduledLecture>? LecturesBySemester { get; set; }

    public ReactiveCommand<int, Unit> NavigateToRecordedLecturesCommand { get; set; }

    public LibraryViewModel(ILogger<LibraryViewModel> logger, IScreenFactory screenFactory, 
        IViewModelFactory viewModelFactory, IScheduledLectureRepository scheduledLectureRepository)
    {
        var mainMenuViewModel = screenFactory.GetMainMenuViewModel();

        _logger = logger;
        _scheduledLectureRepository = scheduledLectureRepository;
        HostScreen = mainMenuViewModel;

        NavigateToRecordedLecturesCommand = ReactiveCommand.CreateFromTask<int>(async lectureId =>
        {
            var recordedLecturesViewModel = await viewModelFactory.CreateRecordedLecturesViewModel(lectureId);
            mainMenuViewModel.Navigate(recordedLecturesViewModel);
        });

        Observable.FromAsync(FetchScheduledLecturesBySemester)
            .Subscribe().DisposeWith(_disposables);

        this.WhenActivated(disposables =>
        {
            _disposables.DisposeWith(disposables);
        });
    }

    private async Task FetchScheduledLecturesBySemester()
    {
        var lecturesBySemester = await _scheduledLectureRepository.GetScheduledLecturesOrderedBySemesterAsync();
        if (lecturesBySemester == null || lecturesBySemester.Any() == false) return;

        LecturesBySemester = new ObservableCollection<ReactiveScheduledLecture>(lecturesBySemester);
    }
}
