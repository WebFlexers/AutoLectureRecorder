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
        private void checkboxSettingsYoutube_Load(object sender, EventArgs e)
        {
            checkboxSettingsYoutube.AddClickEvents(new EventHandler(checkboxSettingsYoutube_Click));
            checkboxSettingsYoutube.SetText("Automatically upload videos to youtube");
        }

        private void checkboxSettingsYoutube_Click(object sender, EventArgs e)
        {
            if (checkboxSettingsYoutube.Checked)
            {
                checkboxSettingsYoutube.Unckeck();
            }
            else if (!youtubeUploader.authenticate)
            {
                
                var userResponse = MessageBox.Show("In order to automatically upload videos to your youtube channel you need to" +
                                " authenticate your youtube account. " + Environment.NewLine + "Do you want to do it now?", 
                                "Authentication required", MessageBoxButtons.YesNo, MessageBoxIcon.Information);


                if (userResponse == DialogResult.Yes)
                {
                    // Authenticate youtube
                    youtubeUploader.Authentication();
                    // Check if authenticated
                    if (youtubeUploader.authenticate)
                    {
                        checkboxSettingsYoutube.Check();
                    }
                        
                    else
                    {
                        MessageBox.Show("Failed to authenticate your youtube account. \nPlease try again", "Unable to authenticate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                               
            }
            else
            {
                checkboxSettingsYoutube.Check();
            }

        }

        

        // Audio options
        // Dropdown on loads
        private void dropdownOutputDevice_Load(object sender, EventArgs e)
        {
            // Set default
            dropdownOutputDevices.AddDefaultOption("Default");

            for (int i = recorder.OutputDevices.Count - 1; i > 0; i--)
            {
                dropdownOutputDevices.AddOption(recorder.OutputDevices.ElementAt(i).Value);
            }
            
            // To save the selection
            dropdownOutputDevices.AddSelectionClickEvent(new ToolStripItemClickedEventHandler(SaveOutputSelection));
        }

        private void SaveOutputSelection(object sender, ToolStripItemClickedEventArgs e)
        {
            recorder.SelectedOutputDevice = recorder.OutputDevices.FirstOrDefault(name => name.Value == e.ClickedItem.Text).Key;
        }

        private void dropdownInputDevice_Load(object sender, EventArgs e)
        {
            // Add default
            dropdownInputDevices.AddDefaultOption("Default");

            for (int i = recorder.InputDevices.Count - 1; i > 0; i--)
            {
                dropdownInputDevices.AddOption(recorder.InputDevices.ElementAt(i).Value);
            }

            dropdownInputDevices.AddSelectionClickEvent(new ToolStripItemClickedEventHandler(SaveInputSelection));
        }

        private void SaveInputSelection(object sender, ToolStripItemClickedEventArgs e)
        {
            recorder.SelectedInputDevice = recorder.InputDevices.FirstOrDefault(name => name.Value == e.ClickedItem.Text).Key;
        }

        // Checkboxes On Loads
        private void checkboxOutputEnabled_Load(object sender, EventArgs e)
        {
            checkboxOutputEnabled.AddClickEvents(new EventHandler(checkboxOutputEnabled.Component_Click));
            checkboxOutputEnabled.AddClickEvents(new EventHandler(checkboxOutputEnabled_Click));
            checkboxOutputEnabled.SetText("Record audio output");
            checkboxOutputEnabled.Check();
        }

        private void checkboxInputEnabled_Load(object sender, EventArgs e)
        {
            checkboxInputEnabled.AddClickEvents(new EventHandler(checkboxInputEnabled.Component_Click));
            checkboxInputEnabled.AddClickEvents(new EventHandler(checkboxInputEnabled_Click));
            checkboxInputEnabled.SetText("Record audio input");
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
        }

        // Video Options
        private void dropdownFPS_Load(object sender, EventArgs e)
        {
            dropdownFPS.AddOption(15.ToString());
            dropdownFPS.AddDefaultOption(30.ToString());
            dropdownFPS.AddOption(60.ToString());

            dropdownFPS.AddSelectionClickEvent(new ToolStripItemClickedEventHandler(SaveFpsSelection));
        }

        private void dropdownQuality_Load(object sender, EventArgs e)
        {
            for (int i = 10; i <= 100; i += 10)
            {
                dropdownQuality.AddOption(i.ToString());
            }
            dropdownQuality.AddDefaultOption(70.ToString());

            dropdownQuality.AddSelectionClickEvent(new ToolStripItemClickedEventHandler(SaveQualitySelection));
        }

        private void SaveFpsSelection(object sender, ToolStripItemClickedEventArgs e)
        {
            recorder.Options.VideoOptions.Framerate = Convert.ToInt32(e.ClickedItem.Text);
        }

        private void SaveQualitySelection(object sender, ToolStripItemClickedEventArgs e)
        {
            recorder.Options.VideoOptions.Quality = Convert.ToInt32(e.ClickedItem.Text);
        }
    }
}
