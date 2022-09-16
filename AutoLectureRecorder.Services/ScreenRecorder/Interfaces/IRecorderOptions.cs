namespace AutoLectureRecorder.Services.ScreenRecorder.Interfaces;
public interface IRecorderOptions
{
    public string OutputDirectory { get; set; }
    public string OutputFileName { get; set; }
    public VideoExtensions OutputFileExtension { get; set; }
    public string OutputFilePath { get;}
}
