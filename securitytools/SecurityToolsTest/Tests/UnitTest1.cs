using System.Security.Cryptography.X509Certificates;
using System.Text;
using GGolbik.SecurityTools.X509;
using GGolbik.SecurityTools.X509.Builders;
using GGolbik.SecurityTools.X509.Models;

namespace GGolbik.SecurityToolsTest;


public class UnitTest1
{
    [Fact]
    public void testExample()
    {
        var service = new CertBuilder();
        {
            var rootCaConfig = new ConfigCert()
            {
                Csr = new ConfigCsr()
                {
                    SubjectName = new ConfigSubjectName("Web")
                    {
                        EmailAddress = "ggolbik@example.com",
                        DomainComponents = ["127.0.0.1"]
                    },
                },
                Extensions = new ConfigExtensions()
                {
                    BasicConstraints = new ConfigBasicConstraintsExtension()
                    {
                        CertificateAuthority = false,
                        Critical = true,
                        HasPathLengthConstraint = false,
                    },
                    KeyUsage = new ConfigKeyUsageExtension()
                    {
                        Critical = true,
                        KeyUsages = X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign | X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment
                    },
                    ExtendedKeyUsage = new ConfigExtendedKeyUsageExtension()
                    {
                        Critical = true,
                        ExtendedKeyUsages = ExtendedKeyUsageFlags.ServerAuth
                    },
                    SubjectAlternativeName = new ConfigSubjectAlternativeName()
                    {
                        Critical = true,
                        DnsNames = ["ggolbik.de"],
                        IPAddresses = ["127.0.0.1"],
                        EmailAddresses = ["ggolbik@example.com"],
                        Uris = ["http://example.com"]
                    }
                },
            };
            var result = service.Build(rootCaConfig, out var cert);
            var resultSub = cert.Subject;
            var config = cert.ToConfigCsr();
            var mem = new MemoryStream();
            cert.Print(mem);
            var srt = Encoding.UTF8.GetString(mem.ToArray());
            rootCaConfig.Csr = config;
            var result2 = service.Build(rootCaConfig, out var cert2);
            var result2Sub = cert2.Subject;
        }
    }
}