using AutoLectureRecorder.Services.ShortcutManager;
using System.Text;

namespace AutoLectureRecorder.UnitTests.Services.StartupManager;

public class WindowsShortcutManagerMock : IShortcutManager
{
    public bool CreateShortcut(string targetPath, string shortcutPath, string description = "")
    {
        try
        {
            // Create the file, or overwrite if the file exists.
            using (FileStream fs = File.Create(shortcutPath))
            {
                byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file.");
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }

            // Open the stream and read it back.
            using (StreamReader sr = File.OpenText(shortcutPath))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    Console.WriteLine(s);
                }
            }

            return true;
        }

        catch (Exception ex)
        {
            return false;
        }
    }
}
