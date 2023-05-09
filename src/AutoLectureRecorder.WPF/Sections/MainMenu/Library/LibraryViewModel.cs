using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.Services.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace AutoLectureRecorder.Sections.MainMenu.Library;

public class LibraryViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();

    private readonly ILogger<LibraryViewModel> _logger;
    private readonly IScheduledLectureRepository _scheduledLectureRepository;

    public string UrlPathSegment => nameof(LibraryViewModel);
    public IScreen HostScreen { get; }

    [Reactive]
    public ObservableCollection<ReactiveScheduledLecture>? LecturesBySemester { get; set; }

    public LibraryViewModel(ILogger<LibraryViewModel> logger, IScreenFactory screenFactory, 
        IScheduledLectureRepository scheduledLectureRepository)
    {
        _logger = logger;
        _scheduledLectureRepository = scheduledLectureRepository;
        HostScreen = screenFactory.GetMainMenuViewModel();

        Observable.FromAsync(FetchScheduledLecturesBySemester)
            .Subscribe();
    }

    private async Task FetchScheduledLecturesBySemester()
    {
        var lecturesBySemester = await _scheduledLectureRepository.GetScheduledLecturesOrderedBySemesterAsync();
        if (lecturesBySemester == null || lecturesBySemester.Any() == false) return;

        LecturesBySemester = new ObservableCollection<ReactiveScheduledLecture>(lecturesBySemester);
    }
}
