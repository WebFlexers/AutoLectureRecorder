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
    }
}
