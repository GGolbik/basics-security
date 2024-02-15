using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using GGolbik.SecurityTools.Models;

namespace GGolbik.SecurityTools.Builders;

public class CsrBuilder : SecurityBuilder<CsrBuilder>
{
    public ConfigCsr Build(ConfigCsr config, CertificateRequestLoadOptions loadOptions)
    {
        Logger.Information("Build certificate signing request (CSR) based on existing CSR.");
        var result = JsonSerializer.Deserialize<ConfigCsr>(JsonSerializer.Serialize(config))!;
        var oldCsr = this.LoadCsr(result, loadOptions);

        Logger.Debug("Load or generate private key.");
        result.KeyPair ??= new ConfigKeyPair();
        AsymmetricAlgorithm privateKey = this.LoadOrGeneratePrivateKey(result.KeyPair);

        Logger.Debug("Initialize CSR.");
        var csr = this.InitCsr(privateKey, oldCsr.HashAlgorithm, ((X500DistinguishedName?)result.SubjectName) ?? oldCsr.SubjectName);

        Logger.Debug("Add extensions");
        foreach (var ext in oldCsr.CertificateExtensions)
        {
            csr.CertificateExtensions.Add(ext);
        }

        result.Csr ??= new X509File();
        result.KeyPair.PrivateKey ??= new X509File();
        result.KeyPair.PublicKey ??= new X509File();
        HashAlgorithmName? keyAlg = string.IsNullOrEmpty(result.KeyPair.HashAlgorithm) ? null : ToHashAlgorithm(result.KeyPair.HashAlgorithm);
        Logger.Debug("Save key pair.");
        this.SaveKeyPair(result.KeyPair.PrivateKey, result.KeyPair.PublicKey, privateKey, keyAlg);
        Logger.Debug("Save CSR.");
        this.SaveCsr(result.Csr, csr);

        Logger.Information("Built CSR.");
        return result;
    }

    public ConfigCsr Build(ConfigCsr config)
    {
        Logger.Information("Build certificate signing request (CSR).");
        var result = JsonSerializer.Deserialize<ConfigCsr>(JsonSerializer.Serialize(config))!;

        Logger.Debug("Load or generate private key.");
        result.KeyPair ??= new ConfigKeyPair();
        AsymmetricAlgorithm privateKey = this.LoadOrGeneratePrivateKey(result.KeyPair);

        // The hash algorithm to use when signing and when storing enrypted key.
        Logger.Debug("Select hash algorithm for signing.");
        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
        if (!String.IsNullOrEmpty(result.HashAlgorithm))
        {
            hashAlgorithm = ToHashAlgorithm(result.HashAlgorithm);
        }
        result.HashAlgorithm = hashAlgorithm.Name;

        Logger.Debug("Initialize CSR.");
        var csr = this.InitCsr(privateKey, hashAlgorithm, result.SubjectName);

        Logger.Debug("Add extensions");
        this.AddExtensions(csr, result.Extensions);

        result.Csr ??= new X509File();
        result.KeyPair.PrivateKey ??= new X509File();
        result.KeyPair.PublicKey ??= new X509File();
        HashAlgorithmName? keyAlg = string.IsNullOrEmpty(result.KeyPair.HashAlgorithm) ? null : ToHashAlgorithm(result.KeyPair.HashAlgorithm);
        Logger.Debug("Save key pair.");
        this.SaveKeyPair(result.KeyPair.PrivateKey, result.KeyPair.PublicKey, privateKey, keyAlg);
        Logger.Debug("Save CSR.");
        this.SaveCsr(result.Csr, csr);
        Logger.Information("Built CSR.");
        return result;
    }
}
