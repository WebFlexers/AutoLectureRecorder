using ReactiveUI;

namespace AutoLectureRecorder.Domain.ReactiveModels;

public class ReactiveStudentAccount : ReactiveObject
{
    public ReactiveStudentAccount(string registrationNumber, string emailAddress, string? encryptedPassword, byte[]? entropy)
    {
        _registrationNumber = registrationNumber;
        _emailAddress = emailAddress;
        _encryptedPassword = encryptedPassword;
        _entropy = entropy;
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

    private string? _encryptedPassword;
    public string? EncryptedPassword 
    {
        get => _encryptedPassword;
        set => this.RaiseAndSetIfChanged(ref _encryptedPassword, value);
    }
    
    private byte[]? _entropy;
    public byte[]? Entropy
    {
        get => _entropy;
        set => this.RaiseAndSetIfChanged(ref _entropy, value);
    }
}
