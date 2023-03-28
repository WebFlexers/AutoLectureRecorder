using AutoLectureRecorder.DependencyInjection.Factories;
using AutoLectureRecorder.ReactiveUiUtilities;
using AutoLectureRecorder.Sections.LoginWebView;
using AutoLectureRecorder.Services.WebDriver;
using Microsoft.Extensions.Configuration;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using MessageBox = System.Windows.MessageBox;

namespace AutoLectureRecorder.Sections.Login;

public class LoginViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IWebDriverDownloader _webDriverDownloader;
    private readonly IConfiguration _config;

    public ViewModelActivator Activator { get; }
    public string UrlPathSegment => nameof(LoginViewModel);
    public IScreen HostScreen { get; }

    public ReactiveCommand<Unit, Unit>? LoginCommand { get; private set; }

    [Reactive]
    public string ErrorMessage { get; set; } = string.Empty;

    private readonly ObservableAsPropertyHelper<bool> _isErrorMessageVisible;
    public bool IsErrorMessageVisible => _isErrorMessageVisible.Value;
    private readonly ObservableAsPropertyHelper<bool> _isErrorMessageInvisible;
    public bool IsErrorMessageInvisible => _isErrorMessageInvisible.Value;

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

    public LoginViewModel(IScreenFactory screenFactory, IViewModelFactory viewModelFactory, IWebDriverDownloader webDriverDownloader,
        IConfiguration config)
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
        _config = config;
        _viewModelFactory = viewModelFactory;
        HostScreen = screenFactory.GetMainWindowViewModel();

        MessageBus.Current.Listen<string>(PubSubMessages.UpdateLoginErrorMessage).Subscribe(m => ErrorMessage = m);

        _isErrorMessageVisible = 
            this.WhenAnyValue(vm => vm.ErrorMessage)
                .Select(error => string.IsNullOrWhiteSpace(error) == false)
                .ToProperty(this, vm => vm.IsErrorMessageVisible);

        _isErrorMessageInvisible = 
            this.WhenAnyValue(vm => vm.ErrorMessage)
                .Select(error => string.IsNullOrWhiteSpace(error))
                .ToProperty(this, vm => vm.IsErrorMessageInvisible);

        this.WhenActivated(disposables =>
        {
            LoginCommand = ReactiveCommand.CreateFromTask(Login)
                .DisposeWith(disposables);

            _downloadWebDriverTask = Task.Run(async () => 
                await _webDriverDownloader.Download(_webDriverDownloadProgress));
        });
    }

    private async Task Login()
    {
        if (_downloadWebDriverTask == null) return;

        var supportedUniversityDomainsSection = _config.GetSection("SupportedUniversityDomains")
            .GetChildren().ToArray();
        var isGivenDomainSupported = supportedUniversityDomainsSection
            .Any(domain => AcademicEmailAddress.Contains(domain.Value!));

        var stringBuilder = new StringBuilder();
        if (isGivenDomainSupported == false)
        {
            stringBuilder.Append("Wrong domain. The supported domains are: ");
            for (int i = 0; i < supportedUniversityDomainsSection.Length; i++)
            {
                stringBuilder.Append(supportedUniversityDomainsSection[i].Value);
                stringBuilder.Append(i < supportedUniversityDomainsSection.Length - 1 ? ", " : ".");
            }
            ErrorMessage = stringBuilder.ToString();
            return;
        }

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
