namespace AutoLectureRecorder.Data.Models;

public class RecordingSettings
{
    public int Id { get; set; }
    public string RecordingsLocalPath { get; set; }
    public string OutputDevice { get; set; }
    public string InputDevice { get; set; }
    public int IsInputDeviceEnabled { get; set; }
    public int Quality { get; set; }
    public double Fps { get; set; }
}
