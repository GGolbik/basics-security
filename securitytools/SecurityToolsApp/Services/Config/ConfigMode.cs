
namespace GGolbik.SecurityTools.Services;

public enum ConfigMode : int
{
    None = 0,
    /// <summary>
    /// Create a self-signed certificate or create certificate from a certificate signing request.
    /// </summary>
    Certificate = 1,
    /// <summary>
    /// Create a certificate signing request (CSR).
    /// </summary>
    CertificateSigningRequest = 2,
    /// <summary>
    /// Transform a file format.
    /// </summary>
    Transform = 3,
    PrivateKey = 4,
    PublicKey = 5,
    KeyPair = 6,
    /// <summary>
    /// Create a config from a key or certificate.
    /// </summary>
    Config = 7,
    /// <summary>
    /// Create a certificate revocation list (CRL).
    /// </summary>
    Crl = 8,
}
