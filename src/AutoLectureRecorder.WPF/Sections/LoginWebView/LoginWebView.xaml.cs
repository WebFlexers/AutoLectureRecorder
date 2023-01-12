using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace AutoLectureRecorder.Sections.LoginWebView;

public partial class LoginWebView : ReactiveUserControl<LoginWebViewModel>
{
    public LoginWebView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.WebViewSource, v => v.MainWebView.Source)
                .DisposeWith(disposables);

            // Bind to LoginToMicrosoftTeams Command and clear the browsing data
            // after to prevent a change in the login proccess
            MainWebView
                .Events().CoreWebView2InitializationCompleted
                .Subscribe(async (e) =>
                {
                    await MainWebView.CoreWebView2.Profile.ClearBrowsingDataAsync().DisposeWith(disposables);
                    ViewModel?.LoginToMicrosoftTeamsCommand.Execute().Subscribe().DisposeWith(disposables);   
                }).DisposeWith(disposables);

            MainWebView.DisposeWith(disposables);
        });
    }
}
