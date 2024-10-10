
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace GGolbik.SecurityTools.X509.Models;


/// <summary>
/// Config to create a certificate.
/// </summary>
public class ConfigCert : ICloneable
{
    /// <summary>
    /// Indicates a particular version of the <see cref="ConfigCert"/>.
    /// </summary>
    public SchemaVersion? SchemaVersion { get; set; } = "1.0";

    /// <summary>
    /// The certificate.
    /// </summary>
    public X509File? Cert { get; set; }

    /// <summary>
    /// Specifies behavior when loading a CertificateRequest.
    /// </summary>
    public CertificateRequestLoadOptions? CsrLoadOptions { get; set; }

    /// <summary>
    /// The CSR to sign.
    /// If no issuer is provided, we assume a self-signed certificate.
    /// </summary>
    public ConfigCsr? Csr { get; set; }

    /// <summary>
    /// The the issuer certificate
    /// </summary>
    public X509File? Issuer { get; set; }

    /// <summary>
    /// The key pair of the issuer.
    /// </summary>
    public ConfigKeyPair? KeyPair { get; set; }

    /// <summary>
    /// In a certificate, the serial number is chosen by the CA which issued the certificate.
    /// It is just written in the certificate. The CA can choose the serial number in any way as it sees fit, 
    /// not necessarily randomly (and it has to fit in 20 bytes).
    /// A CA is supposed to choose unique serial numbers, that is, unique for the CA.
    /// You cannot count on a serial number being unique worldwide; in the dream world of X.509,
    /// it is the pair issuerDN+serial which is unique worldwide (each CA having its own unique distinguished name,
    /// and taking care not to reuse serial numbers).
    /// </summary>
    public byte[]? SerialNumber { get; set; }

    /// <summary>
    /// The certificate validity period is the time interval during which the CA warrants 
    /// that it will maintain information about the status of the certificate.
    /// </summary>
    public ConfigValidity? Validity { get; set; }

    /// <summary>
    /// Whether the <see cref="ConfigCert.Extensions"/> shall overwrite the same CSR extension or shall only be added if not already exist.
    /// </summary>
    public bool? ReplaceExtensions { get; set; }

    /// <summary>
    /// X509 extensions
    /// </summary>
    public ConfigExtensions? Extensions { get; set; }

    /// <summary>
    /// The hash algorithm to use for the signature algorithm
    /// <see cref="HashAlgorithmName.SHA1"/>, <see cref="HashAlgorithmName.SHA256"/>, <see cref="HashAlgorithmName.SHA512"/>
    /// </summary>
    public string? HashAlgorithm { get; set; }

    public object Clone()
    {
        return JsonSerializer.Deserialize<ConfigCert>(JsonSerializer.Serialize(this))!;
    }
}