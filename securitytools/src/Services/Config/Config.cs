using System.Security.Cryptography;

using GGolbik.SecurityTools.Models;

namespace GGolbik.SecurityTools.Services;

/// <summary>
/// <para>
/// This config is related to <see href="https://datatracker.ietf.org/doc/html/rfc5280/">RFC 5280: Internet X.509 Public Key Infrastructure Certificate and Certificate Revocation List (CRL) Profile</see>.
/// </para>
/// 
/// <para>
/// The X.509 v3 certificate basic syntax is as follows:
/// (TBS means "to be signed". Certificate is a container for a signed TBSCertificate.)
/// </para>
/// 
/// <para>
/// Certificate  ::=  SEQUENCE  {
///    tbsCertificate       TBSCertificate,
///    signatureAlgorithm   AlgorithmIdentifier,
///    signatureValue       BIT STRING  }
/// </para>
/// <para>
/// TBSCertificate  ::=  SEQUENCE  {
///    version         [0]  EXPLICIT Version DEFAULT v1,
///    serialNumber         CertificateSerialNumber,
///    signature            AlgorithmIdentifier,
///    issuer               Name,
///    validity             Validity,
///    subject              Name,
///    subjectPublicKeyInfo SubjectPublicKeyInfo,
///    issuerUniqueID  [1]  IMPLICIT UniqueIdentifier OPTIONAL,
///                         -- If present, version MUST be v2 or v3
///    subjectUniqueID [2]  IMPLICIT UniqueIdentifier OPTIONAL,
///                         -- If present, version MUST be v2 or v3
///    extensions      [3]  EXPLICIT Extensions OPTIONAL
///                         -- If present, version MUST be v3
/// }
/// </para>
/// <para>
/// Version  ::=  INTEGER  {  v1(0), v2(1), v3(2)  }
/// </para>
/// <para>
/// CertificateSerialNumber  ::=  INTEGER
/// </para>
/// <para>
/// Validity ::= SEQUENCE {
///    notBefore      Time,
///    notAfter       Time }
/// </para>
/// <para>
/// Time ::= CHOICE {
///    utcTime        UTCTime,
///    generalTime    GeneralizedTime }
/// </para>
/// <para>
/// UniqueIdentifier  ::=  BIT STRING
/// </para>
/// <para>
/// SubjectPublicKeyInfo  ::=  SEQUENCE  {
///    algorithm            AlgorithmIdentifier,
///    subjectPublicKey     BIT STRING  }
/// </para>
/// <para>
/// Extensions  ::=  SEQUENCE SIZE (1..MAX) OF Extension
/// </para>
/// <para>
/// Extension  ::=  SEQUENCE  {
///    extnID      OBJECT IDENTIFIER,
///    critical    BOOLEAN DEFAULT FALSE,
///    extnValue   OCTET STRING
///                -- contains the DER encoding of an ASN.1 value
///                -- corresponding to the extension type identified
///                -- by extnID
/// }
/// </para>
/// </summary>
public class Config
{
    /// <summary>
    /// Whether this config is intended to create a CSR or Certificate.
    /// </summary>
    public ConfigMode Mode { get; set; }

    /// <summary>
    /// Whether a self-signed certificate shall be created.
    /// Usually for the creation of root CA certificates. 
    /// 
    /// Only relevant in combination with <see cref="ConfigMode.Certificate"/>.
    /// </summary>
    public bool? IsSelfSigned { get; set; }

    /// <summary>
    /// In a certificate, the serial number is chosen by the CA which issued the certificate.
    /// It is just written in the certificate. The CA can choose the serial number in any way as it sees fit, 
    /// not necessarily randomly (and it has to fit in 20 bytes).
    /// A CA is supposed to choose unique serial numbers, that is, unique for the CA.
    /// You cannot count on a serial number being unique worldwide; in the dream world of X.509,
    /// it is the pair issuerDN+serial which is unique worldwide (each CA having its own unique distinguished name,
    /// and taking care not to reuse serial numbers).
    /// 
    /// Only relevant in combination with <see cref="ConfigMode.Certificate"/>.
    /// </summary>
    public long? SerialNumber { get; set; }

    /// <summary>
    /// The certificate validity period is the time interval during which the CA warrants 
    /// that it will maintain information about the status of the certificate.
    /// 
    /// Only relevant in combination with <see cref="ConfigMode.Certificate"/>.
    /// </summary>
    public ConfigValidity? Validity { get; set; }

    /// <summary>
    /// Provides the name of the computer, user, network device, or service that the CA issues the certificate to.
    /// </summary>
    public ConfigSubjectName? SubjectName { get; set; }

    /// <summary>
    /// The signer key to use to sign the CSR.
    /// 
    /// Only relevant in combination with <see cref="ConfigMode.Certificate"/>.
    /// </summary>
    public string? IssuerPrivateKeyFile { get; set; }
    public X509FileFormat? IssuerPrivateKeyFileFormat { get; set; }

    /// <summary>
    /// An optional password for the signer's private key.
    /// </summary>
    public string? IssuerPrivateKeyPassword { get; set; }

