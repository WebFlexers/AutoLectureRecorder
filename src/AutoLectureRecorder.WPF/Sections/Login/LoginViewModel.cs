using AutoLectureRecorder.DependencyInjection.Factories;
using AutoLectureRecorder.ReactiveUiUtilities;
using AutoLectureRecorder.Sections.LoginWebView;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;

namespace AutoLectureRecorder.Sections.Login;

public class LoginViewModel : ReactiveObject, IRoutableViewModel
{
    private readonly IViewModelFactory _viewModelFactory;

    public string UrlPathSegment => nameof(LoginViewModel);
    public IScreen HostScreen { get; }

    public ReactiveCommand<Unit, Unit> LoginCommand { get; }

    public LoginViewModel(IScreenFactory screenFactory, IViewModelFactory viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
        HostScreen = screenFactory.GetMainWindowViewModel();

        LoginCommand = ReactiveCommand.Create(Login);

        MessageBus.Current.Listen<string>(PubSubMessages.UpdateLoginErrorMessage).Subscribe(m => ErrorMessage = m);
    }

    [Reactive]
    public string ErrorMessage { get; set; } = string.Empty;
    public bool IsErrorMessageVisible => string.IsNullOrWhiteSpace(ErrorMessage) == false;

    [Reactive] 
    public string AcademicEmailAddress { get; set; } = string.Empty;
    [Reactive]
    public string Password { get; set; } = string.Empty;

    private void Login()
    {
        HostScreen.Router.Navigate.Execute(_viewModelFactory.CreateRoutableViewModel(typeof(LoginWebViewModel)));
        MessageBus.Current.SendMessage<(string, string)>((AcademicEmailAddress, Password), PubSubMessages.GetStudentAccount);
    }
}
