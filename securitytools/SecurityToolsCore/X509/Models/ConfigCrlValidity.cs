namespace GGolbik.SecurityTools.X509.Models;

/// <summary>
/// </summary>
public class ConfigCrlValidity
{
    /// <summary>
    /// Indicates a particular version of the <see cref="ConfigCrlValidity"/>.
    /// </summary>
    public SchemaVersion? SchemaVersion { get; set; } = "1.0";

    /// <summary>
    /// 
    /// </summary>
    public DateTime? ThisUpdate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime? NextUpdate { get; set; }
}