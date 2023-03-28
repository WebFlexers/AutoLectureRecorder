namespace AutoLectureRecorder.Services.WebDriver.SeleniumUtilities;

public static class HttpClientExtensions
{
    private static float _previouslyReportedProgress = 0.0f;

    public static async Task DownloadAsync(this HttpClient client, string requestUri, Stream destination, IProgress<float>? progress = null, CancellationToken cancellationToken = default)
    {
        // Get the http headers first to examine the content length
        using var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        var contentLength = response.Content.Headers.ContentLength;

        await using var download = await response.Content.ReadAsStreamAsync(cancellationToken);
        // Ignore progress reporting when no progress reporter was 
        // passed or when the content length is unknown
        if (progress == null || !contentLength.HasValue)
        {
            await download.CopyToAsync(destination, cancellationToken);
            return;
        }

        // Convert absolute progress (bytes downloaded) into relative progress (0.0 - 1.0)
        var relativeProgress = new Progress<long>(totalBytes =>
        {
            var newValue = (float)totalBytes / contentLength.Value;

            if (Math.Abs(_previouslyReportedProgress - newValue) < 0.01) return;

            progress.Report(newValue);
            _previouslyReportedProgress = newValue;
        });

        // Use extension method to report progress while downloading
        await download.CopyToAsync(destination, 81920, relativeProgress, cancellationToken);

        progress.Report(1);
    }
}