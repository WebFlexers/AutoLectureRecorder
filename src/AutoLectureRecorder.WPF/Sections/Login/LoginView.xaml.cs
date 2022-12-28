using ReactiveUI;
using System.Reactive.Disposables;
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
            this.OneWayBind(ViewModel, vm => vm.ErrorMessage, v => v.errorTextBlock.Text)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.IsErrorMessageVisible, v => v.errorTextBlock.Visibility)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.LoginCommand, v => v.submitButton)
                .DisposeWith(disposables);
        });

        
    }
}
