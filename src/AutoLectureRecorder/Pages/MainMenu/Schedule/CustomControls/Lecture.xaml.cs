using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AutoLectureRecorder.Resources.Themes.ThemesManager;
using Splat;

namespace AutoLectureRecorder.Pages.MainMenu.Schedule.CustomControls;

// TODO: Change the CheckChanged events to Click events
public partial class Lecture : UserControl
{
    private readonly ResourceDictionary? _colors;
    private bool _isInitialized;
    
    public Lecture()
    {
        _colors = Locator.Current.GetService<IThemeManager>()?.GetCurrentThemeDictionary();
        InitializeComponent();
    }
    
    public LectureViewModel LectureViewModel
    {
        get { return (LectureViewModel)GetValue(LectureViewModelProperty); }
        set { SetValue(LectureViewModelProperty, value); }
    }

    public static readonly DependencyProperty LectureViewModelProperty = DependencyProperty.Register(
        nameof(LectureViewModel), typeof(LectureViewModel), typeof(Lecture), 
        new PropertyMetadata(default(LectureViewModel), UpdateUI));

    private static void UpdateUI(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var component = d as Lecture;
        var lecture = e.NewValue as LectureViewModel;
        component!.UpdateUI(lecture);
    }

    private void UpdateActiveStatusEllipseColor(bool isActive)
    {
        if (isActive)
        {
            ActiveStatusEllipse.Fill = _colors?["SuccessBrush"] as SolidColorBrush;
        }
        else
        {
            ActiveStatusEllipse.Fill = _colors?["OnBackgroundSecondaryBrush"] as SolidColorBrush;
        }
    }
    
    private void UpdateUI(LectureViewModel? lectureVm)
    {
        if (lectureVm?.ScheduledLecture is null) return;
        
        UpdateActiveStatusEllipseColor(lectureVm.ScheduledLecture.IsScheduled);
        
        SubjectNameTextBlock.Text = lectureVm.ScheduledLecture.SubjectName;
        TimeTextBlock.Text = $"{lectureVm.ScheduledLecture.StartTime.ToString("hh:mmtt")} - " +
                             $"{lectureVm.ScheduledLecture.EndTime.ToString("hh:mmtt")}";
        SelectedCheckBox.IsChecked = lectureVm.IsSelected;
        ActiveCheckBox.IsChecked = lectureVm.ScheduledLecture.IsScheduled;
        UploadCheckBox.IsChecked = lectureVm.ScheduledLecture.WillAutoUpload;
    }
    
    public static readonly RoutedEvent StatusChangedEvent =
        EventManager.RegisterRoutedEvent(nameof(StatusChanged), RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(Lecture));

    public event RoutedEventHandler StatusChanged
    {
        add => AddHandler(StatusChangedEvent, value);
        remove => RemoveHandler(StatusChangedEvent, value);
    }
    
    private void ActiveCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
    {
        if (_isInitialized == false) return;
        
        var isScheduled = ActiveCheckBox.IsChecked!.Value;
        LectureViewModel.ScheduledLecture.IsScheduled = isScheduled;
        
        RaiseEvent(new RoutedEventArgs(StatusChangedEvent));
        
        UpdateActiveStatusEllipseColor(isScheduled);
    }
    
    private void UploadCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
    {
        if (_isInitialized == false) return;
        
        LectureViewModel.ScheduledLecture.WillAutoUpload = UploadCheckBox.IsChecked!.Value;
        RaiseEvent(new RoutedEventArgs(StatusChangedEvent));
    }
    
    public static readonly RoutedEvent SelectedChangedEvent =
        EventManager.RegisterRoutedEvent(nameof(SelectedChanged), RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(Lecture));

    public event RoutedEventHandler SelectedChanged
    {
        add => AddHandler(SelectedChangedEvent, value);
        remove => RemoveHandler(SelectedChangedEvent, value);
    }
    private void SelectorCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
    {
        if (_isInitialized == false) return;
        
        LectureViewModel.IsSelected = SelectedCheckBox.IsChecked!.Value;
        RaiseEvent(new RoutedEventArgs(SelectedChangedEvent));
    }

    public static readonly RoutedEvent EditClickEvent = 
        EventManager.RegisterRoutedEvent( nameof(EditClick), RoutingStrategy.Bubble, 
            typeof(RoutedEventHandler), typeof(Lecture));

    public event RoutedEventHandler EditClick
    {
        add => AddHandler(EditClickEvent, value);
        remove => RemoveHandler(EditClickEvent, value);
    }
    
    private void EditButton_OnClick(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(EditClickEvent));
    }

    private void Lecture_OnLoaded(object sender, RoutedEventArgs e)
    {
        _isInitialized = true;
    }
}