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

    public ReactiveScheduledLecture Lecture
    {
        get { return (ReactiveScheduledLecture)GetValue(LectureProperty); }
        set { SetValue(LectureProperty, value); }
    }

    public static readonly DependencyProperty LectureProperty = DependencyProperty.Register(
        nameof(Lecture), typeof(ReactiveScheduledLecture), typeof(ScheduledLectureComponent), 
        new PropertyMetadata(default(ReactiveScheduledLecture), UpdateUI));

    private static void UpdateUI(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var component = d as ScheduledLectureComponent;
        var lecture = e.NewValue as ReactiveScheduledLecture;
        component!.UpdateUI(lecture);
    }

    private void UpdateUI(ReactiveScheduledLecture? lecture)
    {
        if (lecture?.StartTime == null || lecture.EndTime == null)
        {
            return;
        }

        _colors ??= App.GetCurrentThemeDictionary();
        if (lecture.IsScheduled)
        {
            LeftSidebarBorder.Background = _colors["SuccessBrush"] as SolidColorBrush;
        }
        else
        {
            LeftSidebarBorder.Background = _colors["SecondaryTextBrush"] as SolidColorBrush;
        }

        SubjectNameTextBlock.Text = lecture.SubjectName;
        TimeTextBlock.Text = $"{lecture.StartTime.Value.ToString("hh:mm tt")} - {lecture.EndTime.Value.ToString("hh:mm tt")}";
        ActiveCheckBox.IsChecked = lecture.IsScheduled;
        UploadCheckBox.IsChecked = lecture.WillAutoUpload;
    }

    // Events
    public static readonly RoutedEvent ClickEvent = 
        EventManager.RegisterRoutedEvent( nameof(Click), RoutingStrategy.Bubble, 
            typeof(RoutedEventHandler), typeof(ScheduledLectureComponent));

    public event RoutedEventHandler Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

    public static readonly RoutedEvent CheckedChangedEvent =
        EventManager.RegisterRoutedEvent(nameof(CheckedChanged), RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(ScheduledLectureComponent));

    public event RoutedEventHandler CheckedChanged
    {
        add => AddHandler(CheckedChangedEvent, value);
        remove => RemoveHandler(CheckedChangedEvent, value);
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

    private const double OutlineBorderThickness = 2.5;
    private void OnHover()
    {
        _colors ??= App.GetCurrentThemeDictionary();
        MainBorder.BorderBrush = _colors["PrimaryBrush"] as SolidColorBrush;
        MainBorder.BorderThickness = new Thickness(OutlineBorderThickness);

        LeftSidebarBorder.Margin = new Thickness(OutlineBorderThickness, OutlineBorderThickness, 0, OutlineBorderThickness);
        LeftSidebarBorder.Width -= OutlineBorderThickness;

        MainBorder.Background = _colors["PrimaryBackgroundBrush"] as SolidColorBrush;

    }

    private void OnHoverLeave()
    {
        _colors ??= App.GetCurrentThemeDictionary();
        MainBorder.BorderThickness = new Thickness(0);
        MainBorder.Background = _colors["SecondaryBackgroundBrush"] as SolidColorBrush;

        LeftSidebarBorder.Margin = new Thickness(0);
        LeftSidebarBorder.Width += OutlineBorderThickness;
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

        RaiseEvent(new RoutedEventArgs(CheckedChangedEvent));
    }
}
