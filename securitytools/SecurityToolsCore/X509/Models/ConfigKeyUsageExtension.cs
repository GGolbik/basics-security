
using System.Security.Cryptography.X509Certificates;

namespace GGolbik.SecurityTools.X509.Models;

public class ConfigKeyUsageExtension : ConfigExtension
{
    /// <summary>
    /// When present, conforming CAs SHOULD mark this extension as critical.
    /// </summary>
    public X509KeyUsageFlags KeyUsages { get; set; }

    public override X509Extension ToX509Extension()
    {
        return new X509KeyUsageExtension(this.KeyUsages, this.Critical ?? false);
    }
}
