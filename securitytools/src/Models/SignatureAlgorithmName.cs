
namespace GGolbik.SecurityTools.Models;

public enum SignatureAlgorithmName
{
    None = 0,
    /// <summary>
    /// RSA Signature Algorithm
    /// </summary>
    Rsa = 1,
    /// <summary>
    /// Elliptic Curve Digital Signature Algorithm
    /// </summary>
    Ecdsa = 2,
    /// <summary>
    /// Edwards-curve Digital Signature Algorithm
    /// </summary>
    Eddsa = 3
}