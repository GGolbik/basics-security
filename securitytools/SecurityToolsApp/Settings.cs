using System.Runtime.InteropServices;

namespace GGolbik.SecurityToolsApp;

public class Settings
{
    /// <summary>
    /// The path inside the application data directory for this application.
    /// [ApplicationDataDirectory]/ggolbik/securitytools
    /// </summary>
    private static string ApplicationDataDirectoryExtension
    {
        get
        {
            return Path.Combine("ggolbik", "securitytools");
        }
    }

    private static bool? _userInteractive;
    public static bool UserInteractive
    {
        get
        {
            if (_userInteractive == null)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    _userInteractive = Environment.UserInteractive;
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    _userInteractive = string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("INVOCATION_ID"));
                }
            }
            return _userInteractive ?? true;
        }
    }

    private static string? _applicationDataDirectory;
    public static string ApplicationDataDirectory
    {
        get
        {
            if (_applicationDataDirectory == null)
            {
                _applicationDataDirectory = Settings.ReadApplicationDataDirectory();
            }
            return Path.GetFullPath(_applicationDataDirectory);
        }
    }

    private static string ReadApplicationDataDirectory()
    {
        string path;
        // return default path
        if (UserInteractive)
        {
            // return default path for interactive usage
            // Windows:    C:\Users\<username>\AppData\Roaming
            // Linux:      /home/<username>/.config
            path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }
        else
        {
            // return default path service usage.
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                path = "/var/opt";
            }
            else
            {
                // Windows:    C:\ProgramData
                // Linux:      /usr/share
                path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            }
        }
        return Path.Combine(path, Settings.ApplicationDataDirectoryExtension);
    }

}