using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Threading;

namespace AutoLectureRecorder.Services.WebDriver.SeleniumUtility.Windows;

public class EdgeWebDriverDownloader : IWebDriverDownloader
{
    private const string WEB_DRIVER_NAME = "msedgedriver";

    /// <inheritdoc/>
    /// <remarks>
    /// Downloads the latest edge web driver according to browser version
    /// </remarks>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="System.Security.SecurityException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception> 
    /// <exception cref="IOException"></exception> 
    /// <exception cref="HttpRequestException"></exception> 
    /// <exception cref="TaskCanceledException"></exception> 
    public async Task<string> Download(IProgress<float> progress)
    {
        string edgeVersion = await GetEdgeVersionFromRegistry();

        if (await IsLatestWebDriverVersionInstalled(edgeVersion))
        {
            Debug.WriteLine("The latest version of the edge web driver is already installed");
            return string.Empty; 
        }

        Debug.WriteLine("Starting automated edge web driver download...");

        await DeleteOldDriverIfExists();
        string zippedFilePath = await DownloadDriverToTempFolder(edgeVersion, progress);
        string driverFileName = await ExtractDriverToWorkingDirectory(zippedFilePath, edgeVersion);
        await DeleteZippedFile(zippedFilePath);

        Debug.WriteLine("Edge web driver downloaded successfully!");

        return driverFileName;
    }

    private Task DeleteOldDriverIfExists()
    {
        // Delete Driver_Notes folder
        foreach (var directory in Directory.GetDirectories(Directory.GetCurrentDirectory()))
        {
            try
            {
                if (directory.Contains("Driver_Notes"))
                {
                    Directory.Delete(directory, true);
                    Debug.WriteLine("Driver_Notes folder deleted");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        // Delete the driver
        foreach (var fileName in Directory.GetFiles(Directory.GetCurrentDirectory()))
        {
            if (fileName.Contains(WEB_DRIVER_NAME))
            {
                File.Delete(fileName);
                Debug.WriteLine("Old web driver deleted");
            }
        }

        return Task.CompletedTask;
    }

    private Task<bool> IsLatestWebDriverVersionInstalled(string edgeVersion)
    {
        if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), $"{WEB_DRIVER_NAME}_{edgeVersion}.exe")))
            return Task.FromResult(true);
        else
            return Task.FromResult(false);
    }

    private Task DeleteZippedFile(string zippedFilePath)
    {
        Debug.WriteLine("Deleting zipped file...");
        File.Delete(zippedFilePath);
        return Task.CompletedTask;
    }

    private Task<string> ExtractDriverToWorkingDirectory(string zippedFilePath, string edgeVersion)
    {
        Debug.WriteLine("Extracting zipped file to current directory...");

        string updatedDriverName = string.Empty;
        // Rename web driver to contain the edge version (e.g. msedgedriver_106.0.1370.52.exe)
        // In order to be able to identify later if the latest version of the web driver is installed
        using (var archive = new ZipArchive(File.Open(zippedFilePath, FileMode.Open, FileAccess.ReadWrite), ZipArchiveMode.Update))
        {
            var entries = archive.Entries.ToArray();
            foreach (var entry in entries)
            {
                if (entry.Name.Contains(WEB_DRIVER_NAME))
                {
                    var newEntry = archive.CreateEntry($"{WEB_DRIVER_NAME}_{edgeVersion}.exe");

                    using (var entryWithOldName = entry.Open())
                    using (var entryWithNewName = newEntry.Open())
                        entryWithOldName.CopyTo(entryWithNewName);
                    entry.Delete();

                    updatedDriverName = newEntry.Name;
                }
            }
        }

        // Extract to current directory
        ZipFile.ExtractToDirectory(zippedFilePath, Directory.GetCurrentDirectory());

        // Return the file name of the driver
        return Task.FromResult(updatedDriverName);
    }

    private async Task<string> DownloadDriverToTempFolder(string edgeVersion, IProgress<float> progress)
    {
        string downloadLink = $@"https://msedgedriver.azureedge.net/{ edgeVersion }/edgedriver_win64.zip";
        Debug.WriteLine($"Downloading driver from link { downloadLink } to temp folder...");

        var downloadedFilePath = Path.Combine(Path.GetTempPath(), "edgedriver_win64.zip");

        // Delete the file if it already exists
        if (File.Exists(downloadedFilePath))
        {
            File.Delete(downloadedFilePath);
        }

        // Download the latest driver according to edge version
        // See https://stackoverflow.com/a/46497896/15011818 for details about progress report implementation
        //using (var client = new HttpClient())
        //{
        //    using (var stream = await client.GetStreamAsync(downloadLink))
        //    {
        //        using (var fileStream = new FileStream(downloadedFilePath, FileMode.CreateNew))
        //        {
        //            await stream.CopyToAsync(fileStream);
        //            Debug.WriteLine($"Successfully downloaded driver to { downloadedFilePath } ");
        //            return downloadedFilePath;
        //        }
        //    }
        //}

        // Seting up the http client used to download the data
        using (var client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromMinutes(5);

            // Create a file stream to store the downloaded data.
            // This really can be any type of writeable stream.
            using (var file = new FileStream(downloadedFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                // Use the custom extension method below to download the data.
                // The passed progress-instance will receive the download status updates.
                await client.DownloadAsync(downloadLink, file, progress, CancellationToken.None);
                return downloadedFilePath;
            }
        }
    }

    private Task<string> GetEdgeVersionFromRegistry()
    {
        Debug.WriteLine("Getting edge browser version...");
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Edge\BLBeacon"))
            {
                if (key != null)
                {
                    Object edgeVersionObject = key.GetValue("version");
                    if (edgeVersionObject != null)
                    {
                        Version edgeVersion = new Version(edgeVersionObject as string);
                        Debug.WriteLine($"Edge version found on the computer: { edgeVersion.ToString() }");
                        return Task.FromResult(edgeVersion.ToString());
                    }
                    else
                    {
                        Debug.WriteLine("Registry value \"version\" was null");
                    }
                }
                else
                {
                    Debug.WriteLine($"Registry key was null");
                }
            }
        }

        return Task.FromResult(string.Empty);
    }
}