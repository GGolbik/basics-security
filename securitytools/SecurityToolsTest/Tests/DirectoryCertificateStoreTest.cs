using GGolbik.SecurityTools.Store;

namespace GGolbik.SecurityToolsTest;

public class DirectoryCertificateStoreTest : CertificateStoreTest
{

    private class DirectoryCertificateStoreFactory : ICertificateStoreFactory
    {
        public readonly string _path;
        public DirectoryCertificateStoreFactory(string path)
        {
            _path = path;
            if (Directory.Exists(_path))
                Directory.Delete(_path, true);
        }
        public ICertificateStore Create()
        {
            return new DirectoryCertificateStore(_path);
        }

        public void Dispose()
        {
            if (Directory.Exists(_path))
                Directory.Delete(_path, true);
        }
    }

    public DirectoryCertificateStoreTest() : base(new DirectoryCertificateStoreFactory(Path.Combine(Path.GetDirectoryName(typeof(DirectoryCertificateStoreTest).Assembly.Location)!, "test")))
    {
    }
}