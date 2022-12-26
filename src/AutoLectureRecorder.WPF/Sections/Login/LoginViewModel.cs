using AutoLectureRecorder.WPF.DependencyInjection.Factories;
using ReactiveUI;

namespace AutoLectureRecorder.WPF.Sections.Login;

public class LoginViewModel : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment => nameof(LoginViewModel);
    public IScreen HostScreen { get; }

    public LoginViewModel(IScreenFactory screenFactory)
    {
        HostScreen = screenFactory.GetMainWindowViewModel();
    }
}
