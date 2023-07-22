using ReactiveUI;

namespace AutoLectureRecorder.Domain.ReactiveModels;

public class ReactiveStudentAccount : ReactiveObject
{
    public ReactiveStudentAccount(string registrationNumber, string emailAddress, string password)
    {
        _registrationNumber = registrationNumber;
        _emailAddress = emailAddress;
        _password = password;
    }
    
    private string _registrationNumber;
    public string RegistrationNumber 
    {
        get => _registrationNumber;
        set => this.RaiseAndSetIfChanged(ref _registrationNumber, value);
    }

    private string _emailAddress;
    public string EmailAddress 
    {
        get => _emailAddress;
        set => this.RaiseAndSetIfChanged(ref _emailAddress, value);
    }

    private string _password;
    public string Password 
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }
}
