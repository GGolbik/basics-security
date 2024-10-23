using System.Text;
using GGolbik.SecurityTools.Crypto;
using Org.BouncyCastle.Crypto;

namespace GGolbik.SecurityToolsTest;


public class CryptTest
{
    [Fact]
    public void Test_Crypt_Aes()
    {
        MemoryStream encrypted = new();
        CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(1));
        byte[] password = Encoding.UTF8.GetBytes("1234");
        string dataString = "Hello World";
        Crypt.EncryptAes256Cbc(new MemoryStream(Encoding.UTF8.GetBytes(dataString)), encrypted, password, cancellationTokenSource.Token);
        encrypted.Position = 0;
        MemoryStream data = new();
        Crypt.DecryptAes(encrypted, password, data, cancellationTokenSource.Token);
        var result = Encoding.UTF8.GetString(data.ToArray());
        result.Should().Be(dataString);
    }

    [Fact]
    public void Test_Crypt_Aes_Invalid_Password()
    {
        MemoryStream encrypted = new();
        CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(1));
        byte[] password = Encoding.UTF8.GetBytes("1234");
        string dataString = "Hello World";
        Crypt.EncryptAes256Cbc(new MemoryStream(Encoding.UTF8.GetBytes(dataString)), encrypted, password, cancellationTokenSource.Token);
        encrypted.Position = 0;
        MemoryStream data = new();
        Action act = () =>
        {
            Crypt.DecryptAes(encrypted, Encoding.UTF8.GetBytes("4321"), data, cancellationTokenSource.Token);
        };
        act.Should().Throw<InvalidCipherTextException>();
    }
}