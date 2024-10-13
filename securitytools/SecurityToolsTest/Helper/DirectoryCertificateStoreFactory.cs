using System.Security.Cryptography;
using GGolbik.SecurityTools.Store;

namespace GGolbik.SecurityToolsTest;

public class DirectoryCertificateStoreFactory : ICertificateStoreFactory
{
    public readonly string _path;
    public readonly byte[]? _password = null;
    public DirectoryCertificateStoreFactory(string path) : this(path, null)
    {

    }
    public DirectoryCertificateStoreFactory(string path, byte[]? password)
    {
        _path = path;
        if (Directory.Exists(_path))
            Directory.Delete(_path, true);
        _password = password;
    }

    public ICertificateStore Create()
    {
        if (_password != null)
        {
            return new DirectoryCertificateStore(_path, _password);
        }
        return new DirectoryCertificateStore(_path);
    }

    public ICertificateStore Create(byte[] password, PbeParameters? pbeParameters = null)
    {
        return new DirectoryCertificateStore(_path, password, pbeParameters);
    }

    public void Dispose()
    {
        if (Directory.Exists(_path))
            Directory.Delete(_path, true);
    }
}
