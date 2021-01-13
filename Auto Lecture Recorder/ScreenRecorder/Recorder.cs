using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using ScreenRecorderLib;

namespace Auto_Lecture_Recorder.ScreenRecorder
{
    public class Recorder
    {
        // For recording
        public bool IsRecording { get; set; } = false;
        ScreenRecorderLib.Recorder recorder;
        // Path of the folder where videos will be saved and video name
        public string VideoFolderPath { get; set; }
        public string VideoName { get; set; }
        public string RecordingPath
        {
            get
            {
                string fullPath = Path.Combine(VideoFolderPath, "temp_recording.mp4");
                if (File.Exists(fullPath))
                    return fullPath;
                else
                    return null;
            }
        }

        // Options
        public RecorderOptions Options { get; set; }
        // Audio options
        public Dictionary<string, string> InputDevices { get; }
        public Dictionary<string, string> OutputDevices { get; }
        public string SelectedOutputDevice { get; set; } = null;
        public string SelectedInputDevice { get; set; } = null;
        // All open windows
        public List<RecordableWindow> ActiveWindows { get; set; } = ScreenRecorderLib.Recorder.GetWindows();
        

        public Recorder()
        {
            // Set default Video folder path
            VideoFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Auto Lecture Recorder");
            VideoName = "ΜΑΘΗΜΑΤΙΚΟΣ ΠΡΟΓΡΑΜΜΑΤΙΣΜΟΣ";
            // Instanciate options
            LoadDefaultOptions();
            // Instanciate input and output devices
            InputDevices = ScreenRecorderLib.Recorder.GetSystemAudioDevices(AudioDeviceSource.InputDevices);
            OutputDevices = ScreenRecorderLib.Recorder.GetSystemAudioDevices(AudioDeviceSource.OutputDevices);
        }


        public void StartRecording(string fileName)
        {
            if (IsRecording)
            {
                throw new InvalidOperationException("A recording is already active");
            }

            // Create recorder if it doesnt already exist
            if (recorder == null)
            {
                if (Options == null)
                    LoadDefaultOptions();

                recorder = ScreenRecorderLib.Recorder.CreateRecorder(Options);
                recorder.OnRecordingComplete += Rec_OnRecordingComplete;
                recorder.OnRecordingFailed += Rec_OnRecordingFailed;
                recorder.OnStatusChanged += _rec_OnStatusChanged;
            }

            // Save file name
            VideoName = fileName;

            // Start recording
            recorder.Record(Path.Combine(VideoFolderPath, "temp_recording.mp4"));
            IsRecording = true;
        }

        public void StopRecording()
        {
            if (recorder != null)
            {
                IsRecording = false;
                recorder.Stop();
            }
            
        }

        public void DeleteRecording()
        {
            if (VideoName != null)
            {
                string videoPath = Path.Combine(VideoFolderPath, VideoName + ".mp4");
                if (File.Exists(videoPath))
                {
                    File.Delete(videoPath);
                }
            }
        }

        public void LoadDefaultOptions()
        {
            Options = new RecorderOptions
            {
                RecorderMode = RecorderMode.Video,
                //If throttling is disabled, out of memory exceptions may eventually crash the program,
                //depending on encoder settings and system specifications.
                IsThrottlingDisabled = false,
                //Hardware encoding is enabled by default.
                IsHardwareEncodingEnabled = true,
                //Low latency mode provides faster encoding, but can reduce quality.
                IsLowLatencyEnabled = false,
                //Fast start writes the mp4 header at the beginning of the file, to facilitate streaming.
                IsMp4FastStartEnabled = false,

                AudioOptions = new AudioOptions
                {
                    Bitrate = AudioBitrate.bitrate_128kbps,
                    Channels = AudioChannels.Stereo,
                    IsAudioEnabled = true,
                    IsOutputDeviceEnabled = true,
                    IsInputDeviceEnabled = true,
                    AudioOutputDevice = SelectedOutputDevice,
                    AudioInputDevice = SelectedInputDevice
                },

                VideoOptions = new VideoOptions
                {
                    BitrateMode = BitrateControlMode.Quality,
                    Quality = 70,
                    Framerate = 30,
                    IsFixedFramerate = true,
                    EncoderProfile = H264Profile.Main
                },
                
            };
        }

        public void OpenVideoDirectory()
        {
            if (!Directory.Exists(VideoFolderPath))
            {
                Directory.CreateDirectory(VideoFolderPath);
            }
            System.Diagnostics.Process.Start(VideoFolderPath);
        }

        private void Rec_OnRecordingComplete(object sender, RecordingCompleteEventArgs e)
        {
            IsRecording = false;
            CleanupResources();
        }

        private void Rec_OnRecordingFailed(object sender, RecordingFailedEventArgs e)
        {
            MessageBox.Show(e.Error, "Recording failed" ,MessageBoxButtons.OK, MessageBoxIcon.Error);
            IsRecording = false;
            CleanupResources();
        }

        private void _rec_OnStatusChanged(object sender, RecordingStatusEventArgs e)
        {
            RecorderStatus status = e.Status;
        }

        private void CleanupResources()
        {
            recorder.Dispose();
            recorder = null;
        }

    }
}
