using System.Security.Cryptography;
using System.Text.Json;
using GGolbik.SecurityTools.X509;

namespace GGolbik.SecurityTools.X509.Models;

/// <summary>
/// Config to create a private and public key.
/// </summary>
public class ConfigKeyPair : ICloneable
{
    /// <summary>
    /// Indicates a particular version of the <see cref="ConfigKeyPair"/>.
    /// </summary>
    public SchemaVersion? SchemaVersion { get; set; } = "1.0";

    /// <summary>
    /// The cryptographic algorithm to use for the private key.
    /// </summary>
    public SignatureAlgorithmName? SignatureAlgorithm { get; set; }

    public X50xFile? PrivateKey { get; set; }

    /// <summary>
    /// The key size. For RSA 2048 or 4096.
    /// 
    /// Only relevant in combination with <see cref="SignatureAlgorithmName.Rsa"/>
    /// </summary>
    public int? KeySize { get; set; }

    /// <summary>
    /// The name or OID of the ECCurve.
    /// 
    /// Only relevant in combination with <see cref="SignatureAlgorithmName.Ecdsa"/>
    /// </summary>
    public string? Eccurve { get; set; }

    public X50xFile? PublicKey { get; set; }

    /// <summary>
    /// The name or OID of the hash algorithm to use when storing enrypted key, e.g.
    /// <see cref="HashAlgorithmName.SHA1"/>, <see cref="HashAlgorithmName.SHA256"/>, <see cref="HashAlgorithmName.SHA512"/>
    /// </summary>
    public string? HashAlgorithm { get; set; }

    public object Clone()
    {
        return JsonSerializer.Deserialize<ConfigKeyPair>(JsonSerializer.Serialize(this))!;
    }
}