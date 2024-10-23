
using System.Security.Cryptography;
using System.Text.Json;

namespace GGolbik.SecurityTools.X509.Models;

/// <summary>
/// Config to create a certificate request (CSR).
///     
/// Certification request information shall have ASN.1 type
///    CertificationRequestInfo:
/// 
///    CertificationRequestInfo ::= SEQUENCE {
///      version Version,
///      subject Name,
///      subjectPublicKeyInfo SubjectPublicKeyInfo,
///      attributes [0] IMPLICIT Attributes }
/// 
///    Version ::= INTEGER
/// 
///    Attributes ::= SET OF Attribute
/// </summary>
public class ConfigCsr : ICloneable
{
    /// <summary>
    /// Indicates a particular version of the <see cref="ConfigCsr"/>.
    /// </summary>
    public SchemaVersion? SchemaVersion { get; set; } = "1.0";

    /// <summary>
    /// The certificate signing request.
    /// </summary>
    public X50xFile? Csr { get; set; }

    /// <summary>
    /// The key pair of the certificate signing request (CSR).
    /// </summary>
    public ConfigKeyPair? KeyPair { get; set; }

    /// <summary>
    /// Provides the name of the computer, user, network device, or service that the CA issues the certificate to.
    /// </summary>
    public X50xSubjectName? SubjectName { get; set; }

    /// <summary>
    /// X509 extensions
    /// </summary>
    public X50xExtensions? Extensions { get; set; }

    /// <summary>
    /// The name or OID of the hash algorithm to use when signing, e.g.
    /// <see cref="HashAlgorithmName.SHA1"/>, <see cref="HashAlgorithmName.SHA256"/>, <see cref="HashAlgorithmName.SHA512"/>
    /// </summary>
    public string? HashAlgorithm { get; set; }

    public object Clone()
    {
        return JsonSerializer.Deserialize<ConfigCsr>(JsonSerializer.Serialize(this))!;
    }
}