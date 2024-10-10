
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using GGolbik.SecurityTools.X509;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.X509;

namespace GGolbik.SecurityTools.Store;

/// <summary>
/// Directory layout is based on https://reference.opcfoundation.org/v105/GDS/docs/F.1/
/// 
/// `certs` | Contains the X.509 v3 Certificates associated with the private keys in the ./private directory.
/// `private`| Contains the private keys used by the application.
/// `crl` | Contains the X.509 v3 CRLs for any Certificates in the `./certs` directory.
/// `csr` | Contains the Certificate Signing Requests (CSR) in the `./csr` directory.
/// 
/// The certs are stored as PEM by there thumbprint over the whole DER data.
/// The privates are stored as PEM by there thumbprint over the public key.
/// The crls are stored as DER by there thumbprint over the whole DER data.
/// The csrs are stored as PEM by there thumbprint over the whole DER data.
/// </summary>
public class DirectoryCertificateStore : CertificateStore
{
    private readonly string _certsPath;
    private readonly string _privatePath;
    private readonly string _crlPath;
    private readonly string _csrPath;

    public DirectoryCertificateStore(string path)
    {
        _certsPath = Path.Combine(path, "certs");
        _privatePath = Path.Combine(path, "private");
        _crlPath = Path.Combine(path, "crl");
        _csrPath = Path.Combine(path, "csr");
        var dirs = new string[] { _certsPath, _privatePath, _crlPath, _csrPath };
        foreach (var dir in dirs)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }

    private void AddKeyPair(AsymmetricAlgorithm? key)
    {
        if (key == null)
        {
            return;
        }
        var thumbprint = key.ToThumbprint();
        using (var stream = new FileStream(Path.Combine(_privatePath, thumbprint), FileMode.Create, FileAccess.Write, FileShare.Read))
        {
            string pem = key.ExportPkcs8PrivateKeyPem();
            stream.Write(Encoding.UTF8.GetBytes(pem));
        }
    }

    public override bool MoveCertificateTo(string thumbprint, ICertificateStore store)
    {
        if (store == this)
        {
            return true;
        }
        if (store is DirectoryCertificateStore dirStore)
        {
            var sourcePath = Path.Combine(_certsPath, thumbprint);
            var destPath = Path.Combine(dirStore._certsPath, thumbprint);
            if (!File.Exists(sourcePath))
            {
                return false;
            }
            File.Move(sourcePath, destPath);
            return true;
        }
        return base.MoveCertificateTo(thumbprint, store);
    }

    #region Get

    public override IList<X509Certificate2> GetCertificates(bool includePrivateKey)
    {
        List<X509Certificate2> result = new();
        DirectoryInfo dir = new(_certsPath);
        if (!dir.Exists)
        {
            return result;
        }
        foreach (var fileInfo in dir.GetFiles())
        {
            var cert = this.GetCertificate(fileInfo.Name, includePrivateKey);
            if (cert != null)
            {
                result.Add(cert);
            }
        }
        return result;
    }

    public override X509Certificate2? GetCertificate(string thumbprint, bool includePrivateKey)
    {
        var path = Path.Combine(_certsPath, thumbprint);
        if (!File.Exists(path))
        {
            return null;
        }
        var cert = new X509Certificate2(path);
        if (!includePrivateKey)
        {
            return cert;
        }
        var publicKeyThumbprint = cert.PublicKey.ToThumbprint();
        var pathPrivateKey = Path.Combine(_privatePath, publicKeyThumbprint);
        if (!File.Exists(pathPrivateKey))
        {
            return cert;
        }
        return X509Certificate2.CreateFromPemFile(path, pathPrivateKey);
    }

    public override IList<AsymmetricAlgorithm> GetKeyPairs()
    {
        List<AsymmetricAlgorithm> result = new();
        DirectoryInfo dir = new(_privatePath);
        if (!dir.Exists)
        {
            return result;
        }
        foreach (var fileInfo in dir.GetFiles())
        {
            var keyPair = this.GetKeyPair(fileInfo.Name);
            if (keyPair != null)
            {
                result.Add(keyPair);
            }
        }
        return result;
    }

    public override AsymmetricAlgorithm? GetKeyPair(string thumbprint)
    {
        string path = Path.Combine(_privatePath, thumbprint);
        if (!File.Exists(path))
        {
            return null;
        }
        return X509Reader.ReadKeyPairFromPem(File.ReadAllBytes(path));
    }

    public override IList<X509Crl> GetCrls()
    {
        List<X509Crl> result = new();
        DirectoryInfo dir = new(_crlPath);
        if (!dir.Exists)
        {
            return result;
        }
        foreach (var fileInfo in dir.GetFiles())
        {
            var crl = this.GetCrl(fileInfo.Name);
            if (crl != null)
            {
                result.Add(crl);
            }
        }
        return result;
    }

