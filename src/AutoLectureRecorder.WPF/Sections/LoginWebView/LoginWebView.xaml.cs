﻿using Microsoft.Web.WebView2.Core;
using System;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
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

            mainWebView
                .Events().CoreWebView2InitializationCompleted
                .Subscribe(async (e) =>
                {
                    await mainWebView.CoreWebView2.Profile.ClearBrowsingDataAsync();
                    ViewModel?.LoginToMicrosoftTeamsCommand.Execute().Subscribe();
                });

            //this.BindCommand(ViewModel, vm => vm.LoginToMicrosoftTeamsCommand, v => v.mainWebView, nameof(mainWebView.CoreWebView2InitializationCompleted))
            //    .DisposeWith(disposables);

            mainWebView.DisposeWith(disposables);
        });
    }
}
