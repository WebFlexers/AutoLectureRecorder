using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Forms;
using AutoLectureRecorder.Application.Recording.Common;
using AutoLectureRecorder.Application.Recording.Queries.RecordedLecturesInformation;
using AutoLectureRecorder.Common.Core;
using AutoLectureRecorder.Common.Navigation;
using AutoLectureRecorder.Common.Navigation.Parameters;
using AutoLectureRecorder.Domain.ReactiveModels;
using MediatR;
using ReactiveUI;
using Unit = System.Reactive.Unit;

namespace AutoLectureRecorder.Pages.MainMenu.Library;

public class RecordedLecturesViewModel : RoutableViewModel, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();
    private readonly CompositeDisposable _disposables = new();

    private ReactiveScheduledLecture _scheduledLecture;
    public ReactiveScheduledLecture ScheduledLecture
    {
        get => _scheduledLecture;
        set => this.RaiseAndSetIfChanged(ref _scheduledLecture, value);
    }

    private ObservableCollection<AlrVideoInformation>? _recordedLectures;
    public ObservableCollection<AlrVideoInformation>? RecordedLectures
    {
        get => _recordedLectures;
        set => this.RaiseAndSetIfChanged(ref _recordedLectures, value);
    }
    
    public ReactiveCommand<string, Unit> OpenVideoLocallyCommand { get; private set; }

    public RecordedLecturesViewModel(INavigationService navigationService, ISender mediatorSender) 
        : base(navigationService)
    {
        var parameters = navigationService.GetNavigationParameters(typeof(RecordedLecturesViewModel));
        _scheduledLecture = (ReactiveScheduledLecture)parameters![NavigationParameters.Library.Lecture];

        OpenVideoLocallyCommand = ReactiveCommand.Create<string, Unit>(videoPath =>
        {
            if (File.Exists(videoPath))
            {
                var videoProcessStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = $"/c start \"\" \"{videoPath}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                Process.Start(videoProcessStartInfo);
                return Unit.Default;
            }

            MessageBox.Show("The specified recorded lecture could not be found. " +
                            "Make sure you haven't moved the file, or removed the directory from the recording directories" +
                            "in general settings", "Failed to access local video", MessageBoxButtons.OK, MessageBoxIcon.Error);
            
            return Unit.Default;
        });

        Observable.FromAsync(async () =>
        {
            var query = new RecordedLecturesInformationQuery(ScheduledLecture.SubjectName!, ScheduledLecture.Semester);
            var result = await mediatorSender.Send(query);
            result.Match(videosInformation =>
            {
                RecordedLectures = new ObservableCollection<AlrVideoInformation>(videosInformation);
                return Unit.Default;
            }, errors =>
            {
                // TODO: Show an empty message  
                return Unit.Default;
            });
        }).Subscribe()
        .DisposeWith(_disposables);
        
        this.WhenActivated(disposables =>
        {
            _disposables.DisposeWith(disposables);
        });
    }
}
