
namespace GGolbik.SecurityToolsApp.Certificates;

public class CertificateGroup : ICloneable
{
    /// <summary>
    /// 32 digits separated by hyphens: 00000000-0000-0000-0000-000000000000
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// The name of the certificate group.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The thumbprint (SHA1 hash) of the certificate which shall be the default application instance certificate.
    /// </summary>
    public string? DefaultIdentityThumbprint { get; set; }

    public CertificateGroup()
    {

    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}