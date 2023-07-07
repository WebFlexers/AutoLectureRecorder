using System;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using AutoLectureRecorder.Application.Common.Abstractions.WebAutomation;
using AutoLectureRecorder.Common.Core;
using AutoLectureRecorder.Common.Navigation;
using AutoLectureRecorder.Common.Options.Login;
using ErrorOr;
using ReactiveUI;

namespace AutoLectureRecorder.Pages.Login;

public class LoginViewModel : RoutableViewModel, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }

    private string? _errorMessage;
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    private readonly ObservableAsPropertyHelper<bool> _isErrorMessageVisible;
    public bool IsErrorMessageVisible => _isErrorMessageVisible.Value;
    private readonly ObservableAsPropertyHelper<bool> _isErrorMessageInvisible;
    public bool IsErrorMessageInvisible => _isErrorMessageInvisible.Value;

    private string? _academicEmailAddress;
    public string? AcademicEmailAddress
    {
        get => _academicEmailAddress;
        set => this.RaiseAndSetIfChanged(ref _academicEmailAddress, value);
    }
    
    private string? _password;
    public string? Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }
    
    private double _downloadProgressValue;
    public double DownloadProgressValue
    {
        get => _downloadProgressValue;
        set => this.RaiseAndSetIfChanged(ref _downloadProgressValue, value);
    }
    
    private string? _downloadProgressValueString;
    public string? DownloadProgressValueString
    {
        get => _downloadProgressValueString;
        set => this.RaiseAndSetIfChanged(ref _downloadProgressValueString, value);
    }

    private bool _isWebDriverDownloading;
    public bool IsWebDriverDownloading
    {
        get => _isWebDriverDownloading;
        set => this.RaiseAndSetIfChanged(ref _isWebDriverDownloading, value);
    }

    public ReactiveCommand<Unit, Unit>? LoginCommand { get; private set; }
    
    private Task<ErrorOr<Unit>>? _downloadWebDriverTask;

    public LoginViewModel(INavigationService navigationService, IWebDriverDownloader webDriverDownloader) 
        : base(navigationService)
    {
        Activator = new ViewModelActivator();
        IProgress<float> webDriverDownloadProgress = new Progress<float>(value =>
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                var percentRoundedValue = Math.Round(value * 100);
                DownloadProgressValue = percentRoundedValue;
                DownloadProgressValueString = percentRoundedValue.ToString(CultureInfo.CurrentCulture) + "%";
            });
        });

        _isErrorMessageVisible = 
            this.WhenAnyValue(vm => vm.ErrorMessage)
                .Select(error => string.IsNullOrWhiteSpace(error) == false)
                .ToProperty(this, vm => vm.IsErrorMessageVisible);

        _isErrorMessageInvisible = 
            this.WhenAnyValue(vm => vm.ErrorMessage)
                .Select(error => string.IsNullOrWhiteSpace(error))
                .ToProperty(this, vm => vm.IsErrorMessageInvisible);

        LoginCommand = ReactiveCommand.CreateFromTask(Login);
        
        this.WhenActivated(disposables =>
        {
            _downloadWebDriverTask = Task.Run(async () =>
            {
                IsWebDriverDownloading = true;
                return await webDriverDownloader.Download(webDriverDownloadProgress);
            }).DisposeWith(disposables);
        });
    }

    private async Task Login()
    {
        if (_downloadWebDriverTask == null) return;

        if (AcademicEmailAddress is null)
        {
            ErrorMessage = "Fill in your academic email address";
            return;
        }
        
        var supportedUniversityDomains = University.GetSupportedUniversityDomains;
        
        var isGivenDomainSupported = supportedUniversityDomains
            .Any(domain => AcademicEmailAddress.Contains(domain));

        var stringBuilder = new StringBuilder();
        if (isGivenDomainSupported == false)
        {
            stringBuilder.Append("Wrong domain. The supported domains are: ");
            for (int i = 0; i < supportedUniversityDomains.Length; i++)
            {
                stringBuilder.Append(supportedUniversityDomains[i]);
                stringBuilder.Append(i < supportedUniversityDomains.Length - 1 ? ", " : ".");
            }
            ErrorMessage = stringBuilder.ToString();
            return;
        }
        
        var errorOrWebDriverDownloaded = await _downloadWebDriverTask;
        if (errorOrWebDriverDownloaded.IsError)
        {
            ErrorMessage = "An error occurred while trying to download the necessary WebDriver";
            DownloadProgressValueString = "Failed";
            return;
        }

        /*HostScreen.Router.NavigateAndReset.Execute(_viewModelFactory.CreateRoutableViewModel(typeof(LoginWebViewModel)));
        MessageBus.Current.SendMessage<(string academicEmailAddress, string password)>((AcademicEmailAddress, Password), 
            PubSubMessages.FillLoginCredentials);*/
    }
}
