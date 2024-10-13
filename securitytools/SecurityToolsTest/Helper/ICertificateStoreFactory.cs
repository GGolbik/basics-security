using System.Security.Cryptography;
using GGolbik.SecurityTools.Store;

namespace GGolbik.SecurityToolsTest;

public interface ICertificateStoreFactory : IDisposable
{
    ICertificateStore Create();
    ICertificateStore Create(byte[] password, PbeParameters? pbeParameters = null);
}
