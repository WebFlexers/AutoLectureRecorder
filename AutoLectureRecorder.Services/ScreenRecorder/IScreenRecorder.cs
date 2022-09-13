namespace AutoLectureRecorder.Services.ScreenRecorder;

public interface IScreenRecorder
{
    bool IsSpecificWindowSelected { get; set; }
    void StartRecording();
    void StopRecording();
}
