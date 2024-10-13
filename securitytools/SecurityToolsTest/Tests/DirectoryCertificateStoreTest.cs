using System.Security.Cryptography;
using System.Text;
using GGolbik.SecurityTools.X509;

namespace GGolbik.SecurityToolsTest;

public class DirectoryCertificateStoreTest : CertificateStoreTest
{

    public DirectoryCertificateStoreTest() : base(
        new DirectoryCertificateStoreFactory(
            Path.Combine(
                Path.GetDirectoryName(typeof(DirectoryCertificateStoreTest).Assembly.Location)!,
                "tests" + Random.Shared.Next()
            )
        )
    )
    {
    }

    [Fact]
    public void Test_GetKeyPairsWithError()
    {
        byte[] storePassword = Encoding.UTF8.GetBytes("12345678");
        byte[] password = Encoding.UTF8.GetBytes("1234");
        try
        {
            var store = _factory.Create(storePassword);
            var key = X509Reader.ReadKeyPairFromPem(Encoding.UTF8.GetBytes(TestData.TestRootCaKeyPem));
            PbeParameters pbeParameters = new PbeParameters(PbeEncryptionAlgorithm.Aes256Cbc, HashAlgorithmName.SHA256, 100_000);
            var test = key!.ExportEncryptedPkcs8PrivateKeyPem(password, pbeParameters);
            store.Add(new MemoryStream(key!.ExportEncryptedPkcs8PrivateKey(password, pbeParameters)), password);
            Assert.Single(store.GetKeyPairs());
            Assert.Empty(store.GetKeyPairsWithError());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
        try
        {
            var store = _factory.Create(password);
            Assert.Empty(store.GetKeyPairs());
            Assert.Single(store.GetKeyPairsWithError());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }

        try
        {
            var store = _factory.Create(storePassword);
            Assert.Single(store.GetKeyPairs());
            Assert.Empty(store.GetKeyPairsWithError());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public void Test_UpdatePassword()
    {

    }
}