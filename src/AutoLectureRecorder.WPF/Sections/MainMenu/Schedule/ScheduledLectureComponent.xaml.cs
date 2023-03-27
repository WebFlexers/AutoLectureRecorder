using System;
using AutoLectureRecorder.Data.ReactiveModels;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace AutoLectureRecorder.Sections.MainMenu.Schedule;

public partial class ScheduledLectureComponent
{
    public ScheduledLectureComponent()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty LectureProperty = DependencyProperty.Register(
        nameof(Lecture), typeof(ReactiveScheduledLecture), typeof(ScheduledLectureComponent), 
        new PropertyMetadata(default(ReactiveScheduledLecture), UpdateLecture));

    private static void UpdateLecture(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var component = d as ScheduledLectureComponent;
        var lecture = e.NewValue as ReactiveScheduledLecture;
        component!.UpdateLecture(lecture);
    }

    private void UpdateLecture(ReactiveScheduledLecture? lecture)
    {
        if (lecture?.StartTime == null || lecture.EndTime == null)
        {
            return;
        }

        SubjectNameTextBlock.Text = lecture.SubjectName;
        TimeTextBlock.Text = $"{lecture.StartTime.Value.ToString("hh:mm tt")} - {lecture.EndTime.Value.ToString("hh:mm tt")}";
        ActiveCheckBox.IsChecked = lecture.IsScheduled;
        UploadCheckBox.IsChecked = lecture.WillAutoUpload;
    }

    public ReactiveScheduledLecture Lecture
    {
        get { return (ReactiveScheduledLecture)GetValue(LectureProperty); }
        set { SetValue(LectureProperty, value); }
    }

    // Register the routed event
    public static readonly RoutedEvent ClickEvent = 
        EventManager.RegisterRoutedEvent( nameof(Click), RoutingStrategy.Bubble, 
            typeof(RoutedEventHandler), typeof(ScheduledLectureComponent));

    // .NET wrapper
    public event RoutedEventHandler Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

    private ResourceDictionary? _colors;

    private void ScheduledLectureComponent_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ClickEvent));
    }


    private void ScheduledLectureComponent_OnMouseEnter(object sender, MouseEventArgs e)
    {
        OnHover();
    }

    private void ScheduledLectureComponent_OnMouseLeave(object sender, MouseEventArgs e)
    {
        OnHoverLeave();
    }

    private void CheckboxesStackPanel_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        // Used to eat up the click event from the user control
        e.Handled = true;
    }

    private void OnHover()
    {
        _colors ??= App.GetResourceDictionary("Colors.xaml", "Resources/Colors");
        MainBorder.BorderBrush = _colors["PrimaryDarkerHoverBrush"] as SolidColorBrush;
        MainBorder.BorderThickness = new Thickness(1);
        SubjectNameBackgroundBorder1.Background = _colors["PrimaryDarkerHoverBrush"] as SolidColorBrush;
        SubjectNameBackgroundBorder2.Background = _colors["PrimaryDarkerHoverBrush"] as SolidColorBrush;
    }

    private void OnHoverLeave()
    {
        _colors ??= App.GetResourceDictionary("Colors.xaml", "Resources/Colors");
        MainBorder.BorderBrush = _colors["PrimaryDarkerBrush"] as SolidColorBrush;
        MainBorder.BorderThickness = new Thickness(0);
        SubjectNameBackgroundBorder1.Background = _colors["PrimaryDarkerBrush"] as SolidColorBrush;
        SubjectNameBackgroundBorder2.Background = _colors["PrimaryDarkerBrush"] as SolidColorBrush;
    }

    private void CheckBox_CheckedChanged(object sender, RoutedEventArgs e)
    {
        Lecture = new ReactiveScheduledLecture
        {
            Id = Lecture.Id,
            SubjectName = Lecture.SubjectName,
            Semester = Lecture.Semester,
            MeetingLink = Lecture.MeetingLink,
            Day = Lecture.Day,
            StartTime = Lecture.StartTime,
            EndTime = Lecture.EndTime,
            IsScheduled = ActiveCheckBox.IsChecked!.Value,
            WillAutoUpload = UploadCheckBox.IsChecked!.Value
        };
    }
}
