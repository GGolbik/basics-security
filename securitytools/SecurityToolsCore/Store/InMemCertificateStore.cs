
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using GGolbik.SecurityTools.X509;
using Org.BouncyCastle.X509;

namespace GGolbik.SecurityTools.Store;

public class InMemCertificateStore : CertificateStore
{
    private Dictionary<string, X509Certificate2> _certs = new();
    private Dictionary<string, AsymmetricAlgorithm> _keys = new();
    private Dictionary<string, X509Crl> _crls = new();
    private Dictionary<string, CertificateRequest> _csrs = new();

    public InMemCertificateStore() : base()
    {

    }

    public override void UpdatePassword(byte[]? password, PbeParameters? pbeParameters = null)
    {

    }

    #region Get
    public override IList<X509Certificate2> GetCertificates(bool includePrivateKey)
    {
        List<X509Certificate2> certs = new();
        foreach (var cert in _certs.Values)
        {
            var keyPair = this.GetKeyPair(cert.PublicKey.ToThumbprint());
            if (includePrivateKey && keyPair != null)
            {
                certs.Add(X509Certificate2.CreateFromPem(cert.ToPem(), keyPair.ToPem()));
            }
            else
            {
                certs.Add(cert.Clone(includePrivateKey));
            }
        }
        return certs;
    }

    public override X509Certificate2? GetCertificate(string thumbprint, bool includePrivateKey)
    {
        _certs.TryGetValue(thumbprint, out var cert);
        if (cert == null)
        {
            return cert;
        }
        if (includePrivateKey)
        {
            // TODO add private key
            return cert;
        }
        else
        {
            return new X509Certificate2(cert.ToDer());
        }
    }

    public override IList<AsymmetricAlgorithm> GetKeyPairs()
    {
        List<AsymmetricAlgorithm> keys = new();
        foreach (var key in _keys.Values)
        {
            keys.Add(key.Clone());
        }
        return keys;
    }

    public override AsymmetricAlgorithm? GetKeyPair(string thumbprint)
    {
        _keys.TryGetValue(thumbprint, out var key);
        return key;
    }

    public override IList<string> GetKeyPairsWithError()
    {
        return new List<string>();
    }

    public override IList<X509Crl> GetCrls()
    {
        List<X509Crl> crls = new();
        foreach (var crl in _crls.Values)
        {
            crls.Add(crl.Clone());
        }
        return crls;
    }

    public override X509Crl? GetCrl(string thumbprint)
    {
        _crls.TryGetValue(thumbprint, out var crl);
        return crl;
    }

    public override IList<CertificateRequest> GetCsrs()
    {
        List<CertificateRequest> csrs = new();
        foreach (var csr in _csrs.Values)
        {
            csrs.Add(csr.Clone());
        }
        return csrs;
    }

    public override CertificateRequest? GetCsr(string thumbprint)
    {
        _csrs.TryGetValue(thumbprint, out var csr);
        return csr;
    }
    #endregion

    #region Add
    public override IList<object> Add(Stream stream, byte[]? password)
    {
        List<object> result = new();
        X509Reader.Read(stream, password, (cert) =>
        {
            result.Add(cert);
            var thumbprint = cert.Thumbprint;
            _certs.Add(thumbprint, cert);
        }, (key) =>
        {
            result.Add(key);
            var thumbprint = key.ToThumbprint();
            _keys.Add(thumbprint, key);
        }, (crl) =>
        {
            result.Add(crl);
            var thumbprint = crl.ToThumbprint();
            _crls.Add(thumbprint, crl);
        }, (csr) =>
        {
            result.Add(csr);
            var thumbprint = csr.ToThumbprint();
            _csrs.Add(thumbprint, csr);
        });
        return result;
    }

    public override IList<X509Certificate2> AddCertificates(Stream stream, byte[]? password)
    {
        List<X509Certificate2> result = new();
        X509Reader.ReadCertificates(stream, password, (cert) =>
        {
            result.Add(cert);
            var thumbprint = cert.Thumbprint;
            _certs.Add(thumbprint, cert);
        });
        return result;
    }

    public override IList<AsymmetricAlgorithm> AddKeyPairs(Stream stream, byte[]? password)
    {
        List<AsymmetricAlgorithm> result = new();
        X509Reader.ReadKeyPairs(stream, password, (keyPair) =>
        {
            result.Add(keyPair);
            var thumbprint = keyPair.ToThumbprint();
            _keys.Add(thumbprint, keyPair);
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
            _crls.Add(thumbprint, crl);
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
            _csrs.Add(thumbprint, csr);
        });
        return result;
    }
    #endregion

    #region Delete
    public override bool DeleteCertificate(string thumbprint)
    {
        return _certs.Remove(thumbprint);
    }

    public override bool DeleteKeyPair(string thumbprint)
    {
        return _keys.Remove(thumbprint);
    }

    public override bool DeleteCrl(string thumbprint)
    {
        return _crls.Remove(thumbprint);
    }

    public override bool DeleteCsr(string thumbprint)
    {
        return _csrs.Remove(thumbprint);
    }
    #endregion
}
