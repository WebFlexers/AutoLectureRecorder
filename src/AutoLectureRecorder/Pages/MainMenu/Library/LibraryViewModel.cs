using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Common.Core;
using AutoLectureRecorder.Common.Navigation;
using AutoLectureRecorder.Common.Navigation.Parameters;
using AutoLectureRecorder.Domain.ReactiveModels;
using ReactiveUI;

namespace AutoLectureRecorder.Pages.MainMenu.Library;

public class LibraryViewModel : RoutableViewModel, IActivatableViewModel
{
    private readonly CompositeDisposable _disposables = new();
    public ViewModelActivator Activator { get; } = new();
    
    private readonly IScheduledLectureRepository _scheduledLectureRepository;

    
    public ObservableCollection<ReactiveScheduledLecture>? LecturesBySemester { get; set; }
    
    private ObservableAsPropertyHelper<bool> _doAnyLecturesExist;
    public bool DoAnyLecturesExist => _doAnyLecturesExist.Value;

    public ReactiveCommand<ReactiveScheduledLecture, Unit> NavigateToRecordedLecturesCommand { get; set; }

    public LibraryViewModel(IScheduledLectureRepository scheduledLectureRepository, 
        INavigationService navigationService) 
        : base(navigationService)
    {
        _scheduledLectureRepository = scheduledLectureRepository;

        NavigateToRecordedLecturesCommand = ReactiveCommand.Create<ReactiveScheduledLecture>(lecture =>
        {
            var parameters = new Dictionary<string, object>
            {
                { NavigationParameters.Library.Lecture, lecture }
            };
            NavigationService.Navigate(typeof(RecordedLecturesViewModel), HostNames.MainMenuHost, parameters);
        });

        Observable.FromAsync(FetchScheduledLecturesBySemester)
            .Subscribe().DisposeWith(_disposables);

        this.WhenActivated(disposables =>
        {
            _doAnyLecturesExist =
                this.WhenAnyValue(vm => vm.LecturesBySemester)
                    .Select(lectures => lectures is not null && lectures.Count > 0)
                    .ToProperty(this, vm => vm.DoAnyLecturesExist)
                    .DisposeWith(_disposables);
            
            _disposables.DisposeWith(disposables);
        });
    }

    private async Task FetchScheduledLecturesBySemester()
    {
        var lecturesBySemester = await 
            _scheduledLectureRepository.GetScheduledLecturesOrderedBySemester();
        
        if (lecturesBySemester is null) return;

        LecturesBySemester = new ObservableCollection<ReactiveScheduledLecture>(lecturesBySemester);
    }
}
