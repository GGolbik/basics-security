using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using GGolbik.SecurityTools.Io;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;

namespace GGolbik.SecurityTools.X509;


public static class X509Reader
{
    public static AsymmetricAlgorithm? ReadKeyPairFromPem(byte[] data, byte[]? password = null)
    {
        var span = Encoding.UTF8.GetString(data);
        var rsa = RSA.Create();
        try
        {
            rsa.ImportFromPem(span);
            return rsa;
        }
        catch
        {
            rsa.Dispose();
        }
        var ecdsa = ECDsa.Create();
        try
        {
            ecdsa.ImportFromPem(span);
            return ecdsa;
        }
        catch
        {
            ecdsa.Dispose();
        }
        var dsa = DSA.Create();
        try
        {
            dsa.ImportFromPem(span);
            return dsa;
        }
        catch
        {
            dsa.Dispose();
        }
        var ecdh = ECDiffieHellman.Create();
        try
        {
            ecdh.ImportFromPem(span);
            return ecdh;
        }
        catch
        {
            ecdh.Dispose();
        }

        if (password == null)
        {
            return null;
        }
        rsa = RSA.Create();
        try
        {
            rsa.ImportFromEncryptedPem(span, password);
            return rsa;
        }
        catch
        {
            rsa.Dispose();
        }
        ecdsa = ECDsa.Create();
        try
        {
            ecdsa.ImportFromEncryptedPem(span, password);
            return ecdsa;
        }
        catch
        {
            ecdsa.Dispose();
        }
        dsa = DSA.Create();
        try
        {
            dsa.ImportFromEncryptedPem(span, password);
            return dsa;
        }
        catch
        {
            dsa.Dispose();
        }
        ecdh = ECDiffieHellman.Create();
        try
        {
            ecdh.ImportFromEncryptedPem(span, password);
            return ecdh;
        }
        catch
        {
            ecdh.Dispose();
        }
        return null;
    }

    public static AsymmetricAlgorithm? ReadKeyPairFromDer(byte[] data, out int read, byte[]? password = null)
    {
        var rsa = RSA.Create();
        try
        {
            rsa.ImportPkcs8PrivateKey(data, out read);
            return rsa;
        }
        catch
        {
            rsa.Dispose();
        }
        var ecdsa = ECDsa.Create();
        try
        {
            ecdsa.ImportPkcs8PrivateKey(data, out read);
            return ecdsa;
        }
        catch
        {
            ecdsa.Dispose();
        }
        var dsa = DSA.Create();
        try
        {
            dsa.ImportPkcs8PrivateKey(data, out read);
            return dsa;
        }
        catch
        {
            dsa.Dispose();
        }
        var ecdh = ECDiffieHellman.Create();
        try
        {
            ecdh.ImportPkcs8PrivateKey(data, out read);
            return ecdh;
        }
        catch
        {
            ecdh.Dispose();
        }

        if (password == null)
        {
            read = 0;
            return null;
        }
        rsa = RSA.Create();
        try
        {
            rsa.ImportEncryptedPkcs8PrivateKey(password, data, out read);
            return rsa;
        }
        catch
        {
            rsa.Dispose();
        }
        ecdsa = ECDsa.Create();
        try
        {
            ecdsa.ImportEncryptedPkcs8PrivateKey(password, data, out read);
            return ecdsa;
        }
        catch
        {
            ecdsa.Dispose();
        }
        dsa = DSA.Create();
        try
        {
            dsa.ImportEncryptedPkcs8PrivateKey(password, data, out read);
            return dsa;
        }
        catch
        {
            dsa.Dispose();
        }
        ecdh = ECDiffieHellman.Create();
        try
        {
            ecdh.ImportEncryptedPkcs8PrivateKey(password, data, out read);
            return ecdh;
        }
        catch
        {
            ecdh.Dispose();
        }
        read = 0;
        return null;
    }

    public static ulong Read(Stream stream, PasswordFinder password, Action<X509Certificate2>? certCallback = null, Action<AsymmetricAlgorithm>? keyPairCallback = null, Action<X509Crl>? crlCallback = null, Action<CertificateRequest>? csrCallback = null)
    {
        if (!stream.CanSeek)
        {
            throw new NotSupportedException("A stream which cannot seek is not supported.");
        }
        ulong count = 0;
        BufferStream streamBuffer = new BufferStream(stream);
        streamBuffer.ReadAllBytes();
        count += X509Reader.ReadCertificates(new MemoryStream(streamBuffer.ReadBuffer), password, (cert) =>
        {
            // copy to remove private key
            certCallback?.Invoke(cert.Clone(false));
        });

        count += X509Reader.ReadKeyPairs(new MemoryStream(streamBuffer.ReadBuffer), password, keyPairCallback);

        count += X509Reader.ReadCrls(new MemoryStream(streamBuffer.ReadBuffer), crlCallback);

        count += X509Reader.ReadCsrs(new MemoryStream(streamBuffer.ReadBuffer), csrCallback);
        return count;
    }

