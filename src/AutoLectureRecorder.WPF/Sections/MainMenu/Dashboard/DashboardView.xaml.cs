using AutoLectureRecorder.Data.ReactiveModels;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Text;

namespace AutoLectureRecorder.WPF.Sections.MainMenu.Dashboard;

public partial class DashboardView : ReactiveUserControl<DashboardViewModel>
{
    public DashboardView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(v => v.ViewModel!.NextScheduledLectureTimeDiff)
                .Subscribe(timeDiff => UpdateNextLecture(timeDiff))
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.FindClosestScheduledLectureToNowCommand, v => v.testButton);
        });
    }

    private void UpdateNextLecture(TimeSpan? timeDiff)
    {
        if (ViewModel!.NextScheduledLecture == null)
        {
            this.nextLectureSubjectNameTextBlock.Text = "There is no lecture scheduled";
            this.nextLectureTimeTextBlock.Text = "-";
            return;
        }

        if (ViewModel!.NextScheduledLecture != null)
        {
            this.nextLectureSubjectNameTextBlock.Text = ViewModel!.NextScheduledLecture.SubjectName;

            var timeFormatBuilder = new StringBuilder();

            if (TimeSpan.Compare(timeDiff!.Value, TimeSpan.FromDays(1)) >= 0)
            {
                timeFormatBuilder.Append(string.Format("{0} Day{1}, ",
                                                            timeDiff!.Value.Days, timeDiff.Value.Days == 1 ? "" : "s"));
            }

            if (TimeSpan.Compare(timeDiff!.Value, TimeSpan.FromHours(1)) >= 0)
            {
                timeFormatBuilder.Append(string.Format("{0} Hour{1}, ",
                                                            timeDiff!.Value.Hours, timeDiff.Value.Hours == 1 ? "" : "s"));
            }

            if (TimeSpan.Compare(timeDiff!.Value, TimeSpan.FromMinutes(1)) >= 0)
            {
                timeFormatBuilder.Append(string.Format("{0} Minute{1}, ",
                                            timeDiff!.Value.Minutes, timeDiff.Value.Minutes == 1 ? "" : "s"));
            }

            timeFormatBuilder.Append(string.Format("{0} Second{1}",
                                            timeDiff!.Value.Seconds, timeDiff.Value.Seconds == 1 ? "" : "s"));

            //string formattedTime = string.Format("{0} Day{1}, {2} Hour{3}, {4} Minute{5}, {6} Second{7}",
            //          timeDiff!.Value.Days, timeDiff.Value.Days == 1 ? "" : "s",
            //          timeDiff.Value.Hours, timeDiff.Value.Hours == 1 ? "" : "s",
            //          timeDiff.Value.Minutes, timeDiff.Value.Minutes == 1 ? "" : "s",
            //          timeDiff.Value.Seconds, timeDiff.Value.Seconds == 1 ? "" : "s");

            this.nextLectureTimeTextBlock.Text = timeFormatBuilder.ToString();
        }
    }
}
