
using System.Security.Cryptography;
using GGolbik.SecurityTools.Services;

namespace GGolbik.SecurityTools.Models;

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
public class ConfigCsr
{
    /// <summary>
    /// Indicates a particular version of the <see cref="ConfigCsr"/>.
    /// </summary>
    public SchemaVersion? SchemaVersion { get; set; } = "1.0";

    /// <summary>
    /// The certificate signing request.
    /// </summary>
    public X509File? Csr { get; set; }

    /// <summary>
    /// The key pair of the certificate signing request (CSR).
    /// </summary>
    public ConfigKeyPair? KeyPair { get; set; }

    /// <summary>
    /// Provides the name of the computer, user, network device, or service that the CA issues the certificate to.
    /// </summary>
    public ConfigSubjectName? SubjectName { get; set; }

    /// <summary>
    /// X509 extensions
    /// </summary>
    public ConfigExtensions? Extensions { get; set; }

    /// <summary>
    /// The name or OID of the hash algorithm to use when signing, e.g.
    /// <see cref="HashAlgorithmName.SHA1"/>, <see cref="HashAlgorithmName.SHA256"/>, <see cref="HashAlgorithmName.SHA512"/>
    /// </summary>
    public string? HashAlgorithm { get; set; }
}