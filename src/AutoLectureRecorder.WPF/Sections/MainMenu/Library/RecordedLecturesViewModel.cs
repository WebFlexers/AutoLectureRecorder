using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.Services.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace AutoLectureRecorder.Sections.MainMenu.Library;

public class RecordedLecturesViewModel : ReactiveObject, IRoutableViewModel
{
    private readonly ILogger<RecordedLecturesViewModel> _logger;
    private readonly IScheduledLecturesRepository _scheduledLecturesRepository;
    private readonly IRecordedLecturesRepository _recordedLectureRepository;
    public string UrlPathSegment => nameof(RecordedLecturesViewModel);
    public IScreen HostScreen { get; }

    [Reactive]
    public ReactiveScheduledLecture? ScheduledLecture { get; set; }
    [Reactive]
    public ObservableCollection<ReactiveRecordedLecture> RecordedLectures { get; set; }

    /// <summary>
    /// Initializes the view model with the given parameters
    /// </summary>
    public async Task Initialize(int lectureId)
    {
        ScheduledLecture = await _scheduledLecturesRepository.GetScheduledLectureByIdAsync(lectureId);
        var recordedLectures = await _recordedLectureRepository.GetRecordedLecturesFromIdAsync(lectureId);
        if (recordedLectures != null && recordedLectures.Any())
        {
            RecordedLectures = new ObservableCollection<ReactiveRecordedLecture>(recordedLectures);
        }
    }

    public RecordedLecturesViewModel(ILogger<RecordedLecturesViewModel> logger, IScreenFactory screenFactory,
        IScheduledLecturesRepository scheduledLecturesRepository, IRecordedLecturesRepository recordedLectureRepository)
    {
        _logger = logger;
        _scheduledLecturesRepository = scheduledLecturesRepository;
        _recordedLectureRepository = recordedLectureRepository;
        HostScreen = screenFactory.GetMainMenuViewModel();
    }
}
