namespace GGolbik.SecurityTools.X509.Infos;

/// <summary>
/// https://www.ietf.org/rfc/rfc5280.txt  5.2.3
/// </summary>
public class CrlNumberExtensionInfo : X509ExtensionInfo
{
    public byte[]? CrlNumber { get; set; }
}