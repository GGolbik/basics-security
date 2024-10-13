using System.Text;

namespace GGolbik.SecurityToolsTest;

public class DirectoryCertificateStoreCryptTest : CertificateStoreTest
{

    public DirectoryCertificateStoreCryptTest() : base(
        new DirectoryCertificateStoreFactory(
            Path.Combine(
                Path.GetDirectoryName(typeof(DirectoryCertificateStoreCryptTest).Assembly.Location)!,
                "tests" + Random.Shared.Next()
            ),
            Encoding.UTF8.GetBytes("test")
        )
    )
    {
    }
}