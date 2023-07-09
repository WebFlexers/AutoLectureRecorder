namespace AutoLectureRecorder.Application.Options;

public static class WebView
{
    public static class BrowserArguments
    {
        public const string DebugPort = "9222";
        public const string RemoteDebuggingPortArgument = $"--remote-debugging-port={DebugPort}";
    }
}