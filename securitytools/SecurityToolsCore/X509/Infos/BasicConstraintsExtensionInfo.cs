using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace GGolbik.SecurityTools.X509.Infos;

/// <summary>
/// The basic constraints extension identifies whether the subject of the
/// certificate is a CA and the maximum depth of valid certification
/// paths that include this certificate.
/// 
/// https://datatracker.ietf.org/doc/html/rfc5280/#section-4.2.1.9
/// </summary>
public class BasicConstraintsExtensionInfo : X509ExtensionInfo
{
    public bool? CertificateAuthority { get; set; }

    public bool? HasPathLengthConstraint { get; set; }

    public int? PathLengthConstraint { get; set; }

    public override string? ToString()
    {
        return $"{this.GetType().Name}:{nameof(CertificateAuthority)}={CertificateAuthority};{nameof(HasPathLengthConstraint)}={HasPathLengthConstraint};{nameof(PathLengthConstraint)}={PathLengthConstraint}";
    }

    public override X509Extension ToX509Extension()
    {
        return new X509BasicConstraintsExtension(this.CertificateAuthority ?? false, this.HasPathLengthConstraint ?? false, this.PathLengthConstraint ?? 0, this.Critical ?? false);
    }
}