    public override X509Crl? GetCrl(string thumbprint)
    {
        string path = Path.Combine(_crlPath, thumbprint);
        if (!File.Exists(path))
        {
            return null;
        }
        X509CrlParser parser = new();
        using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            return parser.ReadCrl(stream);
        }
    }

    public override IList<CertificateRequest> GetCsrs()
    {
        List<CertificateRequest> result = new();
        DirectoryInfo dir = new(_csrPath);
        if (!dir.Exists)
        {
            return result;
        }
        foreach (var fileInfo in dir.GetFiles())
        {
            var crl = this.GetCsr(fileInfo.Name);
            if (crl != null)
            {
                result.Add(crl);
            }
        }
        return result;
    }

    public override CertificateRequest? GetCsr(string thumbprint)
    {
        string path = Path.Combine(_csrPath, thumbprint);
        if (!File.Exists(path))
        {
            return null;
        }
        return CertificateRequest.LoadSigningRequestPem(path, HashAlgorithmName.SHA256, CertificateRequestLoadOptions.UnsafeLoadCertificateExtensions, RSASignaturePadding.Pkcs1);
    }
    #endregion

    #region Add


    public override IList<object> Add(Stream stream, byte[]? password)
    {
        List<object> result = new();
        X509Reader.Read(stream, password, (cert) =>
        {
            result.Add(cert);
            using (var outputStream = new FileStream(Path.Combine(_certsPath, cert.Thumbprint), FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var textWriter = new StreamWriter(outputStream))
            {
                textWriter.Write(cert.ToPem());
            }
        }, (key) =>
        {
            result.Add(key);
            this.AddKeyPair(key);
        }, (crl) =>
        {
            result.Add(crl);
            var thumbprint = crl.ToThumbprint();
            using (var outputStream = new FileStream(Path.Combine(_crlPath, thumbprint), FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var textWriter = new StreamWriter(outputStream))
            using (var pemWriter = new PemWriter(textWriter))
            {
                pemWriter.WriteObject(crl);
            }
        });
        return result;
    }

    public override IList<X509Certificate2> AddCertificates(Stream stream, byte[]? password)
    {
        List<X509Certificate2> result = new();
        X509Reader.ReadCertificates(stream, password, (cert) =>
        {
            result.Add(cert);
            using (var outputStream = new FileStream(Path.Combine(_certsPath, cert.Thumbprint), FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var textWriter = new StreamWriter(outputStream))
            {
                textWriter.Write(cert.ToPem());
            }
        });
        return result;
    }

    public override IList<AsymmetricAlgorithm> AddKeyPairs(Stream stream, byte[]? password = null)
    {
        List<AsymmetricAlgorithm> result = new();
        X509Reader.ReadKeyPairs(stream, password, (keyPair) =>
        {
            result.Add(keyPair);
            this.AddKeyPair(keyPair);
        });
        return result;
    }

    public override IList<X509Crl> AddCrls(Stream stream)
    {
        List<X509Crl> result = new();
        X509Reader.ReadCrls(stream, (crl) =>
        {
            result.Add(crl);
            var thumbprint = crl.ToThumbprint();
            using (var outputStream = new FileStream(Path.Combine(_crlPath, thumbprint), FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var textWriter = new StreamWriter(outputStream))
            using (var pemWriter = new PemWriter(textWriter))
            {
                pemWriter.WriteObject(crl);
            }
        });
        return result;
    }

    public override IList<CertificateRequest> AddCsrs(Stream stream)
    {
        List<CertificateRequest> result = new();
        X509Reader.ReadCsrs(stream, (csr) =>
        {
            result.Add(csr);
            var thumbprint = csr.ToThumbprint();
            csr.SavePem(Path.Combine(_csrPath, thumbprint));
        });
        return result;
    }

    #endregion

    #region Delete

    public override bool DeleteCertificate(string thumbprint)
    {
        string path = Path.Combine(_certsPath, thumbprint);
        if (!File.Exists(path))
        {
            return false;
        }
        File.Delete(path);
        return true;
    }

    public override bool DeleteKeyPair(string thumbprint)
    {
        string path = Path.Combine(_privatePath, thumbprint);
        if (!File.Exists(path))
        {
            return false;
        }
        File.Delete(path);
        return true;
    }

    public override bool DeleteCrl(string thumbprint)
    {
        string path = Path.Combine(_crlPath, thumbprint);
        if (!File.Exists(path))
        {
            return false;
        }
        File.Delete(path);
        return true;
    }

    public override bool DeleteCsr(string thumbprint)
    {
        string path = Path.Combine(_csrPath, thumbprint);
        if (!File.Exists(path))
        {
            return false;
        }
        File.Delete(path);
        return true;
    }

    #endregion
}