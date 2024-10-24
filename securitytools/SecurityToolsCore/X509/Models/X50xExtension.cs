
using System.Security.Cryptography.X509Certificates;

namespace GGolbik.SecurityTools.X509.Models;

/// <summary>
/// Extension  ::=  SEQUENCE  {
///     extnID      OBJECT IDENTIFIER,
///     critical    BOOLEAN DEFAULT FALSE,
///     extnValue   OCTET STRING
///                 -- contains the DER encoding of an ASN.1 value
///                 -- corresponding to the extension type identified
///                 -- by extnID
///     }
/// </summary>
public abstract class X50xExtension
{
    /// <summary>
    /// Each extension in a certificate is designated as either critical or non-critical.
    /// A certificate-using system MUST reject the certificate if it encounters a critical extension
    /// it does not recognize or a critical extension that contains information that it cannot process.
    /// A non-critical extension MAY be ignored if it is not recognized, but MUST be processed if it is recognized.
    /// </summary>
    public bool? Critical { get; set; }

    public abstract X509Extension ToX509Extension();
}
