using System.Globalization;
using System.Threading;

namespace AutoLectureRecorder.Services.InternetConnectivity;

public class WebConnection : IWebConnection
{
    public async Task<bool> IsConnectedToTheInternet(TimeSpan? timeoutMs, List<string> urls)
    {
        if (timeoutMs == null)
        {
            timeoutMs = TimeSpan.FromSeconds(5);
        }

        var culture = CultureInfo.InstalledUICulture;
        if (urls == null)
        {
            urls = new List<string>();

            if (culture.Name.StartsWith("fa"))      // Iran
                urls.Add("http://www.aparat.com");
            else if (culture.Name.StartsWith("zh")) // China
                urls.Add("http://www.baidu.com");
            else
            {
                urls.Add("https://www.apple.com/");
                urls.Add("https://www.gstatic.com/generate_204");
            }
        }

        var client = new HttpClient();
        client.Timeout = (TimeSpan)timeoutMs;
        List<Task<string>> tasks = new List<Task<string>>();
        int unresponsiveUrlCount = 0;

        foreach (var url in urls)
        {
            tasks.Add(client.GetStringAsync(url));
        }

        Task aggregationTask = null;

        try
        {
            aggregationTask = Task.WhenAll(tasks);
            await aggregationTask;
        }
        catch (Exception)
        {
            if (aggregationTask?.Exception?.InnerExceptions != null && aggregationTask.Exception.InnerExceptions.Any())
            {
                foreach (var innerEx in aggregationTask.Exception.InnerExceptions)
                {
                    unresponsiveUrlCount++;
                }
            }
        }

        return unresponsiveUrlCount < urls.Count;
    }
}