namespace AutoLectureRecorder.Common.Navigation.Parameters;

public static partial class NavigationParameters
{
    public static class LoginViewModel
    {
        public const string ErrorMessage = nameof(ErrorMessage);
    }
    
    public static class LoginWebViewModel
    {
        public const string AcademicEmailAddress = nameof(AcademicEmailAddress);
        public const string Password = nameof(Password);
    }
}