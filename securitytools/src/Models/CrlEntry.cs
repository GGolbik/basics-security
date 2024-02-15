
using System.Security.Cryptography.X509Certificates;

namespace GGolbik.SecurityTools.Models;

public class CrlEntry
{
    /// <summary>
    /// Indicates a particular version of the <see cref="CrlEntry"/>.
    /// </summary>
    public SchemaVersion? SchemaVersion { get; set; } = "1.0";

    /// <summary>
    /// The certificate to revoke.
    /// </summary>
    public X509File? Cert { get; set; }

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