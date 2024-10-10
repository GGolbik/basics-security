
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.X509;

namespace GGolbik.SecurityTools.Store;


public interface ICertificateStore
{
    bool MoveCertificateTo(string thumbprint, ICertificateStore store);

    #region Get
    IList<X509Certificate2> GetCertificates();
    IList<X509Certificate2> GetCertificates(bool includePrivateKey);
    X509Certificate2? GetCertificate(string thumbprint);
    X509Certificate2? GetCertificate(string thumbprint, bool includePrivateKey);
    IList<AsymmetricAlgorithm> GetKeyPairs();
    AsymmetricAlgorithm? GetKeyPair(string thumbprint);
    IList<X509Crl> GetCrls();
    IList<X509Crl> GetCrlsOfIssuer(string thumbprint);
    X509Crl? GetCrl(string thumbprint);
    IList<CertificateRequest> GetCsrs();
    CertificateRequest? GetCsr(string thumbprint);
    #endregion

    #region Add
    IList<object> Add(Stream stream, byte[]? password);
    IList<X509Certificate2> AddCertificates(Stream stream);
    IList<X509Certificate2> AddCertificates(Stream stream, byte[]? password);
    IList<AsymmetricAlgorithm> AddKeyPairs(Stream stream);
    IList<AsymmetricAlgorithm> AddKeyPairs(Stream stream, byte[]? password);
    IList<X509Crl> AddCrls(Stream stream);
    IList<CertificateRequest> AddCsrs(Stream stream);
    #endregion

    #region Delete
    bool DeleteCertificate(string thumbprint);
    bool DeleteKeyPair(string thumbprint);
    bool DeleteCrl(string thumbprint);
    bool DeleteCsr(string thumbprint);
    #endregion
}
