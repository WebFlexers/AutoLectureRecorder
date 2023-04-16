using AutoLectureRecorder.Data.ReactiveModels;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;
using Splat;

namespace AutoLectureRecorder.Sections.MainMenu.Schedule;

public partial class ScheduledLectureView : ReactiveUserControl<ScheduledLectureViewModel>
{
    public ScheduledLectureView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            ViewModel ??= Locator.Current.GetService<ScheduledLectureViewModel>();

            // Bind the dependency property to the ViewModel
            this.WhenAnyValue(v => v.Lecture)
                .BindTo(this, v => v.ViewModel!.ScheduledLecture)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.ScheduledLecture.SubjectName, v => v.SubjectNameTextBlock.Text)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.DisplayTime, v => v.TimeTextBlock.Text)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.ScheduledLecture.IsScheduled, v => v.ActiveCheckBox.IsChecked)
                .DisposeWith(disposables);
            this.Bind(ViewModel, vm => vm.ScheduledLecture.WillAutoUpload, v => v.UploadCheckBox.IsChecked)
                .DisposeWith(disposables);

            CheckboxesStackPanel
                .Events().MouseDown
                .Subscribe(e => e.Handled = true)
                .DisposeWith(disposables);

            MainScheduledLectureView
                .Events().MouseEnter
                .Subscribe(_ => OnHover())
                .DisposeWith(disposables);

            MainScheduledLectureView
                .Events().MouseLeave
                .Subscribe(_ => OnHoverLeave())
                .DisposeWith(disposables);

            MainScheduledLectureView
                .Events().MouseDown
                .Select(args => Unit.Default)
                .InvokeCommand(ViewModel!.NavigateToCreateLectureCommand)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.ScheduledLecture.IsScheduled)
                .Subscribe(isScheduled =>
                {
                    _colors ??= App.GetCurrentThemeDictionary();
                    if (isScheduled)
                    {
                        LeftSidebarBorder.Background = _colors["SuccessBrush"] as SolidColorBrush;
                    }
                    else
                    {
                        LeftSidebarBorder.Background = _colors["SecondaryTextBrush"] as SolidColorBrush;
                    }
                })
                .DisposeWith(disposables);
        });
    }

    public ReactiveScheduledLecture Lecture
    {
        get { return (ReactiveScheduledLecture)GetValue(LectureProperty); }
        set { SetValue(LectureProperty, value); }
    }

    public static readonly DependencyProperty LectureProperty = DependencyProperty.Register(
        nameof(Lecture), typeof(ReactiveScheduledLecture), typeof(ScheduledLectureView), 
        new PropertyMetadata(default(ReactiveScheduledLecture)));

    //private static void UpdateUI(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //{
    //    var component = d as ScheduledLectureView;
    //    var lecture = e.NewValue as ReactiveScheduledLecture;
    //    component!.UpdateUI(lecture);
    //}

    //private void UpdateUI(ReactiveScheduledLecture? lecture)
    //{
    //    if (lecture?.StartTime == null || lecture.EndTime == null)
    //    {
    //        return;
    //    }

    //    _colors ??= App.GetCurrentThemeDictionary();
    //    if (lecture.IsScheduled)
    //    {
    //        LeftSidebarBorder.Background = _colors["SuccessBrush"] as SolidColorBrush;
    //    }
    //    else
    //    {
    //        LeftSidebarBorder.Background = _colors["SecondaryTextBrush"] as SolidColorBrush;
    //    }

    //    //SubjectNameTextBlock.Text = lecture.SubjectName;
    //    //TimeTextBlock.Text = $"{lecture.StartTime.Value.ToString("hh:mm tt")} - {lecture.EndTime.Value.ToString("hh:mm tt")}";
    //    //ActiveCheckBox.IsChecked = lecture.IsScheduled;
    //    //UploadCheckBox.IsChecked = lecture.WillAutoUpload;
    //}

    private ResourceDictionary? _colors;
    private const double OutlineBorderThickness = 2.5;
    private void OnHover()
    {
        _colors ??= App.GetCurrentThemeDictionary();
        MainBorder.BorderBrush = _colors["PrimaryBrush"] as SolidColorBrush;
        MainBorder.BorderThickness = new Thickness(OutlineBorderThickness);

        LeftSidebarBorder.Margin = new Thickness(OutlineBorderThickness, OutlineBorderThickness, 0, OutlineBorderThickness);
        LeftSidebarBorder.Width -= OutlineBorderThickness;
        LeftSidebarBorder.CornerRadius = new CornerRadius(3);
    }

    private void OnHoverLeave()
    {
        _colors ??= App.GetCurrentThemeDictionary();
        MainBorder.BorderThickness = new Thickness(0);
        MainBorder.Background = _colors["SecondaryBackgroundBrush"] as SolidColorBrush;

        LeftSidebarBorder.Margin = new Thickness(0);
        LeftSidebarBorder.Width += OutlineBorderThickness;
        LeftSidebarBorder.CornerRadius = new CornerRadius(0);
    }
}
