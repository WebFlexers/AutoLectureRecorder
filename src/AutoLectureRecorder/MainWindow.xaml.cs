﻿using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using AutoLectureRecorder.Resources.Themes.ThemesManager;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using Splat;

namespace AutoLectureRecorder
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();

            // Taskbar Icon
            // This is done outside of WhenActivated in order to work when
            // the app is started as a background service, since
            // Show() is not called on main window immediately, thus
            // not executing the code inside this.WhenActivated()
            ViewModel ??= Locator.Current.GetService<MainWindowViewModel>();

            this.BindCommand(ViewModel, 
                vm => vm.ShowAppCommand, 
                v => v.MainTaskbarIcon,
                Observable.Return(this), nameof(MainTaskbarIcon.TrayMouseDoubleClick));
            this.BindCommand(ViewModel, 
                vm => vm.ShowAppCommand, 
                v => v.OpenAppTrayMenuItem,
                Observable.Return(this));
            this.BindCommand(ViewModel, 
                vm => vm.ExitAppCommand, 
                v => v.ExitAppTrayMenuItem,
                Observable.Return(((Window)MainAppWindow, true)));

            MainTaskbarIcon.LeftClickCommand = ViewModel!.ShowAppCommand;
            MainTaskbarIcon.LeftClickCommandParameter = MainAppWindow;
            MainTaskbarIcon.NoLeftClickDelay = true;

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, 
                        vm => vm.Router, 
                        v => v.RoutedViewHost.Router)
                    .DisposeWith(disposables);
                
                // Help modal dialog
                this.Bind(ViewModel, 
                        vm => vm.IsHelpModalOpen, 
                        v => v.ConfirmationDialogHost.IsOpen)
                    .DisposeWith(disposables);
                
                this.BindCommand(ViewModel,
                        vm => vm.OpenBrowserHelpPageCommand,
                        v => v.OpenHelpPageButton)
                    .DisposeWith(disposables);
                
                // TitleBar
                this.BindCommand(ViewModel, 
                        vm => vm.ExitAppCommand,
                        v => v.ExitAppButton, 
                        Observable.Return(((Window)MainAppWindow, false)))
                    .DisposeWith(disposables);
                
                this.BindCommand(ViewModel, 
                        vm => vm.ToggleWindowStateCommand, 
                        v => v.ToggleWindowStateButton)
                    .DisposeWith(disposables);
                
                this.BindCommand(ViewModel, 
                        vm => vm.MinimizeWindowCommand, 
                        v => v.MinimizeWindowButton)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, 
                        vm => vm.MainWindowState, 
                        v => v.MainAppWindow.WindowState)
                    .DisposeWith(disposables);
                
                this.WhenAnyValue(v => v.MainAppWindow.WindowState)
                    .Subscribe(ws =>
                    {
                        this.ToggleWindowStateButton.Style = ws == WindowState.Maximized 
                            ? AlrResources.Styles.TitlebarRestoreDownButton
                            : AlrResources.Styles.TitlebarMaximizeButton;
                    })
                    .DisposeWith(disposables);
                
                // Minimize on close behavior
                this.MainAppWindow.Events().Closing
                    .Select(eventArgs => ((Window)MainAppWindow, eventArgs))
                    .InvokeCommand(ViewModel!.AttemptExitAppCommand)
                    .DisposeWith(disposables);
            });
        }
    }
}