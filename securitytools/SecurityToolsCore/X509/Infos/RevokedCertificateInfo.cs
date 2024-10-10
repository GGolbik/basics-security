
using GGolbik.SecurityTools.X509.Infos;
public class RevokedCertificateInfo
{
    /// <summary>
    /// Certificates revoked by the CA are uniquely identified by the certificate serial number
    /// </summary>
    public string? SerialNumber { get; set; }

    /// <summary>
    /// The date on which the revocation occurred is specified.
    /// </summary>
    public DateTime? RevocationDate { get; set; }

    public IList<X509ExtensionsInfo>? CrlEntryExtensions { get; set; }
}
