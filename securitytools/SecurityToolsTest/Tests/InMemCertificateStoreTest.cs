using GGolbik.SecurityTools.Store;

namespace GGolbik.SecurityToolsTest;

public class InMemCertificateStoreTest : CertificateStoreTest
{

    public InMemCertificateStoreTest() : base(new InMemCertificateStoreFactory())
    {
    }

    [Fact]
    public override void Test_Add_CertificateDer()
    {
        base.Test_Add_CertificateDer();
    }
}