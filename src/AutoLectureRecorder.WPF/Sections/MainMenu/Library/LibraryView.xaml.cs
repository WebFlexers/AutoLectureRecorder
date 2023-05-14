using System.Reactive.Disposables;
using ReactiveUI;

namespace AutoLectureRecorder.Sections.MainMenu.Library;

public partial class LibraryView : ReactiveUserControl<LibraryViewModel>
{
    public LibraryView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            DataContext = ViewModel;
        });
    }
}
