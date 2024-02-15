
using System.Security.Cryptography.X509Certificates;

namespace GGolbik.SecurityTools.Builders;

public static class CertificateRequestExtension
{
    /// <summary>
    /// Creates an ASN.1 DER-encoded PKCS#10 CertificationRequest value representing the state of the current object.
    /// </summary>
    /// <param name="csr"></param>
    /// <param name="path"></param>
    public static void SaveDer(this CertificateRequest csr, string path)
    {
        using (var stream = new FileStream(path, FileMode.Create))
        {
            csr.SaveDer(stream);
        }
    }

    /// <summary>
    /// Creates an ASN.1 DER-encoded PKCS#10 CertificationRequest value representing the state of the current object.
    /// </summary>
    /// <param name="csr"></param>
    /// <param name="stream"></param>
    public static void SaveDer(this CertificateRequest csr, Stream stream)
    {
        byte[] encoded = csr.CreateSigningRequest();
        stream.Write(encoded);
    }

    /// <summary>
    /// Creates a PEM-encoded PKCS#10 CertificationRequest representing the current state of this object using the provided signature generator.
    /// </summary>
    /// <param name="csr"></param>
    /// <param name="path"></param>
    public static void SavePem(this CertificateRequest csr, string path)
    {
        using (var stream = new FileStream(path, FileMode.Create))
        {
            csr.SavePem(stream);
        }
    }

    /// <summary>
    /// Creates a PEM-encoded PKCS#10 CertificationRequest representing the current state of this object using the provided signature generator.
    /// </summary>
    /// <param name="csr"></param>
    /// <param name="stream"></param>
    public static void SavePem(this CertificateRequest csr, Stream stream)
    {
        // encode request to base64
        string encoded = csr.CreateSigningRequestPem();
        // write request to a file
        using (var writer = new StreamWriter(stream, leaveOpen: true))
        {
            writer.Write(encoded);
        }
    }
}
