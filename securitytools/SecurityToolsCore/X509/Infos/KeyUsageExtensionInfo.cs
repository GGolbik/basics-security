
using System.Security.Cryptography.X509Certificates;

namespace GGolbik.SecurityTools.X509.Infos;

public class KeyUsageExtensionInfo : X509ExtensionInfo
{
    /// <summary>
    /// When present, conforming CAs SHOULD mark this extension as critical.
    /// </summary>
    public X509KeyUsageFlags KeyUsages { get; set; }

    public override string? ToString()
    {
        return $"{this.GetType().Name}:{nameof(KeyUsages)}={KeyUsages.ToString()}";
    }

    public override X509Extension ToX509Extension()
    {
        return new X509KeyUsageExtension(this.KeyUsages, this.Critical ?? false);
    }
}