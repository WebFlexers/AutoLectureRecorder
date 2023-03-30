using System;
using System.Threading.Tasks;
using System.Windows;

namespace AutoLectureRecorder.Sections.LoginWebView;

/// <summary>
/// The purpose of this Window is to display it over a window containing a WebView2 control
/// in order to prevent the user from interacting manually with the WebView2 content, since
/// WebView2 is permanently TopMost in wpf with no way to change it (at the time this comment was written 30/03/2023),
/// so it's not possible to create an overlay in the same window to prevent user interaction.
/// Also events like click, key up, key down, etc... cannot be handled in WebView2. 
/// </summary>
public partial class TransparentLoadingWindow : Window
{
    private readonly Func<Task> ? _cancelDelegate;

    public TransparentLoadingWindow(Func<Task> ? cancelDelegate)
    {
        _cancelDelegate = cancelDelegate;
        InitializeComponent();

        if (_cancelDelegate == null)
        {
            CancelButton.Visibility = Visibility.Collapsed;
        }
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        SignInTextBlock.Text = "Cancelling...";
        CancelButton.IsEnabled = false;
        _cancelDelegate?.Invoke();
        CancelButton.IsEnabled = true;
    }
}
