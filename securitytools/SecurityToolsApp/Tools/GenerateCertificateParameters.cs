using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using GGolbik.SecurityTools.X509.Infos;

namespace GGolbik.SecurityToolsApp.Tools;

public class GenerateCsrParameters
{
    public X500DistinguishedName SubjectName { get; }

    public DateTime NotBefore { get; set; }

    public DateTime NotAfter { get; set; }

    public IList<X509ExtensionInfo> Extensions { get; set; } = new List<X509ExtensionInfo>();

    /// <summary>
    /// The hash algorithm to use when signing the certificate.
    /// </summary>
    public HashAlgorithmName? HashAlgorithm { get; set; }

    public KeyPairInfo? KeyPair { get; set; }

    public byte[] SerialNumber { get; set; } = [];

    public GenerateCsrParameters(string subjectName)
    {
        X500DistinguishedNameBuilder builder = new();
        builder.AddCommonName(subjectName);
        this.SubjectName = builder.Build();
        this.NotBefore = DateTime.UtcNow;
        this.NotAfter = DateTime.MaxValue.ToUniversalTime();
    }

    public GenerateCsrParameters(X500DistinguishedName subjectName)
    {
        this.SubjectName = subjectName;
        this.NotBefore = DateTime.UtcNow;
        this.NotAfter = DateTime.MaxValue.ToUniversalTime();
    }
}