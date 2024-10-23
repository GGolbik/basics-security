
using System.Security.Cryptography.X509Certificates;

namespace GGolbik.SecurityTools.X509.Models;

public class CrlEntry
{
    /// <summary>
    /// The certificate to revoke.
    /// </summary>
    public X50xFile? Cert { get; set; }

    /// <summary>
    /// The serial number to revoke.
    /// </summary>
    public byte[]? SerialNumber { get; set; }

    /// <summary>
    /// The time the certificate was revoked, or null to use the current system time.
    /// </summary>
    public DateTime? RevocationTime { get; set; }

    /// <summary>
    /// The reason for the revocation.
    /// </summary>
    public X509RevocationReason? Reason { get; set; }
}