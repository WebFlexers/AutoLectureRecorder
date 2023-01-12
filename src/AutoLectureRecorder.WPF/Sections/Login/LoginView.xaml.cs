using ReactiveUI;
using System.Reactive.Disposables;
using ReactiveMarbles.ObservableEvents;
using System;
using System.Windows;
using System.Windows.Media;
using AutoLectureRecorder.Sections.Login;

namespace AutoLectureRecorder.WPF.Sections.Login;

public partial class LoginView : ReactiveUserControl<LoginViewModel>
{
    public LoginView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.Bind(ViewModel, vm => vm.AcademicEmailAddress, v => v.emailTextbox.Text)
                .DisposeWith(disposables);
            passwordTextbox.Events().PasswordChanged
                .Subscribe(e => ViewModel!.Password = passwordTextbox.Password)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.ErrorMessage, v => v.errorTextBlock.Text)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.IsErrorMessageVisible, v => v.errorTextBlock.Visibility)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.LoginCommand, v => v.submitButton)
                .DisposeWith(disposables);
        });

        
    }
}
