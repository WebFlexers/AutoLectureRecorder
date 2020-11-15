using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Subjects;


namespace Auto_Lecture_Recorder
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
        








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
            int sizeY = (panelMenu.Height - panelTitleLabel.Height) / 4;
            foreach (RadioButton radio in panelMenu.Controls.OfType<RadioButton>())
            {
                radio.Size = new Size(radio.Width, sizeY);
            }
        }













        // Menu functionality
        private void hideMainPanels()
        {
            panelRecord.Visible = false;
            panelSubjects.Visible = false;
            panelAddSubjects.Visible = false;
            panelSettings.Visible = false;
        }
        private void radioMenuRecord_Click(object sender, EventArgs e)
        {
            hideMainPanels();
            panelRecord.Visible = true;
        }

        private void radioMenuSubjects_Click(object sender, EventArgs e)
        {
            hideMainPanels();
            panelSubjects.Visible = true;
        }

        private void radioMenuAdd_Click(object sender, EventArgs e)
        {
            hideMainPanels();
            panelAddSubjects.Visible = true;
        }

        private void radioMenuSettings_Click(object sender, EventArgs e)
        {
            hideMainPanels();
            panelSettings.Visible = true;
        }

        // Menu Styling
        // Reverts all menu images to the white version
        private void revertMenuImages()
        {
            foreach(RadioButton radio in panelMenu.Controls.OfType<RadioButton>())
            {
                if (!radio.Checked)
                {
                    switch (radio.Name)
                    {
                        case "radioMenuRecord":
                            radioMenuRecord.Image = Auto_Lecture_Recorder.Properties.Resources.video_call_white_35px;
                            break;
                        case "radioMenuSubjects":
                            radioMenuSubjects.Image = Auto_Lecture_Recorder.Properties.Resources.book_white_35px;
                            break;
                        case "radioMenuAdd":
                            radioMenuAdd.Image = Auto_Lecture_Recorder.Properties.Resources.add_book_white_35px;
                            break;
                        case "radioMenuSettings":
                            radioMenuSettings.Image = Auto_Lecture_Recorder.Properties.Resources.settings_white_35px;
                            break;
                    }
                }
            }
        }

            // Change to blue on mouse enter and back to white on mouse leave for all menu controls
            // Record
        private void radioMenuRecord_MouseEnter(object sender, EventArgs e)
        {
            radioMenuRecord.Image = Auto_Lecture_Recorder.Properties.Resources.video_call_blue_35px;
        }

        private void radioMenuRecord_MouseLeave(object sender, EventArgs e)
        {
            if (!radioMenuRecord.Checked)
                radioMenuRecord.Image = Auto_Lecture_Recorder.Properties.Resources.video_call_white_35px;
        }
            // Lecture
        private void radioMenuSubjects_MouseEnter(object sender, EventArgs e)
        {
            radioMenuSubjects.Image = Auto_Lecture_Recorder.Properties.Resources.book_blue_35px;
        }

        private void radioMenuSubjects_MouseLeave(object sender, EventArgs e)
        {
            if (!radioMenuSubjects.Checked)
                radioMenuSubjects.Image = Auto_Lecture_Recorder.Properties.Resources.book_white_35px;
        }
            // Add subject
        private void radioMenuAdd_MouseEnter(object sender, EventArgs e)
        {
            radioMenuAdd.Image = Auto_Lecture_Recorder.Properties.Resources.add_book_blue_35px;
        }

        private void radioMenuAdd_MouseLeave(object sender, EventArgs e)
        {
            if (!radioMenuAdd.Checked)
                radioMenuAdd.Image = Auto_Lecture_Recorder.Properties.Resources.add_book_white_35px;
        }
            // Settings
        private void radioMenuSettings_MouseEnter(object sender, EventArgs e)
        {
            radioMenuSettings.Image = Auto_Lecture_Recorder.Properties.Resources.settings_blue_35px;
        }

        private void radioMenuSettings_MouseLeave(object sender, EventArgs e)
        {
            if (!radioMenuSettings.Checked)
                radioMenuSettings.Image = Auto_Lecture_Recorder.Properties.Resources.settings_white_35px;
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

        private void radioMenuSettings_CheckedChanged(object sender, EventArgs e)
        {
            revertMenuImages();
        }










        // Record button Styling
        private void checkBoxRecordButton_MouseEnter(object sender, EventArgs e)
        {
            checkBoxRecordButton.BackgroundImage = Auto_Lecture_Recorder.Properties.Resources.record_button_on;
        }

        private void checkBoxRecordButton_MouseLeave(object sender, EventArgs e)
        {
            if (!checkBoxRecordButton.Checked)
                checkBoxRecordButton.BackgroundImage = Auto_Lecture_Recorder.Properties.Resources.record_button_off;
        }

        private void checkBoxRecordButton_CheckedChanged(object sender, EventArgs e)
        {
            
            if (checkBoxRecordButton.Checked)
            {
                // Change recording button color to bright red
                checkBoxRecordButton.BackgroundImage = Auto_Lecture_Recorder.Properties.Resources.record_button_on;
                // Show the recording armed text
                panelRecordArmed.Visible = true;
            }    
            else
            {
                // Change recording button color to dark red
                checkBoxRecordButton.BackgroundImage = Auto_Lecture_Recorder.Properties.Resources.record_button_off;
                // Hide the recording armed text
                panelRecordArmed.Visible = false;
            }
                         
        }










        // Add Subjects tab -> drop down list functionality

        // All drop down lists are organized in panels that contain 2 labels
        // The first label is the down arrow symbol and the second is where
        // the text from the dropdown will go in

        // The panel of the option that was selected
        Panel labelDropdown = null;
        // Show the dropdown menu
        private void buttonShowList_Click(object sender, EventArgs e)
        {
            if (sender is Label)
            {
                // Get the container panel's name
                string panelName = ((Label)sender).Parent.Name;

                // Check which dropdown menu to show
                if (panelName.Equals("panelDropdownDay"))
                {
                    menuStripDays.Show(((Label)sender).Parent, new Point(0, ((Label)sender).Height));
                }
                else if (panelName.Equals("panelDropdownHours1") || panelName.Equals("panelDropdownHours2"))
                {
                    menuStripHours.Show(((Label)sender).Parent, new Point(0, ((Label)sender).Height));
                }
                else
                {
                    menuStripMinutes.Show(((Label)sender).Parent, new Point(0, ((Label)sender).Height));
                }

                // Save the panel of the clicked label so that the text can be saved in the correct label
                labelDropdown = (Panel)((Label)sender).Parent;
            }

        }

        // Hover effect on dropdown lists
            // on enter
        private void buttonShowList_MouseEnter(object sender, EventArgs e)
        {
            foreach (Label label in ((Label)sender).Parent.Controls.OfType<Label>())
            {
                label.BackColor = Color.FromArgb(61, 67, 103);
            }
            
        }
            // on leave
        private void buttonShowList_MouseLeave(object sender, EventArgs e)
        {
            foreach (Label label in ((Label)sender).Parent.Controls.OfType<Label>())
            {
                label.BackColor = Color.FromArgb(51, 56, 86);
            }
        }

        // Assign the selected value to the correct label
        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            foreach (Label label in labelDropdown.Controls.OfType<Label>())
            {
                if (!label.Text.Equals("u"))
                {
                    label.Text = e.ClickedItem.Text;
                }
            }

        }

    }
}
