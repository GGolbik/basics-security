
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using GGolbik.SecurityTools.X509.Models;

namespace GGolbik.SecurityTools.X509.Builders;

public class CertBuilder : SecurityBuilder<CertBuilder>
{
    public ConfigCert Build(ConfigCert config)
    {
        return this.Build(config, out var cert);
    }

    public ConfigCert Build(ConfigCert config, out X509Certificate2 cert)
    {
        var result = (ConfigCert)config.Clone();

        X509Certificate2? issuer = null;
        AsymmetricAlgorithm? privateKey;
        bool hasIssuer = config.Issuer?.Exists() ?? false;
        if (hasIssuer)
        {
            this.InvokeOnAction(SecurityBuilderEventKind.BuildingCertificate);
            this.InvokeOnAction(SecurityBuilderEventKind.LoadingIssuer);
            this.LoadIssuer(result, out issuer, out privateKey);
        }
        else if (result.Csr != null) // create self-signed
        {
            this.InvokeOnAction(SecurityBuilderEventKind.BuildingSelfSignedCertificate);
            this.CreateSelfSigned(result, out privateKey);
        }
        else
        {
            throw new ArgumentException("Issuer", "A certificate must be signed by an issuer but no issuer could be found.");
        }

        var signatureGenerator = this.CreateX509SignatureGenerator(privateKey);

        result.Csr ??= new ConfigCsr();
        var csr = this.LoadCsr(result.Csr, result.CsrLoadOptions) ?? throw new ArgumentException("CSR", "A certificate is created by a CSR but no CSR could be found.");

        this.InvokeOnAction(SecurityBuilderEventKind.AddingExtensions);
        this.AddExtensions(csr, result.Extensions, config.ReplaceExtensions ?? true);

        result.SerialNumber ??= BitConverter.GetBytes(DateTime.UtcNow.Ticks);
        result.Validity ??= new X50xValidity();
        result.Validity.NotBefore ??= DateTime.UtcNow;
        result.Validity.NotAfter ??= DateTime.MaxValue.ToUniversalTime();

        cert = csr.Create(issuer?.SubjectName ?? csr.SubjectName, signatureGenerator, (DateTime)result.Validity!.NotBefore!, (DateTime)result.Validity!.NotAfter!, result.SerialNumber);
        this.InvokeOnAction(SecurityBuilderEventKind.BuiltCertificate);

        result.KeyPair ??= new ConfigKeyPair();
        result.KeyPair.PrivateKey ??= new X50xFile();
        result.KeyPair.PublicKey ??= new X50xFile();
        result.Cert ??= new X50xFile();
        HashAlgorithmName? keyAlg = result.KeyPair.HashAlgorithm?.ToHashAlgorithm();
        this.SaveKeyPair(result.KeyPair.PrivateKey, result.KeyPair.PublicKey, privateKey, keyAlg);
        this.SaveCert(result.Cert, cert);

        result.Issuer ??= result.Cert;

        return result;
    }

    private void LoadIssuer(ConfigCert result, out X509Certificate2? issuer, [NotNull] out AsymmetricAlgorithm? privateKey)
    {
        issuer = this.LoadCertificate(result.Issuer);
        privateKey = issuer.GetPrivateKey();
        if (privateKey == null)
        {
            this.InvokeOnAction(SecurityBuilderEventKind.LoadingIssuerKeyPair);
            if (this.TryLoadPrivateKey(result.KeyPair?.PrivateKey, out privateKey))
            {
                result.KeyPair!.SignatureAlgorithm = privateKey.ToKeyPairInfo().SignatureAlgorithm;
            }
        }
        if (privateKey == null)
        {
            throw new ArgumentException("Issuer private Key", "A certificate must be signed by an issuer but no private key could be found.");
        }
    }

    private void CreateSelfSigned(ConfigCert result, [NotNull] out AsymmetricAlgorithm? privateKey)
    {
        // copy key pair to CSR if necessary
        result.Csr!.KeyPair ??= result.KeyPair;
        result.Csr.HashAlgorithm ??= result.HashAlgorithm;
        // build CSR including private key
        if (result.Csr.Csr?.Exists() ?? false)
        {
            result.Csr = new CsrBuilder().Build(result.Csr, result.CsrLoadOptions ?? CertificateRequestLoadOptions.UnsafeLoadCertificateExtensions);
        }
        else
        {
            result.Csr = new CsrBuilder().Build(result.Csr);
        }

        // copy created key
        result.KeyPair = result.Csr.KeyPair;
        privateKey = this.LoadOrGenerateKeyPair(result.KeyPair!);
    }
}