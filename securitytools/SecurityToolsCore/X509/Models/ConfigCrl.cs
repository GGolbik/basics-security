
using System.Security.Cryptography;
using System.Text.Json;

namespace GGolbik.SecurityTools.X509.Models;


/// <summary>
/// Config to create a certificate revocation list (CRL).
/// </summary>
public class ConfigCrl : ICloneable
{
    /// <summary>
    /// Indicates a particular version of the <see cref="ConfigCrl"/>.
    /// </summary>
    public SchemaVersion? SchemaVersion { get; set; } = "1.0";

    /// <summary>
    /// The CRL.
    /// </summary>
    public X509File? Crl { get; set; }

    /// <summary>
    /// The issuer certificate.
    /// </summary>
    public X509File? Issuer { get; set; }

    /// <summary>
    /// The issuer key.
    /// </summary>
    public ConfigKeyPair? KeyPair { get; set; }

    /// <summary>
    /// The CRL number is a non-critical CRL extension that conveys a monotonically increasing sequence number for a given CRL scope and CRL issuer.
    /// This extension allows users to easily determine when a particular CRL supersedes another CRL.
    /// 
    /// https://datatracker.ietf.org/doc/html/rfc5280#section-5.2.3
    /// </summary>
    public byte[]? CrlNumber { get; set; }

    /// <summary>
    /// The list of revoked certificates.
    /// </summary>
    public IList<CrlEntry>? CrlEntries { get; set; }

    /// <summary>
    /// The valid time span of the CRL.
    /// </summary>
    public ConfigCrlValidity? Validity { get; set; }

    /// <summary>
    /// The name or OID of the hash algorithm to use for signing the CRL e.g.:
    /// <see cref="HashAlgorithmName.SHA1"/>, <see cref="HashAlgorithmName.SHA256"/>, <see cref="HashAlgorithmName.SHA512"/>.
    /// By default the hash algorithm of the issuer cert is taken.
    /// </summary>
    public string? HashAlgorithm { get; set; }

    public object Clone()
    {
        return JsonSerializer.Deserialize<ConfigCrl>(JsonSerializer.Serialize(this))!;
    }
}