    /// <summary>
    /// The cryptographic algorithm used by the CA to sign this certificate.
    /// 
    /// Only relevant in combination with <see cref="ConfigMode.Certificate"/>.
    /// </summary>
    public SignatureAlgorithmName? IssuerSignatureAlgorithm { get; set; }

    /// <summary>
    /// The hash algorithm to use when signing the certificate or certificate request, e.g.
    /// <see cref="HashAlgorithmName.SHA1"/>, <see cref="HashAlgorithmName.SHA256"/>, <see cref="HashAlgorithmName.SHA512"/>
    /// 
    /// Only relevant in combination with <see cref="ConfigMode.Certificate"/>.
    /// </summary>
    public string? IssuerHashAlgorithm { get; set; }

    /// <summary>
    /// The signer cert to use to sign the CSR.
    /// 
    /// Only relevant in combination with <see cref="ConfigMode.Certificate"/>.
    /// </summary>
    public string? IssuerCertFile { get; set; }
    public X509FileFormat? IssuerCertFileFormat { get; set; }

    /// <summary>
    /// 
    /// Only relevant in combination with <see cref="ConfigMode.Certificate"/>.
    /// </summary>
    public string? IssuerStoreFile { get; set; }
    public X509FileFormat? IssuerStoreFileFormat { get; set; }

    /// <summary>
    /// The identifier of the issuer if a key store is provided.
    /// 
    /// Only relevant in combination with <see cref="ConfigMode.Certificate"/>.
    /// </summary>
    public string? IssuerAlias { get; set; }

    /// <summary>
    /// Path to the store file which shall be created.
    /// </summary>
    public string? StoreFile { get; set; }
    public X509FileFormat? StoreFileFormat { get; set; }

    /// <summary>
    /// The filename of the key.
    /// If the file does not exist, a new key will be generated.
    /// 
    /// Only relevant in combination with <see cref="ConfigMode.CertificateSigningRequest"/> or
    /// with <see cref="ConfigMode.Certificate"/> and <see cref="IsSelfSigned"/> is true.
    /// </summary>
    public string? PrivateKeyFile { get; set; }
    public X509FileFormat? PrivateKeyFileFormat { get; set; }

    public string? PublicKeyFile { get; set; }
    public X509FileFormat? PublicKeyFileFormat { get; set; }

    /// <summary>
    /// An optional password for the private key.
    /// </summary>
    public string? PrivateKeyPassword { get; set; }

    /// <summary>
    /// The cryptographic algorithm to use for the private key.
    /// 
    /// Only relevant in combination with <see cref="ConfigMode.CertificateSigningRequest"/>
    /// </summary>
    public SignatureAlgorithmName? SignatureAlgorithm { get; set; }

    /// <summary>
    /// The key size. For RSA 2048 or 4096.
    /// 
    /// Only relevant in combination with <see cref="SignatureAlgorithmName.Rsa"/>
    /// </summary>
    public int? KeySize { get; set; }

    /// <summary>
    /// The name or OID of the ECCurve.
    /// 
    /// Only relevant in combination with <see cref="SignatureAlgorithmName.Ecdsa"/>
    /// </summary>
    public string? Eccurve { get; set; }

    /// <summary>
    /// </summary>
    public ConfigExtensions? Extensions { get; set; }

    /// <summary>
    /// The filename of the certificate to create.
    /// 
    /// Only relevant in combination with <see cref="ConfigMode.Certificate"/>.
    /// </summary>
    public string? CertFile { get; set; }
    public X509FileFormat? CertFileFormat { get; set; }

    public bool? LoadCsrExtensions { get; set; }

    /// <summary>
    /// In case of <see cref="ConfigMode.Certificate"/> the filename of the CSR to sign.
    /// In case of <see cref="ConfigMode.CertificateSigningRequest"/> the filename of the CSR to create.
    /// </summary>
    public string? CsrFile { get; set; }
    public X509FileFormat? CsrFileFormat { get; set; }

    public HashAlgorithmName? GetIssuerHashAlgorithm()
    {
        if (this.IssuerHashAlgorithm == null)
        {
            return HashAlgorithmName.SHA512;
        }
        var algs = new List<HashAlgorithmName>()
        {
            HashAlgorithmName.MD5,
            HashAlgorithmName.SHA1,
            HashAlgorithmName.SHA256,
            HashAlgorithmName.SHA384,
            HashAlgorithmName.SHA512,
        };
        var hashAlgorithmName = this.IssuerHashAlgorithm.ToUpper();
        foreach (var alg in algs)
        {
            if (hashAlgorithmName == alg.Name?.ToUpper())
            {
                return alg;
            }
        }
        HashAlgorithmName.TryFromOid(this.IssuerHashAlgorithm, out var hashAlg);
        return hashAlg;
    }

    public DateTimeOffset? CrlNextUpdate {get;set;}

    public IList<string>? Files { get; set; }

    /// <summary>
    /// The path to the output of a transform request.
    /// </summary>
    public string? TransformFile { get; set; }

    public X509FileFormat? TransformFileFormat { get; set; }

    public string? CrlFile { get; set; }
    public X509FileFormat? CrlFileFormat { get; set; }
    public byte[]? CrlNumber {get;set;}
    public IList<CrlEntry>? CrlEntries { get;set;}
}
