using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

using Auto_Lecture_Recorder.Lectures;
using Auto_Lecture_Recorder.ScreenRecorder;
using Auto_Lecture_Recorder.Youtube;
using System.IO;
namespace Auto_Lecture_Recorder
{
    partial class MainForm
    {
        // SETTINGS PANEL
        // Settings menu functionality
        private void menuSettingsRecording_Click(object sender, EventArgs e)
        {
            if (menuSettingsRecording.Checked)
            {
                panelSettingsRecording.Show();
                panelSettingsYoutube.Hide();
                panelSettingsGeneral.Hide();
            }
        }

        private void menuSettingsGeneral_Click(object sender, EventArgs e)
        {
            if (menuSettingsGeneral.Checked)
            {
                panelSettingsGeneral.Show();
                panelSettingsYoutube.Hide();
                panelSettingsRecording.Hide();
            }
        }

        private void menuSettingsYoutube_Click(object sender, EventArgs e)
        {
            panelSettingsYoutube.Show();
            panelSettingsGeneral.Hide();
            panelSettingsRecording.Hide();
        }

        // General options
        private void buttonBrowseLocation_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserVideo.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserVideo.SelectedPath))
            {
                labelVideoLocation.Text = folderBrowserVideo.SelectedPath;
                recorder.VideoFolderPath = folderBrowserVideo.SelectedPath;
            }
        }

        private void textboxAutoQuitNum_Load(object sender, EventArgs e)
        {
            textboxAutoQuitNum.SetText(minimumParticipantsLeft.ToString());
            textboxAutoQuitNum.AllowOnlyNumbers();
            textboxAutoQuitNum.textBox.MaxLength = 3;
        }

        // Youtube Options
        bool youtubeEnabled = false;
        private void checkboxSettingsYoutube_Load(object sender, EventArgs e)
        {
            checkboxSettingsYoutube.AddClickEvents(new EventHandler(checkboxSettingsYoutube_Click));
            checkboxSettingsYoutube.SetText("Automatically upload videos to youtube");
            if (youtubeEnabled)
                checkboxSettingsYoutube.Check();
            else
                checkboxSettingsYoutube.Uncheck();
        }

        bool youtubeToggleInProgress = false;
        private void checkboxSettingsYoutube_Click(object sender, EventArgs e)
        {
            if (!youtubeToggleInProgress)
            {
                Thread toggleThread = new Thread(ToggleAutoUpload);
                toggleThread.Start();
            }
        }

        delegate void ToggleYoutubeBox();
        private async void ToggleAutoUpload()
        {
            youtubeToggleInProgress = true;

            ToggleYoutubeBox checker = checkboxSettingsYoutube.Check;
            ToggleYoutubeBox unchecker = checkboxSettingsYoutube.Uncheck;

            if (checkboxSettingsYoutube.Checked)
            {
                panelSettings.Invoke(unchecker);
            }
            else if (!youtubeEnabled)
            {

                var userResponse = MessageBox.Show("In order to automatically upload videos to your youtube channel you need to" +
                                " authenticate your youtube account. " + Environment.NewLine + "Do you want to do it now?",
                                "Authentication required", MessageBoxButtons.YesNo, MessageBoxIcon.Information);


                if (userResponse == DialogResult.Yes)
                {
                    // Check if authenticated
                    if (await youtubeUploader.Authenticate())
                    {
                        youtubeEnabled = true;
                        panelSettings.Invoke(checker);
                    }
                    else
                    {
                        youtubeEnabled = false;
                        MessageBox.Show("Failed to authenticate your youtube account. \nPlease try again", "Unable to authenticate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

            }
            else
            {
                panelSettings.Invoke(checker);
            }

            youtubeToggleInProgress = false;
        }


        // Audio options
        // Dropdown on loads
        string defaultOutputDevice = "Default"; 
        private void dropdownOutputDevice_Load(object sender, EventArgs e)
        {
            dropdownOutputDevices.AddOption("Default");

            for (int i = recorder.OutputDevices.Count - 1; i >= 0; i--)
            {
                string outputDevice = recorder.OutputDevices.ElementAt(i).Value;
                if (!dropdownOutputDevices.ContainsOption(outputDevice))
                    dropdownOutputDevices.AddOption(outputDevice);
            }

            // Set default
            dropdownOutputDevices.AddDefaultOption(defaultOutputDevice);

            // To save the selection
            dropdownOutputDevices.AddSelectionClickEvent(new ToolStripItemClickedEventHandler(SaveOutputSelection));
        }

        private void SaveOutputSelection(object sender, ToolStripItemClickedEventArgs e)
        {
            recorder.SelectedOutputDevice = recorder.OutputDevices.FirstOrDefault(name => name.Value == e.ClickedItem.Text).Key;
            SerializeSettings();
        }

        string defaultInputDevice = "Default";
        private void dropdownInputDevice_Load(object sender, EventArgs e)
        {
            dropdownInputDevices.AddOption("Default");

            for (int i = recorder.InputDevices.Count - 1; i >= 0; i--)
            {
                string inputDevice = recorder.InputDevices.ElementAt(i).Value;
                if (!dropdownInputDevices.ContainsOption(inputDevice))
                    dropdownInputDevices.AddOption(inputDevice);
            }

            // Add default
            dropdownInputDevices.AddDefaultOption(defaultInputDevice);

            dropdownInputDevices.AddSelectionClickEvent(new ToolStripItemClickedEventHandler(SaveInputSelection));
        }

        private void SaveInputSelection(object sender, ToolStripItemClickedEventArgs e)
        {
            recorder.SelectedInputDevice = recorder.InputDevices.FirstOrDefault(name => name.Value == e.ClickedItem.Text).Key;
            SerializeSettings();
        }

        // Checkboxes On Loads
        bool recordOutput = true;
        private void checkboxOutputEnabled_Load(object sender, EventArgs e)
        {
            checkboxOutputEnabled.AddClickEvents(new EventHandler(checkboxOutputEnabled.Component_Click));
            checkboxOutputEnabled.AddClickEvents(new EventHandler(checkboxOutputEnabled_Click));
            checkboxOutputEnabled.SetText("Record audio output");
            if (recordOutput)
                checkboxOutputEnabled.Check();
        }

        bool recordInput = true;
        private void checkboxInputEnabled_Load(object sender, EventArgs e)
        {
            checkboxInputEnabled.AddClickEvents(new EventHandler(checkboxInputEnabled.Component_Click));
            checkboxInputEnabled.AddClickEvents(new EventHandler(checkboxInputEnabled_Click));
            checkboxInputEnabled.SetText("Record audio input");
            if (recordInput)
                checkboxInputEnabled.Check();
        }

        // Checkbox transfer value
        private void checkboxOutputEnabled_Click(object sender, EventArgs e)
        {
            if (checkboxOutputEnabled.Checked)
            {
                recorder.Options.AudioOptions.IsOutputDeviceEnabled = true;
            }
            else
            {
                recorder.Options.AudioOptions.IsOutputDeviceEnabled = false;
            }
            SerializeSettings();
        }

        private void checkboxInputEnabled_Click(object sender, EventArgs e)
        {
            if (checkboxInputEnabled.Checked)
            {
                recorder.Options.AudioOptions.IsInputDeviceEnabled = true;
            }
            else
            {
                recorder.Options.AudioOptions.IsInputDeviceEnabled = false;
            }
            SerializeSettings();
        }

        // Video Options
        int defaultFPS = 30;
        private void dropdownFPS_Load(object sender, EventArgs e)
        {
            dropdownFPS.AddOption(15.ToString());
            dropdownFPS.AddOption(30.ToString());
            dropdownFPS.AddOption(60.ToString());

            dropdownFPS.AddDefaultOption(defaultFPS.ToString());

            dropdownFPS.AddSelectionClickEvent(new ToolStripItemClickedEventHandler(SaveFpsSelection));
        }

        int defaultQuality = 70;
        private void dropdownQuality_Load(object sender, EventArgs e)
        {
            for (int i = 10; i <= 100; i += 10)
            {
                dropdownQuality.AddOption(i.ToString());
            }

            dropdownQuality.AddDefaultOption(defaultQuality.ToString());

            dropdownQuality.AddSelectionClickEvent(new ToolStripItemClickedEventHandler(SaveQualitySelection));
        }

        private void SaveFpsSelection(object sender, ToolStripItemClickedEventArgs e)
        {
            recorder.Options.VideoOptions.Framerate = Convert.ToInt32(e.ClickedItem.Text);
            SerializeSettings();
        }

        private void SaveQualitySelection(object sender, ToolStripItemClickedEventArgs e)
        {
            recorder.Options.VideoOptions.Quality = Convert.ToInt32(e.ClickedItem.Text);
            SerializeSettings();
        }

        private void SerializeSettings()
        {
            Serializer.SerializeSettings(Convert.ToInt32(textboxAutoQuitNum.GetText()), dropdownInputDevices.GetText(),
            dropdownOutputDevices.GetText(), checkboxInputEnabled.Checked, checkboxOutputEnabled.Checked,
            Convert.ToInt32(dropdownFPS.GetText()), Convert.ToInt32(dropdownQuality.GetText()), 
            checkboxSettingsYoutube.Checked);
        }

        private void DeserializeSettings()
        {
            object[] settings = Serializer.DeserializeSettings();
            minimumParticipantsLeft = (int)settings[(int)Serializer.Settings.MinimumParticipants];
            defaultInputDevice = (string)settings[(int)Serializer.Settings.InputDevice];
            defaultOutputDevice = (string)settings[(int)Serializer.Settings.OutputDevice];
            recordInput = (bool)settings[(int)Serializer.Settings.InputEnabled];
            recordOutput = (bool)settings[(int)Serializer.Settings.OutputEnabled];
            defaultFPS = (int)settings[(int)Serializer.Settings.Fps];
            defaultQuality = (int)settings[(int)Serializer.Settings.Quality];
            youtubeEnabled = (bool)settings[(int)Serializer.Settings.YoutubeEnabled];
        }
    }
 
}
