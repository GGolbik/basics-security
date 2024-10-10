namespace GGolbik.SecurityTools.X509.Infos;

/// <summary>
/// Validity ::= SEQUENCE {
///    notBefore      Time,
///    notAfter       Time }
/// </summary>
public class CertificateValidity
{
    /// <summary>
    /// Provides the date and time when the certificate becomes valid.
    /// </summary>
    public DateTime? NotBefore { get; set; }

    /// <summary>
    /// Provides the date and time when the certificate is no longer considered valid.
    /// </summary>
    public DateTime? NotAfter { get; set; }
}