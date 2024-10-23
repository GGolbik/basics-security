using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using GGolbik.SecurityTools.Store;
using GGolbik.SecurityTools.X509;
using GGolbik.SecurityTools.X509.Infos;
using Org.BouncyCastle.X509;
using Serilog;

namespace GGolbik.SecurityToolsApp.Certificates;

/// <summary>
/// The application implements the certificate store directory layout as recommended by [OPC 10000-12: UA Part 12: Discovery and Global Services: F.1 Certificate Store Directory Layout](https://reference.opcfoundation.org/v105/GDS/docs/F.1/).
/// 
/// Path | Description
/// ---- | -----------
/// `root` | A descriptive name for the trust list.
/// `root/own` | The Certificate store which contains private keys used by the application.
/// `root/own/certs` | Contains the X.509 v3 Certificate sassociated with the private keys in the ./private directory.
/// `root/own/private`| Contains the private keys used by the application.
/// `root/trusted` | The Certificate store which contains trusted Certificates.
/// `root/trusted/certs` | Contains the X.509 v3 Certificates which are trusted.
/// `root/trusted/crl` | Contains the X.509 v3 CRLs for any Certificates in the `./certs` directory.
/// `root/issuer` | The Certificate store which contains the CA Certificates needed for validation.
/// `root/issuer/certs` | Contains the X.509 v3 Certificates which are needed for validation.
/// `root/issuer/crl` | Contains the X.509 v3 CRLs for any Certificates in the `./certs` directory.
/// `root/rejected` | The Certificate store which contains certificates which have been rejected.
/// `root/rejected/certs` | Contains the X.509 v3 Certificates which have been rejected.
/// 
/// On Windows it is also possible to use Windows Certificate store which can be managed with the [Windows Certificate Management Console (certmgr.msc)](https://learn.microsoft.com/en-us/dotnet/framework/tools/certmgr-exe-certificate-manager-tool).
/// </summary>
public class CertificateProviderService : ICertificateProviderService
{
    private readonly Serilog.ILogger Logger = Log.ForContext<CertificateProviderService>();
    private const string ownStoreName = "own";
    private readonly string _appData;
    private readonly string _storePath;
    private readonly string _configPath;
    private CertificateProviderConfig _config;

    public CertificateProviderService(string appData)
    {
        _appData = Path.Combine(appData, "GGolbik.IoT.Platform.CertificateProvider");
        _storePath = Path.Combine(_appData, "stores");
        _configPath = Path.Combine(_appData, "config.json");
        try
        {
            _config = this.LoadConfig(_configPath);
        }
        catch (Exception e)
        {
            Logger.Error(e, e.Message);
            _config ??= new();
        }
    }

    public void Dispose()
    {
        
    }

