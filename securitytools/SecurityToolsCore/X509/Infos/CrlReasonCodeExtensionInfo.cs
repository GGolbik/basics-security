using System.Security.Cryptography.X509Certificates;

namespace GGolbik.SecurityTools.X509.Infos;

/// <summary>
/// </summary>
public class CrlReasonCodeExtensionInfo : X509ExtensionInfo
{
    /// <summary>
    /// The reason for the revocation. TODO: is part of crl entry extension
    /// </summary>
    public X509RevocationReason? Reason { get; set; }

    public override string? ToString()
    {
        return Reason?.ToString() ?? base.ToString();
    }
}