namespace AutoLectureRecorder.Services.ScreenRecorder.Interfaces;

public interface IScreenRecorder
{
    bool IsSpecificWindowSelected { get; set; }
    void StartRecording();
    void StopRecording();
}