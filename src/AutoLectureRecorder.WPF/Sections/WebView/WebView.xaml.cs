using ReactiveUI;

namespace AutoLectureRecorder.WPF.Sections.WebView;

public partial class WebView : ReactiveUserControl<WebViewModel>
{
    public WebView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.WebViewSource, v => v.mainWebView.Source);
            this.BindCommand(ViewModel, vm => vm.LoginToMicrosoftTeamsCommand, v => v.mainWebView, nameof(mainWebView.CoreWebView2InitializationCompleted));
        });
    }
}
