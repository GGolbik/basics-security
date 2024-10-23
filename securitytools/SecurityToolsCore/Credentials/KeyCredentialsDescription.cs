
namespace GGolbik.SecurityTools.Credentials;

public class KeyCredentialsDescription : ICloneable
{
    public string? Label { get; set; }
    public string? Details { get; set; }
    public KeyCredentialsKind Kind { get; set; }
    public bool Encrypted { get; set; }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}