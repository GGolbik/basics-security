
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using GGolbik.SecurityTools.X509;

namespace GGolbik.SecurityTools.X509.Models;

/// <summary>
/// https://datatracker.ietf.org/doc/html/rfc5280/#section-4.2.1.12
/// </summary>
public class ConfigExtendedKeyUsageExtension : ConfigExtension
{
    public ExtendedKeyUsageFlags? ExtendedKeyUsages { get; set; }

    public ISet<string>? Oids { get; set; }

    public override X509Extension ToX509Extension()
    {
        var oids = this.ExtendedKeyUsages?.ToOidCollection() ?? new OidCollection();
        foreach (var oid in this.Oids ?? new HashSet<string>())
        {
            oids.Add(new Oid(oid));
        }
        return new X509EnhancedKeyUsageExtension(oids, this.Critical ?? false);
    }
}
