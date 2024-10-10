using GGolbik.SecurityTools.X509.Models;

namespace GGolbik.SecurityTools.Services;

public interface ISecurityToolsService : IDisposable
{
    public ConfigCert Build(ConfigCert config);
    public ConfigCsr Build(ConfigCsr config);
    public ConfigCrl Build(ConfigCrl config);
    public ConfigKeyPair Build(ConfigKeyPair config);
    public ConfigTransform Build(ConfigTransform config);
}