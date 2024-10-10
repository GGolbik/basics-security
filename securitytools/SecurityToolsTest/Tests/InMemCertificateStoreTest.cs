using GGolbik.SecurityTools.Store;

namespace GGolbik.SecurityToolsTest;

public class InMemCertificateStoreTest : CertificateStoreTest
{

    private class InMemCertificateStoreFactory : ICertificateStoreFactory
    {
        public InMemCertificateStoreFactory()
        {
        }
        public ICertificateStore Create()
        {
            return new InMemCertificateStore();
        }

        public void Dispose()
        {
        }
    }

    public InMemCertificateStoreTest() : base(new InMemCertificateStoreFactory())
    {
    }
    [Fact]
    public override void Test_Add_CertificateDer()
    {
        base.Test_Add_CertificateDer();
    }
}