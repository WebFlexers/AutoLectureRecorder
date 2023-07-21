using AutoLectureRecorder.Common.Core;
using AutoLectureRecorder.Common.Navigation;
using ReactiveUI;

namespace AutoLectureRecorder.Pages.MainMenu.Upload;

public class UploadViewModel : RoutableViewModel, IRoutableViewModel
{
    public UploadViewModel(INavigationService navigationService) : base(navigationService)
    {
        
    }
}
