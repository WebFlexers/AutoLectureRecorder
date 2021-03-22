using AutoLectureRecorder.Pages;
using AutoLectureRecorder.Structure;
using ScreenRecorderLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;
using YoutubeAPI;

namespace AutoLectureRecorder
{
    public class ScreenRecorder
    {
        public Dictionary<string, string> AudioInputDevices { get => Recorder.GetSystemAudioDevices(AudioDeviceSource.InputDevices); }
        public Dictionary<string, string> AudioOutputDevices { get => Recorder.GetSystemAudioDevices(AudioDeviceSource.OutputDevices); }
        public static string SelectedInputDevice { get { return Options.AudioOptions.AudioInputDevice; } set { Options.AudioOptions.AudioInputDevice = value; } }
        public static string SelectedOutputDevice { get { return Options.AudioOptions.AudioOutputDevice; } set { Options.AudioOptions.AudioOutputDevice = value; } }
        public static string RecordingPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AutoLectureRecorder", "Recorder", "temp.mp4");
        public List<RecordableWindow> RecordableWindows { get => Recorder.GetWindows(); }
        public bool IsRecording { get; set; } = false;
        
        public static RecorderOptions Options { get; set; } = new RecorderOptions
        {
            RecorderMode = RecorderMode.Video,
            //If throttling is disabled, out of memory exceptions may eventually crash the program,
            //depending on encoder settings and system specifications.
            IsThrottlingDisabled = false,
            //Hardware encoding is enabled by default.
            IsHardwareEncodingEnabled = true,
            //Low latency mode provides faster encoding, but can reduce quality.
            IsLowLatencyEnabled = true,
            //Fast start writes the mp4 header at the beginning of the file, to facilitate streaming.
            IsMp4FastStartEnabled = false,

            //DisplayOptions = new DisplayOptions
            //{
            //    WindowHandle = RecordableWindows[0].Handle
            //},

            AudioOptions = new AudioOptions
            {
                Bitrate = AudioBitrate.bitrate_128kbps,
                Channels = AudioChannels.Stereo,
                IsAudioEnabled = true,
                IsInputDeviceEnabled = true,
                IsOutputDeviceEnabled = true,
                AudioInputDevice = null,
                AudioOutputDevice = null
            },

            VideoOptions = new VideoOptions
            {
                BitrateMode = BitrateControlMode.Quality,
                Quality = 70,
                Framerate = 60,
                IsFixedFramerate = true,
                EncoderProfile = H264Profile.Main
            }
        };

        Recorder _recorder;
        string _lectureName;
        public void CreateRecording(string lectureName)
        {
            new Thread(() => RecordIfNoRecordingIsActive(lectureName)).Start();
        }

        private void RecordIfNoRecordingIsActive(string lectureName)
        {
            if (IsRecording)
            {
                Thread.Sleep(5000);
            }
            else
            {
                _recorder = Recorder.CreateRecorder(Options);
                _recorder.OnRecordingComplete += Rec_OnRecordingComplete;
                _recorder.OnRecordingFailed += Rec_OnRecordingFailed;
                _recorder.OnStatusChanged += Rec_OnStatusChanged;

                //Record to a file
                _lectureName = lectureName;

                if (File.Exists(RecordingPath))
                {
                    File.Delete(RecordingPath);
                }

                _recorder.Record(RecordingPath);

                IsRecording = true;
            }
        }

        public bool WillUploadToYoutube { get; set; } = false;
        public void EndRecording(bool uploadToYoutube, ProgressBar progressBar)
        {
            WillUploadToYoutube = uploadToYoutube;
            _progressBar = progressBar;
            _recorder.Stop();
        }

        ProgressBar _progressBar;
        private async void Rec_OnRecordingComplete(object sender, RecordingCompleteEventArgs e)
        {
            //Get the file path if recorded to a file
            Trace.WriteLine("Successfully saved recording!");
            string videoName = DateTime.Now.ToString("dd-MM-yy hh-mm-ss");
            string directoryPath = Path.Combine(Settings.VideoDirectory, _lectureName);
            string newFile = Path.Combine(directoryPath, videoName + ".mp4");
            
            try
            {
                Directory.CreateDirectory(directoryPath);
                File.Move(RecordingPath, newFile, true);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                Trace.WriteLine(RecordingPath);
                Trace.WriteLine(newFile);
                IsRecording = false;
                return;
            }

            IsRecording = false;

            if (WillUploadToYoutube)
            {
                YoutubeUploader youtube = new YoutubeUploader();
                string description = "Your daily lecture delivery powered by Auto Lecture Recorder!";
                await youtube.UploadVideo(newFile, videoName, description, _lectureName, _progressBar);
            }
        }

        private void Rec_OnRecordingFailed(object sender, RecordingFailedEventArgs e)
        {
            Trace.WriteLine(e.Error);
            IsRecording = false;
        }

        private void Rec_OnStatusChanged(object sender, RecordingStatusEventArgs e)
        {
            RecorderStatus status = e.Status;
            Trace.WriteLine(status);
        }
    }
}
