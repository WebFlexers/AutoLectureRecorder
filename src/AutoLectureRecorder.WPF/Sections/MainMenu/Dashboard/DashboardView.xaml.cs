using ReactiveUI;
using System;
using System.Globalization;
using System.Reactive.Disposables;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AutoLectureRecorder.Sections.MainMenu.Dashboard;

public partial class DashboardView : ReactiveUserControl<DashboardViewModel>
{
    public DashboardView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            DataContext = ViewModel;

            // Item Source bindings
            this.OneWayBind(ViewModel, vm => vm.TodaysLectures, v => v.TodaysLecturesListView.ItemsSource)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.NextScheduledLectureTimeDiff)
                .Subscribe(UpdateNextLecture)
                .DisposeWith(disposables);

            // Text bindings
            this.OneWayBind(ViewModel, vm => vm.RegistrationNumber, v => v.RegistrationNumTextBox.Text,
                    rn => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(rn))
                .DisposeWith(disposables);

            // Visibilities
            this.OneWayBind(ViewModel, vm => vm.AreLecturesScheduledToday, v => v.TodaysLecturesScrollViewer.Visibility,
                    value => value ? Visibility.Visible : Visibility.Collapsed)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.AreLecturesScheduledToday, v => v.TodaysLecturesErrorTextBlock.Visibility,
                    value => value ? Visibility.Collapsed : Visibility.Visible)
                .DisposeWith(disposables);

            // Commands
            this.BindCommand(ViewModel, vm => vm.NavigateToCreateLectureCommand, v => v.ScheduleLecturesButton)
                .DisposeWith(disposables);

            // Responsiveness
            this.WhenAnyValue(v => v.MainGrid.ActualHeight)
            .Subscribe(_ =>
            {
                TodaysLecturesScrollViewer.Height = MainGrid.ActualHeight - 313;
            }).DisposeWith(disposables);
        });
    }

    /// <summary>
    /// Updates the subject name and time distance text 
    /// </summary>
    private void UpdateNextLecture(TimeSpan? timeDiff)
    {
        if (timeDiff == null || ViewModel!.NextScheduledLecture == null)
        {
            this.NextLectureSubjectNameTextBlock.Text = "There is no lecture scheduled";
            this.NextLectureTimeTextBlock.Text = "-";
            return;
        }

        this.NextLectureSubjectNameTextBlock.Text = ViewModel.NextScheduledLecture.SubjectName;

        // The goal here is to show only the relevant times on screen.
        // So for example, if the time distance from now to the closest scheduled lecture
        // is less than a day, we will not show 0 Days, but hide the days completely.
        // The same applies for hours and minutes
        var timeFormatBuilder = new StringBuilder();

        if (TimeSpan.Compare(timeDiff.Value, TimeSpan.FromDays(1)) >= 0)
        {
            timeFormatBuilder.Append($"{timeDiff.Value.Days} Day{(timeDiff.Value.Days == 1 ? "" : "s")}, ");
        }

        if (TimeSpan.Compare(timeDiff.Value, TimeSpan.FromHours(1)) >= 0)
        {
            timeFormatBuilder.Append($"{timeDiff.Value.Hours} Hour{(timeDiff.Value.Hours == 1 ? "" : "s")}, ");
        }

        if (TimeSpan.Compare(timeDiff.Value, TimeSpan.FromMinutes(1)) >= 0)
        {
            timeFormatBuilder.Append($"{timeDiff.Value.Minutes} Minute{(timeDiff.Value.Minutes == 1 ? "" : "s")}, ");
        }

        timeFormatBuilder.Append($"{timeDiff.Value.Seconds} Second{(timeDiff.Value.Seconds == 1 ? "" : "s")}");

        this.NextLectureTimeTextBlock.Text = timeFormatBuilder.ToString();
    }

    private void TodaysLecturesScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (sender is not ScrollViewer scrollViewer) return;

        var downArrowPackIcon = LecturesDownArrowPackIcon;

        if (downArrowPackIcon == null) return;

        if (scrollViewer.VerticalOffset.Equals(scrollViewer.ScrollableHeight))
        {
            downArrowPackIcon.Visibility = Visibility.Hidden;
        }
        else
        {
            downArrowPackIcon.Visibility = Visibility.Visible;
        }
    }
}