    private CertificateProviderConfig LoadConfig(string path)
    {
        try
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return JsonSerializer.Deserialize<CertificateProviderConfig>(stream) ?? throw new Exception();
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, e.Message);
            throw;
        }
    }

    private void SaveConfig(CertificateProviderConfig config)
    {
        string tmpPath = _configPath + ".tmp";
        try
        {
            var dir = Path.GetDirectoryName(tmpPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir!);
            }
            using (var stream = new FileStream(tmpPath, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var strStream = new StreamWriter(stream))
            {
                strStream.Write(JsonSerializer.Serialize(config));
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, e.Message);
            throw;
        }
        this.LoadConfig(tmpPath);
        try
        {
            File.Move(tmpPath, _configPath, overwrite: true);
        }
        catch (Exception e)
        {
            Logger.Error(e, e.Message);
            throw;
        }
    }

    private CertificateInfo GetCertificate(DirectoryCertificateStore store, string thumbprint, Stream stream, X509Encoding encoding)
    {
        var cert = store.GetCertificate(thumbprint);
        if (cert == null)
        {
            throw new ArgumentException("Not Found");
        }
        switch (encoding)
        {
            case X509Encoding.None:
                return cert.ToCertificateInfo();
            case X509Encoding.Der:
                stream.Write(cert.Export(X509ContentType.Cert));
                return cert.ToCertificateInfo();
            case X509Encoding.Pem:
                stream.Write(Encoding.UTF8.GetBytes(cert.ExportCertificatePem()));
                return cert.ToCertificateInfo();
            default:
                throw new ArgumentException("Encoding is not supported.");
        }
    }

    private KeyPairInfo GetKeyPair(DirectoryCertificateStore store, string thumbprint, Stream stream, X509Encoding encoding, byte[]? password, PbeParameters? pbeParameters)
    {
        var keyPair = store.GetKeyPair(thumbprint);
        if (keyPair == null)
        {
            throw new ArgumentException("Not found");
        }
        pbeParameters ??= new PbeParameters(PbeEncryptionAlgorithm.Aes128Cbc, HashAlgorithmName.SHA256, 100_000);
        switch (encoding)
        {
            case X509Encoding.None:
                return keyPair.ToKeyPairInfo();
            case X509Encoding.Der:
                if (password == null)
                    stream.Write(keyPair.ToDer());
                else
                    stream.Write(keyPair.ToDer(password, pbeParameters));
                return keyPair.ToKeyPairInfo();
            case X509Encoding.Pem:
                if (password == null)
                    stream.Write(Encoding.UTF8.GetBytes(keyPair.ToPem()));
                else
                    stream.Write(Encoding.UTF8.GetBytes(keyPair.ToPem(password, pbeParameters)));
                return keyPair.ToKeyPairInfo();
            default:
                throw new ArgumentException("Encoding is not supported.");
        }
    }

    private CrlInfo GetCrl(DirectoryCertificateStore store, string thumbprint, Stream stream, X509Encoding encoding)
    {
        var crl = store.GetCrl(thumbprint);
        if (crl == null)
        {
            throw new ArgumentException("Not Found");
        }
        switch (encoding)
        {
            case X509Encoding.None:
                return crl.ToCrlInfo();
            case X509Encoding.Der:
                stream.Write(crl.ToDer());
                return crl.ToCrlInfo();
            case X509Encoding.Pem:
                stream.Write(Encoding.UTF8.GetBytes(crl.ToPem()));
                return crl.ToCrlInfo();
            default:
                throw new ArgumentException("Encoding is not supported.");
        }
    }

    private CertificateRequest CreateCertificateRequest(X500DistinguishedName distinguishedName, AsymmetricAlgorithm keyPair, HashAlgorithmName? hashAlgorithm)
    {
        hashAlgorithm ??= HashAlgorithmName.SHA256;
        if (keyPair is RSA rsa)
        {
            // The RSA signature padding to apply if self-signing or being signed with an System.Security.Cryptography.X509Certificates.X509Certificate2.
            var padding = RSASignaturePadding.Pkcs1;
            return new CertificateRequest(distinguishedName, rsa, (HashAlgorithmName)hashAlgorithm, padding);
        }
        else if (keyPair is ECDsa ecdsa)
        {
            return new CertificateRequest(distinguishedName, ecdsa, (HashAlgorithmName)hashAlgorithm);
        }
        throw new ArgumentException("Asymmetric Algorithm is not supported");
    }

    private X509SignatureGenerator CreateX509SignatureGenerator(AsymmetricAlgorithm keyPair)
    {
        if (keyPair is RSA)
        {
            return X509SignatureGenerator.CreateForRSA((keyPair as RSA)!, RSASignaturePadding.Pkcs1);
        }
        else if (keyPair is ECDsa)
        {
            return X509SignatureGenerator.CreateForECDsa((keyPair as ECDsa)!);
        }
        else
        {
            throw new ArgumentException("Failed to create signaturegenerator");
        }
    }

    #region Certificate Group

    public IList<CertificateGroup> GetCertificateGroups()
    {
        lock (this)
        {
            return ((CertificateProviderConfig)_config.Clone()).CertificateGroups ?? new List<CertificateGroup>();
        }
    }

    public CertificateGroup GetCertificateGroup(string groupId)
    {
        lock (this)
        {
            var group = _config.CertificateGroups?.FirstOrDefault((item) =>
            {
                return groupId.Equals(item.Id);
            });
            return (CertificateGroup?)group?.Clone() ?? throw new ArgumentException("Not found");
        }
    }

    public string AddCertificateGroup(CertificateGroup group)
    {
        group = (CertificateGroup)group.Clone();
        lock (this)
        {
            var config = (CertificateProviderConfig)_config.Clone();
            config.CertificateGroups ??= new List<CertificateGroup>();
            do
            {
                group.Id = Guid.NewGuid().ToString();
            } while (config.CertificateGroups.FirstOrDefault((item) => group.Id.Equals(item.Id)) != null);
            config.CertificateGroups.Add(group);
            this.SaveConfig(config);
            _config = config;
        }
        return group.Id;
    }

    public void UpdateCertificateGroup(CertificateGroup group)
    {
        group = (CertificateGroup)group.Clone();
        lock (this)
        {
            var config = (CertificateProviderConfig)_config.Clone();
            var currentGroup = config.CertificateGroups?.FirstOrDefault((item) =>
            {
                return string.Equals(group.Id, item.Id);
            });
            if (currentGroup == null)
            {
                throw new ArgumentException("Not found");
            }
            config.CertificateGroups!.Remove(currentGroup);
            config.CertificateGroups.Add(group);
            this.SaveConfig(config);
            _config = config;
        }
    }

    public void DeleteCertificateGroup(string groupId)
    {
        CertificateGroup? group;
        lock (this)
        {
            var config = (CertificateProviderConfig)_config.Clone();
            group = config.CertificateGroups?.FirstOrDefault((item) =>
            {
                return groupId.Equals(item.Id);
            });
            if (group == null)
            {
                throw new ArgumentException("Not found");
            }
            var path = Path.Combine(_storePath, groupId);
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                throw;
            }
            config.CertificateGroups!.Remove(group);
            this.SaveConfig(config);
            _config = config;
        }
    }

    #endregion

    #region Add

    public CertificateInfoCollection AddCertificates(string groupId, Stream certificates)
    {
        IList<X509Certificate2> list;
        lock (this)
        {
            this.GetCertificateGroup(groupId);
            DirectoryCertificateStore store = new(Path.Combine(_storePath, groupId, ownStoreName));
            list = store.AddCertificates(certificates);
        }
        return new CertificateInfoCollection(list);
    }

    public KeyPairInfoCollection AddKeyPairs(string groupId, Stream keyPairs, byte[]? password)
    {
        IList<AsymmetricAlgorithm> list;
        lock (this)
        {
            this.GetCertificateGroup(groupId);
            DirectoryCertificateStore store = new(Path.Combine(_storePath, groupId, ownStoreName));
            list = store.AddKeyPairs(keyPairs, password);
        }
        return new KeyPairInfoCollection(list);
    }

    public CrlInfoCollection AddCrls(string groupId, Stream crls)
    {
        IList<X509Crl> list;
        lock (this)
        {
            this.GetCertificateGroup(groupId);
            DirectoryCertificateStore store = new(Path.Combine(_storePath, groupId, ownStoreName));
            list = store.AddCrls(crls);
        }
        return new CrlInfoCollection(list);
    }

    #endregion

    #region Delete

    public void DeleteCertificate(string groupId, string thumbprint)
    {
        CertificateInfo info;
        lock (this)
        {
            info = this.GetCertificate(groupId, thumbprint);
            DirectoryCertificateStore store = new(Path.Combine(_storePath, groupId, ownStoreName));
            store.DeleteCertificate(thumbprint);
        }
    }

    public void DeleteKeyPair(string groupId, string thumbprint)
    {
        KeyPairInfo info;
        lock (this)
        {
            info = this.GetKeyPair(groupId, thumbprint);
            DirectoryCertificateStore store = new(Path.Combine(_storePath, groupId, ownStoreName));
            store.DeleteKeyPair(thumbprint);
        }
    }

    public void DeleteCrl(string groupId, string thumbprint)
    {
        CrlInfo info;
        lock (this)
        {
            info = this.GetCrl(groupId, thumbprint);
            DirectoryCertificateStore store = new(Path.Combine(_storePath, groupId, ownStoreName));
            store.DeleteCrl(thumbprint);
        }
    }

    #endregion

    #region Get

    public CertificateInfo GetCertificate(string groupId, string thumbprint)
    {
        lock (this)
        {
            this.GetCertificateGroup(groupId);
            return this.GetCertificate(groupId, thumbprint, new MemoryStream(), X509Encoding.None);
        }
    }

    public CertificateInfo GetCertificate(string groupId, string thumbprint, Stream stream, X509Encoding encoding)
    {
        lock (this)
        {
            this.GetCertificateGroup(groupId);
            DirectoryCertificateStore store = new(Path.Combine(_storePath, groupId, ownStoreName));
            return this.GetCertificate(store, thumbprint, stream, encoding);
        }
    }

    public IList<CertificateInfo> GetCertificates(string groupId)
    {
        lock (this)
        {
            this.GetCertificateGroup(groupId);
            DirectoryCertificateStore store = new(Path.Combine(_storePath, groupId, ownStoreName));
            return new List<CertificateInfo>(store.GetCertificates().Select((item) =>
            {
                return item.ToCertificateInfo();
            }));
        }
    }

    public KeyPairInfo GetKeyPair(string groupId, string thumbprint)
    {
        lock (this)
        {
            return this.GetKeyPair(groupId, thumbprint, new MemoryStream(), X509Encoding.None);
        }
    }

    public KeyPairInfo GetKeyPair(string groupId, string thumbprint, Stream stream, X509Encoding encoding)
    {
        lock (this)
        {
            return this.GetKeyPair(groupId, thumbprint, stream, encoding, null, null);
        }
    }

    public KeyPairInfo GetKeyPair(string groupId, string thumbprint, Stream stream, X509Encoding encoding, byte[]? password, PbeParameters? pbeParameters)
    {
        lock (this)
        {
            this.GetCertificateGroup(groupId);
            DirectoryCertificateStore store = new(Path.Combine(_storePath, groupId, ownStoreName));
            return this.GetKeyPair(store, thumbprint, stream, encoding, password, pbeParameters);
        }
    }

    public IList<KeyPairInfo> GetKeyPairs(string groupId)
    {
        lock (this)
        {
            this.GetCertificateGroup(groupId);
            DirectoryCertificateStore store = new(Path.Combine(_storePath, groupId, ownStoreName));
            return new List<KeyPairInfo>(store.GetKeyPairs().Select((item) =>
            {
                return item.ToKeyPairInfo();
            }));
        }
    }

    public CrlInfo GetCrl(string groupId, string thumbprint)
    {
        lock (this)
        {
            this.GetCertificateGroup(groupId);
            DirectoryCertificateStore store = new(Path.Combine(_storePath, groupId, ownStoreName));
            return this.GetCrl(store, thumbprint, new MemoryStream(), X509Encoding.None);
        }
    }

    public CrlInfo GetCrl(string groupId, string thumbprint, Stream stream, X509Encoding encoding)
    {
        lock (this)
        {
            this.GetCertificateGroup(groupId);
            DirectoryCertificateStore store = new(Path.Combine(_storePath, groupId, ownStoreName));
            return this.GetCrl(store, thumbprint, stream, encoding);
        }
    }

    public IList<CrlInfo> GetCrls(string groupId)
    {
        lock (this)
        {
            this.GetCertificateGroup(groupId);
            DirectoryCertificateStore store = new(Path.Combine(_storePath, groupId, ownStoreName));
            return new List<CrlInfo>(store.GetCrls().Select((item) =>
            {
                return item.ToCrlInfo();
            }));
        }
    }

    #endregion

    public KeyPairInfo GenerateKeyPair(string groupId, KeyPairInfo info)
    {
        lock (this)
        {
            AsymmetricAlgorithm? keyPair = null;
            try
            {
                switch (info.SignatureAlgorithm)
                {
                    case SignatureAlgorithmName.Rsa:
                        keyPair = RSA.Create(info.KeySize ?? 2048);
                        break;
                    case SignatureAlgorithmName.Dsa:
                        keyPair = DSA.Create(info.KeySize ?? 2048);
                        break;
                    case SignatureAlgorithmName.Ecdsa:
                        keyPair = ECDsa.Create(info.Eccurve?.ToEccurve() ?? ECCurve.CreateFromOid(new Oid("1.2.840.10045.3.1.7", "nistP256")));
                        break;
                    case SignatureAlgorithmName.Ecdh:
                        keyPair = ECDiffieHellman.Create(info.Eccurve?.ToEccurve() ?? ECCurve.CreateFromOid(new Oid("1.2.840.10045.3.1.7", "nistP256")));
                        break;
                    default:
                        throw new ArgumentException($"Signature algorithm {info.SignatureAlgorithm.ToString()} is not supported.");
                }
                var result = this.AddKeyPairs(groupId, new MemoryStream(keyPair.ToDer()), null);
                if (result.Count() == 0)
                {
                    throw new ArgumentException("Key could not be added.");
                }
                return result[0];
            }
            finally
            {
                keyPair?.Dispose();
            }
        }
    }
}