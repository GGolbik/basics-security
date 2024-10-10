using System.Security.Cryptography;
using GGolbik.SecurityTools.X509;

namespace GGolbik.SecurityTools.X509.Infos;

public class KeyPairInfo : ICloneable
{
    /// <summary>
    /// SHA1 hash of the public key.
    /// </summary>
    public string? Thumbprint { get; set; }

    /// <summary>
    /// The cryptographic algorithm to use for the private key.
    /// </summary>
    public SignatureAlgorithmName? SignatureAlgorithm { get; set; }

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

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}

public class KeyPairInfoCollection : List<KeyPairInfo>, ICloneable
{ 
    public KeyPairInfoCollection()
    {

    }
    public KeyPairInfoCollection(IEnumerable<AsymmetricAlgorithm> list)
    {
        this.AddRange(list.Select((item) => {
            return item.ToKeyPairInfo();
        }));
    }

    public object Clone()
    {
        KeyPairInfoCollection result = new();
        result.AddRange(this.Select((item) => {
            return (KeyPairInfo)item.Clone();
        }));
        return result;
    }
}