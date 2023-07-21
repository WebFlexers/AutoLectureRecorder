﻿using ReactiveUI;

namespace AutoLectureRecorder.Pages.MainMenu.Library;

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