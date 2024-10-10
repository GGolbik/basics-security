
using System.Security.Cryptography.X509Certificates;

namespace GGolbik.SecurityTools.X509.Models;

/// <summary>
/// 4.2.1.1.  Authority Key Identifier:
/// The keyIdentifier field of the authorityKeyIdentifier extension MUST
/// be included in all certificates generated by conforming CAs to
/// facilitate certification path construction.
/// </summary>
public class ConfigAuthorityKeyIdentifierExtension : ConfigExtension
{

    public bool? IncludeKeyIdentifier { get; set; }
    public bool? IncludeIssuerAndSerial { get; set; }

    public override X509Extension ToX509Extension()
    {
        throw new NotImplementedException();
    }

    public X509Extension? ToX509Extension(X509SubjectKeyIdentifierExtension? subjectKeyIdentifier)
    {
        if(subjectKeyIdentifier == null)
        {
            return null;
        }
        return X509AuthorityKeyIdentifierExtension.CreateFromSubjectKeyIdentifier(subjectKeyIdentifier);
    }

    public X509Extension? ToX509Extension(X509Certificate2? issuer)
    {
        if(issuer == null)
        {
            return null;
        }
        return X509AuthorityKeyIdentifierExtension.CreateFromCertificate(issuer, this.IncludeKeyIdentifier ?? true, this.IncludeIssuerAndSerial ?? false);
    }
}