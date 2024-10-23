using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using GGolbik.SecurityTools.X509.Models;
using Org.BouncyCastle.X509;

namespace GGolbik.SecurityTools.X509.Builders;

public abstract class SecurityBuilder<T>
{
    public event EventHandler<SecurityBuilderEventArgs>? OnAction;

    protected void InvokeOnAction(SecurityBuilderEventArgs e)
    {
        this.OnAction?.Invoke(this, e);
    }

    /// <summary>
    /// Loads or generate a private key.
    /// </summary>
    /// <param name="config">The config to update.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    protected AsymmetricAlgorithm LoadOrGenerateKeyPair(ConfigKeyPair config)
    {
        if (!(config.PrivateKey?.Exists() ?? false))
        {
            return this.CreateKeyPair(config);
        }

        var data = config.PrivateKey.Data ?? File.ReadAllBytes(config.PrivateKey.FileName!);
        var loadedPrivateKey = this.LoadPrivateKey(config.PrivateKey);
        config.SignatureAlgorithm = loadedPrivateKey.ToKeyPairInfo().SignatureAlgorithm;
        return loadedPrivateKey;
    }

    /// <summary>
    /// Creates a private key.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    protected AsymmetricAlgorithm CreateKeyPair(ConfigKeyPair config)
    {
        config.SignatureAlgorithm ??= SignatureAlgorithmName.Rsa;
        switch (config.SignatureAlgorithm)
        {
            case SignatureAlgorithmName.Rsa:
                config.KeySize ??= 4096;
                return RSA.Create((int)config.KeySize);
            case SignatureAlgorithmName.Ecdsa:
                config.Eccurve ??= ECCurve.NamedCurves.nistP521.Oid.Value;
                return ECDsa.Create(config.Eccurve?.ToEccurve() ?? throw new ArgumentNullException("Eccurve"));
            default:
                throw new ArgumentException("The signature algorithm is not supported yet.");
        }
    }

    protected bool TryLoadPrivateKey(X50xFile? file, [NotNullWhen(true)] out AsymmetricAlgorithm? privateKey)
    {
        try
        {
            privateKey = this.LoadPrivateKey(file);
            return true;
        }
        catch
        {
            privateKey = null;
            return false;
        }
    }

    protected AsymmetricAlgorithm LoadPrivateKey(X50xFile? file)
    {
        AsymmetricAlgorithm? keyPair = null;
        if (file?.Exists() ?? false)
        {
            using (var stream = file.ToStream())
            {
                X509Reader.ReadKeyPairs(stream, file.Password, (key) =>
                {
                    keyPair = key;
                });
            }
        }
        if (keyPair != null)
        {
            return keyPair;
        }
        throw new ArgumentNullException("private key");
    }

    protected CertificateRequest InitCsr(AsymmetricAlgorithm privateKey, HashAlgorithmName hashAlgorithm, X50xSubjectName? subjectName)
    {
        if (subjectName == null)
        {
        }
        var distinguishedName = subjectName?.ToX500DistinguishedName() ?? new X500DistinguishedNameBuilder().Build();
        return InitCsr(privateKey, hashAlgorithm, distinguishedName);
    }

    protected CertificateRequest InitCsr(AsymmetricAlgorithm privateKey, HashAlgorithmName hashAlgorithm, X500DistinguishedName distinguishedName)
    {
        if (privateKey is RSA)
        {
            // The RSA signature padding to apply if self-signing or being signed with an System.Security.Cryptography.X509Certificates.X509Certificate2.
            var padding = RSASignaturePadding.Pkcs1;
            return new CertificateRequest(
                    distinguishedName,
                    (privateKey as RSA)!,
                    hashAlgorithm,
                    padding
            );
        }
        else if (privateKey is ECDsa)
        {
            return new CertificateRequest(
                    distinguishedName,
                    (privateKey as ECDsa)!,
                    hashAlgorithm
            );
        }
        throw new ArgumentException("Asymmetric Algorithm is not supported");
    }


