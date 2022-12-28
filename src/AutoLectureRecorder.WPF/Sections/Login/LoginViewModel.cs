using AutoLectureRecorder.WPF.DependencyInjection.Factories;
using AutoLectureRecorder.WPF.Sections.LoginWebView;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;
using System;

namespace AutoLectureRecorder.WPF.Sections.Login;

public class LoginViewModel : ReactiveObject, IRoutableViewModel
{
    private readonly IViewModelFactory _viewModelFactory;

    public string? UrlPathSegment => nameof(LoginViewModel);
    public IScreen HostScreen { get; }

    public ReactiveCommand<Unit, Unit> LoginCommand { get; private set; }

    public LoginViewModel(IScreenFactory screenFactory, IViewModelFactory viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
        HostScreen = screenFactory.GetMainWindowViewModel();

        LoginCommand = ReactiveCommand.Create(Login);

        MessageBus.Current.Listen<string>("LoginErrorMessage").Subscribe(m => ErrorMessage = m);
    }

    [Reactive]
    public string ErrorMessage { get; set; }
    public bool IsErrorMessageVisible { get => string.IsNullOrWhiteSpace(ErrorMessage) == false; }

    [Reactive]
    public string AcademicEmailAddress { get; set; }
    [Reactive]
    public string Password { get; set; }
    private void Login()
    {
        HostScreen.Router.Navigate.Execute(_viewModelFactory.CreateRoutableViewModel(typeof(LoginWebViewModel)));
        MessageBus.Current.SendMessage<(string, string)>((AcademicEmailAddress, Password), "StudentAccount");
    }
}
