namespace AutoLectureRecorder.Application.Common.Options;

public static class WebViewOptions
{
    public static class BrowserArguments
    {
        public const string DebugPort = "9222";
        public const string RemoteDebuggingPortArgument = $"--remote-debugging-port={DebugPort}";
    }
}