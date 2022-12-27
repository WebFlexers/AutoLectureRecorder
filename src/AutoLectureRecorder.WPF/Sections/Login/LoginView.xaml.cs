using ReactiveUI;
using System.Windows;
using System.Windows.Media;

namespace AutoLectureRecorder.WPF.Sections.Login;

public partial class LoginView : ReactiveUserControl<LoginViewModel>
{
    public LoginView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.BindCommand(ViewModel, vm => vm.LoginCommand, v => v.submitButton);
        });
    }
}
