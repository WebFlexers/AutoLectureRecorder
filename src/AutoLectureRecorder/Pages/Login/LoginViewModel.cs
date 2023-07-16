using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using AutoLectureRecorder.Application.Common.Abstractions.WebAutomation;
using AutoLectureRecorder.Application.Common.Options;
using AutoLectureRecorder.Common.Core;
using AutoLectureRecorder.Common.Navigation;
using AutoLectureRecorder.Common.Navigation.Parameters;
using ErrorOr;
using ReactiveUI;

namespace AutoLectureRecorder.Pages.Login;

public class LoginViewModel : RoutableViewModel, IActivatableViewModel
{
    private readonly IWebDriverDownloader _webDriverDownloader;
    private CompositeDisposable _disposables = new();
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

    private bool _isWebDriverDownloading = false;
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
        _webDriverDownloader = webDriverDownloader;
        
        var parameters = NavigationService.GetNavigationParameters(typeof(LoginViewModel));
        if (parameters is not null)
        {
            ErrorMessage = (string)parameters[NavigationParameters.LoginViewModel.ErrorMessage];
        }

        Activator = new ViewModelActivator();

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
            Observable.FromAsync(PrepareLogin)
                .Subscribe()
                .DisposeWith(disposables);

            _disposables.DisposeWith(disposables);
        });
    }

    private async Task PrepareLogin()
    {
        IsWebDriverDownloading = true;

        IProgress<float> webDriverDownloadProgress = new Progress<float>(value =>
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                var percentRoundedValue = Math.Round(value * 100);
                DownloadProgressValue = percentRoundedValue;
                DownloadProgressValueString = percentRoundedValue.ToString(CultureInfo.CurrentCulture) + "%";
            });
        });
        
        _downloadWebDriverTask = Task.Run(async () =>
        {
            var errorOrDriverDownloader = await _webDriverDownloader.Download(webDriverDownloadProgress);
            if (errorOrDriverDownloader.IsError)
            {
                ErrorMessage = "An error occurred while trying to download the necessary WebDriver";
                DownloadProgressValueString = "Failed";
            }

            return errorOrDriverDownloader;
        }).DisposeWith(_disposables);

        LoginCommand = ReactiveCommand.CreateFromTask(Login);

        var errorOrDriverDownloaded = await _downloadWebDriverTask;
        errorOrDriverDownloaded.Match(_ =>
        {
            IsWebDriverDownloading = false;
            return Unit.Default;
        },
        errors =>
        {
            MessageBox.Show(errors[0].Description, errors[0].Code, 
                MessageBoxButton.OK, MessageBoxImage.Error);
            return Unit.Default;
        });
    }

    private async Task Login()
    {
        if (_downloadWebDriverTask is not null && _downloadWebDriverTask.IsCompleted == false) return;
        
        if (AcademicEmailAddress is null)
        {
            ErrorMessage = "Fill in your academic email address";
            return;
        }

        if (Password is null)
        {
            ErrorMessage = "Fill in your password";
            return;
        }
        
        var supportedUniversityDomains = LoginOptions.GetSupportedUniversityDomains;
        
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

        var parameters = new Dictionary<string, object>()
        {
            { NavigationParameters.LoginWebViewModel.AcademicEmailAddress, AcademicEmailAddress },
            { NavigationParameters.LoginWebViewModel.Password, Password }
        };
        NavigationService.NavigateAndReset(typeof(LoginWebViewModel), HostNames.MainWindowHost, parameters);
    }
}
