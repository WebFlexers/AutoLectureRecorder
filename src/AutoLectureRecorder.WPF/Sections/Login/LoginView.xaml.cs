using ReactiveUI;
using System.Windows;
using System.Windows.Media;

namespace AutoLectureRecorder.WPF.Sections.Login;

public partial class LoginView : ReactiveUserControl<LoginViewModel>
{
    public LoginView()
    {
        InitializeComponent();
        var colors = ((App)Application.Current).GetResourceDictionary("Colors.xaml", "Resources/Colors");
        this.Resources["MaterialDesignFlatButtonClick"] = new SolidColorBrush(Colors.Black);
    }
}
