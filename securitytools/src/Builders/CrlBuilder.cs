using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using GGolbik.SecurityTools.Builders;
using GGolbik.SecurityTools.Core;
using GGolbik.SecurityTools.Models;

public class CrlBuilder : SecurityBuilder<CsrBuilder>
{
    public ConfigCrl Build(ConfigCrl config)
    {
        Logger.Information("Build certificate revocation list (CRL).");
        var result = JsonSerializer.Deserialize<ConfigCrl>(JsonSerializer.Serialize(config))!;

        var builder = new CertificateRevocationListBuilder();
        result.CrlNumber ??= BitConverter.GetBytes(DateTime.UtcNow.Ticks);
        var crlNumber = new BigInteger(result.CrlNumber);
        if (config.Crl?.Exists() ?? false)
        {
            Logger.Debug("Load CRL");
            builder = this.LoadCrl(config.Crl, out var number);
            Logger.Debug("CRL number: {0}", number);
        }

        Logger.Debug("Load issuer.");
        var issuer = this.LoadCertificate(config.Issuer);
        if (!issuer.HasPrivateKey && (result.KeyPair?.PrivateKey?.Exists() ?? false))
        {
            Logger.Debug("Load issuer private key.");
            var data = result.KeyPair.PrivateKey?.Data ?? File.ReadAllBytes(result.KeyPair.PrivateKey?.FileName!);
            var privateKey = LoadPrivateKey(data, result.KeyPair.PrivateKey?.Password, out var signatureAlgorithm);
            result.KeyPair.SignatureAlgorithm = signatureAlgorithm;
            switch (signatureAlgorithm)
            {
                case SignatureAlgorithmName.Rsa:
                    issuer = issuer.CopyWithPrivateKey((RSA)privateKey);
                    break;
                case SignatureAlgorithmName.Ecdsa:
                    issuer = issuer.CopyWithPrivateKey((ECDsa)privateKey);
                    break;
                default:
                    throw Errors.CreateInvalidArgumentError("Signature algorithm", "Unsupported signature algorithm.");
            }
        }
        else
        {
            throw Errors.CreateInvalidArgumentError("Issuer private key", "A CRL must be signed by an issuer but no private key could be found for the issuer.");
        }

        // add entries
        Logger.Debug("Add CRL entries.");
        foreach (var crlEntry in config.CrlEntries ?? new List<CrlEntry>())
        {
            if (crlEntry.Cert?.Exists() ?? false)
            {
                Logger.Debug("Load entry by certificate.");
                var cert = LoadCertificate(crlEntry.Cert);
                builder.AddEntry(cert, crlEntry.RevocationTime, crlEntry.Reason);
            }
            else if (crlEntry.SerialNumber != null && crlEntry.SerialNumber.Count() > 0)
            {
                Logger.Debug("Load entry by serial number.");
                builder.AddEntry(crlEntry.SerialNumber, crlEntry.RevocationTime, crlEntry.Reason);
            }
        }

        // The hash algorithm to use when signing the CRL.
        Logger.Debug("Select hash algorithm for signing.");
        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
        if (!String.IsNullOrEmpty(result.HashAlgorithm))
        {
            hashAlgorithm = ToHashAlgorithm(result.HashAlgorithm);
        }
        result.HashAlgorithm = hashAlgorithm.Name;

        Logger.Debug("Create CRL.");
        var crl = builder.Build(issuer, crlNumber, result.Validity?.NextUpdate ?? DateTimeOffset.MaxValue.ToUniversalTime(), hashAlgorithm, RSASignaturePadding.Pkcs1, result.Validity?.ThisUpdate);

        Logger.Debug("Save CRL.");
        result.Crl ??= new X509File();
        this.SaveCrl(result.Crl, crl);

        Logger.Information("Built certificate revocation list (CRL).");
        return result;
    }
}
