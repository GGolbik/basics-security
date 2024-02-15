using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using GGolbik.SecurityTools.Models;
using GGolbik.SecurityTools.Services;
using Serilog;

namespace GGolbik.SecurityTools.Builders;

public abstract class SecurityBuilder<T>
{
    protected readonly Serilog.ILogger Logger;

    protected SecurityBuilder()
    {
        this.Logger = Log.ForContext<T>();
    }

    /// <summary>
    /// Loads or generate a private key.
    /// </summary>
    /// <param name="config">The config to update.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    protected AsymmetricAlgorithm LoadOrGeneratePrivateKey(ConfigKeyPair config)
    {
        if (!(config.PrivateKey?.Exists() ?? false))
        {
            Logger.Debug("Private key is not provided. Create private key.");
            return this.CreatePrivateKey(config);
        }

        Logger.Debug("Load private key");
        var data = config.PrivateKey.Data ?? File.ReadAllBytes(config.PrivateKey.FileName!);
        var loadedPrivateKey = LoadPrivateKey(data, config.PrivateKey.Password, out var signatureAlgorithm);
        config.SignatureAlgorithm = signatureAlgorithm;
        return loadedPrivateKey;
    }

    /// <summary>
    /// Creates a private key.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    protected AsymmetricAlgorithm CreatePrivateKey(ConfigKeyPair config)
    {
        config.SignatureAlgorithm ??= SignatureAlgorithmName.Rsa;
        switch (config.SignatureAlgorithm)
        {
            case SignatureAlgorithmName.Rsa:
                config.KeySize ??= 4096;
                return RSA.Create((int)config.KeySize);
            case SignatureAlgorithmName.Ecdsa:
                config.Eccurve ??= ECCurve.NamedCurves.nistP521.Oid.Value;
                return ECDsa.Create(ToEccurve(config.Eccurve ?? ""));
            default:
                throw new ArgumentException("The signature algorithm is not supported yet.");
        }
    }

    public static readonly string OidPattern = "^([0-9]+)(\\.[0-9]+)*$";

    public static ECCurve ToEccurve(string value)
    {
        var ex = new List<Exception>();
        if (new Regex(OidPattern).IsMatch(value))
        {
            try
            {
                return ECCurve.CreateFromValue(value);
            }
            catch (Exception e)
            {
                ex.Add(e);
            }
        }
        try
        {
            return ECCurve.CreateFromFriendlyName(value);
        }
        catch (Exception e)
        {
            ex.Add(e);
            throw new AggregateException(ex);
        }
    }

    protected bool TryLoadPrivateKey(X509File? file, [NotNullWhen(true)] out AsymmetricAlgorithm? privateKey, [NotNullWhen(true)] out SignatureAlgorithmName? signatureAlgorithm)
    {
        try
        {
            privateKey = LoadPrivateKey(file, out var alg);
            signatureAlgorithm = alg;
            return true;
        }
        catch
        {
            signatureAlgorithm = null;
            privateKey = null;
            return false;
        }
    }

    protected AsymmetricAlgorithm LoadPrivateKey(X509File? file, out SignatureAlgorithmName signatureAlgorithm)
    {
        if (!(file?.Exists() ?? false))
        {
            Logger.Information("Private key is not provided. Create private key.");
        }
        return LoadPrivateKey(file!.Data ?? File.ReadAllBytes(file.FileName!), file.Password, out signatureAlgorithm);
    }

    protected bool TryLoadPrivateKey(byte[] data, string? password, [NotNullWhen(true)] out AsymmetricAlgorithm? privateKey, [NotNullWhen(true)] out SignatureAlgorithmName? signatureAlgorithm)
    {
        try
        {
            privateKey = LoadPrivateKey(data, password, out var alg);
            signatureAlgorithm = alg;
            return true;
        }
        catch
        {
            signatureAlgorithm = null;
            privateKey = null;
            return false;
        }
    }

