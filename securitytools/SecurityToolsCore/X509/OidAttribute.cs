namespace GGolbik.SecurityTools.X509;

[AttributeUsage(AttributeTargets.Property)]
public class OidAttribute : Attribute
{
    public string Id;
    public OidAttribute(string id)
    {
        Id = id;
    }
}
