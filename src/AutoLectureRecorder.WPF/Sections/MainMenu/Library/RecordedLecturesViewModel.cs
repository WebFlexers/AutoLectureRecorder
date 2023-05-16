using AutoLectureRecorder.Data.ReactiveModels;
using AutoLectureRecorder.DependencyInjection.Factories.Interfaces;
using AutoLectureRecorder.Services.DataAccess.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace AutoLectureRecorder.Sections.MainMenu.Library;

public class RecordedLecturesViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();
    private CompositeDisposable _disposables = new();
    
    private readonly ILogger<RecordedLecturesViewModel> _logger;
    private readonly IScheduledLectureRepository _scheduledLectureRepository;
    private readonly IRecordedLectureRepository _recordedLectureRepository;
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
        ScheduledLecture = await _scheduledLectureRepository.GetScheduledLectureById(lectureId);
        var recordedLectures = await _recordedLectureRepository.GetRecordedLecturesFromIdAsync(lectureId);
        if (recordedLectures != null && recordedLectures.Any())
        {
            RecordedLectures = new ObservableCollection<ReactiveRecordedLecture>(recordedLectures);
        }
    }

    public RecordedLecturesViewModel(ILogger<RecordedLecturesViewModel> logger, IScreenFactory screenFactory,
        IScheduledLectureRepository scheduledLectureRepository, IRecordedLectureRepository recordedLectureRepository)
    {
        _logger = logger;
        _scheduledLectureRepository = scheduledLectureRepository;
        _recordedLectureRepository = recordedLectureRepository;
        HostScreen = screenFactory.GetMainMenuViewModel();

        this.WhenActivated(disposables =>
        {
            _disposables.DisposeWith(disposables);
        });
    }
}
