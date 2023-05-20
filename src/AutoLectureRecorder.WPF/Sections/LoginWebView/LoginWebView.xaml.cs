using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace AutoLectureRecorder.Sections.LoginWebView;

public partial class LoginWebView : ReactiveUserControl<LoginWebViewModel>
{
    public LoginWebView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            if (ViewModel == null) return;

            this.OneWayBind(ViewModel, vm => vm.WebViewSource, v => v.MainWebView.Source)
                .DisposeWith(disposables);

            MainWebView
                .Events().CoreWebView2InitializationCompleted
                .Select(args => this.MainWebView)
                .InvokeCommand(this, v => v.ViewModel!.LoginToMicrosoftTeamsCommand)
                .DisposeWith(disposables);

            MainWebView.DisposeWith(disposables);
        });
    }
}
