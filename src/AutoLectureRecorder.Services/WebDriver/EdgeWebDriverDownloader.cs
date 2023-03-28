using AutoLectureRecorder.Services.WebDriver.SeleniumUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace AutoLectureRecorder.Services.WebDriver;

public class EdgeWebDriverDownloader : IWebDriverDownloader
{
    private readonly ILogger<EdgeWebDriverDownloader> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _appDataAlrPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "AutoLectureRecorder");
    private const string WebDriverName = "msedgedriver";
    private const string CurrentEdgeDriverStoreFileName = "current_edge_version.txt";

    public EdgeWebDriverDownloader(ILogger<EdgeWebDriverDownloader> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    /// <remarks>
    /// Downloads the latest edge web driver according to browser version
    /// </remarks>
    public async Task<bool> Download(IProgress<float>? progress)
    {
        try
        {
            string edgeVersion = GetEdgeVersionFromRegistry();

            if (await IsLatestWebDriverVersionInstalled(edgeVersion))
            {
                _logger.LogInformation("The latest version of the edge web driver is already installed");
                return true; 
            }

            _logger.LogInformation("Starting automated edge web driver download...");

            DeleteOldDriverIfExists();
            string zippedFilePath = await DownloadDriverToTempFolder(edgeVersion, progress);
            ExtractDriverToAppData(zippedFilePath, edgeVersion);
            var storeVersion = StoreCurrentEdgeVersionToFile(edgeVersion);
            DeleteZippedFile(zippedFilePath);

            await storeVersion;

            _logger.LogInformation("Edge web driver downloaded successfully!");

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to download driver...");
            return false;
        }
    }

    private void DeleteOldDriverIfExists()
    {
        var webDriverDirectory = _appDataAlrPath;

        if (Directory.Exists(webDriverDirectory) == false) return;

        // Delete Driver_Notes folder
        foreach (var directory in Directory.GetDirectories(webDriverDirectory))
        {
            try
            {
                if (!directory.Contains("Driver_Notes")) continue;

                Directory.Delete(directory, true);
                _logger.LogDebug("Driver_Notes folder deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete the old existing driver");
            }
        }

        // Delete the driver
        foreach (var fileName in Directory.GetFiles(webDriverDirectory))
        {
            if (fileName.Contains(WebDriverName) == false) continue;

            File.Delete(fileName);
            _logger.LogInformation("Old web driver deleted");
        }
    }

    private async Task<bool> IsLatestWebDriverVersionInstalled(string latestEdgeVersion)
    {
        (bool success, string edgeVersion) = await RetrieveCurrentEdgeVersionFromFile();

        if (success == false) return false;

        var webDriverPath = Path.Combine(_appDataAlrPath, $"{WebDriverName}.exe");

        return File.Exists(webDriverPath) && edgeVersion == latestEdgeVersion;
    }

    private void DeleteZippedFile(string zippedFilePath)
    {
        try
        {
            File.Delete(zippedFilePath);
            _logger.LogDebug("Zipped file deleted successfully");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete zipped file...");
        }
    }

    private void ExtractDriverToAppData(string zippedFilePath, string edgeVersion)
    {
        // Extract to Auto Lecture Recorder app data directory
        ZipFile.ExtractToDirectory(zippedFilePath, _appDataAlrPath);

        _logger.LogDebug("Successfully extracted zipped file to current directory");
    }

    private async Task<string> DownloadDriverToTempFolder(string edgeVersion, IProgress<float>? progress)
    {
        string downloadLink = $@"https://msedgedriver.azureedge.net/{ edgeVersion }/edgedriver_win64.zip";

        var downloadedFilePath = Path.Combine(Path.GetTempPath(), "edgedriver_win64.zip");

        // Delete the file if it already exists
        if (File.Exists(downloadedFilePath))
        {
            File.Delete(downloadedFilePath);
        }

        using var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromMinutes(1);

        await using var file = new FileStream(downloadedFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await client.DownloadAsync(downloadLink, file, progress, CancellationToken.None);

        _logger.LogDebug("Successfully downloaded driver from link {downloadLink} to temp folder", downloadLink);

        return downloadedFilePath;
    }

    private string GetEdgeVersionFromRegistry()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) == false) return string.Empty;

        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Edge\BLBeacon");

        if (key == null)
        {
            _logger.LogWarning("Registry key was null");
            return string.Empty;
        }

        var edgeVersionObject = key.GetValue("version");

        if (edgeVersionObject == null)
        {
            _logger.LogWarning("Registry value \"version\" was null");
            return string.Empty;
        }

        var edgeVersion = new Version(edgeVersionObject as string);
        _logger.LogDebug("Edge version found on the computer: {latestEdgeVersion}", edgeVersion);
        return edgeVersion.ToString();
    }

    private async Task<bool> StoreCurrentEdgeVersionToFile(string edgeVersion)
    {
        try
        {
            string filePath = Path.Combine(_appDataAlrPath, CurrentEdgeDriverStoreFileName);
            await File.WriteAllTextAsync(filePath, edgeVersion);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to store edge version to a file");
            return false;
        }
    }

    private async Task<(bool success, string edgeVersion)> RetrieveCurrentEdgeVersionFromFile()
    {
        try
        {
            string filePath = Path.Combine(_appDataAlrPath, CurrentEdgeDriverStoreFileName);

            if (File.Exists(filePath) == false)
            {
                _logger.LogWarning("The {file} file did not exist", CurrentEdgeDriverStoreFileName);
                return (false, string.Empty);
            }

            string edgeVersion = await File.ReadAllTextAsync(filePath);
            _logger.LogInformation("Retrieved current edge version: {latestEdgeVersion}", edgeVersion);

            return (true, edgeVersion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve current Edge Version");
            return (false, string.Empty);
        }
    }
}