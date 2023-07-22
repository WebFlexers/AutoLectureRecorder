using System.Diagnostics;
using System.IO.Compression;
using System.Reactive;
using System.Runtime.InteropServices;
using AutoLectureRecorder.Application.Common.Abstractions.WebAutomation;
using AutoLectureRecorder.Application.Common.Logging;
using AutoLectureRecorder.Domain.Errors;
using AutoLectureRecorder.Infrastructure.Common.HttpUtilities;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace AutoLectureRecorder.Infrastructure.WebAutomation;

public class EdgeWebDriverDownloader : IWebDriverDownloader
{
    private readonly ILogger<EdgeWebDriverDownloader> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    
    private readonly string _downloadDriverDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "AutoLectureRecorder", "WebDriver");
    private const string WebDriverName = "msedgedriver";
    private const string WebDriverProcessName = "msedgedriver";
    private const string CurrentEdgeDriverStoreFileName = "current_edge_driver_version.txt";

    public EdgeWebDriverDownloader(ILogger<EdgeWebDriverDownloader> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    /// <remarks>
    /// Downloads the latest edge web driver according to browser version
    /// </remarks>
    public async Task<ErrorOr<Unit>> Download(IProgress<float>? progress = null)
    {
        try
        {
            string edgeVersion = GetEdgeVersionFromRegistry();

            if (await IsLatestWebDriverVersionInstalled(edgeVersion))
            {
                _logger.LogInformation("The latest version of the edge web driver is already installed");
                return Unit.Default; 
            }

            _logger.LogInformation("Starting automated edge web driver download...");

            var errorOrDriverDeleted = await DeleteOldDriverIfExists();
            if (errorOrDriverDeleted.IsError) return errorOrDriverDeleted.Errors;
            
            var errorOrZippedFilePath = await DownloadDriverToTempFolder(edgeVersion, progress);
            if (errorOrZippedFilePath.IsError) return errorOrZippedFilePath.Errors;

            var zippedFilePath = errorOrZippedFilePath.Value;
            
            ExtractDriver(zippedFilePath);
            var storeVersion = StoreCurrentEdgeVersionToFile(edgeVersion);
            DeleteZippedFile(zippedFilePath);

            var errorOrStoredVersion = await storeVersion;

            _logger.LogInformation("Edge web driver downloaded successfully!");

            return errorOrStoredVersion;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to download driver...");
            return Error.Unexpected();
        }
    }

    private async Task<ErrorOr<Unit>> DeleteOldDriverIfExists()
    {
        var webDriverDirectory = _downloadDriverDirectory;

        if (Directory.Exists(webDriverDirectory) == false) return Unit.Default;

        try
        {
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
            foreach (var filePath in Directory.GetFiles(webDriverDirectory))
            {
                if (filePath.Contains(WebDriverName) == false) continue;

                Process[] processes = Process.GetProcessesByName(WebDriverProcessName);

                if (processes.Any())
                {
                    var waitForKillTasks = new List<Task>();
                    foreach (Process process in processes)
                    {
                        process.Kill();
                        waitForKillTasks.Add(process.WaitForExitAsync());
                    }

                    await Task.WhenAll(waitForKillTasks);
                }

                File.Delete(filePath);
                _logger.LogInformation("Old web driver deleted");
            }
            
            return Unit.Default;
        }
        catch (Exception e)
        {
            var error = Errors.IO.FailedToDeleteFileOrDirectory("webdriver files");
            _logger.EfficientlyLogError(e,  error.Description, error.Code);
            return error;
        }
    }

    private async Task<bool> IsLatestWebDriverVersionInstalled(string latestEdgeVersion)
    {
        var errorOrEdgeVersion = await RetrieveCurrentEdgeVersionFromFile();

        if (errorOrEdgeVersion.IsError) return false;

        var webDriverPath = Path.Combine(_downloadDriverDirectory, $"{WebDriverName}.exe");

        var edgeVersion = errorOrEdgeVersion.Value;
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

    private void ExtractDriver(string zippedFilePath)
    {
        CreateHiddenDirectory(_downloadDriverDirectory);
        
        // Extract to Auto Lecture Recorder app data directory
        ZipFile.ExtractToDirectory(zippedFilePath, _downloadDriverDirectory);

        _logger.LogInformation("Successfully extracted zipped file to current directory");
    }

    private async Task<ErrorOr<string>> DownloadDriverToTempFolder(string edgeVersion, IProgress<float>? progress)
    {
        try
        {
            string downloadLink = $@"https://msedgedriver.azureedge.net/{edgeVersion}/edgedriver_win64.zip";

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
            
            _logger.LogInformation("Successfully downloaded driver from link {DownloadLink} to temp folder", 
                downloadLink);

            return downloadedFilePath;
        }
        catch (Exception ex)
        {
            var error = Errors.IO.FailedToDownloadError("edgedriver_win64.zip");
            _logger.EfficientlyLogError(ex, error.Description, error.Code);
            return error;
        }
    }

    private string GetEdgeVersionFromRegistry()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) == false) return string.Empty;

#pragma warning disable CA1416
        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Edge\BLBeacon");

        if (key == null)
        {
            _logger.LogWarning("Registry key was null");
            return string.Empty;
        }

        var edgeVersionObject = key.GetValue("version");
#pragma warning restore CA1416
        
        if (edgeVersionObject == null)
        {
            _logger.LogWarning("Registry value \"version\" was null");
            return string.Empty;
        }

        var edgeVersion = new Version((string)edgeVersionObject);
        _logger.LogDebug("Edge version found on the computer: {EdgeVersion}", edgeVersion);
        return edgeVersion.ToString();
    }

    private async Task<ErrorOr<Unit>> StoreCurrentEdgeVersionToFile(string edgeVersion)
    {
        CreateHiddenDirectory(_downloadDriverDirectory);
        string filePath = Path.Combine(_downloadDriverDirectory, CurrentEdgeDriverStoreFileName);
        
        try
        {
            await File.WriteAllTextAsync(filePath, edgeVersion);
            return Unit.Default;
        }
        catch (Exception ex)
        {
            var error = Errors.IO.FailedToWriteToFile(filePath);
            _logger.EfficientlyLogError(ex, error.Description, error.Code);
            return error;
        }
    }

    private async Task<ErrorOr<string>> RetrieveCurrentEdgeVersionFromFile()
    {
        try
        {
            string filePath = Path.Combine(_downloadDriverDirectory, CurrentEdgeDriverStoreFileName);

            if (File.Exists(filePath) == false)
            {
                var error = Errors.IO.FileDoesNotExist(filePath);
                _logger.EfficientlyLogWarning(error.Description, error.Code);
                return error;
            }

            string edgeVersion = await File.ReadAllTextAsync(filePath);
            _logger.LogInformation("Retrieved current edge version: {LatestEdgeVersion}", edgeVersion);

            return edgeVersion;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve current Edge Version");
            return Error.Unexpected(description: $"An unexpected error occurred: {ex.Message}");
        }
    }
    
    public void CreateHiddenDirectory(string path)
    {
        DirectoryInfo directoryInfo = Directory.CreateDirectory(path);  // Creates directory at the specified path
        
        // Sets the directory to hidden
        directoryInfo.Attributes |= FileAttributes.Hidden;
    }

}