
using System.Security.Cryptography;
using GGolbik.SecurityTools.Store;

namespace GGolbik.SecurityToolsTest;


public class InMemCertificateStoreFactory : ICertificateStoreFactory
{
    public InMemCertificateStoreFactory()
    {
    }

    public ICertificateStore Create()
    {
        return new InMemCertificateStore();
    }

    public ICertificateStore Create(byte[] password, PbeParameters? pbeParameters = null)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
    }
}
