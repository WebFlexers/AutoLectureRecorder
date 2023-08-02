using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using AutoLectureRecorder.Application.Common.Abstractions.Persistence;
using AutoLectureRecorder.Application.Common.Abstractions.Recording;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using Unit = System.Reactive.Unit;

namespace AutoLectureRecorder.Application.Recording.Commands.RecordMeeting;

public class RecordMeetingHandler : ReactiveObject, IRequestHandler<RecordMeetingCommand, ErrorOr<Unit>>
{
    private readonly ILogger<RecordMeetingHandler> _logger;
    private readonly ISettingsRepository _settingsRepository;
    private readonly IRecorder _recorder;

    public RecordMeetingHandler(ILogger<RecordMeetingHandler> logger, ISettingsRepository settingsRepository, 
        IRecorder recorder)
    {
        _logger = logger;
        _settingsRepository = settingsRepository;
        _recorder = recorder;
    }
    
    public async Task<ErrorOr<Unit>> Handle(RecordMeetingCommand command, CancellationToken cancellationToken)
    {
        var recordingSettings = await _settingsRepository.GetRecordingSettings() ?? 
            _recorder.GetDefaultSettings(1366, 768);
        
        _recorder.RecordingDirectoryPath = Path.Combine(recordingSettings.RecordingsLocalPath, 
            $"Semester {command.LectureSemester}", 
            command.LectureSubjectName); 
        
        if (Directory.Exists(_recorder.RecordingDirectoryPath) == false)
        {
            Directory.CreateDirectory(recordingSettings.RecordingsLocalPath);
        }
        
        var recordingStartTime = DateTime.Now;
        _recorder.RecordingFileName = recordingStartTime.ToString("yyyy-MM-d hh-mm");

        _recorder.ApplyRecordingSettings(recordingSettings);

        _recorder.StartRecording(command.WindowHandle, false);
            // TODO: Upload to the cloud
            /*.OnRecordingComplete(() =>
            {
                
            })*/

        if (command.OnRecordingFailed is not null)
        {
            _recorder.OnRecordingFailed(command.OnRecordingFailed);
        }

        // Wait until the lecture is finished to stop the recording
        try
        {
            var now = DateTime.Now;
            var endTime = command.LectureEndTime;
            var recordingDuration = new TimeSpan(endTime.Hour, endTime.Minute, endTime.Second)
                .Subtract(new TimeSpan(now.Hour, now.Minute, now.Second));

            await Task.Delay(recordingDuration, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("The recording was cancelled manually");
        }

        var waitUntilRecordingIsFinishedTask = this.WhenAnyValue(handler => handler._recorder.IsRecording)
            .Do(isRecording => _logger.LogInformation("IsRecording: {IsRecording}", isRecording))
            .SkipWhile(isRecording => isRecording)
            .Take(1)
            .ToTask(cancellationToken);

        _recorder.InitiateStopRecording();

        await waitUntilRecordingIsFinishedTask;

        return Unit.Default;
    }
}