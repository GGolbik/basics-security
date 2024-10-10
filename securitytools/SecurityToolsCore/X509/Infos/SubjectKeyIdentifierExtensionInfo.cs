

using System.Security.Cryptography.X509Certificates;

namespace GGolbik.SecurityTools.X509.Infos;
/// <summary>
/// 4.2.1.2.  Subject Key Identifier:
/// To facilitate certification path construction, this extension MUST
/// appear in all conforming CA certificates
/// </summary>
public class SubjectKeyIdentifierExtensionInfo : X509ExtensionInfo
{
    public byte[]? SubjectKeyIdentifier { get; set; }

    public override X509Extension ToX509Extension()
    {
        return base.ToX509Extension();
    }
}