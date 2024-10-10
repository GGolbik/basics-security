
using System.Security.Cryptography;

namespace GGolbik.SecurityTools.X509;

[Flags]
public enum ExtendedKeyUsageFlags
{
    None = 0,
    ServerAuth = 1,
    ClientAuth = 1 << 1,
    CodeSigning = 1 << 2,
    EmailProtection = 1 << 3,
    TimeStamping = 1 << 4,
    OCSPSigning = 1 << 5
}

public static class ExtendedKeyUsageFlagsExtension
{
    public static OidCollection ToOidCollection(this ExtendedKeyUsageFlags flags)
    {
        var oids = new OidCollection();
        foreach (var value in Enum.GetValues<ExtendedKeyUsageFlags>())
        {
            if (!flags.HasFlag(value))
            {
                continue;
            }
            switch (value)
            {
                case ExtendedKeyUsageFlags.ServerAuth:
                    oids.Add(new Oid("1.3.6.1.5.5.7.3.1"));
                    break;
                case ExtendedKeyUsageFlags.ClientAuth:
                    oids.Add(new Oid("1.3.6.1.5.5.7.3.2"));
                    break;
                case ExtendedKeyUsageFlags.CodeSigning:
                    oids.Add(new Oid("1.3.6.1.5.5.7.3.3"));
                    break;
                case ExtendedKeyUsageFlags.EmailProtection:
                    oids.Add(new Oid("1.3.6.1.5.5.7.3.4"));
                    break;
                case ExtendedKeyUsageFlags.TimeStamping:
                    oids.Add(new Oid("1.3.6.1.5.5.7.3.8"));
                    break;
                case ExtendedKeyUsageFlags.OCSPSigning:
                    oids.Add(new Oid("1.3.6.1.5.5.7.3.9"));
                    break;
                default:
                    break;
            }
        }
        return oids;
    }
}