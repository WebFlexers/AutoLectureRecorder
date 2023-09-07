using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;

namespace AutoLectureRecorder.Pages.MainMenu.Library;

public partial class LibraryView : ReactiveUserControl<LibraryViewModel>
{
    public LibraryView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            DataContext = ViewModel;

            this.OneWayBind(ViewModel,
                    vm => vm.DoAnyLecturesExist,
                    v => v.NoLecturesTextBlock.Visibility,
                    doAnyLecturesExist => doAnyLecturesExist ? Visibility.Collapsed : Visibility.Visible)
                .DisposeWith(disposables);
        });
    }
}
