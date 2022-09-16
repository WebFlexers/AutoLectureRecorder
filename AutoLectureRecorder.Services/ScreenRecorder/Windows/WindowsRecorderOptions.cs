using AutoLectureRecorder.Services.ScreenRecorder.Interfaces;
using ScreenRecorderLib;

namespace AutoLectureRecorder.Services.ScreenRecorder.Windows;

public class WindowsRecorderOptions : RecorderOptions, IRecorderOptions
{
    public string OutputDirectory { get; set; }
    public string OutputFileName { get; set; }
    public VideoExtensions OutputFileExtension { get; set; }

    public string OutputFilePath
    {
        get
        {
            return Path.Combine(OutputDirectory, OutputFileName + $".{OutputFileExtension}");
        }
    }

    public WindowsRecorderOptions()
    {
        AudioOptions = new AudioOptions
        {
            Bitrate = AudioBitrate.bitrate_128kbps,
            Channels = AudioChannels.Stereo,
            IsAudioEnabled = true,
            IsInputDeviceEnabled = false,
            IsOutputDeviceEnabled = true,
            AudioInputDevice = null, // Null means default
            AudioOutputDevice = null
        };

        OutputOptions = new OutputOptions
        {
            RecorderMode = RecorderMode.Video,
        };

        VideoEncoderOptions = new VideoEncoderOptions
        {
            Encoder = new H264VideoEncoder(),
            Framerate = 60,
            IsLowLatencyEnabled = true,
        };
    }
}