    protected AsymmetricAlgorithm LoadPrivateKey(byte[] data, string? password, out SignatureAlgorithmName signatureAlgorithm)
    {
        Logger.Debug("Try Load RSA Key");
        AsymmetricAlgorithm privateKey = RSA.Create();
        Exception? loadRsaEx;
        try
        {
            LoadPrivateKey(privateKey, data, password);
            signatureAlgorithm = SignatureAlgorithmName.Rsa;
            return privateKey;
        }
        catch (Exception e)
        {
            privateKey.Dispose();
            loadRsaEx = e;
        }

        privateKey = ECDsa.Create();
        try
        {
            LoadPrivateKey(privateKey, data, password);
            signatureAlgorithm = SignatureAlgorithmName.Ecdsa;
            return privateKey;
        }
        catch (Exception e)
        {
            privateKey.Dispose();
            throw new AggregateException(loadRsaEx, e);
        }
    }

    private bool TryLoadPrivateKey(AsymmetricAlgorithm alg, byte[] data)
    {
        try
        {
            LoadPrivateKey(alg, data);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void LoadPrivateKey(AsymmetricAlgorithm alg, byte[] data)
    {
        Logger.Information("Try to read private key from DER.");
        Exception? loadDerEx;
        try
        {
            alg.ImportPkcs8PrivateKey(data, out var read);
            return;
        }
        catch (Exception e)
        {
            loadDerEx = e;
        }

        Logger.Information("Try to read private key from PEM.");
        try
        {
            alg.ImportFromPem(new ReadOnlySpan<char>(System.Text.Encoding.UTF8.GetString(data).ToCharArray()));
            return;
        }
        catch (Exception e)
        {
            throw new AggregateException(loadDerEx, e);
        }
    }

    protected bool TryLoadPrivateKey(AsymmetricAlgorithm alg, byte[] data, string? password)
    {
        try
        {
            LoadPrivateKey(alg, data, password);
            return true;
        }
        catch
        {
            return false;
        }
    }

    protected void LoadPrivateKey(AsymmetricAlgorithm alg, byte[] data, string? password)
    {
        if (String.IsNullOrEmpty(password))
        {
            LoadPrivateKey(alg, data);
            return;
        }

        var passwordSpan = new ReadOnlySpan<char>(password.ToArray());
        Logger.Information("Try to read encrypted private key from DER.");
        Exception? loadDerEx;
        try
        {
            alg.ImportEncryptedPkcs8PrivateKey(passwordSpan, data, out var read);
            return;
        }
        catch (Exception e)
        {
            loadDerEx = e;
        }

        Logger.Information("Try to read encrypted private key from PEM.");
        try
        {
            alg.ImportFromEncryptedPem(new ReadOnlySpan<char>(System.Text.Encoding.UTF8.GetString(data).ToCharArray()), passwordSpan);
            return;
        }
        catch (Exception e)
        {
            throw new AggregateException(loadDerEx, e);
        }
    }

    public static HashAlgorithmName ToHashAlgorithm(string hashAlgorithm)
    {
        var algs = new List<HashAlgorithmName>()
        {
            HashAlgorithmName.MD5,
            HashAlgorithmName.SHA1,
            HashAlgorithmName.SHA256,
            HashAlgorithmName.SHA384,
            HashAlgorithmName.SHA512,
        };
        var hashAlgorithmName = hashAlgorithm.ToUpper();
        foreach (var alg in algs)
        {
            if (hashAlgorithmName == alg.Name?.ToUpper())
            {
                return alg;
            }
        }
        return HashAlgorithmName.FromOid(hashAlgorithm);
    }

    protected CertificateRequest InitCsr(AsymmetricAlgorithm privateKey, HashAlgorithmName hashAlgorithm, ConfigSubjectName? subjectName)
    {
        if (subjectName == null)
        {
            Logger.Information("Subject name is not provided.");
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
            Logger.Information("extension already exists.");
            return;
        }
        Logger.Information("add extension.");
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
            Logger.Information("remove extension.");
            csr.CertificateExtensions.Remove(ext);
        }
    }


    protected void AddExtensions(CertificateRequest csr, ConfigExtensions? extensions, bool replace = true)
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
            Logger.Information("No extensions provided.");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="privateKeyFile"></param>
    /// <param name="publicKeyFile"></param>
    /// <param name="privateKey"></param>
    /// <param name="hashAlgorithm">The name of a hash algorithm to use with the Key Derivation Function (KDF) to turn a password into an encryption key.</param>
    protected void SaveKeyPair(X509File privateKeyFile, X509File publicKeyFile, AsymmetricAlgorithm privateKey, HashAlgorithmName? hashAlgorithm = null)
    {
        this.SavePrivateKey(privateKeyFile, privateKey, hashAlgorithm);
        this.SavePublicKey(publicKeyFile, privateKey);
    }
    protected void SavePrivateKey(X509File privateKeyFile, AsymmetricAlgorithm privateKey, HashAlgorithmName? hashAlgorithm = null)
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
            privateKeyFile.FileFormat = new X509FileFormat(X509Encoding.Der);
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
    protected void SavePublicKey(X509File publicKeyFile, AsymmetricAlgorithm privateKey)
    {
        if (publicKeyFile.FileFormat?.Encoding == X509Encoding.Pem)
        {
            publicKeyFile.Data = Encoding.UTF8.GetBytes(privateKey.ExportSubjectPublicKeyInfoPem());
        }
        else
        {
            publicKeyFile.Data = privateKey.ExportSubjectPublicKeyInfo();
            publicKeyFile.FileFormat = new X509FileFormat(X509Encoding.Der);
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

    protected void SaveCsr(X509File file, CertificateRequest csr)
    {
        var stream = new MemoryStream();
        file ??= new X509File();
        if (file.FileFormat?.Encoding == X509Encoding.Pem)
        {
            csr.SavePem(stream);
        }
        else
        {
            csr.SaveDer(stream);
            file.FileFormat = new X509FileFormat(X509Encoding.Der);
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


    protected void SaveCert(X509File file, X509Certificate2 cert)
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
            file.FileFormat = new X509FileFormat(X509ContentType.Cert, X509Encoding.Der);
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

        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
        if (!String.IsNullOrEmpty(config.HashAlgorithm))
        {
            hashAlgorithm = ToHashAlgorithm(config.HashAlgorithm);
        }
        config.HashAlgorithm = hashAlgorithm.Name;

        Logger.Information("Try load CSR from DER.");
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

        Logger.Information("Try load CSR from PEM.");
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

    protected bool TryLoadCertificate(X509File? file, [NotNullWhen(true)] out X509Certificate2? certificate)
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

    protected X509Certificate2 LoadCertificate(X509File? file)
    {
        if (!(file?.Exists() ?? false))
        {
            throw new ArgumentNullException("certificate");
        }

        if (file.FileFormat?.ContentKind == X509ContentType.Pkcs12)
        {
            Logger.Information("Try load certificate from store");
            return LoadCertificateFromStore(file);
        }

        Logger.Information("Try load certificate");
        const X509KeyStorageFlags flags = X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable;
        Exception? loadCertEx = null;
        try
        {
            if (file.Data != null)
            {
                return new X509Certificate2(file.Data, file.Password, flags);
            }
            return new X509Certificate2(file.FileName!, file.Password, X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable);
        }
        catch (Exception e)
        {
            loadCertEx = e;
        }

        Logger.Information("Try load certificate from store");
        try
        {
            return LoadCertificateFromStore(file);
        }
        catch (Exception e)
        {
            throw new AggregateException(loadCertEx, e);
        }
    }

    protected X509Certificate2 LoadCertificateFromStore(X509File? file)
    {
        Logger.Information("Try load store.");
        var store = LoadStore(file);

        if (string.IsNullOrEmpty(file?.Alias))
        {
            Logger.Information("Alias not specified. Take first certificate.");
            return store.First();
        }
        Logger.Information("Search for {0} in store", file.Alias);

        if (file.AliasType == null)
        {
            Logger.Information("Alias type not specified. Search certificate by index.");
            var index = Int32.Parse(file.Alias);
            if (index < 0)
            {
                throw new IndexOutOfRangeException("Index must not be negative.");
            }
            Logger.Information("Search certificate at index {0}.", index);
            return store.ElementAt(index);
        }
        Logger.Information("Search certificate by {0}", file.AliasType);
        return store.Find((X509FindType)file.AliasType!, file.Alias, false).First();
    }

    public bool TryLoadStore(X509File? file, [NotNullWhen(true)] out X509Certificate2Collection? store)
    {
        try
        {
            store = LoadStore(file);
            return true;
        }
        catch
        {
            store = null;
            return false;
        }
    }

    public X509Certificate2Collection LoadStore(X509File? file)
    {
        if (!(file?.Exists() ?? false))
        {
            throw new ArgumentNullException("store");
        }

        const X509KeyStorageFlags flags = X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable;

        var store = new X509Certificate2Collection();

        if (file.Data != null)
        {
            store.ImportFromPem(Encoding.UTF8.GetString(file.Data).ToCharArray());
            if(store.Count() == 0)
            {
                store.Import(file.Data, file.Password, flags);
            }
        }
        else
        {
            if(store.Count() == 0)
            {
                store.ImportFromPemFile(file.FileName!);
            }
            else
            {
                store.Import(file.FileName!, file.Password, flags);
            }
        }
        return store;
    }

    private bool TryLoadCrl(X509File? file, [NotNullWhen(true)] out CertificateRevocationListBuilder? crl, [NotNullWhen(true)] out BigInteger? crlNumber)
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

    public CertificateRevocationListBuilder LoadCrl(X509File? file, out BigInteger crlNumber)
    {
        if (!(file?.Exists() ?? false))
        {
            throw new ArgumentNullException("crl");
        }

        Logger.Information("Try load CRL from DER.");
        Exception? loadDerEx;
        try
        {
            return CertificateRevocationListBuilder.Load(file.Data ?? File.ReadAllBytes(file.FileName!), out crlNumber);
        }
        catch (Exception e)
        {
            loadDerEx = e;
        }

        Logger.Information("Try load CRL from PEM.");
        try
        {
            if (file.Data != null)
            {
                return CertificateRevocationListBuilder.LoadPem(Encoding.UTF8.GetString(file.Data).ToCharArray(), out crlNumber);
            }
            return CertificateRevocationListBuilder.LoadPem(File.ReadAllText(file.FileName!, Encoding.UTF8), out crlNumber);
        }
        catch (Exception e)
        {
            throw new AggregateException(loadDerEx, e);
        }
    }

    protected void SaveCrl(X509File config, byte[] crl)
    {
        if (config.FileFormat?.Encoding == X509Encoding.Pem)
        {
            var stream = new MemoryStream();
            // encode request to base64
            string base64 = Convert.ToBase64String(crl, Base64FormattingOptions.InsertLineBreaks);
            // write request to a file
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                writer.WriteLine("-----BEGIN CERTIFICATE REVOCATION LIST-----");
                writer.WriteLine(base64);
                writer.Write("-----END CERTIFICATE REVOCATION LIST-----");
            }
            config.Data = stream.ToArray();
        }
        else
        {
            config.Data = crl;
            config.FileFormat = new X509FileFormat(X509ContentType.Cert, X509Encoding.Der);
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

    protected void SaveStore(X509File file, X509Certificate2Collection store)
    {
        file.Data = store.Export(X509ContentType.Pkcs12, file.Password);

        // save to file
        if (!String.IsNullOrEmpty(file.FileName))
        {
            var dir = Path.GetDirectoryName(file.FileName);
            if (!String.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllBytes(file.FileName, file.Data!);
        }
    }

    protected bool TryLoadPrivateKey(X509Certificate2? cert, [NotNullWhen(true)] out AsymmetricAlgorithm? privateKey)
    {
        privateKey = null;
        if (cert == null)
        {
            return false;
        }
        else if (cert.GetRSAPrivateKey() != null)
        {
            privateKey = cert.GetRSAPrivateKey()!;
            return true;
        }
        else if (cert.GetECDsaPrivateKey() != null)
        {
            privateKey = cert.GetECDsaPrivateKey()!;
            return true;
        }
        else if (cert.GetDSAPrivateKey() != null)
        {
            privateKey = cert.GetDSAPrivateKey()!;
            return true;
        }
        return false;
    }

    protected void SaveStorePem(X509File file, X509Certificate2Collection store)
    {
        var stream = new MemoryStream();
        using (StreamWriter writer = new StreamWriter(stream))
        {
            foreach (var cert in store)
            {
                writer.WriteLine(cert.ExportCertificatePem());
                if (this.TryLoadPrivateKey(cert, out var privateKey))
                {
                    var keyFile = new X509File()
                    {
                        Password = file.Password,
                        FileFormat = new X509FileFormat(X509Encoding.Pem)
                    };
                    this.SaveKeyPair(keyFile, new X509File(), privateKey);
                    writer.WriteLine(Encoding.UTF8.GetString(keyFile.Data!));
                }
            }
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
            File.WriteAllBytes(file.FileName, file.Data!);
        }
    }
}
