
namespace GGolbik.SecurityTools.Models;

public enum X509Encoding
{
    None = 0,
    /// <summary>
    /// DER (Distinguished Encoding Rules) is a binary encoding for X.509 certificates and private keys. Unlike PEM, DER-encoded files do not contain plain text statements such as -----BEGIN CERTIFICATE-----.
    /// DER-encoded files are usually found with the extensions .der and .cer.
    /// </summary>
    Der = 1,
    /// <summary>
    /// PEM ("Privacy Enhanced Mail") is the most common format for X.509 certificates, CSRs, and cryptographic keys.
    /// A PEM file is a text file containing one or more items in Base64 ASCII encoding, each with plain-text headers
    /// and footers (e.g. -----BEGIN CERTIFICATE----- and -----END CERTIFICATE-----).
    /// PEM files are usually seen with the extensions .crt, .pem, .cer, and .key (for private keys),
    /// but you may also see them with different extensions.
    /// </summary>
    Pem = 2
}