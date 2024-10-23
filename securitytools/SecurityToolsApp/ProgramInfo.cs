using System.Reflection;
using System.Text;

namespace GGolbik.SecurityToolsApp;

/// <summary>
/// Info about the application.
/// </summary>
public class ProgramInfo
{
    /// <summary>
    /// The name of the application.
    /// </summary>
    public string? Title { get; }
    /// <summary>
    /// The manufacturer of the applicaton.
    /// </summary>
    public string? Manufacturer { get; }
    /// <summary>
    /// Description of the application.
    /// </summary>
    public string? Description { get; }
    /// <summary>
    /// Copyright notice.
    /// </summary>
    public string? Copyright { get; }
    /// <summary>
    /// MAJOR version: Might indicate incompatible changes.
    /// </summary>
    public int Major { get; }
    /// <summary>
    /// MINOR version: Might indicate additional functionality in a backward compatible manner.
    /// </summary>
    public int Minor { get; }
    /// <summary>
    /// PATCH version: Bug fixes only.
    /// </summary>
    public int Patch { get; }
    /// <summary>
    /// Build version: Indicates only the 
    /// </summary>
    public int Build { get; }
    /// <summary>
    /// The release date of the version.
    /// </summary>
    public DateTime? ReleaseDate { get; }
    /// <summary>
    /// The name of the version.
    /// </summary>
    public string Version { get; }

    public ProgramInfo()
    {
        this.Title = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
        this.Manufacturer = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
        this.Copyright = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
        this.Description = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
        var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyMetadataAttribute>();
        string? versionName = null;
        foreach (var attribute in attributes)
        {
            if ("ReleaseDate".Equals(attribute.Key, StringComparison.InvariantCultureIgnoreCase))
            {
                if (DateTime.TryParse(attribute.Value, out var releaseDate))
                {
                    this.ReleaseDate = releaseDate.ToUniversalTime();
                }
            }

            if ("VersionName".Equals(attribute.Key, StringComparison.InvariantCultureIgnoreCase))
            {
                versionName = attribute.Value;
            }
        }

        var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
        if (assemblyVersion != null)
        {
            this.Major = assemblyVersion.Major;
            this.Minor = assemblyVersion.Minor;
            this.Patch = assemblyVersion.Build;
        }

        var version = ToVersion(Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion);
        if(version != null)
        {
            this.Major = version.Major;
            this.Minor = version.Minor;
            this.Patch = version.Build;
            this.Build = version.Revision;
        }

        if (string.IsNullOrEmpty(versionName))
        {
            this.Version = string.Format("{0}.{1}.{2}.{3}", this.Major, this.Minor, this.Patch, this.Build);
        }
        else
        {
            this.Version = versionName;
        }
    }

    private static Version? ToVersion(string? versionStr)
    {
        if (versionStr == null)
        {
            return null;
        }
        var versionNumber = versionStr.Split('.');
        if (!Int32.TryParse(versionNumber[0], out var major))
        {
            return null;
        }
        if (!Int32.TryParse(versionNumber.Count() > 1 ? versionNumber[1] : "0", out var minor))
        {
            return null;
        }
        if (!Int32.TryParse(versionNumber.Count() > 2 ? versionNumber[2] : "0", out var patch))
        {
            return null;
        }
        if (!Int32.TryParse(versionNumber.Count() > 3 ? versionNumber[3].Split('+')[0] : "0", out var build))
        {
            return null;
        }
        return new Version(major, minor, patch, build);
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        foreach (var property in this.GetType().GetProperties())
        {
            builder.AppendLine(string.Format("{0}={1}", property.Name, property.GetValue(this)));
        }
        return builder.ToString();
    }
}
