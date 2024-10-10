using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace GGolbik.SecurityTools.X509.Infos;

public class CertificateInfo : ICloneable
{
    /// <summary>
    /// SHA1 hash of the entire certificate in DER form.
    /// </summary>
    public string? Thumbprint { get; set; }

    /// <summary>
    /// SHA1 hash of the public key.
    /// </summary>
    public string? PublicKeyThumbprint { get; set; }

    #region X.509 Version 1

    /// <summary>
    /// This field describes the version of the encoded certificate.
    /// </summary>
    public int? Version { get; set; }

    /// <summary>
    /// The serial number of the certificate as a big-endian hexadecimal string.
    /// 
    /// In a certificate, the serial number is chosen by the CA which issued the certificate.
    /// It is just written in the certificate. The CA can choose the serial number in any way as it sees fit, 
    /// not necessarily randomly (and it has to fit in 20 bytes).
    /// A CA is supposed to choose unique serial numbers, that is, unique for the CA.
    /// You cannot count on a serial number being unique worldwide; in the dream world of X.509,
    /// it is the pair issuerDN+serial which is unique worldwide (each CA having its own unique distinguished name,
    /// and taking care not to reuse serial numbers).
    /// </summary>
    public string? SerialNumber { get; set; }

    /// <summary>
    /// This field contains the algorithm identifier for the algorithm used by the CA to sign the certificate e.g., RSA-2048 and SHA-256
    /// </summary>
    public string? SignatureAlgorithm { get; set; }

    /// <summary>
    /// Provides a distinguished name for the CA that issued the certificate.
    /// The issuer name is commonly represented by using an X.500 or LDAP format.
    /// For a root CA, the Issuer and Subject are identical.
    /// For all other CA certificates and for end entity certificates, the Subject and Issuer will be different.
    /// The issuer field must contain a non-empty distinguished name (DN).
    /// However, [RFC 5280](https://tools.ietf.org/html/rfc5280) does not make any requirement on which RDN(s) should be present.
    /// </summary>
    public string? Issuer { get; set; }

    /// <summary>
    /// The certificate validity period is the time interval during which the CA warrants 
    /// that it will maintain information about the status of the certificate.
    /// </summary>
    public CertificateValidity? Validity { get; set; }

    /// <summary>
    /// Provides the name of the computer, user, network device, or service that the CA issues the certificate to.
    /// The subject name is commonly represented by using an X.500 or Lightweight Directory Access Protocol (LDAP) format.
    /// The subject field must contain a non-empty distinguished name (DN).
    /// However, [RFC 5280](https://datatracker.ietf.org/doc/html/rfc5280) does not make any requirement on which RDN(s) should be present.
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// Contains the public key of the key pair that is associated with the certificate.
    /// </summary>
    public KeyPairInfo? SubjectPublicKeyInfo { get; set; }

    #endregion

    #region X.509 Version 2
    // In addition to the fields defined in X.509 version 1, X.509 version 2 certificates include optional fields that provide additional functionality and features to the certificate. These fields are not necessarily included in each certificate that a CA issues.

    /// <summary>
    /// The issuer unique identifier is present in the certificate to handle the possibility of reuse of issuer names over time.
    /// It is not recommended to reuse names for different entities.
    /// </summary>
    public string? IssuerUniqueIdentifier { get; set; }

    /// <summary>
    /// The subject unique identifiers are present in the certificate to handle the possibility of reuse of subject names over time.
    /// It is not recommended to reuse names for different entities.
    /// </summary>
    public string? SubjectUniqueIdentifier { get; set; }

    #endregion

    #region  X.509 Version 3

    /// <summary>
    /// In addition to the fields defined in X.509 version 1 and version 2, X.509 version 3 certificates include optional extensions that provide additional functionality and features to the certificate.
    /// These extensions are not necessarily included in each certificate that a CA issues.
    /// The certificate extensions can be declared as critical.
    /// That means that a system will reject a certificate if it encounters a critical extension that doesn't recognize.
    /// </summary>
    public X509ExtensionsInfo? Extensions { get; set; }

    #endregion

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}

public class CertificateInfoCollection : List<CertificateInfo>, ICloneable
{ 
    public CertificateInfoCollection()
    {

    }
    public CertificateInfoCollection(IEnumerable<X509Certificate2> list)
    {
        this.AddRange(list.Select((item) => {
            return item.ToCertificateInfo();
        }));
    }

    public object Clone()
    {
        CertificateInfoCollection result = new();
        result.AddRange(this.Select((item) => {
            return (CertificateInfo)item.Clone();
        }));
        return result;
    }
}