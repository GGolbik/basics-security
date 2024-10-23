
using System.Security.Cryptography.X509Certificates;
using GGolbik.SecurityTools.X509;

namespace GGolbik.SecurityTools.Credentials;

/// <summary>
/// Related to chapter 7.36.5 X509IdentityTokens in OPC 10000-4: UA Part 4: Services
/// https://reference.opcfoundation.org/Core/Part4/v104/docs/7.36.5
/// 
/// Related to chapter 7.37 UserTokenPolicy in OPC 10000-4: UA Part 4: Services
/// https://reference.opcfoundation.org/Core/Part4/v104/docs/7.37
/// </summary>
public class CertificateCredentials : KeyCredentialsValue
{
    public override KeyCredentialsKind Kind => KeyCredentialsKind.Certificate;

    /// <summary>
    /// The X.509 v3 Certificate.
    /// </summary>
    public byte[]? Certificate { get; set; }
    /// <summary>
    /// The key pair.
    /// </summary>
    public byte[]? KeyPair { get; set; }

    public CertificateCredentials()
    {

    }

    public CertificateCredentials(X509Certificate2 certificate)
    {
        this.Certificate = certificate.RawData;
        this.KeyPair = certificate.GetPrivateKey()?.ToDer();
    }

    public CertificateCredentials(byte[]? certificate, byte[]? key)
    {
        this.Certificate = certificate;
        this.KeyPair = key;
    }


    protected override string? GetHint()
    {
        if (Certificate == null)
        {
            return null;
        }
        try
        {
            return new X509Certificate2(Certificate).Subject;
        }
        catch
        {
            return null;
        }
    }
}