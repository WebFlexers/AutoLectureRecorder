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

using Auto_Lecture_Recorder.BotController.Unipi;
using Auto_Lecture_Recorder.Lectures;
using Auto_Lecture_Recorder.ScreenRecorder;
using Auto_Lecture_Recorder.Youtube;
using System.IO;
using System.Diagnostics;

namespace Auto_Lecture_Recorder
{
    public partial class MainForm : Form
    {
        // Chromebot
        ChromeBot teamsBot;
        bool teamsAuthenticated = false;
        string RN;
        string password;
        int minimumParticipantsLeft = 5;
        // Responsive object
        Responsive responsive;
        const int MENU_BUTTONS_NUM = 5; 
        // Recorder
        Recorder recorder;
        // Youtube
        YoutubeUploader youtubeUploader = new YoutubeUploader();

        public MainForm()
        {
            InitializeComponent();
            // Instanciate responsiveness
            responsive = new Responsive(panelMainWindows.Controls, 853, 696);
            // To eliminate flickering
            foreach (Control control in Controls)
            {
                makeControlNonFlickering(control);
            }

            teamsBot = new ChromeBot();


        }

        // Make all control and its insides non flickering
        private void makeControlNonFlickering(Control control)
        {        
            SetDoubleBuffered(control);
            foreach (Panel innerPanel in control.Controls.OfType<Panel>())
            {
                makeControlNonFlickering(innerPanel);
            }     
        }
        // List that contains all days of the week
        public Dictionary<string, Lectures.Day> week;

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Instanciate the week
            week = new Dictionary<string, Lectures.Day>();
            week.Add("Monday", new Lectures.Day("Monday"));
            week.Add("Tuesday", new Lectures.Day("Tuesday"));
            week.Add("Wednesday", new Lectures.Day("Wednesday"));
            week.Add("Thursday", new Lectures.Day("Thursday"));
            week.Add("Friday", new Lectures.Day("Friday"));
            week.Add("Saturday", new Lectures.Day("Saturday"));
            week.Add("Sunday", new Lectures.Day("Sunday"));
            // Instanciate recorder
            recorder = new Recorder();
            // Instanciate output and input devices
            dropdownOutputDevice_Load(dropdownOutputDevices, EventArgs.Empty);
            dropdownInputDevice_Load(dropdownInputDevices, EventArgs.Empty);
            // Instanciate video location label
            label.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Auto Lecture Recorder");
        }

        // Eliminate flickering
        #region .. Double Buffered function ..
        public static void SetDoubleBuffered(Control c)
        {
            if (SystemInformation.TerminalServerSession)
                return;
            System.Reflection.PropertyInfo aProp = typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            aProp.SetValue(c, true, null);
        }

        #endregion








