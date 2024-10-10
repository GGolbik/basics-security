
using GGolbik.SecurityTools.X509.Models;
using Org.BouncyCastle.X509;

namespace GGolbik.SecurityTools.X509.Infos;

public class CrlInfo : ICloneable
{
    /// <summary>
    /// SHA1 hash of the entire CRL in DER form.
    /// </summary>
    public string? Thumbprint { get; set; }

    #region CertificateList

    /// <summary>
    /// The signatureAlgorithm field contains the algorithm identifier for the algorithm used by the CRL issuer to sign the CertificateList.
    /// This field MUST contain the same algorithm identifier as the signature field in the sequence tbsCertList (Section 5.1.2.2).
    /// </summary>
    public string? SignatureAlgorithm { get; set; }

    #region tbsCertList

    /// <summary>
    /// This optional field describes the version of the encoded CRL.
    /// When extensions are used, as required by this profile, this field MUST be present and MUST specify version 2 (the integer value is 1).
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// The issuer name identifies the entity that has signed and issued the CRL.
    /// </summary>
    public string? Issuer { get; set; }

    /// <summary>
    /// </summary>
    public CrlValidity? Validity { get; set; }

    /// <summary>
    /// When there are no revoked certificates, the revoked certificates list MUST be absent.
    /// Otherwise, revoked certificates are listed by their serial numbers.
    /// </summary>
    public IList<RevokedCertificateInfo>? RevokedCertificates { get; set; }

    public IList<X509ExtensionsInfo>? CrlExtensions { get; set; }

    #endregion

    #endregion

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}

public class CrlInfoCollection : List<CrlInfo>, ICloneable
{ 
    public CrlInfoCollection()
    {

    }
    public CrlInfoCollection(IEnumerable<X509Crl> list)
    {
        this.AddRange(list.Select((item) => {
            return item.ToCrlInfo();
        }));
    }

    public object Clone()
    {
        CrlInfoCollection result = new();
        result.AddRange(this.Select((item) => {
            return (CrlInfo)item.Clone();
        }));
        return result;
    }
}