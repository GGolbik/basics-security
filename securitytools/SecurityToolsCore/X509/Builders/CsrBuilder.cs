using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using GGolbik.SecurityTools.X509.Models;

namespace GGolbik.SecurityTools.X509.Builders;

public class CsrBuilder : SecurityBuilder<CsrBuilder>
{
    public ConfigCsr Build(ConfigCsr config, CertificateRequestLoadOptions loadOptions)
    {
        return this.Build(config, loadOptions, out var csr);
    }
    public ConfigCsr Build(ConfigCsr config, CertificateRequestLoadOptions loadOptions, out CertificateRequest csr)
    {
        this.InvokeOnAction(SecurityBuilderEventKind.BuildingCsr);
        var result = (ConfigCsr)config.Clone();

        this.InvokeOnAction(SecurityBuilderEventKind.LoadingCsr);
        var oldCsr = this.LoadCsr(result, loadOptions);

        result.KeyPair ??= new ConfigKeyPair();
        new KeyPairBuilder().Build(result.KeyPair, out var privateKey);

        csr = this.InitCsr(privateKey, oldCsr.HashAlgorithm, ((X500DistinguishedName?)result.SubjectName) ?? oldCsr.SubjectName);

        this.InvokeOnAction(SecurityBuilderEventKind.AddingExtensions);
        foreach (var ext in oldCsr.CertificateExtensions)
        {
            csr.CertificateExtensions.Add(ext);
        }
        this.InvokeOnAction(SecurityBuilderEventKind.BuiltCsr);

        result.Csr ??= new X50xFile();
        result.KeyPair.PrivateKey ??= new X50xFile();
        result.KeyPair.PublicKey ??= new X50xFile();
        HashAlgorithmName? keyAlg = result.KeyPair.HashAlgorithm?.ToHashAlgorithm();
        this.SaveKeyPair(result.KeyPair.PrivateKey, result.KeyPair.PublicKey, privateKey, keyAlg);
        this.SaveCsr(result.Csr, csr);

        return result;
    }

    public ConfigCsr Build(ConfigCsr config)
    {
        return this.Build(config, out var csr);
    }

    public ConfigCsr Build(ConfigCsr config, out CertificateRequest csr)
    {
        this.InvokeOnAction(SecurityBuilderEventKind.BuildingCsr);
        var result = (ConfigCsr)config.Clone();

        result.KeyPair ??= new ConfigKeyPair();
        new KeyPairBuilder().Build(result.KeyPair, out var privateKey);

        // The hash algorithm to use when signing and when storing enrypted key.
        HashAlgorithmName hashAlgorithm = result.HashAlgorithm?.ToHashAlgorithm() ?? HashAlgorithmName.SHA512;
        result.HashAlgorithm = hashAlgorithm.Name;

        csr = this.InitCsr(privateKey, hashAlgorithm, result.SubjectName);

        this.InvokeOnAction(SecurityBuilderEventKind.AddingExtensions);
        this.AddExtensions(csr, result.Extensions);
        this.InvokeOnAction(SecurityBuilderEventKind.BuiltCsr);

        result.Csr ??= new X50xFile();
        result.KeyPair.PrivateKey ??= new X50xFile();
        result.KeyPair.PublicKey ??= new X50xFile();
        HashAlgorithmName? keyAlg = result.KeyPair.HashAlgorithm?.ToHashAlgorithm();
        this.SaveKeyPair(result.KeyPair.PrivateKey, result.KeyPair.PublicKey, privateKey, keyAlg);
        this.SaveCsr(result.Csr, csr);
        return result;
    }
}
