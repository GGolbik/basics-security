using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using GGolbik.SecurityTools.X509;

namespace GGolbik.SecurityToolsTest;

public abstract class CertificateStoreTest : IDisposable
{
    protected ICertificateStoreFactory _factory;
    protected CertificateStoreTest(ICertificateStoreFactory factory)
    {
        _factory = factory;
    }

    public virtual void Dispose()
    {
        _factory.Dispose();
    }

    [Fact]
    public virtual void Test_Add_CertificateDer()
    {
        try
        {
            var store = _factory.Create();
            var cert = new X509Certificate2(Encoding.UTF8.GetBytes(TestData.TestRootCaPem));
            store.Add(new MemoryStream(cert.Export(X509ContentType.Cert)), null);
            Assert.Single(store.GetCertificates());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

        [Fact]
    public virtual void Test_AddCertificates_CertificateDer()
    {
        try
        {
            var store = _factory.Create();
            var cert = new X509Certificate2(Encoding.UTF8.GetBytes(TestData.TestRootCaPem));
            store.AddCertificates(new MemoryStream(cert.Export(X509ContentType.Cert)), null);
            Assert.Single(store.GetCertificates());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public virtual void Test_Add_CertificatePem()
    {
        try
        {
            var store = _factory.Create();
            store.Add(new MemoryStream(Encoding.UTF8.GetBytes(TestData.TestRootCaPem)), null);
            Assert.Single(store.GetCertificates());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public virtual void Test_AddCertificates_CertificatePem()
    {
        try
        {
            var store = _factory.Create();
            store.AddCertificates(new MemoryStream(Encoding.UTF8.GetBytes(TestData.TestRootCaPem)), null);
            Assert.Single(store.GetCertificates());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public virtual void Test_Add_KeyPairsDer()
    {
        try
        {
            var store = _factory.Create();
            var key = X509Reader.ReadKeyPairFromPem(Encoding.UTF8.GetBytes(TestData.TestRootCaKeyPem));
            store.Add(new MemoryStream(key!.ExportPkcs8PrivateKey()), null);
            Assert.Single(store.GetKeyPairs());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public virtual void Test_AddKeyPairs_Der()
    {
        try
        {
            var store = _factory.Create();
            var key = X509Reader.ReadKeyPairFromPem(Encoding.UTF8.GetBytes(TestData.TestRootCaKeyPem));
            store.AddKeyPairs(new MemoryStream(key!.ExportPkcs8PrivateKey()));
            Assert.Single(store.GetKeyPairs());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public virtual void Test_Add_KeyPairsDerEncrypted()
    {
        try
        {
            var store = _factory.Create();
            var key = X509Reader.ReadKeyPairFromPem(Encoding.UTF8.GetBytes(TestData.TestRootCaKeyPem));
            byte[] password = Encoding.UTF8.GetBytes("1234");
            PbeParameters pbeParameters = new PbeParameters(PbeEncryptionAlgorithm.Aes256Cbc, HashAlgorithmName.SHA256, 100_000);
            var test = key!.ExportEncryptedPkcs8PrivateKeyPem(password, pbeParameters);
            store.Add(new MemoryStream(key!.ExportEncryptedPkcs8PrivateKey(password, pbeParameters)), password);
            Assert.Single(store.GetKeyPairs());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public virtual void Test_AddKeyPairs_DerEncrypted()
    {
        try
        {
            var store = _factory.Create();
            var key = X509Reader.ReadKeyPairFromPem(Encoding.UTF8.GetBytes(TestData.TestRootCaKeyPem));
            byte[] password = Encoding.UTF8.GetBytes("1234");
            PbeParameters pbeParameters = new PbeParameters(PbeEncryptionAlgorithm.Aes256Cbc, HashAlgorithmName.SHA256, 100_000);
            var test = key!.ExportEncryptedPkcs8PrivateKeyPem(password, pbeParameters);
            store.AddKeyPairs(new MemoryStream(key!.ExportEncryptedPkcs8PrivateKey(password, pbeParameters)), password);
            Assert.Single(store.GetKeyPairs());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public virtual void Test_Add_KeyPairsPem()
    {
        try
        {
            var store = _factory.Create();
            store.Add(new MemoryStream(Encoding.UTF8.GetBytes(TestData.TestRootCaKeyPem)), null);
            Assert.Single(store.GetKeyPairs());
            store.GetKeyPairs()[0].ToKeyPairInfo();
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public virtual void Test_AddKeyPairs_Pem()
    {
        try
        {
            var store = _factory.Create();
            store.AddKeyPairs(new MemoryStream(Encoding.UTF8.GetBytes(TestData.TestRootCaKeyPem)));
            Assert.Single(store.GetKeyPairs());
            store.GetKeyPairs()[0].ToKeyPairInfo();
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public virtual void Test_Add_KeyPairsPemEncrypted()
    {
        try
        {
            var store = _factory.Create();
            var key = X509Reader.ReadKeyPairFromPem(Encoding.UTF8.GetBytes(TestData.TestRootCaKeyPem));
            byte[] password = Encoding.UTF8.GetBytes("1234");
            PbeParameters pbeParameters = new PbeParameters(PbeEncryptionAlgorithm.Aes256Cbc, HashAlgorithmName.SHA256, 100_000);
            string encryptedPem = key!.ExportEncryptedPkcs8PrivateKeyPem(password, pbeParameters);
            store.Add(new MemoryStream(Encoding.UTF8.GetBytes(encryptedPem)), password);
            Assert.Single(store.GetKeyPairs());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public virtual void Test_AddKeyPairs_PemEncrypted()
    {
        try
        {
            var store = _factory.Create();
            var key = X509Reader.ReadKeyPairFromPem(Encoding.UTF8.GetBytes(TestData.TestRootCaKeyPem));
            byte[] password = Encoding.UTF8.GetBytes("1234");
            PbeParameters pbeParameters = new PbeParameters(PbeEncryptionAlgorithm.Aes256Cbc, HashAlgorithmName.SHA256, 100_000);
            string encryptedPem = key!.ExportEncryptedPkcs8PrivateKeyPem(password, pbeParameters);
            store.AddKeyPairs(new MemoryStream(Encoding.UTF8.GetBytes(encryptedPem)), password);
            Assert.Single(store.GetKeyPairs());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public virtual void Test_Add_Pkcs12DerSingle()
    {
        try
        {
            var store = _factory.Create();
            var cert = X509Certificate2.CreateFromPem(TestData.TestRootCaPem, TestData.TestRootCaKeyPem);
            byte[] rawData = cert.Export(X509ContentType.Pkcs12);
            var collection = new X509Certificate2Collection();
            collection.Import(rawData);
            store.Add(new MemoryStream(collection.Export(X509ContentType.Pkcs12)!), null);
            Assert.Single(store.GetKeyPairs());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public virtual void Test_AddCertificates_Pkcs12DerSingle()
    {
        try
        {
            var store = _factory.Create();
            var cert = X509Certificate2.CreateFromPem(TestData.TestRootCaPem, TestData.TestRootCaKeyPem);
            byte[] rawData = cert.Export(X509ContentType.Pkcs12);
            var collection = new X509Certificate2Collection();
            collection.Import(rawData);
            store.AddCertificates(new MemoryStream(collection.Export(X509ContentType.Pkcs12)!), null);
            Assert.Single(store.GetCertificates());
            Assert.Empty(store.GetKeyPairs());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public virtual void Test_Add_Pkcs12DerMultiple()
    {
        try
        {
            var store = _factory.Create();
            var cert = X509Certificate2.CreateFromPem(TestData.TestRootCaPem, TestData.TestRootCaKeyPem);
            var cert2 = X509Certificate2.CreateFromPem(TestData.TestCodeCaPem, TestData.TestCodeCaKeyPem);
            X509Certificate2Collection pkcs12 = [cert, cert2];
            store.Add(new MemoryStream(pkcs12.Export(X509ContentType.Pkcs12)!), null);
            Assert.Equal(2, store.GetCertificates().Count());
            Assert.Equal(2, store.GetKeyPairs().Count());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public virtual void Test_AddCertificates_Pkcs12DerMultiple()
    {
        try
        {
            var store = _factory.Create();
            var cert = X509Certificate2.CreateFromPem(TestData.TestRootCaPem, TestData.TestRootCaKeyPem);
            var cert2 = X509Certificate2.CreateFromPem(TestData.TestCodeCaPem, TestData.TestCodeCaKeyPem);
            X509Certificate2Collection pkcs12 = [cert, cert2];
            store.AddCertificates(new MemoryStream(pkcs12.Export(X509ContentType.Pkcs12)!), null);
            Assert.Equal(2, store.GetCertificates().Count());
            Assert.Empty(store.GetKeyPairs());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public virtual void Test_Add_Pkcs12DerSingleEncrypted()
    {
        try
        {
            var store = _factory.Create();
            var cert = X509Certificate2.CreateFromPem(TestData.TestRootCaPem, TestData.TestRootCaKeyPem);
            byte[] rawData = cert.Export(X509ContentType.Pkcs12);
            var collection = new X509Certificate2Collection();
            collection.Import(rawData);
            store.Add(new MemoryStream(collection.Export(X509ContentType.Pkcs12, "1234")!), Encoding.UTF8.GetBytes("1234"));
            Assert.Single(store.GetKeyPairs());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public virtual void Test_AddCertificates_Pkcs12DerSingleEncrypted()
    {
        try
        {
            var store = _factory.Create();
            var cert = X509Certificate2.CreateFromPem(TestData.TestRootCaPem, TestData.TestRootCaKeyPem);
            byte[] rawData = cert.Export(X509ContentType.Pkcs12);
            var collection = new X509Certificate2Collection();
            collection.Import(rawData);
            store.AddCertificates(new MemoryStream(collection.Export(X509ContentType.Pkcs12, "1234")!), Encoding.UTF8.GetBytes("1234"));
            Assert.Single(store.GetCertificates());
            Assert.Empty(store.GetKeyPairs());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public virtual void Test_Add_Pkcs12DerMultipleEncrypted()
    {
        try
        {
            var store = _factory.Create();
            var cert = X509Certificate2.CreateFromPem(TestData.TestRootCaPem, TestData.TestRootCaKeyPem);
            var cert2 = X509Certificate2.CreateFromPem(TestData.TestCodeCaPem, TestData.TestCodeCaKeyPem);
            X509Certificate2Collection pkcs12 = [cert, cert2];
            store.Add(new MemoryStream(pkcs12.Export(X509ContentType.Pkcs12, "1234")!), Encoding.UTF8.GetBytes("1234"));
            Assert.Equal(2, store.GetCertificates().Count());
            Assert.Equal(2, store.GetKeyPairs().Count());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    
    [Fact]
    public virtual void Test_AddCertificates_Pkcs12DerMultipleEncrypted()
    {
        try
        {
            var store = _factory.Create();
            var cert = X509Certificate2.CreateFromPem(TestData.TestRootCaPem, TestData.TestRootCaKeyPem);
            var cert2 = X509Certificate2.CreateFromPem(TestData.TestCodeCaPem, TestData.TestCodeCaKeyPem);
            X509Certificate2Collection pkcs12 = [cert, cert2];
            store.AddCertificates(new MemoryStream(pkcs12.Export(X509ContentType.Pkcs12, "1234")!), Encoding.UTF8.GetBytes("1234"));
            Assert.Equal(2, store.GetCertificates().Count());
            Assert.Empty(store.GetKeyPairs());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public virtual void Test_Add_KeyPairsPkcs12Der()
    {
        try
        {
            var store = _factory.Create();
            var cert = X509Certificate2.CreateFromPem(TestData.TestRootCaPem, TestData.TestRootCaKeyPem);
            store.Add(new MemoryStream(cert.Export(X509ContentType.Pkcs12)), null);
            Assert.Single(store.GetKeyPairs());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public virtual void Test_AddKeyPairs_Pkcs12Der()
    {
        try
        {
            var store = _factory.Create();
            var cert = X509Certificate2.CreateFromPem(TestData.TestRootCaPem, TestData.TestRootCaKeyPem);
            store.AddKeyPairs(new MemoryStream(cert.Export(X509ContentType.Pkcs12)));
            Assert.Single(store.GetKeyPairs());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public virtual void Test_Add_CrlDer()
    {
        try
        {
            var store = _factory.Create();
            store.Add(new MemoryStream(TestData.TestRootCaCrl0Der), null);
            Assert.Single(store.GetCrls());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public virtual void Test_AddCrls_CrlDer()
    {
        try
        {
            var store = _factory.Create();
            store.AddCrls(new MemoryStream(TestData.TestRootCaCrl0Der));
            Assert.Single(store.GetCrls());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public virtual void Test_Add_CrlPem()
    {
        try
        {
            var store = _factory.Create();
            StringBuilder builder = new("-----BEGIN X509 CRL-----\n");
            builder.Append(Convert.ToBase64String(TestData.TestRootCaCrl0Der));
            builder.Append("\n-----END X509 CRL-----\n");
            store.Add(new MemoryStream(Encoding.UTF8.GetBytes(builder.ToString())), null);
            Assert.Single(store.GetCrls());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public virtual void Test_AddCrls_CrlPem()
    {
        try
        {
            var store = _factory.Create();
            StringBuilder builder = new("-----BEGIN X509 CRL-----\n");
            builder.Append(Convert.ToBase64String(TestData.TestRootCaCrl0Der));
            builder.Append("\n-----END X509 CRL-----\n");
            store.AddCrls(new MemoryStream(Encoding.UTF8.GetBytes(builder.ToString())));
            Assert.Single(store.GetCrls());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public virtual void Test_GetCrlsOfIssuer()
    {
        try
        {
            var store = _factory.Create();

            var cert = new X509Certificate2(Encoding.UTF8.GetBytes(TestData.TestRootCaPem));
            store.Add(new MemoryStream(cert.Export(X509ContentType.Cert)), null);
            Assert.Single(store.GetCertificates());

            store.AddCrls(new MemoryStream(TestData.TestRootCaCrl0Der));
            Assert.Single(store.GetCrls());

            Assert.Single(store.GetCrlsOfIssuer(cert.Thumbprint));
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public virtual void Test_GetCrlsOfIssuerVerify()
    {
        try
        {
            var store = _factory.Create();

            var cert = new X509Certificate2(Encoding.UTF8.GetBytes(TestData.TestRootCaPem));
            store.Add(new MemoryStream(cert.Export(X509ContentType.Cert)), null);
            Assert.Single(store.GetCertificates());

            store.AddCrls(new MemoryStream(TestData.TestRootCaCrl0Der));
            Assert.Single(store.GetCrls());

            Assert.Single(store.GetCrlsOfIssuer(cert.Thumbprint, true));
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }
}