        // Title bar functionality
        private void buttonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void buttonMaximize_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
                buttonMaximize.BackgroundImage = Properties.Resources.restore_down_20px;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                buttonMaximize.BackgroundImage = Properties.Resources.maximize_button_16px;
            }
        }

        private void buttonMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        // For moving form from title bar
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void panelTitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }









        // Responsiveness
        private void panelMenu_Resize(object sender, EventArgs e)
        {
            // Change the size of the radio buttons in panel menu when form is resized
            int sizeY = panelMenu.Height / MENU_BUTTONS_NUM;
            foreach (RadioButton radio in panelMenu.Controls.OfType<RadioButton>())
            {
                radio.Size = new Size(radio.Width, sizeY);
            }
        }

        private void panelMainWindows_Resize(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
            {
                responsive.UpdateFormSize(panelMainWindows.Width, panelMainWindows.Height);
                responsive.ScaleAllControls(panelMainWindows.Controls);
            }
                
        }











        // Menu functionality
        private void ShowPanel(Panel panelToShow)
        {
            foreach (Panel panel in panelMainWindows.Controls.OfType<Panel>())
            {
                panelToShow.Visible = true;
                if (!panel.Name.Equals(panelToShow.Name))
                {
                    panel.Visible = false;
                }
            }
        }
        private void radioMenuRecord_Click(object sender, EventArgs e)
        {
            ShowPanel(panelRecord);
        }

        private void radioMenuSubjects_Click(object sender, EventArgs e)
        {
            ShowPanel(panelLectures);
        }

        private void radioMenuAdd_Click(object sender, EventArgs e)
        {
            ShowPanel(panelAddLectures);
        }

        private void radioMenuAuthenticate_Click(object sender, EventArgs e)
        {
            ShowPanel(panelAuthentication);
        }

        private void radioMenuSettings_Click(object sender, EventArgs e)
        {
            ShowPanel(panelSettings);
        }

        // Menu Styling
        // Reverts all menu images to the white version
        private void revertMenuImages()
        {
            foreach (RadioButton radio in panelMenu.Controls.OfType<RadioButton>())
            {
                if (!radio.Checked)
                {
                    switch (radio.Name)
                    {
                        case "radioMenuRecord":
                            radioMenuRecord.Image = Properties.Resources.video_call_white_35px;
                            break;
                        case "radioMenuSubjects":
                            radioMenuSubjects.Image = Properties.Resources.book_white_35px;
                            break;
                        case "radioMenuAdd":
                            radioMenuAdd.Image = Properties.Resources.add_book_white_35px;
                            break;
                        case "radioMenuAuthenticate":
                            radioMenuAuthenticate.Image = Properties.Resources.key_white_35px;
                            break;
                        case "radioMenuSettings":
                            radioMenuSettings.Image = Properties.Resources.settings_white_35px;
                            break;
                    }
                }
            }
        }

        // Change to blue on mouse enter and back to white on mouse leave for all menu controls
        // Record
        private void radioMenuRecord_MouseEnter(object sender, EventArgs e)
        {
            radioMenuRecord.Image = Properties.Resources.video_call_blue_35px;
        }

        private void radioMenuRecord_MouseLeave(object sender, EventArgs e)
        {
            if (!radioMenuRecord.Checked)
                radioMenuRecord.Image = Properties.Resources.video_call_white_35px;
        }
        // Lecture
        private void radioMenuSubjects_MouseEnter(object sender, EventArgs e)
        {
            radioMenuSubjects.Image = Properties.Resources.book_blue_35px;
        }

        private void radioMenuSubjects_MouseLeave(object sender, EventArgs e)
        {
            if (!radioMenuSubjects.Checked)
                radioMenuSubjects.Image = Properties.Resources.book_white_35px;
        }
        // Add subject
        private void radioMenuAdd_MouseEnter(object sender, EventArgs e)
        {
            radioMenuAdd.Image = Properties.Resources.add_book_blue_35px;
        }

        private void radioMenuAdd_MouseLeave(object sender, EventArgs e)
        {
            if (!radioMenuAdd.Checked)
                radioMenuAdd.Image = Properties.Resources.add_book_white_35px;
        }
        // Authenticate
        private void radioMenuAuthenticate_MouseEnter(object sender, EventArgs e)
        {
            radioMenuAuthenticate.Image = Properties.Resources.key_blue_35px;
        }

        private void radioMenuAuthenticate_MouseLeave(object sender, EventArgs e)
        {
            if (!radioMenuAuthenticate.Checked)
            {
                radioMenuAuthenticate.Image = Properties.Resources.key_white_35px;
            }
        }

        // Settings
        private void radioMenuSettings_MouseEnter(object sender, EventArgs e)
        {
            radioMenuSettings.Image = Properties.Resources.settings_blue_35px;
        }

        private void radioMenuSettings_MouseLeave(object sender, EventArgs e)
        {
            if (!radioMenuSettings.Checked)
                radioMenuSettings.Image = Properties.Resources.settings_white_35px;
        }

        // Revert the rest button images when one is selected
        private void radioMenuRecord_CheckedChanged(object sender, EventArgs e)
        {
            revertMenuImages();
        }

        private void radioMenuSubjects_CheckedChanged(object sender, EventArgs e)
        {
            revertMenuImages();
        }

        private void radioMenuAdd_CheckedChanged(object sender, EventArgs e)
        {
            revertMenuImages();
        }

        private void radioMenuAuthenticate_CheckedChanged(object sender, EventArgs e)
        {
            revertMenuImages();
        }

        private void radioMenuSettings_CheckedChanged(object sender, EventArgs e)
        {
            revertMenuImages();
        }






        // Temp
        private void button1_Click(object sender, EventArgs e)
        {
            recorder.StartRecording(textboxLectureName.GetText());
        }


        private void button3_Click(object sender, EventArgs e)
        {
            recorder.StopRecording();

            // Make a new file name and path
            string videoName = recorder.VideoName + " " + DateTime.Now.ToString("dd-MM-yyyy");
            string newVideoPath = Path.Combine(recorder.VideoFolderPath, videoName + ".mp4");

            // Move to the recording folder and change name
            MoveRecordingToRecFolder(recorder.RecordingPath, newVideoPath);
        }


        private void button5_Click(object sender, EventArgs e)
        {
            Console.WriteLine(textboxLectureName.GetText());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Thread joinAndRecord = new Thread(JoinMeatingAndRecord);
            joinAndRecord.IsBackground = true;
            joinAndRecord.Start();
        }

        private void buttonYoutubeUpload_Click(object sender, EventArgs e)
        {
            if (checkboxSettingsYoutube.Checked)
            {
                UploadRecording("C:/Users/Michalis/Documents/Auto Lecture Recorder/ΜΑΘΗΜΑΤΙΚΟΣ ΠΡΟΓΡΑΜΜΑΤΙΣΜΟΣ 08-01-2021.mp4", 
                                "Μαθηματικός Προγραμματισμός", "Test playlist");
                panelYoutubeUpload.Show();
                labelYoutubeUploadStatus.ForeColor = Color.FromArgb(42, 123, 245);
                labelYoutubeUploadStatus.Text = "Uploading...";
                progressBarYoutube.Value = 0;
                timerUpdateYoutubeProgressbar.Start();
            }
        }
    }
}

