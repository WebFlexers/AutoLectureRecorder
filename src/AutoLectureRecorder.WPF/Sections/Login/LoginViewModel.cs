using AutoLectureRecorder.DependencyInjection.Factories;
using AutoLectureRecorder.ReactiveUiUtilities;
using AutoLectureRecorder.Sections.LoginWebView;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Globalization;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using AutoLectureRecorder.Services.WebDriver;
using MessageBox = System.Windows.MessageBox;
using System.Diagnostics;

namespace AutoLectureRecorder.Sections.Login;

public class LoginViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IWebDriverDownloader _webDriverDownloader;

    public ViewModelActivator Activator { get; }
    public string UrlPathSegment => nameof(LoginViewModel);
    public IScreen HostScreen { get; }

    public ReactiveCommand<Unit, Unit>? LoginCommand { get; private set; }

    [Reactive]
    public string ErrorMessage { get; set; } = string.Empty;

    public bool IsErrorMessageVisible => string.IsNullOrWhiteSpace(ErrorMessage) == false;
    public bool IsErrorMessageInvisible => string.IsNullOrWhiteSpace(ErrorMessage);

    [Reactive] 
    public string AcademicEmailAddress { get; set; } = string.Empty;
    [Reactive]
    public string Password { get; set; } = string.Empty;
    [Reactive] 
    public double DownloadProgressValue { get; set; }
    [Reactive] 
    public string DownloadProgressValueString { get; set; } = "0%";
    [Reactive]
    public Visibility WebDriverProgressVisibility { get; set; } = Visibility.Collapsed;

    private Task<bool>? _downloadWebDriverTask;
    private IProgress<float> _webDriverDownloadProgress;

    public LoginViewModel(IScreenFactory screenFactory, IViewModelFactory viewModelFactory, IWebDriverDownloader webDriverDownloader)
    {
        Activator = new ViewModelActivator();
        _webDriverDownloadProgress = new Progress<float>(value =>
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                var percentRoundedValue = Math.Round(value * 100);
                DownloadProgressValue = percentRoundedValue;
                DownloadProgressValueString = percentRoundedValue.ToString(CultureInfo.CurrentCulture) + "%";
            });
        });
        _webDriverDownloader = webDriverDownloader;
        _viewModelFactory = viewModelFactory;
        HostScreen = screenFactory.GetMainWindowViewModel();

        MessageBus.Current.Listen<string>(PubSubMessages.UpdateLoginErrorMessage).Subscribe(m => ErrorMessage = m);

        this.WhenActivated(disposables =>
        {
            LoginCommand = ReactiveCommand.CreateFromTask(Login)
                .DisposeWith(disposables);

            _downloadWebDriverTask = Task.Run(async () =>
            {
                return await _webDriverDownloader.Download(_webDriverDownloadProgress);
            });
        });
    }

    private async Task Login()
    {
        if (_downloadWebDriverTask == null) return;

        if (_downloadWebDriverTask.IsCompleted == false)
        {
            WebDriverProgressVisibility = Visibility.Visible;
        }

        bool downloadWebDriverResult = await _downloadWebDriverTask;

        if (downloadWebDriverResult == false)
        {
            MessageBox.Show("Failed to initialize AutoLectureRecorder",
                "Initialization failed. Make sure you have internet access and try again",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            DownloadProgressValueString = "Failed";
            return;
        }

        HostScreen.Router.Navigate.Execute(_viewModelFactory.CreateRoutableViewModel(typeof(LoginWebViewModel)));
        MessageBus.Current.SendMessage<(string academicEmailAddress, string password)>((AcademicEmailAddress, Password), 
            PubSubMessages.PrepareLogin);

        _downloadWebDriverTask.Dispose();
    }
}