    protected void AddExtension(CertificateRequest csr, X509Extension? ext, bool replace = true)
    {
        if (ext == null)
        {
            return;
        }

        if (replace)
        {
            RemoveExtension(csr, ext.Oid?.Value);
        }

        var foundItem = csr.CertificateExtensions.FirstOrDefault((item) =>
        {
            return item.Oid?.Value == ext.Oid?.Value;
        });
        if (foundItem != null)
        {
            return;
        }
        csr.CertificateExtensions.Add(ext);
    }

    protected void RemoveExtension(CertificateRequest csr, string? oid)
    {
        if (string.IsNullOrWhiteSpace(oid))
        {
            return;
        }
        var ext = csr.CertificateExtensions.FirstOrDefault((item) =>
        {
            return item.Oid?.Value == oid;
        });
        if (ext != null)
        {
            csr.CertificateExtensions.Remove(ext);
        }
    }


    protected void AddExtensions(CertificateRequest csr, X50xExtensions? extensions, bool replace = true)
    {
        var before = csr.CertificateExtensions.Count();

        if (extensions != null)
        {
            var subjectKeyIdentifier = extensions.SubjectKeyIdentifier?.ToX509Extension(csr.PublicKey);
            this.AddExtension(csr, subjectKeyIdentifier, replace);
            this.AddExtension(csr, extensions.AuthorityKeyIdentifier?.ToX509Extension(subjectKeyIdentifier as X509SubjectKeyIdentifierExtension), replace);

            this.AddExtension(csr, extensions.BasicConstraints?.ToX509Extension(), replace);

            this.AddExtension(csr, extensions.KeyUsage?.ToX509Extension(), replace);
            this.AddExtension(csr, extensions.ExtendedKeyUsage?.ToX509Extension(), replace);

            this.AddExtension(csr, extensions.SubjectAlternativeName?.ToX509Extension(), replace);
            if (extensions.Extensions != null)
            {
                foreach (var extension in extensions.Extensions)
                {
                    this.AddExtension(csr, extension.ToX509Extension(), replace);
                }
            }
        }

        var after = csr.CertificateExtensions.Count();
        if (after <= before)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="privateKeyFile"></param>
    /// <param name="publicKeyFile"></param>
    /// <param name="privateKey"></param>
    /// <param name="hashAlgorithm">The name of a hash algorithm to use with the Key Derivation Function (KDF) to turn a password into an encryption key.</param>
    protected void SaveKeyPair(X50xFile privateKeyFile, X50xFile publicKeyFile, AsymmetricAlgorithm privateKey, HashAlgorithmName? hashAlgorithm = null)
    {
        this.SavePrivateKey(privateKeyFile, privateKey, hashAlgorithm);
        this.SavePublicKey(publicKeyFile, privateKey);
    }
    protected void SavePrivateKey(X50xFile privateKeyFile, AsymmetricAlgorithm privateKey, HashAlgorithmName? hashAlgorithm = null)
    {
        hashAlgorithm ??= HashAlgorithmName.SHA512;

        if (privateKeyFile.FileFormat?.Encoding == X509Encoding.Pem)
        {
            string pem;
            if (!String.IsNullOrEmpty(privateKeyFile.Password))
            {
                pem = privateKey.ExportEncryptedPkcs8PrivateKeyPem(
                    privateKeyFile.Password.ToArray(),
                    new PbeParameters(
                        PbeEncryptionAlgorithm.Aes256Cbc,
                        (HashAlgorithmName)hashAlgorithm,
                        100000
                    )
                );
            }
            else
            {
                pem = privateKey.ExportPkcs8PrivateKeyPem();
            }
            privateKeyFile.Data = Encoding.UTF8.GetBytes(pem);
        }
        else
        {
            byte[] der;
            if (!String.IsNullOrEmpty(privateKeyFile.Password))
            {
                der = privateKey.ExportEncryptedPkcs8PrivateKey(
                    privateKeyFile.Password?.ToArray(),
                    new PbeParameters(
                        PbeEncryptionAlgorithm.Aes256Cbc,
                        (HashAlgorithmName)hashAlgorithm,
                        100000
                    )
                );
            }
            else
            {
                der = privateKey.ExportPkcs8PrivateKey();
            }
            privateKeyFile.Data = der;
            privateKeyFile.FileFormat = new X50xFileFormat(X509Encoding.Der);
        }
        // save to file
        if (!String.IsNullOrEmpty(privateKeyFile?.FileName))
        {
            var dir = Path.GetDirectoryName(privateKeyFile.FileName);
            if (!String.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllBytes(privateKeyFile.FileName, privateKeyFile.Data!);
        }
    }
    protected void SavePublicKey(X50xFile publicKeyFile, AsymmetricAlgorithm privateKey)
    {
        if (publicKeyFile.FileFormat?.Encoding == X509Encoding.Pem)
        {
            publicKeyFile.Data = Encoding.UTF8.GetBytes(privateKey.ExportSubjectPublicKeyInfoPem());
        }
        else
        {
            publicKeyFile.Data = privateKey.ExportSubjectPublicKeyInfo();
            publicKeyFile.FileFormat = new X50xFileFormat(X509Encoding.Der);
        }
        // save to file
        if (!String.IsNullOrEmpty(publicKeyFile.FileName))
        {
            var dir = Path.GetDirectoryName(publicKeyFile.FileName);
            if (!String.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllBytes(publicKeyFile.FileName, publicKeyFile.Data!);
        }
    }

    protected void SaveCsr(X50xFile file, CertificateRequest csr)
    {
        var stream = new MemoryStream();
        file ??= new X50xFile();
        if (file.FileFormat?.Encoding == X509Encoding.Pem)
        {
            csr.SavePem(stream);
        }
        else
        {
            csr.SaveDer(stream);
            file.FileFormat = new X50xFileFormat(X509Encoding.Der);
        }
        file.Data = stream.ToArray();

        // save to file
        if (!String.IsNullOrEmpty(file.FileName))
        {
            var dir = Path.GetDirectoryName(file.FileName);
            if (!String.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllBytes(file.FileName, file.Data);
        }
    }


    protected void SaveCert(X50xFile file, X509Certificate2 cert)
    {
        if (file.FileFormat?.Encoding == X509Encoding.Pem)
        {
            var pem = cert.ExportCertificatePem();
            file.Data = Encoding.UTF8.GetBytes(pem);
        }
        else
        {
            var der = cert.Export(X509ContentType.Cert);
            file.Data = der;
            file.FileFormat = new X50xFileFormat(X509ContentType.Cert, X509Encoding.Der);
        }

        // save to file
        if (!String.IsNullOrEmpty(file.FileName))
        {
            var dir = Path.GetDirectoryName(file.FileName);
            if (!String.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllBytes(file.FileName, file.Data);
        }
    }


    protected X509SignatureGenerator CreateX509SignatureGenerator(AsymmetricAlgorithm privateKey)
    {
        if (privateKey is RSA)
        {
            return X509SignatureGenerator.CreateForRSA((privateKey as RSA)!, RSASignaturePadding.Pkcs1);
        }
        else if (privateKey is ECDsa)
        {
            return X509SignatureGenerator.CreateForECDsa((privateKey as ECDsa)!);
        }
        else
        {
            throw new ArgumentException("Failed to create signaturegenerator");
        }
    }

    protected bool TryLoadCsr(ConfigCsr config, CertificateRequestLoadOptions? loadOptions, [NotNullWhen(true)] out CertificateRequest? csr)
    {
        try
        {
            csr = LoadCsr(config, loadOptions);
            return true;
        }
        catch
        {
            csr = null;
            return false;
        }
    }

    protected CertificateRequest LoadCsr(ConfigCsr config, CertificateRequestLoadOptions? loadOptions)
    {
        if (!(config.Csr?.Exists() ?? false))
        {
            throw new ArgumentNullException("csr");
        }

        HashAlgorithmName hashAlgorithm = config.HashAlgorithm?.ToHashAlgorithm() ?? HashAlgorithmName.SHA512;
        config.HashAlgorithm = hashAlgorithm.Name;

        Exception? loadDerEx;
        try
        {
            return CertificateRequest.LoadSigningRequest(
                config.Csr?.Data ?? File.ReadAllBytes(config.Csr?.FileName!),
                hashAlgorithm,
                loadOptions ?? CertificateRequestLoadOptions.UnsafeLoadCertificateExtensions);
        }
        catch (Exception e)
        {
            loadDerEx = e;
        }

        try
        {
            char[] strData;
            if (config.Csr.Data != null)
            {
                strData = Encoding.UTF8.GetString(config.Csr.Data).ToCharArray();
            }
            else
            {
                strData = File.ReadAllText(config.Csr!.FileName!).ToCharArray();
            }
            return CertificateRequest.LoadSigningRequestPem(
                strData,
                hashAlgorithm,
                loadOptions ?? CertificateRequestLoadOptions.UnsafeLoadCertificateExtensions);
        }
        catch (Exception e)
        {
            throw new AggregateException(loadDerEx, e);
        }
    }

    protected bool TryLoadCertificate(X50xFile? file, [NotNullWhen(true)] out X509Certificate2? certificate)
    {
        try
        {
            certificate = LoadCertificate(file);
            return true;
        }
        catch
        {
            certificate = null;
            return false;
        }
    }

    protected X509Certificate2 LoadCertificate(X50xFile? file)
    {
        X509Certificate2? result = null;
        if (!(file?.Exists() ?? false))
        {
            throw new ArgumentNullException("certificate");
        }

        using (Stream stream = file.ToStream())
        {
            X509Reader.ReadCertificates(stream, new PasswordFinder(), (cert) =>
            {
                result = cert;
            });
        }
        if (result == null)
        {
            throw new ArgumentNullException("certificate");
        }
        return result;
    }

    private bool TryLoadCrl(X50xFile? file, [NotNullWhen(true)] out CertificateRevocationListBuilder? crl, [NotNullWhen(true)] out BigInteger? crlNumber)
    {
        try
        {
            crl = LoadCrl(file, out var number);
            crlNumber = number;
            return true;
        }
        catch
        {
            crl = null;
            crlNumber = null;
            return false;
        }
    }

    public CertificateRevocationListBuilder LoadCrl(X50xFile? file, out BigInteger crlNumber)
    {
        if (!(file?.Exists() ?? false))
        {
            throw new ArgumentNullException("crl");
        }

        X509Crl? crl = null;
        using (var stream = file.ToStream())
        {
            X509Reader.ReadCrls(stream, (c) =>
            {
                crl = c;
            });
        }
        if (crl != null)
        {
            return CertificateRevocationListBuilder.Load(crl.ToDer(), out crlNumber);
        }
        throw new ArgumentNullException("Failed to load CRL");
    }

    protected void SaveCrl(X50xFile config, X509Crl crl)
    {
        if (config.FileFormat?.Encoding == X509Encoding.Pem)
        {
            config.Data = Encoding.UTF8.GetBytes(crl.ToPem());
        }
        else
        {
            config.Data = crl.ToDer();
            config.FileFormat = new X50xFileFormat(X509ContentType.Cert, X509Encoding.Der);
        }

        // save to file
        if (!String.IsNullOrEmpty(config.FileName))
        {
            var dir = Path.GetDirectoryName(config.FileName);
            if (!String.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllBytes(config.FileName, config.Data);
        }
    }

    protected void SaveStore(X50xFile file, X509Certificate2Collection store)
    {
        file.Data = store.Export(X509ContentType.Pkcs12, file.Password);

        // save to file
        if (!string.IsNullOrEmpty(file.FileName))
        {
            var dir = Path.GetDirectoryName(file.FileName);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllBytes(file.FileName, file.Data!);
        }
    }
}
