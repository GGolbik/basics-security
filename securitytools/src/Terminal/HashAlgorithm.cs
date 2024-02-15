
using System.Security.Cryptography;

namespace GGolbik.SecurityTools.Terminal;

public enum HashAlgorithm : int
{
    Exit = 0,
    Sha1 = 1,
    Sha256 = 2,
    Sha512 = 3
}

public static class HashAlgorithmExtension
{
    public static HashAlgorithmName? ToHashAlgorithmName(this HashAlgorithm hashAlgorithm)
    {
        switch(hashAlgorithm)
        {
            case HashAlgorithm.Sha1:
                return HashAlgorithmName.SHA1;
            case HashAlgorithm.Sha256:
                return HashAlgorithmName.SHA256;
            case HashAlgorithm.Sha512:
                return HashAlgorithmName.SHA512;
            default:
                return null;
        }
    }
}