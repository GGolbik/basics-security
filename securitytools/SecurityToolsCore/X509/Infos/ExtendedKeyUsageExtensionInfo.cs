using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using GGolbik.SecurityTools.X509.Infos;


/// <summary>
/// https://datatracker.ietf.org/doc/html/rfc5280/#section-4.2.1.12
/// </summary>
public class ExtendedKeyUsageExtensionInfo : X509ExtensionInfo
{
    /// <summary>
    /// key is the OID; Value is the name
    /// </summary>
    public IDictionary<string, string?>? Oids { get; set; }

    public override string? ToString()
    {
        if (Oids == null)
        {
            return $"{this.GetType().Name}";
        }
        return string.Join(";", Oids.Select((entry) =>
        {
            return $"{this.GetType().Name}:{entry.Value ?? ""}({entry.Key})";
        }));
    }

    public override X509Extension ToX509Extension()
    {
        var collection = new OidCollection();
        foreach(var oid in this.Oids?.Keys ?? new List<string>())
        {
            collection.Add(new Oid(oid));
        }
        return new X509EnhancedKeyUsageExtension(collection, this.Critical ?? false);
    }
}