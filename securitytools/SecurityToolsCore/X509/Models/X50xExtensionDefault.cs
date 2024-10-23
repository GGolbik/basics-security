
using System.Security.Cryptography.X509Certificates;

namespace GGolbik.SecurityTools.X509.Models;

public class X50xExtensionDefault : X50xExtension
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
