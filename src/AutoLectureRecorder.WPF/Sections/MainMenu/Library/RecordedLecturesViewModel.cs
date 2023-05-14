using System.Threading.Tasks;
using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.Services.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AutoLectureRecorder.Sections.MainMenu.Library;

public class RecordedLecturesViewModel : ReactiveObject, IRoutableViewModel
{
    private readonly ILogger<RecordedLecturesViewModel> _logger;
    private readonly IScheduledLectureRepository _lectureRepository;
    public string UrlPathSegment => nameof(RecordedLecturesViewModel);
    public IScreen HostScreen { get; }

    [Reactive]
    public ReactiveScheduledLecture? ScheduledLecture { get; set; }

    /// <summary>
    /// Initializes the view model with the given parameters
    /// </summary>
    public async Task Initialize(int lectureId)
    {
        ScheduledLecture = await _lectureRepository.GetScheduledLectureByIdAsync(lectureId);
    }

    public RecordedLecturesViewModel(ILogger<RecordedLecturesViewModel> logger, IScreenFactory screenFactory,
        IScheduledLectureRepository lectureRepository)
    {
        _logger = logger;
        _lectureRepository = lectureRepository;
        HostScreen = screenFactory.GetMainMenuViewModel();
    }
}