    public static ulong ReadCertificates(Stream stream, PasswordFinder password, Action<X509Certificate2>? callback = null)
    {
        ulong count = 0;
        BufferStream streamBuffer = new(stream);
        X509CertificateParser parser = new();
        // read PEM and DER
        try
        {
            var certs = parser.ReadCertificates(streamBuffer);
            foreach (var cert in certs)
            {
                callback?.Invoke(new X509Certificate2(cert.ToDer()));
                count++;
            }
        }
        catch
        {

        }
        streamBuffer.ReadAllBytes();
        if (count == 0)
        {
            // read PKCS#12
            try
            {
                var collection = new X509Certificate2Collection();
                collection.Import(streamBuffer.ReadBuffer.ToArray(), password.ToString(), X509KeyStorageFlags.Exportable);
                foreach (var cert in collection)
                {
                    callback?.Invoke(cert);
                    count++;
                }
            }
            catch
            {

            }
        }
        return count;
    }

    public static ulong ReadKeyPairs(Stream stream, PasswordFinder password, Action<AsymmetricAlgorithm>? callback = null)
    {
        ulong count = 0;
        BufferStream streamBuffer = new(stream);
        using (var textReader = new StreamReader(streamBuffer, leaveOpen: true))
        using (PemReader parser = new(textReader, password))
        {
            // ReadObject: Read the next PEM object attempting to interpret the header and create a higher level object from the content.
            var obj = parser.ReadObject();
            while (obj != null)
            {
                var keyPair = obj as AsymmetricKeyParameter;
                if (keyPair != null)
                {
                    var mem = new MemoryStream();
                    using (var st = new StreamWriter(mem))
                    using (var pemWriter = new PemWriter(st))
                    {
                        pemWriter.WriteObject(keyPair);
                    }
                    var key = X509Reader.ReadKeyPairFromPem(mem.ToArray(), password);
                    if (key != null)
                    {
                        callback?.Invoke(key);
                        count++;
                    }
                }
                //keyPair.
                obj = parser.ReadObject();
            }
        }

        if (count == 0)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var mem = new MemoryStream();
            const int maxSize = 10 * 1024 * 1024;
            stream.CopyTo(maxSize, mem);
            AsymmetricAlgorithm? key = null;
            int read;
            do
            {
                key = X509Reader.ReadKeyPairFromDer(mem.ToArray(), out read, password);
                if (key != null)
                {
                    callback?.Invoke(key);
                    count++;
                }
                mem = new MemoryStream(mem.ToArray(), read, (int)mem.Length - read, true);
                stream.CopyTo(read, mem);
            } while (key != null && mem.Length == 0);
        }

        if (count == 0)
        {
            try
            {
                var collection = new X509Certificate2Collection();
                collection.Import(streamBuffer.ReadBuffer.ToArray(), password.ToString(), X509KeyStorageFlags.Exportable);
                foreach (var cert in collection)
                {
                    var keyPair = cert.GetPrivateKey();
                    if (keyPair != null)
                    {
                        callback?.Invoke(keyPair);
                        count++;
                    }
                }
            }
            catch
            {

            }
        }
        return count;
    }

    public static ulong ReadCrls(Stream stream, Action<X509Crl>? callback = null)
    {
        ulong count = 0;
        try
        {
            X509CrlParser parser = new();
            var crls = parser.ReadCrls(stream);
            if (crls != null)
            {
                foreach (var crl in crls)
                {
                    callback?.Invoke(crl);
                    count++;
                }
            }
        }
        catch
        {

        }
        return count;
    }

    public static ulong ReadCsrs(Stream stream, Action<CertificateRequest>? callback = null)
    {
        // TODO: Update to read multiple CSRs
        ulong count = 0;
        try
        {
            BufferStream bufferStream = new(stream);
            bufferStream.ReadAllBytes();
            var csr = CertificateRequest.LoadSigningRequest(
                bufferStream.ReadBuffer,
                HashAlgorithmName.SHA256,
                CertificateRequestLoadOptions.UnsafeLoadCertificateExtensions);
            callback?.Invoke(csr);
            count++;
            return count;
        }
        catch
        {

        }
        try
        {
            BufferStream bufferStream = new(stream);
            bufferStream.ReadAllBytes();
            var csr = CertificateRequest.LoadSigningRequestPem(
                Encoding.UTF8.GetString(bufferStream.ReadBuffer),
                HashAlgorithmName.SHA256,
                CertificateRequestLoadOptions.UnsafeLoadCertificateExtensions);
            callback?.Invoke(csr);
            count++;
        }
        catch
        {

        }
        return count;
    }
}