using System.Security.Cryptography;
using GGolbik.SecurityTools.X509.Models;

namespace GGolbik.SecurityTools.X509.Builders;

public class KeyPairBuilder : SecurityBuilder<CsrBuilder>
{
    public ConfigKeyPair Build(ConfigKeyPair config)
    {
        return this.Build(config, out var privateKey);
    }
    
    public ConfigKeyPair Build(ConfigKeyPair config, out AsymmetricAlgorithm privateKey)
    {
        this.InvokeOnAction(SecurityBuilderEventKind.BuildingKeyPair);
        var result = (ConfigKeyPair)config.Clone();

        privateKey = this.LoadKeyPair(result);
        this.InvokeOnAction(SecurityBuilderEventKind.BuiltKeyPair);

        result.PrivateKey ??= new X50xFile();
        result.PublicKey ??= new X50xFile();
        HashAlgorithmName? keyAlg = result.HashAlgorithm?.ToHashAlgorithm();
        this.SaveKeyPair(result.PrivateKey, result.PublicKey, privateKey, keyAlg);
        return result;
    }

    private AsymmetricAlgorithm LoadKeyPair(ConfigKeyPair result)
    {
        AsymmetricAlgorithm privateKey;
        if (result.PrivateKey?.Exists() ?? false)
        {
            this.InvokeOnAction(SecurityBuilderEventKind.LoadingKeyPair);
            privateKey = this.LoadOrGenerateKeyPair(result);
        }
        else
        {
            privateKey = this.LoadOrGenerateKeyPair(result);
        }
        return privateKey;
    }
}
