namespace GGolbik.SecurityTools.Models;

public enum TransformMode
{
    None = 0,
    /// <summary>
    /// Put all input files into a single PKCS#12 store file.
    /// </summary>
    Store = 1,
    /// <summary>
    /// Put all input files into a single PEM file.
    /// </summary>
    SinglePem = 2,
    /// <summary>
    /// Convert each input file to a PEM file.
    /// </summary>
    Pem = 3,
    /// <summary>
    /// Convert each input file to a DER file.
    /// </summary>
    Der = 4,
    /// <summary>
    /// Create a config for each provided input file.
    /// </summary>
    Config = 5,
    /// <summary>
    /// Prints the file to a textual represantation.
    /// </summary>
    Print = 6,
}