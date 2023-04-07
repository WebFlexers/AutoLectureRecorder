using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using AutoLectureRecorder.Resources.Themes;
using Microsoft.Web.WebView2.Core;

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
            // after to prevent a change in the login process
            MainWebView
                .Events().CoreWebView2InitializationCompleted
                .Subscribe(async (e) =>
                {
                    if (ThemeManager.CurrentColorTheme == ColorTheme.Dark)
                    {
                        MainWebView.CoreWebView2.Profile.PreferredColorScheme = CoreWebView2PreferredColorScheme.Dark;
                    }
                    else
                    {
                        MainWebView.CoreWebView2.Profile.PreferredColorScheme = CoreWebView2PreferredColorScheme.Light;
                    }
                    
                    await MainWebView.CoreWebView2.Profile.ClearBrowsingDataAsync().DisposeWith(disposables);
                    ViewModel?.LoginToMicrosoftTeamsCommand.Execute().Subscribe().DisposeWith(disposables);   
                }).DisposeWith(disposables);

            MainWebView.DisposeWith(disposables);
        });
    }
}
