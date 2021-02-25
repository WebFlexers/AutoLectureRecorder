using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoLectureRecorder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonMenu_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Brush initialBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#333856");
            Brush pressedBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#454D73");
            button.Background = button.Background.Equals(initialBrush) ? pressedBrush : initialBrush;
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ButtonResize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
                ImageResize.Source = new BitmapImage(new Uri("/restore_down_20px.png", UriKind.Relative));
            }
            else if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                ImageResize.Source = new BitmapImage(new Uri("/maximize_button_18px.png", UriKind.Relative));
            }
        }

        private void ButtonWindowState_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                ImageResize.Source = new BitmapImage(new Uri("/maximize_button_18px.png", UriKind.Relative));
            else if (this.WindowState == WindowState.Maximized)
                ImageResize.Source = new BitmapImage(new Uri("/restore_down_20px.png", UriKind.Relative));
        }
    }
}
