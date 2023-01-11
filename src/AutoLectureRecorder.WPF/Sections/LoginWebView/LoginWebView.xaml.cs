using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace AutoLectureRecorder.WPF.Sections.LoginWebView;

public partial class LoginWebView : ReactiveUserControl<LoginWebViewModel>
{
    public LoginWebView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.WebViewSource, v => v.mainWebView.Source)
                .DisposeWith(disposables);

            // Bind to LoginToMicrosoftTeams Command and clear the browsing data
            // after to prevent a change in the login proccess
            mainWebView
                .Events().CoreWebView2InitializationCompleted
                .Subscribe(async (e) =>
                {
                    await mainWebView.CoreWebView2.Profile.ClearBrowsingDataAsync().DisposeWith(disposables);
                    ViewModel?.LoginToMicrosoftTeamsCommand.Execute().Subscribe().DisposeWith(disposables);   
                }).DisposeWith(disposables);

            mainWebView.DisposeWith(disposables);
        });
    }
}
