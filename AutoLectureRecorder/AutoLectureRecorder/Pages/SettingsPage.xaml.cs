using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AutoLectureRecorder.Structure;
using Ookii.Dialogs.Wpf;

namespace AutoLectureRecorder.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            _componentsLoaded = true;
            LoadData();
            InitializeUI();
        }

        bool _componentsLoaded = false;
        private void LoadData()
        {
            foreach (var outputDevice in ScreenRecorder.AudioOutputDevices)
            {
                ComboboxOutputDevice.Items.Add(outputDevice.Value);
            }

            if (Settings.AudioOutputDevice != null)
            {
                ComboboxOutputDevice.SelectedItem = Settings.AudioOutputDevice;
                Trace.WriteLine(Settings.AudioOutputDevice);
                UpdateAudioOutputDevice();
            }
        }

        private void InitializeUI()
        {
            TextBlockRecordingPath.Text = "Recording path: " + Settings.VideoDirectory;
        }

        private void ButtonBrowsePath_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();

            if (dialog.ShowDialog().GetValueOrDefault())
            {
                Settings.VideoDirectory = dialog.SelectedPath;
                TextBlockRecordingPath.Text = "Recording path: " + dialog.SelectedPath;
                Serialize.SerializeRecordingPath(dialog.SelectedPath);
            }
        }

        private void UpdateAudioOutputDevice()
        {
            if (_componentsLoaded)
            {
                Trace.WriteLine(ComboboxOutputDevice.Text);
                string selectedDevice = ScreenRecorder.AudioOutputDevices.FirstOrDefault(x => x.Value == ComboboxOutputDevice.Text).Key;

                if (selectedDevice != null)
                {
                    ScreenRecorder.SelectedOutputDevice = selectedDevice;
                    Serialize.SerializeAudioOutput(ComboboxOutputDevice.Text);
                }
                else
                {
                    ScreenRecorder.SelectedOutputDevice = null;
                    Serialize.SerializeAudioOutput("Default");
                }
            }
        }

        private void ComboboxOutputDevice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_componentsLoaded)
            {
                string comboboxText = (sender as ComboBox).SelectedItem as string;
                Trace.WriteLine(comboboxText);
                string selectedDevice = ScreenRecorder.AudioOutputDevices.FirstOrDefault(x => x.Value == comboboxText).Key;

                if (selectedDevice != null)
                {
                    ScreenRecorder.SelectedOutputDevice = selectedDevice;
                    Serialize.SerializeAudioOutput(comboboxText);
                }
                else
                {
                    ScreenRecorder.SelectedOutputDevice = null;
                    Serialize.SerializeAudioOutput("Default");
                }
            }
        }
    }
}
