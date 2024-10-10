using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using GGolbik.SecurityTools.X509.Builders;
using GGolbik.SecurityTools.X509.Models;
using GGolbik.SecurityTools.X509;
using Org.BouncyCastle.X509;

public class CrlBuilder : SecurityBuilder<CsrBuilder>
{
    public ConfigCrl Build(ConfigCrl config)
    {
        return this.Build(config, out var crl);
    }

    public ConfigCrl Build(ConfigCrl config, out X509Crl crl)
    {
        this.InvokeOnAction(SecurityBuilderEventKind.BuildingCrl);
        var result = (ConfigCrl)config.Clone();

        var builder = new CertificateRevocationListBuilder();
        result.CrlNumber ??= BitConverter.GetBytes(DateTime.UtcNow.Ticks);
        var crlNumber = new BigInteger(result.CrlNumber);
        if (config.Crl?.Exists() ?? false)
        {
            this.InvokeOnAction(SecurityBuilderEventKind.LoadingCrl);
            builder = this.LoadCrl(config.Crl, out var number);
        }

        this.InvokeOnAction(SecurityBuilderEventKind.LoadingIssuer);
        var issuer = this.LoadIssuer(result);

        this.InvokeOnAction(SecurityBuilderEventKind.AddingCrlEntries);
        foreach (var crlEntry in config.CrlEntries ?? new List<CrlEntry>())
        {
            if (crlEntry.Cert?.Exists() ?? false)
            {
                this.InvokeOnAction(SecurityBuilderEventKind.LoadingCertificate);
                var cert = this.LoadCertificate(crlEntry.Cert);
                builder.AddEntry(cert, crlEntry.RevocationTime, crlEntry.Reason);
            }
            else if (crlEntry.SerialNumber != null && crlEntry.SerialNumber.Count() > 0)
            {
                builder.AddEntry(crlEntry.SerialNumber, crlEntry.RevocationTime, crlEntry.Reason);
            }
        }

        // The hash algorithm to use when signing the CRL.
        HashAlgorithmName hashAlgorithm = result.HashAlgorithm?.ToHashAlgorithm() ?? HashAlgorithmName.SHA512;
        result.HashAlgorithm = hashAlgorithm.Name;

        crl = new X509Crl(builder.Build(issuer, crlNumber, result.Validity?.NextUpdate ?? DateTimeOffset.MaxValue.ToUniversalTime(), hashAlgorithm, RSASignaturePadding.Pkcs1, result.Validity?.ThisUpdate));
        this.InvokeOnAction(SecurityBuilderEventKind.BuiltCrl);

        result.Crl ??= new X509File();
        this.SaveCrl(result.Crl, crl);

        return result;
    }

    private X509Certificate2 LoadIssuer(ConfigCrl result)
    {
        var issuer = this.LoadCertificate(result.Issuer);
        if (issuer.HasPrivateKey)
        {
            return issuer;
        }
        if (result.KeyPair?.PrivateKey?.Exists() ?? false)
        {
            this.InvokeOnAction(SecurityBuilderEventKind.LoadingIssuerKeyPair);
            var privateKey = this.LoadPrivateKey(result.KeyPair.PrivateKey);
            result.KeyPair.SignatureAlgorithm = privateKey.ToKeyPairInfo().SignatureAlgorithm;
            switch (result.KeyPair.SignatureAlgorithm)
            {
                case SignatureAlgorithmName.Rsa:
                    issuer = issuer.CopyWithPrivateKey((RSA)privateKey);
                    break;
                case SignatureAlgorithmName.Ecdsa:
                    issuer = issuer.CopyWithPrivateKey((ECDsa)privateKey);
                    break;
                default:
                    throw new ArgumentException("Signature algorithm", "Unsupported signature algorithm.");
            }
            return issuer;
        }
        throw new ArgumentException("Issuer private key", "A CRL must be signed by an issuer but no private key could be found for the issuer.");
    }
}
