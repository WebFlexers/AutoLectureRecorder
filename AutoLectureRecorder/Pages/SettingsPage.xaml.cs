using System;
using System.Collections.Generic;
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
            InitializeUI();
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
    }
}
