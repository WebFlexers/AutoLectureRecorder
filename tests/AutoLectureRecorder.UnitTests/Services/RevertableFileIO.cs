using System.Diagnostics;

namespace AutoLectureRecorder.UnitTests.Services;

/// <summary>
/// Use this class to keep backups before attemting to
/// modify files and restore them if you want
/// </summary>
public class RevertableFileIO
{
    private List<(string backupPath, string initialFilePath)> _backupFilePaths = new();

    private readonly string _backupsDirectory;

    public RevertableFileIO(string backupsDirectory)
    {
        _backupsDirectory = backupsDirectory;

        Directory.CreateDirectory(_backupsDirectory);
        foreach (string file in Directory.GetFiles(backupsDirectory))
        {
            File.Delete(file);
        }
    }

    /// <summary>
    /// Deletes the file if it exists.
    /// </summary>
    public bool DeleteFile(string filePath)
    {
		try
		{
            if (File.Exists(filePath) == false)
            {
                Debug.WriteLine("The file did not exist");
                return true;
            }

            // Create a backup of the file before deleting it
            string backupPath = Path.Combine(_backupsDirectory, Path.GetFileName(filePath));
            File.Copy(filePath, backupPath);

            _backupFilePaths.Add((backupPath, filePath));

            File.Delete(filePath);

            return true;
		}
		catch (Exception ex)
		{
            Debug.WriteLine(ex);
            return false;
		}
    }

    /// <summary>
    /// Backs up a specific file
    /// </summary>
    public void BackupFile(string filePath)
    {
        string backupPath = Path.Combine(_backupsDirectory, Path.GetFileName(filePath));
        File.Copy(filePath, backupPath);
        _backupFilePaths.Add((backupPath, filePath));
    }

    /// <summary>
    /// Restores all the backed up files
    /// </summary>
    public void RestoreBackup()
    {
        foreach ((string backupPath, string initialFilePath) in _backupFilePaths)
        {
            if (File.Exists(initialFilePath))
            {
                File.Delete(initialFilePath);
            }

            File.Copy(backupPath, initialFilePath);
        }

        _backupFilePaths.Clear();
    }
}
