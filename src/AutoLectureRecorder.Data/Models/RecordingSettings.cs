namespace AutoLectureRecorder.Data.Models;

public class RecordingSettings
{
    public string RecordingsLocalPath { get; set; }
    public string OutputDeviceName { get; set; }
    public string OutputDeviceFriendlyName { get; set; }
    public string InputDeviceName { get; set; }
    public string InputDeviceFriendlyName { get; set; }
    public int IsInputDeviceEnabled { get; set; }
    public int Quality { get; set; }
    public int Fps { get; set; }
    public int OutputFrameWidth { get; set; }
    public int OutputFrameHeight { get; set; }
}
