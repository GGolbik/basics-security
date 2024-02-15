
using GGolbik.SecurityTools.Builders;
using GGolbik.SecurityTools.Models;
using Serilog;

namespace GGolbik.SecurityTools.Services;


public class SecurityToolsService : ISecurityToolsService
{
    private readonly Serilog.ILogger Logger = Log.ForContext<SecurityToolsService>();

    private readonly object _mutex = new();

    public ConfigCert Build(ConfigCert config)
    {
        lock(_mutex)
        {
            return new CertBuilder().Build(config);
        }
    }

    public ConfigCsr Build(ConfigCsr config)
    {
        lock(_mutex)
        {
            return new CsrBuilder().Build(config);
        }
    }

    public ConfigCrl Build(ConfigCrl config)
    {
        lock(_mutex)
        {
            return new CrlBuilder().Build(config);
        }
    }

    public ConfigKeyPair Build(ConfigKeyPair config)
    {
        lock(_mutex)
        {
            return new KeyPairBuilder().Build(config);
        }
    }

    public ConfigTransform Build(ConfigTransform config)
    {
        lock(_mutex)
        {
            return new TransformBuilder().Build(config);
        }
    }

    public void Dispose()
    {
    }
}