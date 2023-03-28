using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Data;

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
                .Subscribe(e => 
                    ViewModel!.Password = PasswordTextbox.Password)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.DownloadProgressValue, v => v.DownloadProgressBar.Value)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.DownloadProgressValueString, v => v.DownloadProgressTextBlock.Text)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.WebDriverProgressVisibility, v => v.ProgressGrid.Visibility)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.WebDriverProgressVisibility, v => v.ProgressHelperTextBlock.Visibility)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.ErrorMessage, v => v.ErrorTextBlock.Text)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.IsErrorMessageVisible, v => v.ErrorTextBlock.Visibility)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.IsErrorMessageInvisible, v => v.Disclaimer1TextBlock.Visibility)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.IsErrorMessageInvisible, v => v.Disclaimer2TextBlock.Visibility)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.LoginCommand, v => v.SubmitButton)
                .DisposeWith(disposables);
        });

        
    }

    private void LoginView_OnLoaded(object sender, RoutedEventArgs e)
    {
        EmailTextbox.Focus();
    }
}
