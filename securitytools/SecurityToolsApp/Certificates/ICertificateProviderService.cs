using System.Security.Cryptography;
using GGolbik.SecurityTools.X509;
using GGolbik.SecurityTools.X509.Infos;

namespace GGolbik.SecurityToolsApp.Certificates;

/// <summary>
/// 
/// </summary>
public interface ICertificateProviderService : IDisposable
{
    #region Certificate Group
    /// <summary>
    /// Returns all created certificate groups.
    /// </summary>
    /// <returns>The certificate groups.</returns>
    IList<CertificateGroup> GetCertificateGroups();

    /// <summary>
    /// Returns the group for the ID.
    /// </summary>
    /// <param name="groupId">The ID of the certificate group.</param>
    /// <returns>The config exists.</returns>
    /// <exception cref="Exception"></exception>
    CertificateGroup GetCertificateGroup(string groupId);

    /// <summary>
    /// Adds a new group.
    /// </summary>
    /// <param name="group">The new group.</param>
    /// <returns>The group ID.</returns>
    /// <exception cref="Exception"></exception>
    string AddCertificateGroup(CertificateGroup group);

    /// <summary>
    /// Updates an existing group.
    /// </summary>
    /// <param name="group">The updated group.</param>
    /// <exception cref="Exception"></exception>
    void UpdateCertificateGroup(CertificateGroup group);

    /// <summary>
    /// Deletes the group.
    /// </summary>
    /// <param name="groupId">The ID of the group.</param>
    void DeleteCertificateGroup(string groupId);

    #endregion

    #region Add

    /// <summary>
    /// Add a certificate to the own certificate store.
    /// </summary>
    /// <param name="groupId">The ID of the config.</param>
    /// <param name="certificates">The certificate with a private key to add as own certificate.</param>
    CertificateInfoCollection AddCertificates(string groupId, Stream certificates);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="groupId">The ID of the config.</param>
    /// <param name="keyPairs"></param>
    /// <param name="password"></param>
    KeyPairInfoCollection AddKeyPairs(string groupId, Stream keyPairs, byte[]? password);

    /// <summary>
    /// Adds the CRL to the trusted peer certificate store.
    /// </summary>
    /// <param name="groupId">The ID of the config.</param>
    /// <param name="crls">https://www.ietf.org/rfc/rfc3280.txt</param>
    CrlInfoCollection AddCrls(string groupId, Stream crls);

    #endregion

    #region Delete

    /// <summary>
    /// Deletes the certificate from the own certificate store.
    /// </summary>
    /// <param name="groupId">The ID of the config.</param>
    /// <param name="thumbprint">The SHA1 hash of the certificate formatted as a hexadecimal string.</param>
    void DeleteCertificate(string groupId, string thumbprint);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="thumbprint"></param>
    void DeleteKeyPair(string groupId, string thumbprint);

    /// <summary>
    /// Deletes the CRLs from the trusted peer certificate store.
    /// </summary>
    /// <param name="groupId">The ID of the config.</param>
    /// <param name="thumbprint">The SHA1 hash of the issuer certificate formatted as a hexadecimal string.</param>
    void DeleteCrl(string groupId, string thumbprint);

    #endregion

    #region Get

    /// <summary>
    /// Returns all certificates from the own certificate store without private key.
    /// </summary>
    /// <param name="groupId">The ID of the config.</param>
    /// <returns>All certificates from the own certificate store.</returns>
    IList<CertificateInfo> GetCertificates(string groupId);

    CertificateInfo GetCertificate(string groupId, string thumbprint);

    CertificateInfo GetCertificate(string groupId, string thumbprint, Stream stream, X509Encoding encoding);

    IList<KeyPairInfo> GetKeyPairs(string groupId);

    KeyPairInfo GetKeyPair(string groupId, string thumbprint);
    KeyPairInfo GetKeyPair(string groupId, string thumbprint, Stream stream, X509Encoding encoding);
    KeyPairInfo GetKeyPair(string groupId, string thumbprint, Stream stream, X509Encoding encoding, byte[]? password, PbeParameters? pbeParameters);

    /// <summary>
    /// Returns all CRLs from the trusted issuer certificate store.
    /// </summary>
    /// <param name="groupId">The ID of the config.</param>
    /// <returns>All CRLs from the trusted issuer certificate store.</returns>
    IList<CrlInfo> GetCrls(string groupId);
    CrlInfo GetCrl(string groupId, string thumbprint);
    CrlInfo GetCrl(string groupId, string thumbprint, Stream stream, X509Encoding encoding);

    #endregion

}