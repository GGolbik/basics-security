
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.OpenSsl;


namespace GGolbik.SecurityTools.X509;

public class PasswordFinder : IPasswordFinder
{
    private readonly char[]? _password;
    public PasswordFinder()
    {

    }
    public PasswordFinder(string? password)
    {
        _password = password?.ToArray();
    }
    public PasswordFinder(char[]? password)
    {
        _password = password;
    }
    public PasswordFinder(byte[]? password)
    {
        if (password != null)
            _password = Encoding.UTF8.GetChars(password);
    }
    public static implicit operator PasswordFinder(string? password) => new PasswordFinder(password);
    public static implicit operator PasswordFinder(char[]? password) => new PasswordFinder(password);
    public static implicit operator PasswordFinder(byte[]? password) => new PasswordFinder(password);
    public static implicit operator byte[]?(PasswordFinder password) => password.ToBytes();
    public char[] GetPassword()
    {
        return _password ?? [];
    }
    public byte[]? ToBytes()
    {
        return _password == null ? null : Encoding.UTF8.GetBytes(this.GetPassword());
    }
    public override string? ToString()
    {
        return _password == null ? null : new string(_password);
    }
}