
using System.Security.Cryptography.X509Certificates;

namespace GGolbik.SecurityTools.Models;

/// <summary>
/// 4.2.1.2.  Subject Key Identifier:
/// To facilitate certification path construction, this extension MUST
/// appear in all conforming CA certificates
/// </summary>
public class ConfigSubjectKeyIdentifierExtension : ConfigExtension
{

    public override X509Extension ToX509Extension()
    {
        throw new NotImplementedException();
    }

    public X509Extension ToX509Extension(PublicKey key)
    {
        return new X509SubjectKeyIdentifierExtension(key, this.Critical ?? false);
    }
}
