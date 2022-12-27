using AutoLectureRecorder.WPF.DependencyInjection.Factories;
using AutoLectureRecorder.WPF.Sections.WebView;
using ReactiveUI;
using System.Reactive;

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
    }

    private void Login()
    {
        HostScreen.Router.Navigate.Execute(_viewModelFactory.CreateRoutableViewModel(typeof(WebViewModel)));
    }
}
