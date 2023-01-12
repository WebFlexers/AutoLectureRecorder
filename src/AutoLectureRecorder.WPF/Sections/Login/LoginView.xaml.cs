using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace AutoLectureRecorder.Sections.Login;

public partial class LoginView : ReactiveUserControl<LoginViewModel>
{
    public LoginView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.Bind(ViewModel, vm => vm.AcademicEmailAddress, v => v.EmailTextbox.Text)
                .DisposeWith(disposables);
            PasswordTextbox.Events().PasswordChanged
                .Subscribe(e => ViewModel!.Password = PasswordTextbox.Password)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.ErrorMessage, v => v.ErrorTextBlock.Text)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.IsErrorMessageVisible, v => v.ErrorTextBlock.Visibility)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.LoginCommand, v => v.SubmitButton)
                .DisposeWith(disposables);
        });

        
    }
}
