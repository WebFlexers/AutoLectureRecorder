using ReactiveUI.Fody.Helpers;

namespace AutoLectureRecorder.WPF.ReactiveModels;

public class ReactiveStudentAccount
{
    [Reactive]
    public int Id { get; set; }
    [Reactive]
    public string RegistrationNumber { get; set; }
    [Reactive]
    public string EmailAddress { get; set; }
    [Reactive]
    public string Password { get; set; }
}
