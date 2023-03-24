using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Text;

namespace AutoLectureRecorder.Sections.MainMenu.Dashboard;

public partial class DashboardView : ReactiveUserControl<DashboardViewModel>
{
    public DashboardView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            // Item Source bindings
            this.OneWayBind(ViewModel, vm => vm.TodaysLectures, v => v.TodaysLecturesListView.ItemsSource)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.NextScheduledLectureTimeDiff)
                .Subscribe(UpdateNextLecture)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.MainGrid.ActualHeight)
                .Subscribe(_ =>
                {
                    TodaysLecturesScrollViewer.Height = MainGrid.ActualHeight - 180;
                })
                .DisposeWith(disposables);
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

        // The goal here is show only the relevant times on screen.
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
}
