namespace AutoLectureRecorder.Services.ScreenRecorder.Interfaces;

public interface IRecorder : IAudioDevices, IScreenRecorder 
{
    IRecorderOptions Options { get; }
}