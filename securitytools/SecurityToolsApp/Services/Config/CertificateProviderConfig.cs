
namespace GGolbik.SecurityTools.Store;

public class CertificateProviderConfig : ICloneable
{
    public IList<CertificateGroup>? CertificateGroups { get; set; }

    public CertificateProviderConfig()
    {

    }

    public object Clone()
    {
        CertificateProviderConfig config = new();
        if (this.CertificateGroups != null)
        {
            config.CertificateGroups = new List<CertificateGroup>();
            foreach (var group in this.CertificateGroups)
            {
                config.CertificateGroups.Add((CertificateGroup)group.Clone());
            }
        }
        return config;
    }
}