
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using GGolbik.SecurityTools.Core;
using GGolbik.SecurityTools.Models;

namespace GGolbik.SecurityTools.Builders;

public class CertBuilder : SecurityBuilder<CertBuilder>
{
    public ConfigCert Build(ConfigCert config)
    {
        Logger.Information("Build certificate.");
        var result = JsonSerializer.Deserialize<ConfigCert>(JsonSerializer.Serialize(config))!;

        X509Certificate2? issuer = null;
        AsymmetricAlgorithm privateKey;
        if (config.Issuer != null)
        {
            Logger.Debug("Load issuer.");
            issuer = this.LoadCertificate(result.Issuer);
            if (!issuer.HasPrivateKey && (result.KeyPair?.PrivateKey?.Exists() ?? false))
            {
                Logger.Debug("Load private key.");
                var data = result.KeyPair.PrivateKey?.Data ?? File.ReadAllBytes(result.KeyPair.PrivateKey?.FileName!);
                privateKey = LoadPrivateKey(data, result.KeyPair.PrivateKey?.Password, out var signatureAlgorithm);
                result.KeyPair.SignatureAlgorithm = signatureAlgorithm;
            }
            else
            {
                Logger.Debug("Try to load private key from issuer.");
                if (this.TryLoadPrivateKey(issuer, out var key))
                {
                    Logger.Debug("Loaded private key from issuer.");
                    privateKey = key;
                }
                else
                {
                    throw Errors.CreateInvalidArgumentError("Issuer private Key", "A certificate must be signed by an issuer but no private key could be found.");
                }
            }
        }
        else if (result.Csr != null) // create self-signed
        {
            Logger.Debug("No issuer has been provided. Creating self-signed certificate.");
            // copy key pair to CSR if necessary
            result.Csr.KeyPair ??= result.KeyPair;
            result.Csr.HashAlgorithm ??= result.HashAlgorithm;
            // build CSR including private key
            if (result.Csr.Csr?.Exists() ?? false)
            {
                Logger.Debug("Load CSR.");
                result.Csr = new CsrBuilder().Build(result.Csr, result.CsrLoadOptions ?? CertificateRequestLoadOptions.UnsafeLoadCertificateExtensions);
            }
            else
            {
                Logger.Debug("Build CSR from config.");
                result.Csr = new CsrBuilder().Build(result.Csr);
            }

            Logger.Debug("Load or generate private key.");
            // copy created key
            result.KeyPair = result.Csr.KeyPair;
            privateKey = this.LoadOrGeneratePrivateKey(result.KeyPair!);
        }
        else
        {
            throw Errors.CreateInvalidArgumentError("Issuer", "A certificate must be signed by an issuer but no issuer could be found.");
        }

        var signatureGenerator = this.CreateX509SignatureGenerator(privateKey);

        Logger.Debug("Load CSR");
        result.Csr ??= new ConfigCsr();
        var csr = this.LoadCsr(result.Csr, result.CsrLoadOptions) ?? throw Errors.CreateInvalidArgumentError("CSR", "A certificate is created by a CSR but no CSR could be found.");

        Logger.Debug("Add extensions.");
        this.AddExtensions(csr, result.Extensions, config.ReplaceExtensions ?? true);

        result.SerialNumber ??= BitConverter.GetBytes(DateTime.UtcNow.Ticks);
        result.Validity ??= new ConfigValidity();
        result.Validity.NotBefore ??= DateTime.UtcNow;
        result.Validity.NotAfter ??= DateTime.MaxValue.ToUniversalTime();

        Logger.Debug("Create certificate.");
        var cert = csr.Create(issuer?.SubjectName ?? csr.SubjectName, signatureGenerator, (DateTime)result.Validity!.NotBefore!, (DateTime)result.Validity!.NotAfter!, result.SerialNumber);

        result.KeyPair ??= new ConfigKeyPair();
        result.KeyPair.PrivateKey ??= new X509File();
        result.KeyPair.PublicKey ??= new X509File();
        result.Cert ??= new X509File();
        HashAlgorithmName? keyAlg = string.IsNullOrEmpty(result.KeyPair.HashAlgorithm) ? null : ToHashAlgorithm(result.KeyPair.HashAlgorithm);
        Logger.Debug("Save certificate key pair.");
        this.SaveKeyPair(result.KeyPair.PrivateKey, result.KeyPair.PublicKey, privateKey, keyAlg);
        Logger.Debug("Save created certificate.");
        this.SaveCert(result.Cert, cert);

        result.Issuer ??= result.Cert;

        Logger.Information("Built certificate.");
        return result;
    }
}