using ReactiveUI;

namespace AutoLectureRecorder.Pages.MainMenu.Settings;

public class ReactiveResolution : ReactiveObject
{
    public int Width { get; set; }
    public int Height { get; set; }
    public string StringValue => $"{Width}x{Height}";
}
