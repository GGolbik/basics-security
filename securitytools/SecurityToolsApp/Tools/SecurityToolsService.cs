
using GGolbik.SecurityTools.X509.Builders;
using GGolbik.SecurityTools.X509.Models;
using Serilog;

namespace GGolbik.SecurityToolsApp.Tools;


public class SecurityToolsService : ISecurityToolsService
{
    private readonly Serilog.ILogger Logger = Log.ForContext<SecurityToolsService>();

    private void OnAction(object? sender, SecurityBuilderEventArgs args)
    {
        Logger.Information(args.Kind.ToString());
    }

    public ConfigCert Build(ConfigCert config)
    {
        lock(this)
        {
            CertBuilder builder = new();
            builder.OnAction += this.OnAction;
            return builder.Build(config);
        }
    }

    public ConfigCsr Build(ConfigCsr config)
    {
        lock(this)
        {
            CsrBuilder builder = new();
            builder.OnAction += this.OnAction;
            return builder.Build(config);
        }
    }

    public ConfigCrl Build(ConfigCrl config)
    {
        lock(this)
        {
            CrlBuilder builder = new();
            builder.OnAction += this.OnAction;
            return builder.Build(config);
        }
    }

    public ConfigKeyPair Build(ConfigKeyPair config)
    {
        lock(this)
        {
            KeyPairBuilder builder = new();
            builder.OnAction += this.OnAction;
            return builder.Build(config);
        }
    }

    public ConfigTransform Build(ConfigTransform config)
    {
        lock(this)
        {
            TransformBuilder builder = new();
            builder.OnAction += this.OnAction;
            return builder.Build(config);
        }
    }

    public void Dispose()
    {
    }
}