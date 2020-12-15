using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScreenRecorderLib;
using System.IO;

namespace Auto_Lecture_Recorder.ScreenRecorder
{
    public class Recorder
    {
        // For recording
        private bool isRecording;
        ScreenRecorderLib.Recorder recorder;
        // Path of the folder where videos will be saved and video name
        public string VideoFolderPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AutoLectureRecorder");
        private string videoName;
        // Options
        RecorderOptions Options { get; set; }


        public void StartRecording(string fileName)
        {
            if (isRecording)
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
            videoName = fileName;

            // Start recording
            recorder.Record(Path.Combine(VideoFolderPath, fileName));
            isRecording = true;
        }

        public void StopRecording()
        {
            if (recorder != null)
            {
                recorder.Stop();
            }
            
        }

        public void DeleteRecording()
        {
            if (videoName != null)
            {
                string videoPath = Path.Combine(VideoFolderPath, videoName);
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
                    IsAudioEnabled = true
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
            isRecording = false;
            CleanupResources();
        }

        private void Rec_OnRecordingFailed(object sender, RecordingFailedEventArgs e)
        {
            MessageBox.Show("The recording failed. Try again", "Recording failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            isRecording = false;
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
