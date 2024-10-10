
namespace GGolbik.SecurityTools.X509;

public enum SignatureAlgorithmName
{
    None = 0,
    /// <summary>
    /// RSA Signature Algorithm:
    /// 
    /// RSA cryptography is based on the held belief that factoring large semi-prime numbers is difficult by nature.
    /// Given that no general-purpose formula has been found to factor a compound number into its prime factors,
    /// there is a direct relationship between the size of the factors chosen and the time required to compute the solution.
    /// </summary>
    Rsa = 1,
    /// <summary>
    /// Discrete Logarithm Problem:
    /// 
    /// DSA follows a similar schema, as RSA with public/private keypairs that are mathematically related. 
    /// What makes DSA different from RSA is that DSA uses a different algorithm. 
    /// It solves an entirely different problem using different elements, equations, and steps.
    /// 
    /// ECDSA/EdDSA and DSA differ in that DSA uses a mathematical operation known as modular exponentiation
    /// while ECDSA/EdDSA uses elliptic curves.
    /// </summary>
    Dsa = 2,
    /// <summary>
    /// Elliptic Curve Digital Signature Algorithm
    /// </summary>
    Ecdsa = 3,
    /// <summary>
    /// Edwards-curve Digital Signature Algorithm
    /// </summary>
    Eddsa = 4,
    /// <summary>
    /// Elliptic Curve Diffie-Hellman
    /// </summary>
    Ecdh = 5,
}