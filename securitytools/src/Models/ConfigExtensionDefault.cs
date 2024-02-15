
using System.Security.Cryptography.X509Certificates;

namespace GGolbik.SecurityTools.Models;

public class ConfigExtensionDefault : ConfigExtension
{
    public string? Oid { get; set; }

    /// <summary>
    /// Base64 Asn Encoded Data.
    /// </summary>
    public byte[]? Value { get; set; }

    public override X509Extension ToX509Extension()
    {
        return new X509Extension(this.Oid ?? "", this.Value ?? new byte[0], this.Critical ?? false);
    }
}
