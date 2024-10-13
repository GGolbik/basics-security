
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using GGolbik.SecurityTools.X509;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;

namespace GGolbik.SecurityTools.Store;


public abstract class CertificateStore : ICertificateStore
{
    /// <summary>
    /// The password to encrypt and decrypt keys.
    /// </summary>
    protected byte[]? _password = null;
    /// <summary>
    /// The parameters to encrypt and decrypt keys.
    /// </summary>
    protected PbeParameters? _pbeParameters = null;

    protected CertificateStore()
    {

    }

    protected CertificateStore(byte[]? password, PbeParameters? pbeParameters = null)
    {
        _password = password;
        _pbeParameters = pbeParameters;
    }

    public abstract void UpdatePassword(byte[]? password, PbeParameters? pbeParameters = null);

    public virtual bool MoveCertificateTo(string thumbprint, ICertificateStore store)
    {
        if (store == this)
        {
            return true;
        }
        var cert = this.GetCertificate(thumbprint);
        if (cert == null)
        {
            return false;
        }
        MemoryStream mem = new(cert.ToDer());
        if (store.AddCertificates(mem).Count() > 0)
        {
            return this.DeleteCertificate(thumbprint);
        }
        return false;
    }

    #region Get
    public virtual IList<X509Certificate2> GetCertificates()
    {
        return this.GetCertificates(false);
    }
    public abstract IList<X509Certificate2> GetCertificates(bool includePrivateKey);
    public virtual X509Certificate2? GetCertificate(string thumbprint)
    {
        return this.GetCertificate(thumbprint, false);
    }
    public abstract X509Certificate2? GetCertificate(string thumbprint, bool includePrivateKey);
    public abstract IList<AsymmetricAlgorithm> GetKeyPairs();
    public abstract AsymmetricAlgorithm? GetKeyPair(string thumbprint);
    public abstract IList<string> GetKeyPairsWithError();
    public abstract IList<X509Crl> GetCrls();
    public virtual IList<X509Crl> GetCrlsOfIssuer(string thumbprint)
    {
        return this.GetCrlsOfIssuer(thumbprint, false);
    }
    public virtual IList<X509Crl> GetCrlsOfIssuer(string thumbprint, bool verify)
    {
        List<X509Crl> result = new();
        var issuerCert = this.GetCertificate(thumbprint);
        if (issuerCert == null)
        {
            return result;
        }
        var crls = this.GetCrls();
        foreach (var crl in crls)
        {
            if (!Arrays.AreEqual(crl!.IssuerDN.ToAsn1Object().GetDerEncoded(), issuerCert.IssuerName.RawData))
            {
                continue;
            }
            try
            {
                // Need not to be verified. the crl might be issued by a differnt CA.
                if(verify)
                {
                    crl.Verify(new X509CertificateParser().ReadCertificate(issuerCert.ToDer()).GetPublicKey());
                }
                result.Add(crl);
            }
            catch
            {

            }
        }
        return result;
    }
    public abstract X509Crl? GetCrl(string thumbprint);

    public abstract IList<CertificateRequest> GetCsrs();

    public abstract CertificateRequest? GetCsr(string thumbprint);
    #endregion

    #region Add
    public abstract IList<object> Add(Stream stream, byte[]? password);
    public virtual IList<X509Certificate2> AddCertificates(Stream stream)
    {
        return this.AddCertificates(stream, null);
    }
    public abstract IList<X509Certificate2> AddCertificates(Stream stream, byte[]? password);
    public virtual IList<AsymmetricAlgorithm> AddKeyPairs(Stream stream)
    {
        return this.AddKeyPairs(stream, null);
    }
    public abstract IList<AsymmetricAlgorithm> AddKeyPairs(Stream stream, byte[]? password);
    public abstract IList<X509Crl> AddCrls(Stream stream);
    public abstract IList<CertificateRequest> AddCsrs(Stream stream);
    #endregion

    #region Delete
    public abstract bool DeleteCertificate(string thumbprint);
    public abstract bool DeleteKeyPair(string thumbprint);
    public abstract bool DeleteCrl(string thumbprint);
    public abstract bool DeleteCsr(string thumbprint);
    #endregion
}
